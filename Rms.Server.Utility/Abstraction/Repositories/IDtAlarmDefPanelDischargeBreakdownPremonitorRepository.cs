using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Utility.Utility.Models;
using System.Collections.Generic;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// DT_ALARM_DEF_PANEL_DISCHARGE_BREAKDOWN_PREMONITORテーブルリポジトリのインターフェース
    /// </summary>
    public interface IDtAlarmDefPanelDischargeBreakdownPremonitorRepository : IRepository
    {
        /// <summary>
        /// DT_ALARM_DEF_PANEL_DISCHARGE_BREAKDOWN_PREMONITORテーブルからDtAlarmDefPanelDischargeBreakdownPremonitorを取得する
        /// </summary>
        /// <param name="panelDischargeBreakdownPredictiveResutLog">パネル放電破壊予兆結果ログ</param>
        /// <returns>取得したデータ</returns>
        IEnumerable<DtAlarmDefPanelDischargeBreakdownPremonitor> ReadDtAlarmDefPanelDischargeBreakdownPremonitor(PanelDischargeBreakdownPredictiveResutLog panelDischargeBreakdownPredictiveResutLog);
    }
}
