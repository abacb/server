using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Extensions;
using Rms.Server.Utility.Service.Services;
using Rms.Server.Utility.Utility;
using System;

namespace Rms.Server.Utility.Azure.Functions.DipAlmiLogPremonitorTest
{
    /// <summary>
    /// 骨塩アルミスロープログ予兆監視テスト
    /// </summary>
    public class DipAlmiLogPremonitorControllerTest
    {
        /// <summary>
        /// 設定
        /// </summary>
        private readonly UtilityAppSettings _settings;

        /// <summary>
        /// 骨塩アルミスロープログ予兆監視サービス
        /// </summary>
        private readonly IDipAlmiLogPremonitorService _service;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">設定</param>
        /// <param name="service">骨塩アルミスロープログ予兆監視サービスクラス</param>
        public DipAlmiLogPremonitorControllerTest(UtilityAppSettings settings, IDipAlmiLogPremonitorService service)
        {
            Assert.IfNull(settings);
            Assert.IfNull(service);

            _settings = settings;
            _service = service;
        }

        /// <summary>
        /// 骨塩アルミスロープログ予兆監視テスト
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="log">ロガー</param>
        [FunctionName("DipAlmiLogPremonitorTest")]
        public void DipAlmiLogPremonitorTest([EventHubTrigger("ms-010", Connection = "ConnectionString")] string message, ILogger log)
        {
            log.EnterJson("{0}", message);
            try
            {
                var result = _service.ReadAlarmJudgementTarget();
                _service.CreateAndEnqueueAlarmInfo(result.Entity);
                _service.UpdateAlarmJudgedAnalysisResult(result.Entity);
            }
            catch (Exception ex)
            {
                // [TODO]:Blobへクライアントメッセージを保存する処理を追加する(ControllerではなくServiceで実行する?)
                log.Error(ex, "想定外のエラー");
            }
            finally
            {
                log.LeaveJson("{0}", message);
            }
        }
    }
}
