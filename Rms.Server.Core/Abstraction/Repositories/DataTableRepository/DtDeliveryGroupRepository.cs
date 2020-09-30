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
using System.Data.SqlClient;
using System.Linq;

namespace Rms.Server.Core.Abstraction.Repositories
{
    public partial class DtDeliveryGroupRepository : IDtDeliveryGroupRepository
    {
        /// <summary>SQLパラメータ設定用キー</summary>
        private static readonly string SqlKeyForDeviceSid = "@DeviceSid";

        /// <summary>
        /// デバイス接続イベント時に、指定した最上位（ゲートウェイ）機器IDを持ち、
        /// 配信グループステータスが開始済みかつ
        /// ダウンロードが完了していない（適用結果ステータスが"notstarted"または"messagesent"）
        /// 配信グループのレコードを取得するためのSQL文
        /// </summary>
        /// <remarks>
        /// 適用結果テーブルには端末SIDと配信結果SIDが同一のレコードが複数蓄積される。
        /// 収集日時データを比較することで、最新のレコードを抽出することが前提となる。
        /// </remarks>
        private static readonly string SqlCommandToGetNotCompletedDownload =
            "WITH " +
                "DT_INSTALL_RESULT_LATEST AS (" +
                    "SELECT " +
                        "* " +
                    "FROM " +
                        "core.DT_INSTALL_RESULT tbl1 " +
                    "WHERE " +
                        "tbl1.SID = (" +
                            "SELECT " +
                                "TOP 1 tbl2.SID " +
                                "FROM " +
                                    "core.DT_INSTALL_RESULT tbl2 " +
                                "WHERE " +
                                    "tbl1.DEVICE_SID = tbl2.DEVICE_SID " +
                                "AND tbl1.DELIVERY_RESULT_SID = tbl2.DELIVERY_RESULT_SID " +
                                "ORDER BY COLLECT_DATETIME DESC, SID DESC" +
                        ")" +
                ") " +
            "SELECT " +
                "deri_group.* " +
            "FROM " +
                "core.DT_DELIVERY_GROUP deri_group " +
            "INNER JOIN " +
                "core.MT_DELIVERY_GROUP_STATUS deri_status " +
            "ON " +
                "deri_status.SID = deri_group.DELIVERY_GROUP_STATUS_SID " +
            "WHERE " +
                "deri_status.CODE = 'started' " +
            "AND" +
                "EXISTS(" +
                    "SELECT DISTINCT " +
                        "ins_result.SID" +
                    "FROM " +
                        "DT_INSTALL_RESULT_LATEST ins_result " +
                    "INNER JOIN " +
                        "core.MT_INSTALL_RESULT_STATUS result_status " +
                    "ON " +
                        "ins_result.INSTALL_RESULT_STATUS_SID = result_status.SID " +
                    "INNER JOIN " +
                        "core.DT_DELIVERY_RESULT deri_result " +
                    "ON " +
                        "ins_result.DELIVERY_RESULT_SID = deri_result.SID " +
                    "WHERE " +
                        "deri_group.SID = deri_result.DELIVERY_GROUP_SID " +
                    "AND " +
                        "deri_result.GW_DEVICE_SID = " + SqlKeyForDeviceSid + 
                    "AND " +
                        "(result_status.CODE = 'notstarted' OR result_status.CODE = 'messagesent')" +
                ")";

        /// <summary>ロガー</summary>
        private readonly ILogger<DtDeliveryGroupRepository> _logger;

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
        public DtDeliveryGroupRepository(ILogger<DtDeliveryGroupRepository> logger, ITimeProvider timePrivder, DBPolly dbPolly, AppSettings appSettings)
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
        /// 引数に指定したDtDeliveryGroupをDT_DELIVERY_GROUPテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <returns>処理結果</returns>
        public DtDeliveryGroup CreateDtDeliveryGroup(DtDeliveryGroup inData)
        {
            DtDeliveryGroup model = null;
            try
            {
                _logger.EnterJson("{0}", inData);

                DBAccessor.Models.DtDeliveryGroup entity = new DBAccessor.Models.DtDeliveryGroup(inData);

                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        var statusNotStart = db.MtDeliveryGroupStatus.FirstOrDefault(x => x.Code.Equals(Const.DeliveryGroupStatus.NotStarted));
                        if (statusNotStart == null)
                        {
                            return;
                        }

                        entity.DeliveryGroupStatusSid = statusNotStart.Sid;
                        var dbdata = db.DtDeliveryGroup.Add(entity).Entity;
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
                throw new RmsException("DT_DELIVERY_GROUPテーブルへのInsertに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// DT_DELIVERY_GROUPテーブルからDtDeliveryGroupを取得する
        /// </summary>
        /// <param name="sid">取得するデータのSID</param>
        /// <returns>取得したデータ</returns>
        public DtDeliveryGroup ReadDtDeliveryGroup(long sid)
        {
            DtDeliveryGroup model = null;
            try
            {
                _logger.Enter($"{nameof(sid)}={sid}");

                DBAccessor.Models.DtDeliveryGroup entity = null;
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        entity = db.DtDeliveryGroup.Include(x => x.DtDeliveryResult).FirstOrDefault(x => x.Sid == sid);
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
                throw new RmsException("DT_DELIVERY_GROUPテーブルのSelectに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// データの削除
        /// </summary>
        /// <param name="sid">削除するデータのSID</param>
        /// <param name="rowVersion">rowversion</param>
        /// <returns>削除したデータ</returns>
        public DtDeliveryGroup DeleteDtDeliveryGroup(long sid, byte[] rowVersion)
        {
            DtDeliveryGroup model = null;
            try
            {
                _logger.Enter($"{nameof(sid)}={sid} {nameof(rowVersion)}={rowVersion}");

                DBAccessor.Models.DtDeliveryGroup entity = new DBAccessor.Models.DtDeliveryGroup() { Sid = sid, RowVersion = rowVersion };
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        // 指定した配信ステータスSIDを検索し、未配信状態であるかチェックする
                        // 備考：該当する配信グループが0個（Whereの結果が空のリスト）の場合、Allは必ずtrueを返す
                        //       そのためすべての配信ステータスが未配信状態であると見なされ、isNotStart=trueとなることに注意
                        var isNotStart = db.DtDeliveryGroup
                            .Where(x => x.Sid == sid)
                            .Include(x => x.DeliveryGroupStatusS)
                            .All(x => x.DeliveryGroupStatusS.Code.Equals(Utility.Const.DeliveryGroupStatus.NotStarted));
                        if (!isNotStart)
                        {
                            throw new RmsCannotChangeDeliveredFileException(string.Format("配信前状態ではないDT_DELIVERY_GROUPテーブルのレコードをDeleteしようとしました。(DT_DELIVERY_GROUP.SID = {0})", sid));
                        }

                        // SIDが一致するレコードが存在しない場合はDeleteを実行しない
                        // (存在しないSIDに対してDeleteを実行してしまうと、DbUpdateConcurrencyExceptionが発生してしまい、
                        //  RowVersion競合とNot Foundとの区別ができないため)
                        if (db.DtDeliveryGroup.Any(x => x.Sid == sid))
                        {
                            db.DtDeliveryGroup.Attach(entity);
                            db.DtDeliveryGroup.Remove(entity);
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
                throw new RmsConflictException("DT_DELIVERY_GROUPテーブルのDeleteで競合が発生しました。", e);
            }
            catch (RmsException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_DELIVERY_GROUPテーブルのDeleteに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// データの更新(配信ステータスが「配信前」の時のみ)
        /// </summary>
        /// <param name="inData">更新データ</param>
        /// <returns>更新されたDB内容</returns>
        public DtDeliveryGroup UpdateDtDeliveryGroupIfDeliveryNotStart(DtDeliveryGroup inData)
        {
            DtDeliveryGroup model = null;
            try
            {
                _logger.EnterJson("inData: {0}", inData);

                _dbPolly.Execute(
                    () =>
                    {
                        model = UpdateIfNotStart(inData);
                    });
                return model;
            }
            catch (RmsException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_DELIVERY_GROUPテーブルのUpdateに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// 配信開始可能なデータを取得する
        /// </summary>
        /// <returns>配信開始可能なデータ群</returns>
        public IEnumerable<DtDeliveryGroup> ReadStartableDtDeliveryGroup()
        {
            IEnumerable<DtDeliveryGroup> models = new List<DtDeliveryGroup>();
            try
            {
                _logger.Enter();

                DateTime nowTime = _timePrivder.UtcNow;
                _dbPolly.Execute(
                    () =>
                    {
                        using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                        {
                            // 現在時刻を経過済みで、中止フラグOFFの配信前データ(配信結果付)を一括取得する
                            var entities = db.DtDeliveryGroup
                                .Include(x => x.DtDeliveryResult)
                                .Include(x => x.DeliveryGroupStatusS)
                                .Include(x => x.DeliveryFileS)
                                .Where(x => x.StartDatetime != null && x.StartDatetime <= nowTime)
                                .Where(x => x.DeliveryGroupStatusS.Code.Equals(Utility.Const.DeliveryGroupStatus.NotStarted))
                                .Where(x => x.DeliveryFileS.IsCanceled != null && x.DeliveryFileS.IsCanceled.Value == false);

                            if (entities != null)
                            {
                                models = entities.Select(x => x.ToModel()).ToList();
                            }
                        }
                    });
                return models;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_DELIVERY_GROUPテーブルのReadに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", models);
            }
        }

        /// <summary>
        /// 配信用に親・子エンティティデータをIncludeしたデータを取得する
        /// </summary>
        /// <param name="sid">配信グループSID</param>
        /// <returns>配信用に親・子エンティティデータをIncludeしたデータ</returns>
        public DtDeliveryGroup ReadDeliveryIncludedDtDeliveryGroup(long sid)
        {
            DtDeliveryGroup includedModel = null;
            try
            {
                _logger.Enter($"{nameof(sid)}: {sid}");

                _dbPolly.Execute(
                    () =>
                    {
                        using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                        {
                            // 配信用に親・子エンティティを一括Includeする
                            var includedEntitiy = db.DtDeliveryGroup
                                .Where(x => x.Sid == sid)
                                //// 端末
                                .Include(group => group.DtDeliveryResult)
                                    .ThenInclude(results => results.DeviceS)
                                        .ThenInclude(device => device.ConnectStatusS)
                                //// 最上位端末
                                .Include(group => group.DtDeliveryResult)
                                    .ThenInclude(results => results.GwDeviceS)
                                        .ThenInclude(device => device.ConnectStatusS)
                                //// 配信ファイル種別
                                .Include(group => group.DeliveryFileS)
                                    .ThenInclude(file => file.DeliveryFileTypeS)
                                //// インストールタイプ
                                .Include(group => group.DeliveryFileS)
                                    .ThenInclude(file => file.InstallTypeS)
                                //// 機器型式
                                .Include(group => group.DeliveryFileS)
                                    .ThenInclude(file => file.DtDeliveryModel)
                                        .ThenInclude(models => models.EquipmentModelS)
                                .FirstOrDefault();

                            if (includedEntitiy != null)
                            {
                                includedModel = includedEntitiy.ToModel();
                            }
                        }
                    });
                return includedModel;
            }
            catch (RmsException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_DELIVERY_GROUPテーブルのReadに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", includedModel);
            }
        }

        /// <summary>
        /// データの更新(配信ステータスを指定SIDのものにする)
        /// </summary>
        /// <param name="sid">更新する配信グループデータのSID</param>
        /// <returns>更新されたDB内容</returns>
        public DtDeliveryGroup UpdateDtDeliveryGroupStatusStarted(long sid)
        {
            DtDeliveryGroup model = null;
            try
            {
                _logger.Enter($"{nameof(sid)}: {sid}");

                _dbPolly.Execute(
                    () =>
                    {
                        model = UpdateStatusStarted(sid);
                    });
                return model;
            }
            catch (RmsException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_DELIVERY_GROUPテーブルのUpdateに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// 指定した最上位（ゲートウェイ）機器IDを持ち
        /// 配信グループステータスが開始済みかつ
        /// ダウンロードが完了していない（適用結果ステータスが"notstarted"または"messagesent"）
        /// 配信グループのリストを取得する
        /// </summary>
        /// <param name="gatewaySid">最上位（ゲートウェイ）機器SID</param>
        /// <returns>結果</returns>
        public List<DtDeliveryGroup> GetDevicesByGatewaySidNotCompletedDownload(long gatewaySid)
        {
            List<DtDeliveryGroup> models = new List<DtDeliveryGroup>();
            try
            {
                _logger.EnterJson("{0}", gatewaySid);

                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        var sqlParamForDeviceSid = new SqlParameter(SqlKeyForDeviceSid, gatewaySid);
                        var records = db.DtDeliveryGroup.FromSql(SqlCommandToGetNotCompletedDownload, sqlParamForDeviceSid);

                        foreach (var record in records)
                        {
                            models.Add(record.ToModel());
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
                throw new RmsException("DT_INSTALL_RESULTテーブルへのInsertに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", models);
            }
        }

        /// <summary>
        /// データの更新(配信ステータスを指定SIDのものにする)
        /// 実際の更新処理を行う
        /// </summary>
        /// <param name="sid">更新する配信グループデータのSID</param>
        /// <returns>更新されたDB内容</returns>
        private DtDeliveryGroup UpdateStatusStarted(long sid)
        {
            using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
            {
                try
                {
                    // 指定した配信グループSIDと一致する情報を取得する
                    var entity = db.DtDeliveryGroup.Include(x => x.DeliveryGroupStatusS).FirstOrDefault(x => x.Sid == sid);
                    if (entity == null)
                    {
                        // 値を設定せずにリターン
                        return null;
                    }

                    // 指定した配信グループが未配信状態であるかチェックする
                    var isNotStart = entity.DeliveryGroupStatusS.Code.Equals(Utility.Const.DeliveryGroupStatus.NotStarted);
                    if (!isNotStart)
                    {
                        throw new RmsCannotChangeDeliveredFileException(string.Format("配信前状態ではないDT_DELIVERY_GROUPテーブルのレコードをUpdateしようとしました。(DT_DELIVERY_GROUP.SID = {0})", sid));
                    }

                    // 「開始済」の配信グループステータスSIDを取得する
                    var startedData = db.MtDeliveryGroupStatus.Where(x => x.Code.Equals(Utility.Const.DeliveryGroupStatus.Started)).FirstOrDefault();
                    if (startedData == null)
                    {
                        // 値を設定せずにリターン
                        return null;
                    }

                    // 情報の更新
                    entity.DeliveryGroupStatusSid = startedData.Sid;

                    var p = db.DtDeliveryGroup.Update(entity);
                    db.SaveChanges(_timePrivder);

                    return p.Entity.ToModel();
                }
                catch (ValidationException e)
                {
                    throw new RmsParameterException(e.ValidationResult.ErrorMessage, e);
                }
                catch (DbUpdateConcurrencyException e)
                {
                    // RowVersion衝突が起きた
                    throw new RmsConflictException("DT_DELIVERY_GROUPテーブルのUpdateで競合が発生しました。", e);
                }
                catch
                {
                    // SqlExceptionであれば再試行される
                    throw;
                }
            }
        }

        /// <summary>
        /// データの更新(配信ステータスが「配信前」の時のみ)
        /// 実際の更新処理を行う
        /// </summary>
        /// <param name="inData">更新データ</param>
        /// <returns>更新されたDB内容</returns>
        private DtDeliveryGroup UpdateIfNotStart(DtDeliveryGroup inData)
        {
            using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
            {
                try
                {
                    // 指定した配信グループSIDと一致する情報を取得する
                    var entity = db.DtDeliveryGroup
                        .Include(x => x.DeliveryGroupStatusS)
                        .FirstOrDefault(x => x.Sid == inData.Sid);
                    if (entity == null)
                    {
                        return null;
                    }

                    // 指定した配信ステータスSIDを検索し、未配信状態であるかチェックする
                    var isNotStart = entity.DeliveryGroupStatusS.Code.Equals(Utility.Const.DeliveryGroupStatus.NotStarted);
                    if (!isNotStart)
                    {
                        throw new RmsCannotChangeDeliveredFileException(string.Format("配信前状態ではないDT_DELIVERY_GROUPテーブルのレコードをUpdateしようとしました。(DT_DELIVERY_GROUP.SID = {0})", inData.Sid));
                    }

                    // 情報の更新
                    entity.Name = inData.Name;
                    entity.StartDatetime = inData.StartDatetime;
                    entity.DownloadDelayTime = inData.DownloadDelayTime;

                    // コンテキスト管理のRowVersionを投入値に置き換えて、DBデータと比較させる(Attatch処理以外では必要)
                    db.Entry(entity).Property(e => e.RowVersion).OriginalValue = inData.RowVersion;

                    var p = db.DtDeliveryGroup.Update(entity);
                    db.SaveChanges(_timePrivder);

                    return p.Entity.ToModel();
                }
                catch (ValidationException e)
                {
                    throw new RmsParameterException(e.ValidationResult.ErrorMessage, e);
                }
                catch (DbUpdateConcurrencyException e)
                {
                    // RowVersion衝突が起きた
                    throw new RmsConflictException("DT_DELIVERY_GROUPテーブルのUpdateで競合が発生しました。", e);
                }
                catch
                {
                    // SqlExceptionであれば再試行される
                    throw;
                }
            }
        }
    }
}
