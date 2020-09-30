using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rms.Server.Core.Utility;
using Rms.Server.Utility.Service.Services;
using Rms.Server.Utility.Utility.Extensions;
using Rms.Server.Utility.Utility.Models;
using Rms.Server.Utility.Utility.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Rms.Server.Utility.Utility.LogAnalysisDllWrapper;

namespace Rms.Server.Utility.Azure.Functions.DipAlmiLogAnalyzer
{
    /// <summary>
    /// 骨塩アルミスロープログ解析アプリ
    /// </summary>
    public class DipAlmiLogAnalyzerController
    {
        /// <summary>
        /// イベントハブ名（メッセージスキーマID）
        /// </summary>
        private const string EventHubName = "ms-012";

        /// <summary>
        /// 骨塩アルミスロープログ解析サービス
        /// </summary>
        private readonly IDipAlmiLogAnalyzerService _service;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="service">骨塩アルミスロープログ解析サービスクラス</param>
        public DipAlmiLogAnalyzerController(IDipAlmiLogAnalyzerService service)
        {
            Assert.IfNull(service);

            _service = service;
        }

        /// <summary>
        /// 骨塩アルミスロープログを受信する
        /// </summary>
        /// <remarks>sd 07-03.アルミログ解析</remarks>
        /// <param name="message">メッセージ</param>
        /// <param name="log">ロガー</param>
        [FunctionName("DipAlmiLogAnalyzer")]
        public void ReceiveDipAlmiSlopeLog([EventHubTrigger(EventHubName, Connection = "ConnectionString")] string message, ILogger log)
        {
            log.EnterJson("{0}", message);
            try
            {
                var eventSchemaList = JsonConvert.DeserializeObject<List<AzureEventGridEventSchema>>(message);
                foreach (var eventSchema in eventSchemaList)
                {
                    string messageId = null;
                    try
                    {
                        var deviceTelemetry = JsonConvert.DeserializeObject<IotHubDeviceTelemetry>(eventSchema.Data.ToStringJson());
                        messageId = deviceTelemetry?.SystemProperties?.MessageId;
                        var dipAlmiSlopeLog = JsonConvert.DeserializeObject<DipAlmiSlopeLog>(deviceTelemetry.Body.ToStringJson());

                        // バリデーション
                        Validator.ValidateObject(dipAlmiSlopeLog, new ValidationContext(dipAlmiSlopeLog, null, null));

                        // 解析を行う前にメッセージのログファイル名をもとにDBの既存データと比較
                        if (_service.CheckDuplicateAnalysisReuslt(dipAlmiSlopeLog.LogFileName, messageId))
                        {
                            // Sq1.1.1: アルミスロープログを解析する
                            if (_service.AnalyzeDipAlmiLog(dipAlmiSlopeLog, messageId, out AlmiLogAnalysisData _analysisData, out AlmiLogAnalysisResult _analysisResult))
                            {
                                // Sq1.1.2: 解析結果を保存する
                                if (_service.RegistAlmiLogAnalysisResultToDb(dipAlmiSlopeLog, messageId, _analysisData, _analysisResult, out DtAlmilogAnalysisResult model))
                                {
                                    // 解析結果保存完了
                                    log.Info(nameof(Resources.UT_DAA_DAA_007), new object[] { messageId });
                                    continue;
                                }
                            }
                        }
                    }
                    catch (ValidationException e)
                    {
                        // 受信メッセージフォーマット異常（基本設計書 5.1.13.1 エラー処理）
                        log.Error(e, nameof(Resources.UT_DAA_DAA_002), new object[] { messageId, e.Message });
                    }
                    catch (Exception e)
                    {
                        log.Error(e, nameof(Resources.UT_DAA_DAA_001), new object[] { messageId });
                    }

                    // 失敗した場合はFailureストレージに書き込み
                    _service.UpdateToFailureStorage(EventHubName, messageId, JsonConvert.SerializeObject(new AzureEventGridEventSchema[] { eventSchema }));
                }
            }
            catch (Exception e)
            {
                log.Error(e, nameof(Resources.UT_DAA_DAA_001), new object[] { null });

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
