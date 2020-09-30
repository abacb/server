using Polly;
using Polly.Retry;
using Rms.Server.Operation.Utility;
using Rms.Server.Utility.Utility;
using System;

namespace Rms.Server.Core.Abstraction.Pollies
{
    /// <summary>
    /// キュー接続用のPollyクラス
    /// </summary>
    public class QueuePolly
    {
        /// <summary>アプリケーション設定</summary>
        private readonly UtilityAppSettings settings;

        /// <summary>リトライポリシー</summary>
        private RetryPolicy retryPolicy;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">アプリケーション設定</param>
        public QueuePolly(UtilityAppSettings settings)
        {
            this.settings = settings;
            this.retryPolicy =
                Policy.Handle<Exception>()
                .WaitAndRetry(settings.AlarmQueueAccessMaxAttempts, retryCount => TimeSpan.FromSeconds(retryCount * settings.AlarmQueueDelayDeltaSeconds));
        }

        /// <summary>
        /// 引数に指定したActionを実行する
        /// </summary>
        /// <param name="action">実行するAction</param>
        public void Execute(Action action)
        {
            retryPolicy.Execute(action);
        }
    }
}
