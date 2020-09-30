using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Utility.Utility.Models;
using System.Collections.Generic;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// DT_ALARM_DEF_TUBE_CURRENT_DETERIORATION_PREMONITORテーブルリポジトリのインターフェース
    /// </summary>
    public interface IDtAlarmDefTubeCurrentDeteriorationPremonitorRepository : IRepository
    {
        /// <summary>
        /// DT_ALARM_DEF_TUBE_CURRENT_DETERIORATION_PREMONITORテーブルからDtAlarmDefTubeCurrentDeteriorationPremonitorを取得する
        /// </summary>
        /// <param name="tubeCurrentDeteriorationPredictiveResutLog">管電流経時劣化予兆結果ログ</param>
        /// <returns>取得したデータ</returns>
        IEnumerable<DtAlarmDefTubeCurrentDeteriorationPremonitor> ReadDtAlarmDefTubeCurrentDeteriorationPremonitor(TubeCurrentDeteriorationPredictiveResutLog tubeCurrentDeteriorationPredictiveResutLog);
    }
}
