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
using System.Linq;

namespace Rms.Server.Utility.Azure.Functions.FailurePremonitor
{
    /// <summary>
    /// 故障予兆監視アプリ
    /// </summary>
    public class FailurePremonitorController
    {
        /// <summary>
        /// イベントハブ名（メッセージスキーマID）
        /// </summary>
        private const string EventHubName = "ms-017";

        /// <summary>
        /// 設定
        /// </summary>
        private readonly UtilityAppSettings _settings;

        /// <summary>
        /// 故障予兆監視サービスクラス
        /// </summary>
        private readonly IFailurePremonitorService _service;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">設定</param>
        /// <param name="service">故障予兆監視サービスクラス</param>
        public FailurePremonitorController(UtilityAppSettings settings, IFailurePremonitorService service)
        {
            Assert.IfNull(settings);
            Assert.IfNull(service);

            _settings = settings;
            _service = service;
        }

        /// <summary>
        /// 故障予兆結果ログを受信する
        /// </summary>
        /// <remarks>sd 07-02.故障予兆監視</remarks>
        /// <param name="message">メッセージ</param>
        /// <param name="log">ロガー</param>
        [FunctionName("FailurePremonitor")]
        public void ReceiveFailurePredictiveResultLog([EventHubTrigger(EventHubName, Connection = "ConnectionString")] string message, ILogger log)
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
                        var failurePredictiveResultLog = JsonConvert.DeserializeObject<FailurePredictiveResultLog>(deviceTelemetry.Body.ToStringJson());

                        // バリデーション
                        Validator.ValidateObject(failurePredictiveResultLog, new ValidationContext(failurePredictiveResultLog, null, null));

                        // Sq1.1.1: 故障予兆監視アラーム定義を取得
                        if (_service.ReadAlarmDefinition(failurePredictiveResultLog, messageId, out IEnumerable<DtAlarmDefFailurePremonitor> models))
                        {
                            // Sq1.1.2: アラーム生成の要否を判断する
                            bool needsAlarm = models != null && models.Count() > 0;
                            if (!needsAlarm)
                            {
                                // アラーム生成不要
                                log.Info(nameof(Resources.UT_FLP_FLP_004), new object[] { messageId });
                                continue;
                            }
                            else
                            {
                                // Sq1.1.3: アラームキューを生成する
                                // Sq1.1.4: キューを登録する
                                if (_service.CreateAndEnqueueAlarmInfo(failurePredictiveResultLog, messageId, models))
                                {
                                    // アラームキュー登録完了
                                    log.Info(nameof(Resources.UT_FLP_FLP_007), new object[] { messageId });
                                    continue;
                                }
                            }
                        }
                    }
                    catch (ValidationException e)
                    {
                        // 受信メッセージフォーマット異常（基本設計書 5.1.2.4 エラー処理）
                        log.Error(e, nameof(Resources.UT_FLP_FLP_002), new object[] { messageId, e.Message });
                    }
                    catch (Exception e)
                    {
                        log.Error(e, nameof(Resources.UT_FLP_FLP_001), new object[] { messageId });
                    }

                    // 失敗した場合はFailureストレージに書き込み
                    _service.UpdateToFailureStorage(EventHubName, messageId, JsonConvert.SerializeObject(new AzureEventGridEventSchema[] { eventSchema }));
                }
            }
            catch (RmsInvalidAppSettingException e)
            {
                log.Error(e, nameof(Resources.UT_FLP_FLP_009), new object[] { e.Message });

                // 失敗した場合はFailureストレージに書き込み
                _service.UpdateToFailureStorage(EventHubName, null, message);
            }
            catch (Exception e)
            {
                log.Error(e, nameof(Resources.UT_FLP_FLP_001), new object[] { null });

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
