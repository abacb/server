using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Operation.Utility.Models;

namespace Rms.Server.Operation.Abstraction.Repositories
{
    /// <summary>
    /// DT_ALARM_CONFIGテーブルリポジトリのインターフェース
    /// </summary>
    public interface IDtAlarmConfigRepository : IRepository
    {
        /// <summary>
        /// DT_ALARM_CONFIGテーブルからDtAlarmConfigを取得する
        /// </summary>
        /// <param name="alarmLevel">アラームレベル</param>
        /// <param name="allowNotExist">取得件数が0件である場合を正常系とする場合はtrueを、異常系とする場合はfalseを指定する</param>
        /// <returns>取得したデータ</returns>
        DtAlarmConfig ReadDtAlarmConfig(byte alarmLevel, bool allowNotExist = true);
    }
}
