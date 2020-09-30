using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Utility.Utility.Models;
using System.Collections.Generic;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// DT_ALARM_DEF_PANEL_DEFECT_PREMONITORテーブルリポジトリのインターフェース
    /// </summary>
    public interface IDtAlarmDefPanelDefectPremonitorRepository : IRepository
    {
        /// <summary>
        /// DT_ALARM_DEF_FAILURE_PREMONITORテーブルからDtAlarmDefPanelDefectPremonitorを取得する
        /// </summary>
        /// <param name="panelDefectPredictiveResutLog">パネル欠陥予兆結果ログ</param>
        /// <returns>取得したデータ</returns>
        IEnumerable<DtAlarmDefPanelDefectPremonitor> ReadDtAlarmDefPanelDefectPremonitor(PanelDefectPredictiveResutLog panelDefectPredictiveResutLog);
    }
}
