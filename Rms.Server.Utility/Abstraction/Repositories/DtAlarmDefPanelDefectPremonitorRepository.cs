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
    /// DT_ALARM_DEF_PANEL_DEFECT_PREMONITORテーブルのリポジトリ
    /// </summary>
    public partial class DtAlarmDefPanelDefectPremonitorRepository : IDtAlarmDefPanelDefectPremonitorRepository
    {
        /// <summary>ロガー</summary>
        private readonly ILogger<DtAlarmDefPanelDefectPremonitorRepository> _logger;

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
        public DtAlarmDefPanelDefectPremonitorRepository(ILogger<DtAlarmDefPanelDefectPremonitorRepository> logger, ITimeProvider timePrivder, DBPolly dbPolly, AppSettings appSettings)
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
        /// DT_ALARM_DEF_PANEL_DEFECT_PREMONITORテーブルからDtAlarmDefPanelDefectPremonitorを取得する
        /// </summary>
        /// <param name="panelDefectPredictiveResutLog">パネル欠陥予兆結果ログ</param>
        /// <returns>取得したデータ</returns>
        public IEnumerable<DtAlarmDefPanelDefectPremonitor> ReadDtAlarmDefPanelDefectPremonitor(PanelDefectPredictiveResutLog panelDefectPredictiveResutLog)
        {
            IEnumerable<DtAlarmDefPanelDefectPremonitor> models = null;
            try
            {
                _logger.EnterJson("{0}", panelDefectPredictiveResutLog);

                List<DBAccessor.Models.DtAlarmDefPanelDefectPremonitor> entities = null;
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        entities = db.DtAlarmDefPanelDefectPremonitor
                                        .Where(x => string.IsNullOrEmpty(x.TypeCode) || x.TypeCode == panelDefectPredictiveResutLog.TypeCode)
                                        .Where(x => string.IsNullOrEmpty(x.ErrorCode) || x.ErrorCode == panelDefectPredictiveResutLog.ErrorCode)
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
                throw new RmsException("DT_ALARM_DEF_PANEL_DEFECT_PREMONITORテーブルのSelectに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", models);
            }
        }
    }
}
