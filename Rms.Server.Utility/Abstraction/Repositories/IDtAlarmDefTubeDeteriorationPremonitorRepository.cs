using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Utility.Utility.Models;
using System.Collections.Generic;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// DT_ALARM_DEF_TUBE_DETERIORATION_PREMONITORテーブルリポジトリのインターフェース
    /// </summary>
    public interface IDtAlarmDefTubeDeteriorationPremonitorRepository : IRepository
    {
        /// <summary>
        /// DT_ALARM_DEF_TUBE_DETERIORATION_PREMONITORテーブルからDtAlarmDefTubeDeteriorationPremonitorを取得する
        /// </summary>
        /// <param name="tubeDeteriorationPredictiveResutLog">管球劣化予兆結果ログ</param>
        /// <returns>取得したデータ</returns>
        IEnumerable<DtAlarmDefTubeDeteriorationPremonitor> ReadDtAlarmDefTubeDeteriorationPremonitor(TubeDeteriorationPredictiveResutLog tubeDeteriorationPredictiveResutLog);
    }
}
