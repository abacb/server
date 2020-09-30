using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Core.Utility.Extensions;
using Rms.Server.Core.Utility.Models.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// DtDeviceFileRepository クラス
    /// </summary>
    public partial class DtDeviceFileRepository : IDtDeviceFileRepository
    {
        /// <summary>ロガー</summary>
        private readonly ILogger<DtDeviceFileRepository> _logger;

        /// <summary>DateTimeの提供元</summary>
        private readonly ITimeProvider _timePrivder;

        /// <summary>DB接続用のPolly</summary>
        private readonly DBPolly _dbPolly;

        /// <summary>アプリケーション設定</summary>
        private readonly AppSettings _appSettings;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="timePrivder">DateTimeの提供元</param>
        /// <param name="dbPolly">DB接続用のPolly</param>
        /// <param name="appSettings">アプリケーション設定</param>
        public DtDeviceFileRepository(ILogger<DtDeviceFileRepository> logger, ITimeProvider timePrivder, DBPolly dbPolly, AppSettings appSettings)
        {
            Assert.IfNull(logger);
            Assert.IfNull(timePrivder);
            Assert.IfNull(dbPolly);
            Assert.IfNull(appSettings);
            _logger = logger;
            _timePrivder = timePrivder;
            _dbPolly = dbPolly;
            _appSettings = appSettings;
        }

        /// <summary>
        /// 引数に指定したDtDeviceFileをDT_DEVICE_FILEテーブルへ登録する
        /// 既にレコードが存在する場合は登録ではなく当該レコードの更新処理を行う
        /// また当該レコードに紐づいたDT_DEVICE_FILE_ATTRIBUTEテーブルの更新を行う
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <returns>処理結果</returns>
        public DtDeviceFile CreateOrUpdateDtDeviceFile(DtDeviceFile inData)
        {
            DtDeviceFile model = null;

            // 更新日時
            // DtDeviceFileAttribute更新時に明示的に日時データを渡す必要があるため、
            // レコード作成および更新日時はすべて明示的に設定する
            var now = _timePrivder.UtcNow;

            try
            {
                _logger.EnterJson("{0}", inData);

                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    using (var tran = db.Database.BeginTransaction())
                    {
                        string container = inData.Container;
                        string filePath = inData.FilePath;

                        var deviceFileModel = db.DtDeviceFile.FirstOrDefault(
                            x => x.Container.Equals(container, StringComparison.OrdinalIgnoreCase) 
                                && x.FilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase));

                        DBAccessor.Models.DtDeviceFile entity;

                        if (deviceFileModel == null || deviceFileModel.Sid == 0)
                        {
                            inData.CreateDatetime = now;
                            inData.UpdateDatetime = now;
                            entity = new DBAccessor.Models.DtDeviceFile(inData);

                            // コンテナ名とファイルパスをキーにして見つからなければレコード追加
                            entity = db.DtDeviceFile.Add(entity).Entity;
                        }
                        else
                        {
                            // 端末ファイル更新
                            deviceFileModel.SourceEquipmentUid = inData.SourceEquipmentUid;
                            deviceFileModel.CollectDatetime = inData.CollectDatetime;
                            deviceFileModel.UpdateDatetime = now;

                            entity = deviceFileModel;
 
                            // コンテナ名とファイルパスをキーにしてレコードが見つかった場合は、
                            // 端末ファイルデータおよび紐づく端末ファイル属性レコードの更新を行う
                            // 端末ファイル属性については、以下の処理を行う
                            // ・同じNameを持つレコードが存在しない場合はレコード追加
                            // ・同じNameを持つレコードが存在する場合はレコードを更新
                            // ・既に存在するレコードのNameが追加するレコードの中に存在しない場合は、当該レコードを削除

                            // 端末ファイル属性
                            var inDeviceFileAttributeModel = inData.DtDeviceFileAttribute;
                            var actualDeviceFileAttributeModel = db.DtDeviceFileAttribute.Where(x => x.DeviceFileSid == deviceFileModel.Sid);

                            // 端末ファイル属性テーブルに存在するが、追加するレコードの中に存在しないものは削除する
                            foreach (var attr in actualDeviceFileAttributeModel)
                            {
                                var inAttrData = inDeviceFileAttributeModel.FirstOrDefault(x => x.Name.Equals(attr.Name, StringComparison.OrdinalIgnoreCase));
                                if (inAttrData == null)
                                {
                                    db.DtDeviceFileAttribute.Remove(attr);
                                }
                            }

                            // 端末ファイル属性テーブルに存在しないものは追加、存在するものは更新
                            foreach (var attr in inDeviceFileAttributeModel)
                            {
                                var found = actualDeviceFileAttributeModel.FirstOrDefault(x => x.Name.Equals(attr.Name, StringComparison.OrdinalIgnoreCase));
                                DBAccessor.Models.DtDeviceFileAttribute deviceFileAttributeEntity;

                                if (found == null || found.Sid == 0)
                                {
                                    attr.DeviceFileSid = deviceFileModel.Sid;
                                    attr.CreateDatetime = now;
                                    attr.UpdateDatetime = now;
                                    deviceFileAttributeEntity = new DBAccessor.Models.DtDeviceFileAttribute(attr);

                                    // レコード追加
                                    _ = db.DtDeviceFileAttribute.Add(deviceFileAttributeEntity).Entity;
                                }
                                else
                                {
                                    // レコード更新
                                    found.Value = attr.Value;
                                    found.UpdateDatetime = now;
                                }
                            }
                        }

                        db.SaveChanges();
                        model = entity.ToModel();

                        // トランザクション終了
                        tran.Commit();
                    }
                });

                return model;
            }
            catch (ValidationException e)
            {
                throw new RmsParameterException(e.ValidationResult.ErrorMessage, e);
            }
            catch (Exception e)
            {
                throw new RmsException("DT_DEVICE_FILEテーブルへのInsertに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// DT_DEVICE_FILEテーブルからDtDeviceFileを取得する
        /// </summary>
        /// <param name="sid">取得するデータのSID</param>
        /// <returns>取得したデータ</returns>
        public DtDeviceFile ReadDtDeviceFile(long sid)
        {
            DtDeviceFile model = null;
            try
            {
                _logger.Enter($"{nameof(sid)}={sid}");

                DBAccessor.Models.DtDeviceFile entity = null;
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        entity = db.DtDeviceFile.Include(x => x.DeviceS).Include(x => x.DtDeviceFileAttribute).FirstOrDefault(x => x.Sid == sid);
                    }
                });

                if (entity != null)
                {
                    model = entity.ToModel();
                }

                return model;
            }
            catch (ValidationException e)
            {
                throw new RmsParameterException(e.ValidationResult.ErrorMessage, e);
            }
            catch (Exception e)
            {
                throw new RmsException("DT_DELIVERY_FILEテーブルのSelectに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// 引数に指定したパスに、ファイルパスが先頭一致するDtDeviceFileを取得する
        /// </summary>
        /// <param name="containerName">コンテナ名</param>
        /// <param name="path">パス。指定したパスに先頭一致するDtDeviceFileを取得する。</param>
        /// <param name="endDateTime">期間(終了)。指定した日時より過去のDtDeviceFileを取得する。</param>
        /// <returns>DtDeviceFileのリスト</returns>
        public IEnumerable<DtDeviceFile> FindByFilePathStartingWithAndUpdateDatetimeLessThan(string containerName, string path, DateTime endDateTime)
        {
            IEnumerable<DtDeviceFile> model = null;
            try
            {
                _logger.EnterJson("{0}", new { containerName, path, endDateTime });

                List<DBAccessor.Models.DtDeviceFile> entities = null;
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        IQueryable<DBAccessor.Models.DtDeviceFile> query = db.DtDeviceFile;

                        // フィルター時にコンテナ名の大文字小文字は区別しない
                        query = query.Where(x => x.Container.Equals(containerName, StringComparison.OrdinalIgnoreCase));
                        if (!string.IsNullOrEmpty(path))
                        {
                            // パスの大文字小文字は区別しない
                            query = query.Where(x => x.FilePath.StartsWith(path, StringComparison.OrdinalIgnoreCase));
                        }

                        query = query.Where(x => x.UpdateDatetime != null && x.UpdateDatetime < endDateTime);
                        entities = query.ToList();
                    }
                });

                if (entities != null && entities.Count > 0)
                {
                    model = entities.Select(x => x.ToModel());
                }

                return model;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_DEVICE_FILEテーブルのSelectに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// DT_DEVICE_FILEテーブルからDtDeviceFileを削除する
        /// </summary>
        /// <param name="sid">削除するデータのSID</param>
        /// <returns>削除したデータ</returns>
        public DtDeviceFile DeleteDtDeviceFile(long sid)
        {
            DtDeviceFile model = null;
            try
            {
                _logger.Enter($"{nameof(sid)}={sid}");

                DBAccessor.Models.DtDeviceFile entity = new DBAccessor.Models.DtDeviceFile() { Sid = sid };
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        db.DtDeviceFile.Attach(entity);
                        db.DtDeviceFile.Remove(entity);

                        if (db.SaveChanges() > 0)
                        {
                            model = entity.ToModel();
                        }
                    }
                });

                return model;
            }
            catch (ValidationException e)
            {
                throw new RmsParameterException(e.ValidationResult.ErrorMessage, e);
            }
            catch (Exception e)
            {
                throw new RmsException("DT_DEVICE_FILEテーブルのDeleteに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }
    }
}
