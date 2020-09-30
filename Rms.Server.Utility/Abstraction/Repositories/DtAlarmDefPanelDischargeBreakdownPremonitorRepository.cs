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
    /// DT_ALARM_DEF_PANEL_DISCHARGE_BREAKDOWN_PREMONITORテーブルのリポジトリ
    /// </summary>
    public partial class DtAlarmDefPanelDischargeBreakdownPremonitorRepository : IDtAlarmDefPanelDischargeBreakdownPremonitorRepository
    {
        /// <summary>ロガー</summary>
        private readonly ILogger<DtAlarmDefPanelDischargeBreakdownPremonitorRepository> _logger;

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
        public DtAlarmDefPanelDischargeBreakdownPremonitorRepository(ILogger<DtAlarmDefPanelDischargeBreakdownPremonitorRepository> logger, ITimeProvider timePrivder, DBPolly dbPolly, AppSettings appSettings)
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
        /// DT_ALARM_DEF_PANEL_DISCHARGE_BREAKDOWN_PREMONITORテーブルからDtAlarmDefPanelDischargeBreakdownPremonitorを取得する
        /// </summary>
        /// <param name="panelDischargeBreakdownPredictiveResutLog">パネル放電破壊予兆結果ログ</param>
        /// <returns>取得したデータ</returns>
        public IEnumerable<DtAlarmDefPanelDischargeBreakdownPremonitor> ReadDtAlarmDefPanelDischargeBreakdownPremonitor(PanelDischargeBreakdownPredictiveResutLog panelDischargeBreakdownPredictiveResutLog)
        {
            IEnumerable<DtAlarmDefPanelDischargeBreakdownPremonitor> models = null;
            try
            {
                _logger.EnterJson("{0}", panelDischargeBreakdownPredictiveResutLog);

                List<DBAccessor.Models.DtAlarmDefPanelDischargeBreakdownPremonitor> entities = null;
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        entities = db.DtAlarmDefPanelDischargeBreakdownPremonitor
                                        .Where(x => string.IsNullOrEmpty(x.TypeCode) || x.TypeCode == panelDischargeBreakdownPredictiveResutLog.TypeCode)
                                        .Where(x => string.IsNullOrEmpty(x.ErrorCode) || x.ErrorCode == panelDischargeBreakdownPredictiveResutLog.ErrorCode)
                                        .ToList();
                    }
                });

                if (entities != null)
                {
                    models = entities.Select(x => x.ToModel());
                }

                return models;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_ALARM_DEF_PANEL_DISCHARGE_BREAKDOWN_PREMONITORテーブルのSelectに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", models);
            }
        }
    }
}
