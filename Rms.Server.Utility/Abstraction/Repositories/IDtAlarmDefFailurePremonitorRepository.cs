using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Utility.Utility.Models;
using System.Collections.Generic;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// DT_ALARM_DEF_FAILURE_PREMONITORテーブルリポジトリのインターフェース
    /// </summary>
    public interface IDtAlarmDefFailurePremonitorRepository : IRepository
    {
        /// <summary>
        /// DT_ALARM_DEF_FAILURE_PREMONITORテーブルからDtAlarmDefFailurePremonitorを取得する
        /// </summary>
        /// <param name="failurePredictiveResultLog">故障予兆結果ログ</param>
        /// <returns>取得したデータ</returns>
        IEnumerable<DtAlarmDefFailurePremonitor> ReadDtAlarmDefFailurePremonitor(FailurePredictiveResultLog failurePredictiveResultLog);
    }
}
