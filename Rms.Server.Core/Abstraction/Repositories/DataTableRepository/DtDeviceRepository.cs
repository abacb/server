using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Core.Utility.Extensions;
using Rms.Server.Core.Utility.Models.Dispatch;
using Rms.Server.Core.Utility.Models.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// DT_DEVICEテーブルのリポジトリ
    /// </summary>
    public partial class DtDeviceRepository : IDtDeviceRepository
    {
        /// <summary>ロガー</summary>
        private readonly ILogger<DtDeviceRepository> _logger;

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
        public DtDeviceRepository(ILogger<DtDeviceRepository> logger, ITimeProvider timePrivder, DBPolly dbPolly, AppSettings appSettings)
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
        /// 引数に指定したDtDeviceをDT_DEVICEテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <returns>処理結果</returns>
        public DtDevice CreateDtDevice(DtDevice inData)
        {
            DtDevice model = null;
            try
            {
                _logger.EnterJson("{0}", inData);

                DBAccessor.Models.DtDevice entity = new DBAccessor.Models.DtDevice(inData);

                // 初期値GUIDはIDを割り振る
                if (entity.EdgeId == Guid.Empty)
                {
                    entity.EdgeId = Guid.NewGuid();
                }

                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        // 接続ステータスが「unconnected」のデータを取得する
                        var unconnectedData = db.MtConnectStatus
                            .Where(x => x.Code.Equals(Const.ConnectStatus.Unconnected))
                            .FirstOrDefault();
                        if (unconnectedData == null)
                        {
                            // データ設定せずに返却
                            return;
                        }

                        // 接続ステータス設定
                        entity.ConnectStatusSid = unconnectedData.Sid;

                        var dbdata = db.DtDevice.Add(entity).Entity;
                        db.SaveChanges(_timePrivder);
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
                throw new RmsException("DT_DEVICEテーブルへのInsertに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// DT_DEVICEテーブルからDtDeviceを取得する
        /// </summary>
        /// <param name="sid">取得するデータのSID</param>
        /// <returns>取得したデータ</returns>
        public DtDevice ReadDtDevice(long sid)
        {
            DtDevice model = null;
            try
            {
                _logger.Enter($"{nameof(sid)}={sid}");

                DBAccessor.Models.DtDevice entity = null;
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        entity = db.DtDevice.FirstOrDefault(x => x.Sid == sid);
                    }
                });

                if (entity != null)
                {
                    model = entity.ToModel();
                }

                return model;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_DEVICEテーブルのSelectに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// DT_DEVICEテーブルからDtDeviceを取得する
        /// </summary>
        /// <param name="equipmentUidOrEdgeId">取得するデータの機器UIDまたはエッジID</param>
        /// <returns>取得したデータ</returns>
        public DtDevice ReadDtDevice(string equipmentUidOrEdgeId)
        {
            DtDevice model = null;
            try
            {
                _logger.Enter($"{nameof(equipmentUidOrEdgeId)}={equipmentUidOrEdgeId}");

                DBAccessor.Models.DtDevice entity = null;
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        entity = db.DtDevice.FirstOrDefault(x => x.EquipmentUid == equipmentUidOrEdgeId);
                        if (entity == null && Guid.TryParse(equipmentUidOrEdgeId, out Guid edgeId))
                        {
                            entity = db.DtDevice.FirstOrDefault(x => x.EdgeId == edgeId);
                        }
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
                throw new RmsException("DT_DEVICEテーブルのSelectに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// DT_DEVICEテーブルからDtDeviceを取得する
        /// </summary>
        /// <param name="edgeId">取得するデータのエッジID</param>
        /// <returns>取得したデータ</returns>
        public DtDevice ReadDtDevice(Guid edgeId)
        {
            DtDevice model = null;
            try
            {
                _logger.Enter($"{nameof(edgeId)}={edgeId}");

                DBAccessor.Models.DtDevice entity = null;

                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        entity = db.DtDevice.FirstOrDefault(x => x.EdgeId == edgeId);
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
                throw new RmsException("DT_DEVICEテーブルのSelectに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// DT_DEVICEテーブルからオンラインなゲートウェイ機器のDtDeviceを取得する
        /// </summary>
        /// <param name="groupData">配信グループ</param>
        /// <returns>オンラインなゲートウェイ機器のDtDeviceデータリスト</returns>
        public IEnumerable<DtDevice> ReadDtDeviceOnlineGateway(DtDeliveryGroup groupData)
        {
            Assert.IfNull(groupData);
            Assert.IfNull(groupData.DtDeliveryResult);

            IEnumerable<DtDevice> models = new List<DtDevice>();
            try
            {
                _logger.EnterJson($"{nameof(groupData)}={0}", groupData);

                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        // ゲートウェイ機器でオンラインなデータを取得する
                        var gwSids = groupData.DtDeliveryResult
                            .Where(x => x.GwDeviceSid == x.DeviceSid)
                            .Select(x => x.GwDeviceSid);
                        var entities = db.DtDevice.Include(x => x.ConnectStatusS)
                               .Where(x => gwSids.Contains(x.Sid))
                               .Where(x => x.ConnectStatusS.Code.Equals(Utility.Const.ConnectStatus.Connected));

                        if (entities != null)
                        {
                            models = entities.Select(x => x.ToModel()).ToList();
                        }
                    }
                });

                return models;
            }
            catch (ValidationException e)
            {
                throw new RmsParameterException(e.ValidationResult.ErrorMessage, e);
            }
            catch (Exception e)
            {
                throw new RmsException("DT_DEVICEテーブルのSelectに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", models);
            }
        }

        /// <summary>
        /// 引数に指定したDtDeviceでDT_DEVICEテーブルを更新する
        /// </summary>
        /// <param name="inData">更新するデータ</param>
        /// <returns>更新したデータ</returns>
        public DtDevice UpdateDtDevice(DtDevice inData)
        {
            DtDevice model = null;
            try
            {
                _logger.EnterJson("{0}", inData);

                DBAccessor.Models.DtDevice entity = new DBAccessor.Models.DtDevice(inData);

                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        // 指定SIDのデータがDBになければnullリターン
                        if (db.DtDevice.AsNoTracking().FirstOrDefault(x => x.Sid == inData.Sid) == null)
                        {
                            return;
                        }

                        db.DtDevice.Attach(entity);

                        // 全フィールドを更新する
                        //     特定フィールドだけUpdateする場合は下記のように記述してください
                        //     db.Entry(entity).Property(x => x.UpdateDatetime).IsModified = true;
                        db.Entry(entity).Property(x => x.InstallTypeSid).IsModified = true;
                        db.Entry(entity).Property(x => x.EquipmentModelSid).IsModified = true;

                        if (db.SaveChanges(_timePrivder) > 0)
                        {
                            // 呼び出し側に更新済みデータを返却する
                            model = db.DtDevice.AsNoTracking().FirstOrDefault(x => x.Sid == inData.Sid)?.ToModel();
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
                throw new RmsException("DT_DEVICEテーブルのUpdateに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// DT_DEVICEテーブルからDtDeviceを削除する
        /// </summary>
        /// <param name="sid">削除するデータのSID</param>
        /// <param name="equipmentUid">削除するデータのデバイスUID</param>
        /// <param name="remoteConnectUid">削除するデータのリモート接続UID</param>
        /// <returns>削除したデータ</returns>
        public DtDevice DeleteDtDevice(long sid, string equipmentUid, string remoteConnectUid)
        {
            DtDevice model = null;
            try
            {
                _logger.Enter($"{nameof(sid)}={sid} {nameof(equipmentUid)}={equipmentUid} {nameof(remoteConnectUid)}={remoteConnectUid}");

                DBAccessor.Models.DtDevice entity = new DBAccessor.Models.DtDevice() { Sid = sid, EquipmentUid = equipmentUid, RemoteConnectUid = remoteConnectUid };
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        db.DtDevice.Attach(entity);
                        db.DtDevice.Remove(entity);

                        if (db.SaveChanges(_timePrivder) > 0)
                        {
                            model = entity.ToModel();
                        }
                    }
                });

                return model;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_DEVICEテーブルのDeleteに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// 端末テーブルの接続ステータスを更新する
        /// </summary>
        /// <param name="sid">端末SID</param>
        /// <param name="connectionEventTimeInfo">接続/切断イベント時刻情報</param>
        /// <returns>更新したデータ。ステータスに変更がなかった場合にはnullを返す</returns>
        public DtDevice UpdateDeviceConnectionStatus(long sid, ConnectionEventTimeInfo connectionEventTimeInfo)
        {
            DtDevice model = null;
            try
            {
                _logger.EnterJson("{0}", sid);
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        // 該当するステータスSIDをマスタテーブルから取得する
                        var record = db.MtConnectStatus.FirstOrDefault(x => x.Code == connectionEventTimeInfo.Status);
                        if (record == null || record.Sid == 0)
                        {
                            throw new RmsException("接続ステータスマスターテーブルに該当するステータスが存在しませんでした。");
                        }

                        // ステータスSID, 接続開始日時, 接続更新日時
                        DtDevice inData = new DtDevice()
                        {
                            Sid = sid,
                            ConnectStatusSid = record.Sid,
                            ConnectStartDatetime = connectionEventTimeInfo.EventTime,
                            ConnectUpdateDatetime = connectionEventTimeInfo.EventTime
                        };

                        DBAccessor.Models.DtDevice entity = new DBAccessor.Models.DtDevice(inData);
                        db.DtDevice.Attach(entity);

                        // 接続ステータスが更新されるのでステータスSIDと接続更新日時は更新対象
                        db.Entry(entity).Property(x => x.ConnectStatusSid).IsModified = true;
                        db.Entry(entity).Property(x => x.ConnectUpdateDatetime).IsModified = true;

                        // 初回接続フラグがtrueのときは接続開始日時も更新対象
                        if (connectionEventTimeInfo.IsFirstConnection)
                        {
                            db.Entry(entity).Property(x => x.ConnectStartDatetime).IsModified = true;
                        }

                        if (db.SaveChanges(_timePrivder) > 0)
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
                throw new RmsException("DT_DEVICEテーブルのUpdateに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// デバイスツイン更新イベントを受信して端末テーブルの「リモート接続UID」と「RMSソフトバージョン」を更新する
        /// </summary>
        /// <param name="sid">端末SID</param>
        /// <param name="data">更新データ</param>
        /// <returns>更新したデータ。テーブルが更新されなかった場合にはnullを返す</returns>
        public DtDevice UpdateDeviceInfoByTwinChanged(long sid, DtTwinChanged data)
        {
            DtDevice model = null;
            try
            {
                _logger.EnterJson("{0}", sid);
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        string remoteConnectionUid = data.RemoteConnectionUid;
                        string rmsSoftVersion = data.SoftVersion;

                        // リモート接続UIDとRMSソフトバージョンを更新する
                        DtDevice inData = new DtDevice() { Sid = sid, RemoteConnectUid = remoteConnectionUid, RmsSoftVersion = rmsSoftVersion };
                        DBAccessor.Models.DtDevice entity = new DBAccessor.Models.DtDevice(inData);

                        db.DtDevice.Attach(entity);

                        // リモート接続UIDとRMSソフトバージョンを更新する
                        db.Entry(entity).Property(x => x.RemoteConnectUid).IsModified = true;
                        db.Entry(entity).Property(x => x.RmsSoftVersion).IsModified = true;

                        if (db.SaveChanges(_timePrivder) > 0)
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
                throw new RmsException("DT_DEVICEテーブルのUpdateに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }
    }
}
