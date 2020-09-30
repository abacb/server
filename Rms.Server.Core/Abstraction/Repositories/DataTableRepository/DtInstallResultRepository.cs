using Microsoft.Data.SqlClient;
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
using static Rms.Server.Core.Utility.Const;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// DT_INSTALL_RESULTテーブルのリポジトリ
    /// </summary>
    public partial class DtInstallResultRepository : IDtInstallResultRepository
    {
        /// <summary>SQLパラメータ設定用キー</summary>
        private static readonly string SqlKeyForSid = "@sid";

        /// <summary>SQLパラメータ設定用キー</summary>
        private static readonly string SqlKeyForGroupStatusSid = "@groupStatusSid";

        /// <summary>
        /// メッセージが所属する配信グループと同一の「適用結果データ」の「適用結果ステータス」が
        /// すべて「未開始」「メッセージ送信済み」以外となっている場合、
        /// 「配信グループデータ」テーブルの「配信グループデータステータス」を「完了」にするSQL文
        /// </summary>
        private static readonly string SqlCommandToUpdateDeliveryGroupStatus
            = @"WITH DT_INSTALL_RESULT_LATEST AS ("
                + "SELECT * FROM core.DT_INSTALL_RESULT tbl1 "
                + "WHERE tbl1.SID = (SELECT TOP 1 tbl2.SID FROM core.DT_INSTALL_RESULT tbl2 WHERE tbl1.DEVICE_SID = tbl2.DEVICE_SID ORDER BY COLLECT_DATETIME DESC, SID DESC)"
            + ")"
            + "UPDATE deri_group "
            + "SET deri_group.DELIVERY_GROUP_STATUS_SID = @groupStatusSid "
            + "FROM core.DT_DELIVERY_GROUP deri_group "
            + "WHERE "
                + "deri_group.SID = @sid "
                + "AND deri_group.DELIVERY_GROUP_STATUS_SID <> @groupStatusSid "
                + "AND NOT EXISTS("
                    + "SELECT DISTINCT ins_result.SID "
                    + "FROM DT_INSTALL_RESULT_LATEST ins_result "
                    + "INNER JOIN core.MT_INSTALL_RESULT_STATUS result_status "
                    + "ON ins_result.INSTALL_RESULT_STATUS_SID = result_status.SID "
                    + "INNER JOIN core.DT_DELIVERY_RESULT deri_result "
                    + "ON ins_result.DELIVERY_RESULT_SID = deri_result.SID "
                    + "WHERE deri_group.SID = deri_result.DELIVERY_GROUP_SID "
                    + "AND (result_status.CODE = 'notstarted' OR result_status.CODE = 'messagesent')"
                + ")";

        /// <summary>ロガー</summary>
        private readonly ILogger<DtInstallResultRepository> _logger;

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
        public DtInstallResultRepository(ILogger<DtInstallResultRepository> logger, ITimeProvider timePrivder, DBPolly dbPolly, AppSettings appSettings)
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
        /// 引数に指定したDtInstallResultをDT_INSTALL_RESULTテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <param name="state">状態。適用結果ステータスマスタテーブルのコード値が入る。</param>
        /// <returns>追加したデータ。配信グループデータテーブルの更新に失敗した場合はnullを返す。</returns>
        /// <remarks>
        /// すべての処理に成功した場合には適用結果データテーブルに登録したデータを返す。
        /// 配信グループデータテーブルの更新に失敗した場合はnullを返す。
        /// 適用結果データテーブルへの登録に失敗した場合には、例外を投げる。
        /// </remarks>
        public DtInstallResult CreateDtInstallResultIfAlreadyMessageThrowEx(DtInstallResult inData, string state)
        {
            DtInstallResult model = null;

            try
            {
                _logger.EnterJson("{0}", inData);

                // グループステータス更新失敗フラグ
                bool failedToUpdateGroupStatus = false;

                DBAccessor.Models.DtDeliveryGroup deliveryGroupEntity = new DBAccessor.Models.DtDeliveryGroup
                {
                    DeliveryGroupStatusSid = 0
                };

                _dbPolly.Execute(() =>
                {
                    // メッセージIDがなければ追加
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        // マスタテーブルから適用結果ステータスSIDを取得
                        var installResultStatus = db.MtInstallResultStatus.FirstOrDefault(x => x.Code == state);
                        if (installResultStatus == null || installResultStatus.Sid == 0)
                        {
                            throw new RmsException(string.Format("状態[{0}]がマスタテーブルに存在しません。", state));
                        }

                        var addedAlready = db.DtInstallResult.FirstOrDefault(x => x.MessageId == inData.MessageId);
                        if (addedAlready != null)
                        {
                            throw new RmsAlreadyExistException(string.Format("MessageId [{0}] はすでに追加済みです。", inData.MessageId));
                        }

                        // 適用結果データテーブルの更新処理
                        DBAccessor.Models.DtInstallResult dbdata;
                        {
                            // 適用結果ステータスSIDを設定
                            inData.InstallResultStatusSid = installResultStatus.Sid;
                            DBAccessor.Models.DtInstallResult entity = new DBAccessor.Models.DtInstallResult(inData);

                            // メッセージを適用結果データテーブルに保存
                            // 適用結果データテーブルの更新は、配信グループデータテーブル更新の成功/失敗に依存しない仕様のため、先にDB保存処理を呼ぶ
                            dbdata = db.DtInstallResult.Add(entity).Entity;
                            db.SaveChanges(_timePrivder);
                        }

                        // 配信グループデータテーブルの更新処理
                        {
                            // 適用結果ステータスマスタテーブルから"completed"に該当するSIDを取得する
                            long completedStatusSid = db.MtDeliveryGroupStatus.FirstOrDefault(x => x.Code == Const.DeliveryGroupStatus.Completed).Sid;

                            // 更新された配信グループデータテーブルのレコード数
                            int affectedRows = 0;

                            // 適用結果データテーブルの配信結果SIDから配信結果データテーブルのレコードを抽出
                            var deliveryResult = db.DtDeliveryResult.FirstOrDefault(x => x.Sid == inData.DeliveryResultSid);
                            if (deliveryResult != null)
                            {
                                // メッセージが所属する配信グループのSID
                                long targetDeliveryGroupSid = deliveryResult.DeliveryGroupSid;

                                // SQL文を発行する
                                var sqlParamForSid = new SqlParameter(SqlKeyForSid, targetDeliveryGroupSid);
                                var sqlParamForGroupStatusSid = new SqlParameter(SqlKeyForGroupStatusSid, completedStatusSid);
                                var sqlParams = new[] { sqlParamForGroupStatusSid, sqlParamForSid };

                                try
                                {
                                    affectedRows = db.Database.ExecuteSqlCommand(SqlCommandToUpdateDeliveryGroupStatus, sqlParams);
                                    if (affectedRows <= 0)
                                    {
                                        _logger.Debug(string.Format("メッセージ[{0}]が所属する配信グループIDのステータスに変更はありませんでした。", inData.MessageId));
                                    }
                                }
                                catch (Exception)
                                {
                                    // 配信グループデータ更新失敗フラグを立てる
                                    failedToUpdateGroupStatus = true;
                                    _logger.Debug(string.Format("メッセージ[{0}]が所属する配信グループIDのステータス更新に失敗しました。", inData.MessageId));
                                }
                            }
                            else
                            {
                                // 配信結果はメッセージの定義上null許可のため、配信結果が存在しないケースがある
                                _logger.Debug(string.Format("メッセージ[{0}]が所属する配信グループIDがnullであるため配信グループデータテーブルは更新しませんでした。", inData.MessageId));
                            }

                            db.SaveChanges(_timePrivder);
                        }

                        model = dbdata.ToModel();
                    }
                });

                if (failedToUpdateGroupStatus)
                {
                    // 配信グループデータ更新失敗時にはnullを返す
                    return null;
                }

                return model;
            }
            catch (RmsAlreadyExistException)
            {
                // メッセージ追加済みエラー
                throw;
            }
            catch (RmsException)
            {
                throw;
            }
            catch (ValidationException e)
            {
                throw new RmsParameterException(e.ValidationResult.ErrorMessage, e);
            }
            catch (Exception e)
            {
                throw new RmsException("DT_INSTALL_RESULTテーブルへのInsertまたはDT_DELIVERY_GROUPのUpdateに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// 引数指定グループ内の配信結果内で、ゲートウェイ機器SIDをもつデータの適用結果ステータスをmessagesentにして作成する
        /// </summary>
        /// <remarks>配信グループデータは参照のみ行い、メンバを変更しない</remarks>
        /// <param name="inData">配信グループデータ</param>
        /// <param name="gatewaySid">ゲートウェイ機器SID</param>
        /// <returns>追加した適用結果データ群</returns>
        public IEnumerable<DtInstallResult> CreateDtInstallResultStatusSent(DtDeliveryGroup inData, long gatewaySid)
        {
            Assert.IfNull(inData);
            Assert.IfNull(inData.DtDeliveryResult);

            IEnumerable<DtInstallResult> models = new List<DtInstallResult>();

            try
            {
                _logger.EnterJson("{0}", inData);

                // 現在時刻はここで記録
                DateTime nowTime = _timePrivder.UtcNow;

                // 指定したゲートウェイ機器SIDをもつ配信結果を抽出する
                var results = inData.DtDeliveryResult.Where(x => x.GwDeviceSid == gatewaySid);
                if (!results.Any())
                {
                    // 空データを返却
                    return models;
                }

                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        List<DBAccessor.Models.DtInstallResult> dbModels = new List<DBAccessor.Models.DtInstallResult>();

                        // 「送信済」の適用結果ステータスSIDを取得する
                        var sentData = db.MtInstallResultStatus.Where(x => x.Code.Equals(InstallResultStatus.MessageSent)).FirstOrDefault();
                        if (sentData == null)
                        {
                            // 値を設定せずに返却
                            return;
                        }

                        // 各配信結果SID・端末SIDをもつ適用結果データを作成する
                        foreach (var result in results)
                        {
                            var installResult = new DBAccessor.Models.DtInstallResult()
                            {
                                DeviceSid = result.DeviceSid,
                                DeliveryResultSid = result.Sid,
                                InstallResultStatusSid = sentData.Sid,
                                CollectDatetime = nowTime
                            };

                            var dbData = db.DtInstallResult.Add(installResult).Entity;
                            dbModels.Add(dbData);
                        }

                        db.SaveChanges(_timePrivder);
                        models = dbModels.Select(x => x.ToModel());
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
        /// 配信結果ステータスを"messagesent"に更新する
        /// </summary>
        /// <param name="gatewaySid">最上位（ゲートウェイ）機器SID</param>
        /// <returns>結果</returns>
        public DtInstallResult UpdateInstallResultStatusToMessageSent(long gatewaySid)
        {
            return UpdateInstallResultStatus(gatewaySid, Utility.Const.InstallResultStatus.MessageSent);
        }

        /// <summary>
        /// 配信結果ステータスを更新する
        /// </summary>
        /// <param name="gatewaySid">最上位（ゲートウェイ）機器SID</param>
        /// <param name="statusCode">ステータス</param>
        /// <returns>結果</returns>
        /// <remarks>ステータスはUtility.Consts.InstallResultStatusに定義されている</remarks>
        private DtInstallResult UpdateInstallResultStatus(long gatewaySid, string statusCode)
        {
            DtInstallResult model = null;
            try
            {
                _logger.EnterJson("{0}", new { gatewaySid, statusCode });

                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        DBAccessor.Models.DtInstallResult entity = null;

                        // 更新対象レコードを取得する
                        entity = db.DtInstallResult.Where(x => /*x.IsLatest &&*/ x.DeliveryResultS.GwDeviceSid == gatewaySid).FirstOrDefault();
                        if (entity == null || entity.Sid == 0)
                        {
                            // 対象レコードなしエラー
                            throw new RmsException("更新対象となる適用結果データレコードが見つかりませんでした。");
                        }

                        // 「送信済」の適用結果ステータスSIDを取得する
                        var status = db.MtInstallResultStatus.Where(x => x.Code.Equals(statusCode)).FirstOrDefault();
                        if (status == null || status.Sid == 0)
                        {
                            // 対象レコードなしエラー
                            throw new RmsException("指定したステータスコードが.MT_INSTALL_RESULT_STATUSテーブルに存在しませんでした。");
                        }

                        // ステータスSIDを更新
                        entity.InstallResultStatusSid = status.Sid;

                        // DB更新処理
                        db.DtInstallResult.Attach(entity);
                        db.Entry(entity).Property(x => x.InstallResultStatusSid).IsModified = true;
                        if (db.SaveChanges() > 0)
                        {
                            model = entity.ToModel();
                        }

                        db.SaveChanges(_timePrivder);
                        model = entity.ToModel();
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
                throw new RmsException("DT_INSTALL_RESULTテーブルへのInsertに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }
    }
}
