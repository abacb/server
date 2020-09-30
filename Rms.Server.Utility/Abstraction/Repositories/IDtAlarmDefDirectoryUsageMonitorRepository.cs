using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Utility.Utility.Models;
using System.Collections.Generic;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// DT_ALARM_DEF_DIRECTORY_USAGE_MONITORテーブルリポジトリのインターフェース
    /// </summary>
    public interface IDtAlarmDefDirectoryUsageMonitorRepository : IRepository
    {
        /// <summary>
        /// DT_ALARM_DEF_DIRECTORY_USAGE_MONITORテーブルからDtAlarmDefDirectoryUsageMonitorを取得する
        /// </summary>
        /// <param name="directoryUsage">ディレクトリ使用量</param>
        /// <returns>取得したデータ</returns>
        IEnumerable<DtAlarmDefDirectoryUsageMonitor> ReadDtAlarmDefDirectoryUsageMonitor(DirectoryUsage directoryUsage);
    }
}
