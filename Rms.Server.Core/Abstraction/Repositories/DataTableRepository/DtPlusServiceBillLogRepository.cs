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
    /// <summary>
    /// DT_PLUS_SERVICE_BILL_LOGテーブルのリポジトリ
    /// </summary>
    public partial class DtPlusServiceBillLogRepository : IDtPlusServiceBillLogRepository
    {
        /// <summary>ロガー</summary>
        private readonly ILogger<DtPlusServiceBillLogRepository> _logger;

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
        public DtPlusServiceBillLogRepository(ILogger<DtPlusServiceBillLogRepository> logger, ITimeProvider timePrivder, DBPolly dbPolly, AppSettings appSettings)
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
        /// 引数に指定したDtPlusServiceBillLogをDT_PLUS_SERVICE_BILL_LOGテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <returns>追加したデータ。エラーが発生した場合には例外を投げるためnullを返さない</returns>
        public DtPlusServiceBillLog Upsert(DtPlusServiceBillLog inData)
        {
            DtPlusServiceBillLog model = null;
            try
            {
                _logger.EnterJson("{0}", inData);

                DBAccessor.Models.DtPlusServiceBillLog entity = new DBAccessor.Models.DtPlusServiceBillLog(inData);

                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        // 重複チェック
                        var existing = db.DtPlusServiceBillLog
                            .FirstOrDefault(x => x.StudyInstanceUid == inData.StudyInstanceUid && x.TypeName == inData.TypeName);

                        if (existing == null)
                        {
                            // 追加
                            db.DtPlusServiceBillLog.Add(entity);
                        }
                        else
                        {
                            // 計測日時が古いデータで更新しないようにする。
                            // 呼び出し側のパラメタチェックでかかるはずだが、
                            // 日時がnullableなので念のため既存日時がnullの場合は更新するようにしておく。
                            if (existing.MeasureDatetime < inData.MeasureDatetime || existing.MeasureDatetime == null)
                            {
                                // 更新処理
                                entity.Sid = existing.Sid;
                                db.DtPlusServiceBillLog.Attach(entity);
                                db.Entry(entity).Property(x => x.DeviceSid).IsModified = true;
                                db.Entry(entity).Property(x => x.SourceEquipmentUid).IsModified = true;
                                //// db.Entry(entity).Property(x => x.TypeName).IsModified = true;
                                db.Entry(entity).Property(x => x.BillFlg).IsModified = true;
                                db.Entry(entity).Property(x => x.PatientId).IsModified = true;
                                db.Entry(entity).Property(x => x.Sex).IsModified = true;
                                db.Entry(entity).Property(x => x.Age).IsModified = true;
                                //// db.Entry(entity).Property(x => x.StudyInstanceUid).IsModified = true;
                                db.Entry(entity).Property(x => x.SopInstanceUid).IsModified = true;
                                db.Entry(entity).Property(x => x.StudyDatetime).IsModified = true;
                                db.Entry(entity).Property(x => x.MeasureDatetime).IsModified = true;
                                db.Entry(entity).Property(x => x.CollectDatetime).IsModified = true;
                                db.Entry(entity).Property(x => x.UpdateDatetime).IsModified = true;
                            }
                            else
                            {
                                // 古いデータでの更新を弾く、AlreadyExistとはニアリーイコールではあるが、丸める
                                new RmsAlreadyExistException(string.Format("StudyInstanceUid [{0}], TypeName[{1}] はすでに追加済みです。", inData.StudyInstanceUid, inData.TypeName));
                            }
                        }

                        if (db.SaveChanges(_timePrivder) > 0)
                        {
                            // 呼び出し側に更新済みデータを返却する
                            model = db.DtPlusServiceBillLog.FirstOrDefault(x => x.Sid == inData.Sid)?.ToModel();
                        }
                    }
                });

                return model;
            }
            catch (RmsAlreadyExistException)
            {
                throw;
            }
            catch (ValidationException e)
            {
                throw new RmsParameterException(e.ValidationResult.ErrorMessage, e);
            }
            catch (Exception e)
            {
                throw new RmsException("DT_PLUS_SERVICE_BILL_LOGテーブルへのInsertに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }
    }
}
