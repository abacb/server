using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Utility.Utility.Models;
using System.Collections.Generic;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// DT_ALARM_DEF_INSTALL_RESULT_MONITORテーブルリポジトリのインターフェース
    /// </summary>
    public interface IDtAlarmDefInstallResultMonitorRepository : IRepository
    {
        /// <summary>
        /// DT_ALARM_DEF_INSTALL_RESULT_MONITORテーブルからDtAlarmDefInstallResultMonitorを取得する
        /// </summary>
        /// <param name="installResult">適用結果</param>
        /// <returns>取得したデータ</returns>
        IEnumerable<DtAlarmDefInstallResultMonitor> ReadDtAlarmDefInstallResultMonitor(InstallResult installResult);
    }
}
