using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Utility.Service.Services;
using Rms.Server.Utility.Utility;
using Rms.Server.Utility.Utility.Extensions;
using Rms.Server.Utility.Utility.Models;
using Rms.Server.Utility.Utility.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Utility.Azure.Functions.DiskDrivePremonitor
{
    /// <summary>
    /// ディスクドライブ予兆監視アプリ
    /// </summary>
    public class DiskDrivePremonitorController
    {
        /// <summary>
        /// イベントハブ名（メッセージスキーマID）
        /// </summary>
        private const string EventHubName = "ms-027";

        /// <summary>
        /// 設定
        /// </summary>
        private readonly UtilityAppSettings _settings;

        /// <summary>
        /// ディスクドライブ予兆監視サービスクラス
        /// </summary>
        private readonly IDiskDrivePremonitorService _service;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">設定</param>
        /// <param name="service">ディスクドライブ予兆監視サービスクラス</param>
        public DiskDrivePremonitorController(UtilityAppSettings settings, IDiskDrivePremonitorService service)
        {
            Assert.IfNull(settings);
            Assert.IfNull(service);

            _settings = settings;
            _service = service;
        }

        /// <summary>
        /// ディスクドライブを受信する
        /// </summary>
        /// <remarks>ディスクドライブ予兆監視</remarks>
        /// <param name="message">メッセージ</param>
        /// <param name="log">ロガー</param>
        [FunctionName("DiskDrivePremonitor")]
        public void ReceiveDiskDrive([EventHubTrigger(EventHubName, Connection = "ConnectionString")] string message, ILogger log)
        {
            log.EnterJson("{0}", message);
            try
            {
                // アプリケーション設定でSystem名・SubSystem名が設定されていない場合、実行時にエラーとする
                if (string.IsNullOrEmpty(_settings.SystemName))
                {
                    throw new RmsInvalidAppSettingException($"{nameof(_settings.SystemName)} is required.");
                }

                if (string.IsNullOrEmpty(_settings.SubSystemName))
                {
                    throw new RmsInvalidAppSettingException($"{nameof(_settings.SubSystemName)} is required.");
                }

                var eventSchemaList = JsonConvert.DeserializeObject<List<AzureEventGridEventSchema>>(message);
                foreach (var eventSchema in eventSchemaList)
                {
                    string messageId = null;
                    try
                    {
                        var deviceTelemetry = JsonConvert.DeserializeObject<IotHubDeviceTelemetry>(eventSchema.Data.ToStringJson());
                        messageId = deviceTelemetry?.SystemProperties?.MessageId;
                        var diskDrive = JsonConvert.DeserializeObject<DiskDrive>(deviceTelemetry.Body.ToStringJson());

                        // バリデーション
                        Validator.ValidateObject(diskDrive, new ValidationContext(diskDrive, null, null));

                        // 受信メッセージからID=C5のSMART属性情報を取得
                        if (_service.ReadSmartAttirubteInfoC5(diskDrive, messageId, out DiskDrive.SmartAttributeInfoSchema smartAttributeInfoC5))
                        {
                            // ディスクドライブ予兆監視アラーム定義を取得
                            if (_service.ReadAlarmDefinition(messageId, out DtAlarmSmartPremonitor alarmDef))
                            {
                                // 登録済みのSMART解析結果を取得
                                if (_service.ReadSmartAnalysisResult(diskDrive, messageId, out DtSmartAnalysisResult analysisResult))
                                {
                                    // アラーム判定と解析結果の更新を行う
                                    if (_service.JudgeAlarmCreationAndUpdateSmartAnalysisResult(diskDrive, messageId, analysisResult, alarmDef, smartAttributeInfoC5, out bool needsAlarmCreation))
                                    {
                                        if (!needsAlarmCreation)
                                        {
                                            log.Info(nameof(Resources.UT_DDP_DDP_006), new object[] { messageId });
                                            continue;
                                        }
                                        else
                                        {
                                            // アラームキューを生成する
                                            // キューを登録する
                                            if (_service.CreateAndEnqueueAlarmInfo(diskDrive, smartAttributeInfoC5, messageId, analysisResult, alarmDef))
                                            {
                                                log.Info(nameof(Resources.UT_DDP_DDP_009), new object[] { messageId });
                                                continue;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Newtonsoft.Json.JsonReaderException e)
                    {
                        // 受信メッセージフォーマット異常（基本設計書 5.1.2.4 エラー処理）
                        log.Error(e, nameof(Resources.UT_DDP_DDP_002), new object[] { messageId, e.Message });
                    }
                    catch (ValidationException e)
                    {
                        // 受信メッセージフォーマット異常（基本設計書 5.1.2.4 エラー処理）
                        log.Error(e, nameof(Resources.UT_DDP_DDP_002), new object[] { messageId, e.Message });
                    }
                    catch (Exception e)
                    {
                        log.Error(e, nameof(Resources.UT_DDP_DDP_001), new object[] { messageId });
                    }

                    // 失敗した場合はFailureストレージに書き込み
                    _service.UpdateToFailureStorage(EventHubName, messageId, JsonConvert.SerializeObject(new AzureEventGridEventSchema[] { eventSchema }));
                }
            }
            catch (RmsInvalidAppSettingException e)
            {
                log.Error(e, nameof(Resources.UT_DDP_DDP_011), new object[] { e.Message });

                // 失敗した場合はFailureストレージに書き込み
                _service.UpdateToFailureStorage(EventHubName, null, message);
            }
            catch (Exception e)
            {
                log.Error(e, nameof(Resources.UT_DDP_DDP_001), new object[] { null });

                // 失敗した場合はFailureストレージに書き込み
                _service.UpdateToFailureStorage(EventHubName, null, message);
            }
            finally
            {
                log.LeaveJson("{0}", message);
            }
        }
    }
}
