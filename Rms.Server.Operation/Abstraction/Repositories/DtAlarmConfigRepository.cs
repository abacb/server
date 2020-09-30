using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Operation.Utility.Extensions;
using Rms.Server.Operation.Utility.Models;
using System;
using System.Linq;

namespace Rms.Server.Operation.Abstraction.Repositories
{
    /// <summary>
    /// DT_ALARM_CONFIGテーブルのリポジトリ
    /// </summary>
    public partial class DtAlarmConfigRepository : IDtAlarmConfigRepository
    {
        /// <summary>ロガー</summary>
        private readonly ILogger<DtAlarmConfigRepository> _logger;

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
        public DtAlarmConfigRepository(ILogger<DtAlarmConfigRepository> logger, ITimeProvider timeProvider, DBPolly dbPolly, AppSettings appSettings)
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
        /// DT_ALARM_CONFIGテーブルからDtAlarmConfigを取得する
        /// </summary>
        /// <param name="alarmLevel">アラームレベル</param>
        /// <param name="allowNotExist">取得件数が0件である場合を正常系とする場合はtrueを、異常系とする場合はfalseを指定する</param>
        /// <returns>取得したデータ</returns>
        public DtAlarmConfig ReadDtAlarmConfig(byte alarmLevel, bool allowNotExist = true)
        {
            DtAlarmConfig model = null;
            try
            {
                _logger.EnterJson("{0}", new { alarmLevel });

                DBAccessor.Models.DtAlarmConfig entity = null;
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        entity = db.DtAlarmConfig.FirstOrDefault(x => x.AlarmLevelFrom <= alarmLevel && alarmLevel <= x.AlarmLevelTo);
                    }
                });

                if (entity != null)
                {
                    model = entity.ToModel();
                }
                else
                {
                    if (!allowNotExist)
                    {
                        var info = new { AlarmLevelFrom = alarmLevel, AlarmLevelTo = alarmLevel };
                        throw new RmsException(string.Format("DT_ALARM_CONFIGテーブルに該当レコードが存在しません。(検索条件: {0})", JsonConvert.SerializeObject(info)));
                    }
                }

                return model;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_ALARM_CONFIGテーブルのSelectに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }
    }
}
