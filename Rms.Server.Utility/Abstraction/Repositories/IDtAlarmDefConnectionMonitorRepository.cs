using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Utility.Utility.Models;
using System.Collections.Generic;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// DT_ALARM_DEF_CONNECTION_MONITORテーブルリポジトリのインターフェース
    /// </summary>
    public interface IDtAlarmDefConnectionMonitorRepository : IRepository
    {
        /// <summary>
        /// DT_ALARM_DEF_CONNECTION_MONITORテーブルからDtAlarmDefConnectionMonitorを取得する
        /// </summary>
        /// <param name="typeCode">機種コード</param>
        /// <param name="targets">監視対象</param>
        /// <returns>取得したデータ</returns>
        IEnumerable<DtAlarmDefConnectionMonitor> ReadDtAlarmDefConnectionMonitor(string typeCode, IEnumerable<string> targets);
    }
}
