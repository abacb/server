using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Utility.Utility.Models;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// DT_ALARM_SMART_PREMONITORテーブルリポジトリのインターフェース
    /// </summary>
    public interface IDtAlarmSmartPremonitorRepository : IRepository
    {
        /// <summary>
        /// DT_ALARM_SMART_PREMONITORテーブルからDtAlarmSmartPremonitorを取得する
        /// </summary>
        /// <param name="smartAttributeInfoId">取得するデータのSMART項目ID</param>
        /// <returns>取得したデータ</returns>
        DtAlarmSmartPremonitor ReadDtAlarmSmartPremonitor(string smartAttributeInfoId);
    }
}
