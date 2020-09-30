using Microsoft.Extensions.Logging;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Utility.Utility.Extensions;
using Rms.Server.Utility.Utility.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// DT_ALARM_DEF_CONNECTION_MONITORテーブルのリポジトリ
    /// </summary>
    public partial class DtAlarmDefConnectionMonitorRepository : IDtAlarmDefConnectionMonitorRepository
    {
        /// <summary>ロガー</summary>
        private readonly ILogger<DtAlarmDefConnectionMonitorRepository> _logger;

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
        public DtAlarmDefConnectionMonitorRepository(ILogger<DtAlarmDefConnectionMonitorRepository> logger, ITimeProvider timePrivder, DBPolly dbPolly, AppSettings appSettings)
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
        /// DT_ALARM_DEF_CONNECTION_MONITORテーブルからDtAlarmDefConnectionMonitorを取得する
        /// </summary>
        /// <param name="typeCode">機種コード</param>
        /// <param name="targets">監視対象</param>
        /// <returns>取得したデータ</returns>
        public IEnumerable<DtAlarmDefConnectionMonitor> ReadDtAlarmDefConnectionMonitor(string typeCode, IEnumerable<string> targets)
        {
            IEnumerable<DtAlarmDefConnectionMonitor> models = null;
            try
            {
                _logger.EnterJson("{0}", new { typeCode, targets });

                List<DBAccessor.Models.DtAlarmDefConnectionMonitor> entities = null;
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        var query = db.DtAlarmDefConnectionMonitor
                                      .Where(x => string.IsNullOrEmpty(x.TypeCode) || x.TypeCode == typeCode);

                        foreach (var target in targets)
                        {
                            query = query.Where(x => string.IsNullOrEmpty(x.Target) || x.Target == target);
                        }

                        entities = query.ToList();
                    }
                });

                if (entities != null)
                {
                    models = entities.Select(x => x.ToModel());
                }

                return models;
            }
            catch (RmsException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_ALARM_DEF_CONNECTION_MONITORテーブルのSelectに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", models);
            }
        }
    }
}
