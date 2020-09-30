using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Core.Utility.Extensions;
using Rms.Server.Core.Utility.Models.Entites;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Rms.Server.Core.Abstraction.Repositories
{
    public partial class DtDeliveryFileRepository : IDtDeliveryFileRepository
    {
        /// <summary>ロガー</summary>
        private readonly ILogger<DtDeliveryFileRepository> _logger;

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
        public DtDeliveryFileRepository(ILogger<DtDeliveryFileRepository> logger, ITimeProvider timePrivder, DBPolly dbPolly, AppSettings appSettings)
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
        /// 更新処理オプション
        /// </summary>
        private enum UpdateOption
        {
            /// <summary>オプションなし</summary>
            None,

            /// <summary>配信ステータスが配信前の場合だけ更新する</summary>
            OnlyNotStart,
        }

        /// <summary>
        /// 引数に指定したDtDeliveryFileをDT_DELIVERY_FILEテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <returns>処理結果</returns>
        public DtDeliveryFile CreateDtDeliveryFile(DtDeliveryFile inData)
        {
            DtDeliveryFile model = null;
            try
            {
                _logger.EnterJson("{0}", inData);

                DBAccessor.Models.DtDeliveryFile entity = new DBAccessor.Models.DtDeliveryFile(inData);

                _dbPolly.Execute(() =>
                {
                    DateTime createdAt = _timePrivder.UtcNow;

                    entity.CreateDatetime = createdAt;
                    entity.UpdateDatetime = createdAt;
                    foreach (DBAccessor.Models.DtDeliveryModel child in entity.DtDeliveryModel)
                    {
                        child.CreateDatetime = createdAt;
                    }

                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        var dbdata = db.DtDeliveryFile.Add(entity).Entity;
                        db.SaveChanges();
                        model = dbdata.ToModel();
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
                throw new RmsException("DT_DELIVERY_FILEテーブルへのInsertに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// DT_DELIVERY_FILEテーブルからDtDeliveryFileを取得する
        /// </summary>
        /// <param name="sid">取得するデータのSID</param>
        /// <returns>取得したデータ</returns>
        public DtDeliveryFile ReadDtDeliveryFile(long sid)
        {
            DtDeliveryFile model = null;
            try
            {
                _logger.Enter($"{nameof(sid)}={sid}");

                DBAccessor.Models.DtDeliveryFile entity = null;
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        entity = db.DtDeliveryFile
                            .Include(x => x.DeliveryFileTypeS)
                            .Include(x => x.DtDeliveryModel)
                            .FirstOrDefault(x => x.Sid == sid);
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
        /// 配信ファイルに紐づいた配信グループが配信開始前であれば、
        /// 引数に指定したDtDeliveryFileでDT_DELIVERY_FILEテーブルを更新する
        /// </summary>
        /// <param name="inData">追加するデータ</param>
        /// <returns>ResultCode付きの追加したデータ</returns>
        public DtDeliveryFile UpdateDtDeliveryFileIfNoGroupStarted(DtDeliveryFile inData)
        {
            DtDeliveryFile model = null;
            try
            {
                _logger.EnterJson("inData: {0}", inData);

                Func<DBAccessor.Models.RmsDbContext, DBAccessor.Models.DtDeliveryFile, DtDeliveryFile, DBAccessor.Models.DtDeliveryFile> updater = (db, entity, input) =>
                {
                    // 子エンティティの削除。
                    var deliveryModels = db.DtDeliveryModel.Where(x => x.DeliveryFileSid == inData.Sid);
                    db.DtDeliveryModel.RemoveRange(deliveryModels);

                    entity.DeliveryFileTypeSid = input.DeliveryFileTypeSid;
                    entity.InstallTypeSid = input.InstallTypeSid;
                    entity.Version = input.Version;
                    entity.InstallableVersion = input.InstallableVersion;
                    entity.Description = input.Description;
                    entity.InformationId = input.InformationId;

                    var updatedAt = _timePrivder.UtcNow;
                    entity.UpdateDatetime = updatedAt;
                    if (input.DtDeliveryModel == null)
                    {
                        return entity;
                    }

                    foreach (var addDeliveryModel in input.DtDeliveryModel)
                    {
                        entity.DtDeliveryModel.Add(new DBAccessor.Models.DtDeliveryModel(addDeliveryModel));
                    }

                    foreach (var child in entity.DtDeliveryModel)
                    {
                        child.CreateDatetime = updatedAt;
                    }

                    return entity;
                };

                _dbPolly.Execute(
                    () =>
                    {
                        model = Update(inData, (x, y) => false, updater, UpdateOption.OnlyNotStart);
                    });
                return model;
            }
            catch (RmsException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_DELIVERY_FILEテーブルとDT_DELIVERY_MODELテーブルのUpdateに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// 中止フラグを更新する
        /// </summary>
        /// <param name="inData">追加するデータ</param>
        /// <returns>ResultCode付きの追加したデータ</returns>
        public DtDeliveryFile UpdateCancelFlag(DtDeliveryFile inData)
        {
            DtDeliveryFile model = null;
            try
            {
                _logger.EnterJson("inData: {0}", inData);

                Func<DBAccessor.Models.DtDeliveryFile, DtDeliveryFile, bool> isSame = (entity, input) =>
                {
                    return entity.IsCanceled == input.IsCanceled;
                };

                Func<DBAccessor.Models.RmsDbContext, DBAccessor.Models.DtDeliveryFile, DtDeliveryFile, DBAccessor.Models.DtDeliveryFile> updater = (db, entity, input) =>
                {
                    entity.IsCanceled = input.IsCanceled;
                    entity.UpdateDatetime = _timePrivder.UtcNow;
                    return entity;
                };

                _dbPolly.Execute(
                    () =>
                    {
                        model = Update(inData, isSame, updater, UpdateOption.None);
                    });
                return model;
            }
            catch (RmsException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_DELIVERY_FILEテーブルのUpdateに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// DT_DELIVERY_FILEテーブルからDtDeliveryFileを削除する
        /// </summary>
        /// <param name="sid">削除するデータのSID</param>
        /// <param name="rowVersion">rowversion</param>
        /// <returns>削除したデータ</returns>
        public DtDeliveryFile DeleteDtDeliveryFile(long sid, byte[] rowVersion)
        {
            DtDeliveryFile model = null;
            try
            {
                _logger.Enter($"{nameof(sid)}={sid} {nameof(rowVersion)}={rowVersion}");

                DBAccessor.Models.DtDeliveryFile entity = new DBAccessor.Models.DtDeliveryFile() { Sid = sid, RowVersion = rowVersion };
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        // 指定した配信ファイルSIDを持つ配信グループを検索し、そのすべての配信ステータスが未配信状態であるかチェックする
                        // 備考：配信ファイルSIDを持つ配信グループが0個（Whereの結果が空のリスト）の場合、Allは必ずtrueを返す
                        //       そのためすべての配信ステータスが未配信状態であると見なされ、isAllNotStarted=trueとなることに注意
                        var isAllNotStarted = db.DtDeliveryGroup
                            .Where(x => x.DeliveryFileSid == sid)
                            .Include(x => x.DeliveryGroupStatusS)
                            .All(x => x.DeliveryGroupStatusS.Code.Equals(Utility.Const.DeliveryGroupStatus.NotStarted));
                        if (!isAllNotStarted)
                        {
                            throw new RmsCannotChangeDeliveredFileException(string.Format("配信前状態ではないDT_DELIVERY_FILEテーブルのレコードをDeleteしようとしました。(DT_DELIVERY_FILE.SID = {0})", sid));
                        }

                        // SIDが一致するレコードが存在しない場合はDeleteを実行しない
                        // (存在しないSIDに対してDeleteを実行してしまうと、DbUpdateConcurrencyExceptionが発生してしまい、
                        //  RowVersion競合とNotFoundとの区別ができないため)
                        if (db.DtDeliveryFile.Any(x => x.Sid == sid))
                        {
                            db.DtDeliveryFile.Attach(entity);
                            db.DtDeliveryFile.Remove(entity);

                            if (db.SaveChanges() > 0)
                            {
                                model = entity.ToModel();
                            }
                        }
                    }
                });

                return model;
            }
            catch (DbUpdateConcurrencyException e)
            {
                // RowVersion衝突が起きた
                throw new RmsConflictException("DT_DELIVERY_FILEテーブルのDeleteで競合が発生しました。", e);
            }
            catch (RmsException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_DELIVERY_FILEテーブルのDeleteに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// データの更新(配信ステータスが「配信前」の時のみ)
        /// 実際の更新処理を行う
        /// </summary>
        /// <param name="inData">更新データ</param>
        /// <param name="isSame">更新要否の判定メソッド</param>
        /// <param name="updater">更新メソッド</param>
        /// <param name="option">更新処理オプション</param>
        /// <returns>更新されたDB内容</returns>
        private DtDeliveryFile Update(
            DtDeliveryFile inData,
            Func<DBAccessor.Models.DtDeliveryFile, DtDeliveryFile, bool> isSame,
            Func<DBAccessor.Models.RmsDbContext, DBAccessor.Models.DtDeliveryFile, DtDeliveryFile, DBAccessor.Models.DtDeliveryFile> updater,
            UpdateOption option)
        {
            // 複数回のSQL実行を競合なく実行・ロールバックさせるためにトランザクションを用いる
            using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    var entity = db.DtDeliveryFile
                        .Include(x => x.DtDeliveryModel)
                        .FirstOrDefault(x => x.Sid == inData.Sid);
                    if (entity == null)
                    {
                        return null;
                    }

                    // isSame()の結果同一と判定されたら更新不要のため処理終了
                    if (isSame(entity, inData))
                    {
                        return entity.ToModel();
                    }

                    if (option == UpdateOption.OnlyNotStart)
                    {
                        // 指定した配信ファイルSIDを持つ配信グループを検索し、そのすべての配信ステータスが未配信状態であるかチェックする
                        var isAllNotStarted = db.DtDeliveryGroup
                            .Where(x => x.DeliveryFileSid == inData.Sid)
                            .Include(x => x.DeliveryGroupStatusS)
                            .All(x => x.DeliveryGroupStatusS.Code.Equals(Utility.Const.DeliveryGroupStatus.NotStarted));
                        if (!isAllNotStarted)
                        {
                            throw new RmsCannotChangeDeliveredFileException(string.Format("配信前状態ではないDT_DELIVERY_FILEテーブルのレコードをUpdateしようとしました。(DT_DELIVERY_FILE.SID = {0})", inData.Sid));
                        }
                    }

                    // 情報の更新
                    entity = updater(db, entity, inData);

                    // コンテキスト管理のRowVersionを投入値に置き換えて、DBデータと比較させる(Attatch処理以外では必要)
                    db.Entry(entity).Property(e => e.RowVersion).OriginalValue = inData.RowVersion;

                    var p = db.DtDeliveryFile.Update(entity);

                    db.SaveChanges();

                    // トランザクション終了
                    tran.Commit();

                    return p.Entity.ToModel();
                }
                catch (ValidationException e)
                {
                    throw new RmsParameterException(e.ValidationResult.ErrorMessage, e);
                }
                catch (DbUpdateConcurrencyException e)
                {
                    // RowVersion衝突が起きた
                    tran.Rollback();
                    throw new RmsConflictException("DT_DELIVERY_FILEテーブルのUpdateで競合が発生しました。", e);
                }
                catch
                {
                    // SqlExceptionであれば再試行される
                    tran.Rollback();
                    throw;
                }
            }
        }
    }
}
