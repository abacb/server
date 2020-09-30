using Microsoft.Extensions.Logging;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Operation.Utility.Extensions;
using Rms.Server.Operation.Utility.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Rms.Server.Operation.Abstraction.Repositories
{
    /// <summary>
    /// DT_ALARMテーブルのリポジトリ
    /// </summary>
    public partial class DtAlarmRepository : IDtAlarmRepository, Core.Abstraction.Repositories.ICleanRepository
    {
        /// <summary>ロガー</summary>
        private readonly ILogger<DtAlarmRepository> _logger;

        /// <summary>DateTimeの提供元</summary>
        private readonly ITimeProvider _timeProvider;

        /// <summary>DB接続用のPolly</summary>
        private readonly DBPolly _dbPolly;

        /// <summary>アプリケーション設定</summary>
        private readonly AppSettings _appSettings;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="timeProvider">DateTimeの提供元</param>
        /// <param name="dbPolly">DB接続用のPolly</param>
        /// <param name="appSettings">アプリケーション設定</param>
        public DtAlarmRepository(ILogger<DtAlarmRepository> logger, ITimeProvider timeProvider, DBPolly dbPolly, AppSettings appSettings)
        {
            Assert.IfNull(logger);
            Assert.IfNull(timeProvider);
            Assert.IfNull(dbPolly);
            Assert.IfNull(appSettings);

            _logger = logger;
            _timeProvider = timeProvider;
            _dbPolly = dbPolly;
            _appSettings = appSettings;
        }

        /// <summary>
        /// 引数に指定したDtAlarmをDT_ALARMテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <returns>処理結果</returns>
        public DtAlarm CreateDtAlarm(DtAlarm inData)
        {
            DtAlarm model = null;
            try
            {
                _logger.EnterJson("{0}", inData);

                DBAccessor.Models.DtAlarm entity = new DBAccessor.Models.DtAlarm();
                entity.CopyExcludingEquipmentFrom(inData);

                // バリデーション
                Validator.ValidateObject(entity, new ValidationContext(entity, null, null));

                _dbPolly.Execute(() =>
                {
                    entity.CreateDatetime = _timeProvider.UtcNow;

                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        var dbdata = db.DtAlarm.Add(entity).Entity;
                        db.SaveChanges();
                        model = dbdata.ToModelExcludedDtEquipment();
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
                throw new RmsException("DT_ALARMテーブルへのInsertに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// DT_ALARMテーブルに条件に一致するDtAlarmが存在するか確認する
        /// </summary>
        /// <param name="messageId">メッセージID</param>
        /// <returns>存在する場合trueを、存在しない場合falseを返す</returns>
        public bool ExistDtAlarm(string messageId)
        {
            bool result = false;
            try
            {
                _logger.EnterJson("{0}", new { messageId });

                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        result = db.DtAlarm.Any(x => x.MessageId == messageId);
                    }
                });

                return result;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_ALARMテーブルのSelectに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", result);
            }
        }

        /// <summary>
        /// DT_ALARMテーブルからメール送信済みの最新DtAlarmを取得する
        /// </summary>
        /// <param name="alarmDefId">アラーム定義ID</param>
        /// <returns>取得したデータ</returns>
        public DtAlarm ReadLatestMailSentDtAlarm(string alarmDefId)
        {
            DtAlarm model = null;
            try
            {
                _logger.EnterJson("{0}", new { alarmDefId });

                DBAccessor.Models.DtAlarm entity = null;
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        entity = db.DtAlarm.Where(x => x.AlarmDefId == alarmDefId && x.HasMail == true).OrderByDescending(x => x.CreateDatetime).FirstOrDefault();
                    }
                });

                model = entity?.ToModelExcludedDtEquipment();
                return model;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_ALARMテーブルのSelectに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// 指定日時より作成日が古いデータを削除する
        /// </summary>
        /// <param name="comparisonSourceDatetime">比較対象日時</param>
        /// <returns>削除数</returns>
        public int DeleteExceedsMonthsAllData(DateTime comparisonSourceDatetime)
        {
            int result = 0;
            try
            {
                _logger.EnterJson("{0}", new { comparisonSourceDatetime });

                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        // 作成日時から指定月数超過しているデータを抽出し、削除する
                        var targets = db.DtAlarm.Where(x => x.CreateDatetime < comparisonSourceDatetime);
                        db.DtAlarm.RemoveRange(targets);
                        result = db.SaveChanges();
                    }
                });

                return result;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_ALARMテーブルのDeleteに失敗しました。", e);
            }
            finally
            {
                _logger.Leave();
            }
        }
    }
}
