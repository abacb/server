using Microsoft.Extensions.Logging;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Utility.Utility.Extensions;
using Rms.Server.Utility.Utility.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// DT_ALARM_DEF_CALIBRATION_PREMONITORテーブルのリポジトリ
    /// </summary>
    public partial class DtAlarmDefCalibrationPremonitorRepository : IDtAlarmDefCalibrationPremonitorRepository
    {
        /// <summary>ロガー</summary>
        private readonly ILogger<DtAlarmDefCalibrationPremonitorRepository> _logger;

        /// <summary>DateTimeの提供元</summary>
        private readonly ITimeProvider _timePrivder;

        /// <summary>DB接続用のPolly</summary>
        private readonly DBPolly _dbPolly;

        /// <summary>アプリケーション設定</summary>
        private readonly AppSettings _appSettings;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="timePrivder">DateTimeの提供元</param>
        /// <param name="dbPolly">DB接続用のPolly</param>
        /// <param name="appSettings">アプリケーション設定</param>
        public DtAlarmDefCalibrationPremonitorRepository(ILogger<DtAlarmDefCalibrationPremonitorRepository> logger, ITimeProvider timePrivder, DBPolly dbPolly, AppSettings appSettings)
        {
            Assert.IfNull(logger);
            Assert.IfNull(timePrivder);
            Assert.IfNull(dbPolly);
            Assert.IfNull(appSettings);
            _logger = logger;
            _timePrivder = timePrivder;
            _dbPolly = dbPolly;
            _appSettings = appSettings;
        }

        /// <summary>
        /// DT_ALARM_DEF_CALIBRATION_PREMONITORテーブルからDtAlarmDefCalibrationPremonitorを取得する
        /// </summary>
        /// <param name="calibrationPredictiveResutLog">キャリブレーション予兆結果ログ</param>
        /// <returns>取得したデータ</returns>
        public IEnumerable<DtAlarmDefCalibrationPremonitor> ReadDtAlarmDefCalibrationPremonitor(CalibrationPredictiveResutLog calibrationPredictiveResutLog)
        {
            IEnumerable<DtAlarmDefCalibrationPremonitor> models = null;
            try
            {
                _logger.EnterJson("{0}", calibrationPredictiveResutLog);

                List<DBAccessor.Models.DtAlarmDefCalibrationPremonitor> entities = null;
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        entities = db.DtAlarmDefCalibrationPremonitor
                                        .Where(x => string.IsNullOrEmpty(x.TypeCode) || x.TypeCode == calibrationPredictiveResutLog.TypeCode)
                                        .Where(x => string.IsNullOrEmpty(x.ErrorCode) || x.ErrorCode == calibrationPredictiveResutLog.ErrorCode)
                                        .ToList();
                    }
                });

                if (entities != null)
                {
                    models = entities.Select(x => x.ToModel());
                }

                return models;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_ALARM_DEF_CALIBRATION_PREMONITORテーブルのSelectに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", models);
            }
        }
    }
}
