using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Utility.Utility.Models;
using System.Collections.Generic;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// DT_ALARM_DEF_TEMPERATURE_SENSOR_LOG_MONITORテーブルリポジトリのインターフェース
    /// </summary>
    public interface IDtAlarmDefTemperatureSensorLogMonitorRepository : IRepository
    {
        /// <summary>
        /// DT_ALARM_DEF_TEMPERATURE_SENSOR_LOG_MONITORテーブルからDtAlarmDefTemperatureSensorLogMonitorを取得する
        /// </summary>
        /// <param name="temperatureSensorLog">温度センサログ</param>
        /// <returns>取得したデータ</returns>
        IEnumerable<DtAlarmDefTemperatureSensorLogMonitor> ReadDtAlarmDefTemperatureSensorLogMonitor(TemperatureSensorLog temperatureSensorLog);
    }
}
