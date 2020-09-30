using Microsoft.Azure.Devices.Common.Exceptions;
using Polly;
using Polly.Retry;
using Rms.Server.Core.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rms.Server.Core.Abstraction.Pollies
{
    /// <summary>
    /// IotHubPolly
    /// </summary>
    public class IotHubPolly
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
        /// リトライしないエラー
        /// </summary>
        /// <remarks>
        /// 基本的なAzureのリトライポリシーに従い、確実にリトライ不要というものを対象としている。
        /// </remarks>
        private readonly List<Type> noRetryTypes = new List<Type>()
        {
            typeof(ArgumentException),
            typeof(UnauthorizedException),
            typeof(OperationCanceledException)
        };

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">アプリケーション設定</param>
        public IotHubPolly(AppSettings settings)
        {
            this.settings = settings;

            // IoTHubのSDKは再試行機能を持たないため、使用者側でリトライを行う必要がある。
            // 基本的な再試行ポリシーはAzureのプラクティスに従う。
            // すなわち、「確実に再試行が無駄な物以外はリトライを行う」
            //// https://github.com/Azure/azure-iot-sdk-csharp/blob/master/iothub/service/src/AmqpServiceClient.cs

            retryPolicy =
                Policy.Handle<Exception>(ex => !noRetryTypes.Contains(ex.GetType()))
                .OrInner<Exception>(ex => !noRetryTypes.Contains(ex.GetType()))
                .WaitAndRetry(settings.IotHubMaxRetryAttempts, retryCount => TimeSpan.FromSeconds(retryCount * settings.IotHubDelayDeltaSeconds));

            asyncRetryPolicy =
                Policy.Handle<Exception>(ex => !noRetryTypes.Contains(ex.GetType()))
                .OrInner<Exception>(ex => !noRetryTypes.Contains(ex.GetType()))
                .WaitAndRetryAsync(settings.IotHubMaxRetryAttempts, retryCount => TimeSpan.FromSeconds(retryCount * settings.IotHubDelayDeltaSeconds));
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
