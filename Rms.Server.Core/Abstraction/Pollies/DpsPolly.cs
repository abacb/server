using Microsoft.Azure.Devices.Provisioning.Service;
using Polly;
using Polly.Retry;
using Rms.Server.Core.Utility;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Rms.Server.Core.Abstraction.Pollies
{
    /// <summary>
    /// DpsPolly
    /// </summary>
    public class DpsPolly
    {
        /// <summary>アプリケーション設定</summary>
        private readonly AppSettings settings;

        /// <summary>リトライポリシー</summary>
        private readonly RetryPolicy retryPolicy;

        /// <summary>
        /// 非同期リトライポリシー
        /// </summary>
        private readonly AsyncRetryPolicy asyncRetryPolicy;

        /// <summary>
        /// リトライしないステータスコード
        /// </summary>
        /// <remarks>
        /// 基本的なAzureのリトライポリシーに従い、確実にリトライ不要というものを対象としている。
        /// </remarks>
        private readonly List<HttpStatusCode> noRetryStatusCodes = new List<HttpStatusCode>()
        {
            HttpStatusCode.BadRequest,
            HttpStatusCode.Unauthorized,
            HttpStatusCode.NotFound
        };

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">アプリケーション設定</param>
        public DpsPolly(AppSettings settings)
        {
            Assert.IfNull(settings);

            this.settings = settings;

            // DPSのSDKは再試行機能を持たないため、使用者側でリトライを行う必要がある。
            // Microsoft.Azure.Devices.Provisioning.Service.ProvisioningServiceClient

            // 本SDKが返す例外は基本的にProvisioningServiceClientHttpExceptionになる。
            // そのプロパティのStatusCodeにより処理する。
            // その際取得されるエラーコードは以下。
            //// https://docs.microsoft.com/en-us/azure/iot-dps/how-to-troubleshoot-dps#common-error-codes

            retryPolicy =
                Policy.Handle<ProvisioningServiceClientHttpException>(ex => !noRetryStatusCodes.Contains(ex.StatusCode))
                .OrInner<ProvisioningServiceClientHttpException>(ex => !noRetryStatusCodes.Contains(ex.StatusCode))
                .WaitAndRetry(settings.DpsMaxRetryAttempts, retryCount => TimeSpan.FromSeconds(retryCount * settings.DpsDelayDeltaSeconds));

            asyncRetryPolicy =
                Policy.Handle<ProvisioningServiceClientHttpException>(ex => !noRetryStatusCodes.Contains(ex.StatusCode))
                .OrInner<ProvisioningServiceClientHttpException>(ex => !noRetryStatusCodes.Contains(ex.StatusCode))
                .WaitAndRetryAsync(settings.DpsMaxRetryAttempts, retryCount => TimeSpan.FromSeconds(retryCount * settings.DpsDelayDeltaSeconds));
        }

        /// <summary>
        /// 引数に指定したActionを実行する
        /// </summary>
        /// <param name="action">実行するAction</param>
        public void Execute(Action action)
        {
            retryPolicy.Execute(action);
        }

        /// <summary>
        /// 引数に指定したActionを実行する
        /// </summary>
        /// <param name="funcAsync">実行するAction</param>
        /// <returns>Task</returns>
        public async Task ExecuteAsync(Func<Task> funcAsync)
        {
            await asyncRetryPolicy.ExecuteAsync(funcAsync);
        }
    }
}
