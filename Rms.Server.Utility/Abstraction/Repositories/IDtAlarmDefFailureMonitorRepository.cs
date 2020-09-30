using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Utility.Utility.Models;
using System.Collections.Generic;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// DT_ALARM_DEF_FAILURE_MONITORテーブルリポジトリのインターフェース
    /// </summary>
    public interface IDtAlarmDefFailureMonitorRepository : IRepository
    {
        /// <summary>
        /// DT_ALARM_DEF_FAILURE_MONITORテーブルからDtAlarmDefFailureMonitorを取得する
        /// </summary>
        /// <param name="errorLog">エラーログ</param>
        /// <returns>取得したデータ</returns>
        IEnumerable<DtAlarmDefFailureMonitor> ReadDtAlarmDefFailureMonitor(ErrorLog errorLog);
    }
}
