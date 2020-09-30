using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Utility.Utility.Models;
using System.Collections.Generic;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// DT_ALARM_DEF_PANEL_DEFECT_PREMONITORテーブルリポジトリのインターフェース
    /// </summary>
    public interface IDtAlarmDefCalibrationPremonitorRepository : IRepository
    {
        /// <summary>
        /// DT_ALARM_DEF_CALIBRATION_PREMONITORテーブルからDtAlarmDefCalibrationPremonitorを取得する
        /// </summary>
        /// <param name="calibrationPredictiveResutLog">キャリブレーション予兆結果ログ</param>
        /// <returns>取得したデータ</returns>
        IEnumerable<DtAlarmDefCalibrationPremonitor> ReadDtAlarmDefCalibrationPremonitor(CalibrationPredictiveResutLog calibrationPredictiveResutLog);
    }
}
