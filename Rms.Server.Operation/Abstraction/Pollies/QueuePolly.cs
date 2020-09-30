using Polly;
using Polly.Retry;
using Rms.Server.Operation.Utility;
using System;

namespace Rms.Server.Core.Abstraction.Pollies
{
    /// <summary>
    /// キュー接続用のPollyクラス
    /// </summary>
    public class QueuePolly
    {
        /// <summary>アプリケーション設定</summary>
        private readonly OperationAppSettings settings;

        /// <summary>リトライポリシー</summary>
        private RetryPolicy retryPolicy;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">アプリケーション設定</param>
        public QueuePolly(OperationAppSettings settings)
        {
            this.settings = settings;
            this.retryPolicy =
                Policy.Handle<Exception>()
                .WaitAndRetry(settings.MailQueueAccessMaxAttempts, retryCount => TimeSpan.FromSeconds(retryCount * settings.MailQueueDelayDeltaSeconds));
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
