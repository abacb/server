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
using System.Linq;

namespace Rms.Server.Utility.Service.Services
{
    /// <summary>
    /// SmartPremonitorService
    /// </summary>
    public class DiskDrivePremonitorService : IDiskDrivePremonitorService
    {
        /// <summary>
        /// SMART属性情報ID:C5
        /// </summary>
        private const string SmartAttributeInfoIdC5 = "C5";

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
        /// ディスクドライブ予兆監視アラーム定義リポジトリ
        /// </summary>
        private readonly IDtAlarmSmartPremonitorRepository _dtAlarmSmartPremonitorRepository;

        /// <summary>
        /// SMART解析判定結果リポジトリ
        /// </summary>
        private readonly IDtSmartAnalysisResultRepository _dtSmartAnalysisResultRepository;

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
        /// <param name="dtAlarmSmartPremonitorRepository">ディスクドライブ予兆監視アラーム定義リポジトリ</param>
        /// <param name="dtSmartAnalysisResultRepository">SMART解析判定結果リポジトリ</param>
        /// <param name="queueRepository">Queueリポジトリ</param>
        /// <param name="failureRepository">Failureストレージリポジトリ</param>
        public DiskDrivePremonitorService(
            UtilityAppSettings settings,
            ILogger<DiskDrivePremonitorService> logger,
            ITimeProvider timeProvider,
            IDtAlarmSmartPremonitorRepository dtAlarmSmartPremonitorRepository,
            IDtSmartAnalysisResultRepository dtSmartAnalysisResultRepository,
            IQueueRepository queueRepository,
            IFailureRepository failureRepository)
        {
            Assert.IfNull(settings);
            Assert.IfNull(logger);
            Assert.IfNull(timeProvider);
            Assert.IfNull(dtAlarmSmartPremonitorRepository);
            Assert.IfNull(dtSmartAnalysisResultRepository);
            Assert.IfNull(queueRepository);
            Assert.IfNull(failureRepository);

            _settings = settings;
            _logger = logger;
            _timeProvider = timeProvider;
            _dtAlarmSmartPremonitorRepository = dtAlarmSmartPremonitorRepository;
            _dtSmartAnalysisResultRepository = dtSmartAnalysisResultRepository;
            _queueRepository = queueRepository;
            _failureRepository = failureRepository;
        }

        /// <summary>
        /// 登録済みのSMART解析結果判定テーブルを取得する
        /// </summary>
        /// <param name="diskDrive">ディスクドライブ</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="model">DBから取得したパラメータ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        public bool ReadSmartAnalysisResult(DiskDrive diskDrive, string messageId, out DtSmartAnalysisResult model)
        {
            model = null;

            try
            {
                _logger.EnterJson("{0}", new { diskDrive, messageId });

                // SMART解析判定結果を取得
                model = _dtSmartAnalysisResultRepository.ReadDtSmartAnalysisResult(diskDrive);

                return true;
            }
            catch (RmsException e)
            {
                // DBにアクセスできない（基本設計書 5.1.2.4 エラー処理）
                _logger.Error(e, nameof(Resources.UT_DDP_DDP_004), new object[] { messageId });
                return false;
            }
            finally
            {
                _logger.LeaveJson("{0}", new { model });
            }
        }

        /// <summary>
        /// アラーム定義を取得する
        /// </summary>
        /// <param name="messageId">メッセージID</param>
        /// <param name="model">DBから取得したパラメータ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        public bool ReadAlarmDefinition(string messageId, out DtAlarmSmartPremonitor model)
        {
            model = null;

            try
            {
                _logger.EnterJson("{0}", new { messageId });

                // ディスクドライブ予兆監視アラーム定義を取得
                model = _dtAlarmSmartPremonitorRepository.ReadDtAlarmSmartPremonitor(SmartAttributeInfoIdC5);

                return true;
            }
            catch (RmsException e)
            {
                // DBにアクセスできない（基本設計書 5.1.2.4 エラー処理）
                _logger.Error(e, nameof(Resources.UT_DDP_DDP_003), new object[] { messageId });
                return false;
            }
            finally
            {
                _logger.LeaveJson("{0}", new { model });
            }
        }

        /// <summary>
        /// 受信メッセージからID=C5のSMART属性情報を取得する
        /// </summary>
        /// <param name="diskDrive">ディスクドライブ</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="smartAttirubteInfoC5">SMART属性情報C5</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        public bool ReadSmartAttirubteInfoC5(DiskDrive diskDrive, string messageId, out DiskDrive.SmartAttributeInfoSchema smartAttirubteInfoC5)
        {
            smartAttirubteInfoC5 = null;

            try
            {
                _logger.EnterJson("{0}", new { diskDrive, messageId });

                // ID=C5のSMART属性情報を取得
                var smartAttributeInfos = diskDrive.SmartAttributeInfo?.Where(x => x.Id == SmartAttributeInfoIdC5);

                // SMART属性情報が存在しない場合は正常
                if (smartAttributeInfos == null || smartAttributeInfos.Count() == 0)
                {
                    return true;
                }

                // SMART属性情報が複数存在する場合はエラー
                if (smartAttributeInfos.Count() > 1)
                {
                    throw new RmsException("SMART属性情報が複数存在しています(ID=C5)");
                }

                smartAttirubteInfoC5 = smartAttributeInfos.First();

                // SMART属性情報のC5生の値が存在しない場合はエラー
                if (string.IsNullOrEmpty(smartAttirubteInfoC5.RawData))
                {
                    throw new RmsException("SMART属性情報の生の値がnullまたは空文字です(ID=C5)");
                }

                return true;
            }
            catch (RmsException e)
            {
                // SMART属性情報異常
                _logger.Error(e, nameof(Resources.UT_DDP_DDP_002), new object[] { messageId, e.Message });
                return false;
            }
            finally
            {
                _logger.LeaveJson("{0}", new { smartAttirubteInfoC5 });
            }
        }

        /// <summary>
        /// アラーム判定を実行しSMART解析判定結果を更新する
        /// </summary>
        /// <param name="diskDrive">ディスクドライブ</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="analysisResult">解析判定結果</param>
        /// <param name="alarmDef">アラーム定義</param>
        /// <param name="smartAttirubteInfoC5">SMART属性情報C5</param>
        /// <param name="needsAlarmCreation">アラーム生成要・不要フラグ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        public bool JudgeAlarmCreationAndUpdateSmartAnalysisResult(DiskDrive diskDrive, string messageId, DtSmartAnalysisResult analysisResult, DtAlarmSmartPremonitor alarmDef, DiskDrive.SmartAttributeInfoSchema smartAttirubteInfoC5, out bool needsAlarmCreation)
        {
            needsAlarmCreation = false;

            try
            {
                _logger.EnterJson("{0}", new { diskDrive, smartAttirubteInfoC5, messageId, analysisResult, alarmDef });

                if (alarmDef == null || smartAttirubteInfoC5 == null)
                {
                    // アラーム生成、解析結果更新不要
                    return true;
                }

                if (analysisResult == null)
                {
                    // 解析結果がまだ登録されていない場合はアラーム判定を行わずに登録する
                    // TODO: ER図の変位回数閾値(ChangeCountThreshold)が整数値に修正されるので後で修正する
                    var c5RawData = ConvertRawDataStringToLong(smartAttirubteInfoC5.RawData);

                    var registAnalysisResult = new DtSmartAnalysisResult
                    {
                        EquipmentUid = diskDrive.SourceEquipmentUid,
                        DiskSerialNumber = diskDrive.SerialNo,
                        C5RawData = c5RawData,
                        C5RawDataChanges = 0,
                        C5ChangesThreshhold = short.Parse(alarmDef.ChangeCountThreshold),
                        C5ChangesThreshholdOverCount = 0,
                        C5ChangesThreshholdLastDatetime = null
                    };

                    _dtSmartAnalysisResultRepository.CreateDtSmartAnalysisResult(registAnalysisResult);
                }
                else
                {
                    var c5RawData = ConvertRawDataStringToLong(smartAttirubteInfoC5.RawData);

                    var c5RawDataDiff = c5RawData - analysisResult.C5RawData;

                    if (c5RawDataDiff > 0)
                    {
                        analysisResult.C5RawData = c5RawData;
                        analysisResult.C5RawDataChanges++;

                        // TODO: ER図の変位回数閾値(ChangeCountThreshold)が整数値に修正されるので後で修正する
                        if (short.Parse(alarmDef.ChangeCountThreshold) <= analysisResult.C5RawDataChanges)
                        {
                            analysisResult.C5ChangesThreshholdOverCount++;
                            analysisResult.C5ChangesThreshholdLastDatetime = DateTime.Parse(diskDrive.CollectDt, null, System.Globalization.DateTimeStyles.RoundtripKind);
                            needsAlarmCreation = true;
                        }
                    }
                    else if (c5RawDataDiff < 0)
                    {
                        analysisResult.C5RawData = c5RawData;
                    }

                    // C5生の値の差分が0の場合も日時の更新は必要
                    _dtSmartAnalysisResultRepository.UpdateDtSmartAnalysisResult(analysisResult);
                }

                return true;
            }
            catch (RmsException e)
            {
                // DBにアクセスできない（基本設計書 5.1.2.4 エラー処理）
                _logger.Error(e, nameof(Resources.UT_DDP_DDP_005), new object[] { messageId });
                return false;
            }
            finally
            {
                _logger.LeaveJson("{0}", new { needsAlarmCreation });
            }
        }

        /// <summary>
        /// アラーム情報を作成しQueueStorageへ登録する
        /// </summary>
        /// <param name="diskDrive">ディスクドライブ</param>
        /// <param name="smartAttirubteInfoC5">SMART属性情報(C5)</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="analysisResult">解析判定結果</param>
        /// <param name="alarmDef">アラーム定義</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        public bool CreateAndEnqueueAlarmInfo(DiskDrive diskDrive, DiskDrive.SmartAttributeInfoSchema smartAttirubteInfoC5, string messageId, DtSmartAnalysisResult analysisResult, DtAlarmSmartPremonitor alarmDef)
        {
            _logger.EnterJson("{0}", new { diskDrive, smartAttirubteInfoC5, messageId, analysisResult, alarmDef });

            string message = null;
            try
            {
                // アラームキューを生成する
                var alarmInfo = new AlarmInfo
                {
                    SourceEquipmentUid = diskDrive.SourceEquipmentUid,
                    TypeCode = string.Empty,
                    ErrorCode = alarmDef.AnalysisResultErrorCode,
                    AlarmLevel = alarmDef.AlarmLevel,
                    AlarmTitle = alarmDef.AlarmTitle,
                    AlarmDescription = string.Format(alarmDef.AlarmDescription, analysisResult.DiskSerialNumber, analysisResult.C5ChangesThreshholdOverCount, analysisResult.C5ChangesThreshholdLastDatetime?.ToString(Utility.Const.AlarmQueueDateTimeFormat)),
                    AlarmDatetime = _timeProvider.UtcNow.ToString(Utility.Const.AlarmQueueDateTimeFormat),
                    EventDatetime = diskDrive.CollectDt,
                    AlarmDefId = $"{_settings.SystemName}_{_settings.SubSystemName}_{alarmDef.Sid.ToString()}",
                    MessageId = messageId
                };

                message = JsonConvert.SerializeObject(alarmInfo);

                // キューを登録する
                _queueRepository.SendMessageToAlarmQueue(message);

                return true;
            }
            catch (Exception e)
            {
                // アラーム生成エラー or アラームキューにアラーム情報を送信できない（基本設計書 5.1.2.4 エラー処理）
                _logger.Error(e, string.IsNullOrEmpty(message) ? nameof(Resources.UT_DDP_DDP_007) : nameof(Resources.UT_DDP_DDP_008), new object[] { messageId });
                return false;
            }
            finally
            {
                _logger.Leave();
            }
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
                _logger.Error(e, nameof(Resources.UT_DDP_DDP_010), new object[] { messageId, message });
            }
            finally
            {
                _logger.Leave();
            }
        }

        /// <summary>
        /// 文字列の生の値を数値に変換する
        /// </summary>
        /// <param name="rawDataString">文字列の生の値</param>
        /// <returns>数値の生の値</returns>
        private long ConvertRawDataStringToLong(string rawDataString)
        {
            var byteStringArray = rawDataString.Split('-');
            Array.Reverse(byteStringArray);
            var byteArray = new byte[sizeof(long)];

            for (int i = 0; i < byteStringArray.Length; i++)
            {
                byteArray[i] = Convert.ToByte(byteStringArray[i], 16);
            }

            return BitConverter.ToInt64(byteArray, 0);
        }
    }
}
