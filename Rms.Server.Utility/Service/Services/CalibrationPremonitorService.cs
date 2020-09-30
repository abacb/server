using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rms.Server.Core.Abstraction.Models;
using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Operation.Utility.Models;
using Rms.Server.Utility.Abstraction.Repositories;
using Rms.Server.Utility.Utility;
using Rms.Server.Utility.Utility.Extensions;
using Rms.Server.Utility.Utility.Models;
using Rms.Server.Utility.Utility.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rms.Server.Utility.Service.Services
{
    /// <summary>
    /// CalibrationPremonitorService
    /// </summary>
    public class CalibrationPremonitorService : ICalibrationPremonitorService
    {
        /// <summary>
        /// 設定
        /// </summary>
        private readonly UtilityAppSettings _settings;

        /// <summary>
        /// ロガー
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// DateTimeの提供元
        /// </summary>
        private readonly ITimeProvider _timeProvider;

        /// <summary>
        /// キャリブレーション予兆監視アラーム定義リポジトリ
        /// </summary>
        private readonly IDtAlarmDefCalibrationPremonitorRepository _dtAlarmDefCalibrationPremonitorRepository;

        /// <summary>
        /// Queueリポジトリ
        /// </summary>
        private readonly IQueueRepository _queueRepository;

        /// <summary>
        /// Failureストレージリポジトリ
        /// </summary>
        private readonly IFailureRepository _failureRepository;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">設定</param>
        /// <param name="logger">ロガー</param>
        /// <param name="timeProvider">DateTimeの提供元</param>
        /// <param name="dtAlarmDefCalibrationPremonitorRepository">キャリブレーション予兆監視アラーム定義リポジトリ</param>
        /// <param name="queueRepository">Queueリポジトリ</param>
        /// <param name="failureRepository">Failureストレージリポジトリ</param>
        public CalibrationPremonitorService(
            UtilityAppSettings settings,
            ILogger<CalibrationPremonitorService> logger,
            ITimeProvider timeProvider,
            IDtAlarmDefCalibrationPremonitorRepository dtAlarmDefCalibrationPremonitorRepository,
            IQueueRepository queueRepository,
            IFailureRepository failureRepository)
        {
            Assert.IfNull(settings);
            Assert.IfNull(logger);
            Assert.IfNull(timeProvider);
            Assert.IfNull(dtAlarmDefCalibrationPremonitorRepository);
            Assert.IfNull(queueRepository);
            Assert.IfNull(failureRepository);

            _settings = settings;
            _logger = logger;
            _timeProvider = timeProvider;
            _dtAlarmDefCalibrationPremonitorRepository = dtAlarmDefCalibrationPremonitorRepository;
            _queueRepository = queueRepository;
            _failureRepository = failureRepository;
        }

        /// <summary>
        /// アラーム定義を取得する
        /// </summary>
        /// <param name="calibrationPredictiveResutLog">キャリブレーション予兆結果ログ</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="models">DBから取得したパラメータ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        public bool ReadAlarmDefinition(CalibrationPredictiveResutLog calibrationPredictiveResutLog, string messageId, out IEnumerable<DtAlarmDefCalibrationPremonitor> models)
        {
            models = null;

            try
            {
                _logger.EnterJson("{0}", new { calibrationPredictiveResutLog, messageId });

                // Sq1.1.1: キャリブレーション予兆監視アラーム定義を取得
                models = _dtAlarmDefCalibrationPremonitorRepository.ReadDtAlarmDefCalibrationPremonitor(calibrationPredictiveResutLog);

                return true;
            }
            catch (RmsException e)
            {
                // DBにアクセスできない（基本設計書 5.1.2.4 エラー処理）
                _logger.Error(e, nameof(Resources.UT_CLP_CLP_003), new object[] { messageId });
                return false;
            }
            finally
            {
                _logger.LeaveJson("{0}", new { models });
            }
        }

        /// <summary>
        /// アラーム情報を作成しQueueStorageへ登録する
        /// </summary>
        /// <param name="calibrationPredictiveResutLog">キャリブレーション予兆結果ログ</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="alarmDef">アラーム定義</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        public bool CreateAndEnqueueAlarmInfo(CalibrationPredictiveResutLog calibrationPredictiveResutLog, string messageId, IEnumerable<DtAlarmDefCalibrationPremonitor> alarmDef)
        {
            bool result = true;
            _logger.EnterJson("{0}", new { calibrationPredictiveResutLog, messageId, alarmDef });

            int index = 1;
            int alarmCount = alarmDef.Count();

            foreach (var alarm in alarmDef)
            {
                string message = null;
                try
                {
                    // Sq1.1.3: アラームキューを生成する
                    var alarmInfo = new AlarmInfo
                    {
                        SourceEquipmentUid = calibrationPredictiveResutLog.SourceEquipmentUid,
                        TypeCode = calibrationPredictiveResutLog.TypeCode,
                        ErrorCode = calibrationPredictiveResutLog.ErrorCode,
                        AlarmLevel = alarm.AlarmLevel,
                        AlarmTitle = alarm.AlarmTitle,
                        AlarmDescription = alarm.AlarmDescription,
                        AlarmDatetime = _timeProvider.UtcNow.ToString(Utility.Const.AlarmQueueDateTimeFormat),
                        EventDatetime = calibrationPredictiveResutLog.EventDt,
                        AlarmDefId = $"{_settings.SystemName}_{_settings.SubSystemName}_{alarm.Sid.ToString()}",
                        MessageId = alarmCount <= 1 ? messageId : $"{messageId}_{index}"
                    };
                    index++;

                    message = JsonConvert.SerializeObject(alarmInfo);

                    // Sq1.1.4: キューを登録する
                    _queueRepository.SendMessageToAlarmQueue(message);
                }
                catch (Exception e)
                {
                    // アラーム生成エラー or アラームキューにアラーム情報を送信できない（基本設計書 5.1.2.4 エラー処理）
                    _logger.Error(e, string.IsNullOrEmpty(message) ? nameof(Resources.UT_CLP_CLP_005) : nameof(Resources.UT_CLP_CLP_006), new object[] { messageId });
                    result = false;
                }
            }

            _logger.Leave();
            return result;
        }

        /// <summary>
        /// Failureストレージに再送用メッセージをアップロードする
        /// </summary>
        /// <param name="messageSchemaId">メッセージスキーマID</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="message">メッセージ</param>
        public void UpdateToFailureStorage(string messageSchemaId, string messageId, string message)
        {
            try
            {
                _logger.EnterJson("{0}", new { messageSchemaId, messageId, message });

                DateTime now = _timeProvider.UtcNow;
                bool noMessageId = string.IsNullOrEmpty(messageId);

                // ファイル情報
                ArchiveFile file = new ArchiveFile() { ContainerName = _settings.FailureBlobContainerName, CreatedAt = now };
                if (noMessageId)
                {
                    file.FilePath = string.Format(Utility.Const.FailureBlobFilenameFormatWithoutMessageId, messageSchemaId, messageId, now);
                }
                else
                {
                    file.FilePath = string.Format(Utility.Const.FailureBlobFilenameFormat, messageSchemaId, messageId, now);
                }

                // アップロード
                _failureRepository.Upload(file, message, noMessageId);
            }
            catch (RmsException e)
            {
                // Blobストレージへの保存処理に失敗した場合、メッセージ内容をログに出力して終了する。
                _logger.Error(e, nameof(Resources.UT_CLP_CLP_008), new object[] { messageId, message });
            }
            finally
            {
                _logger.Leave();
            }
        }
    }
}
