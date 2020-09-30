using Polly;
using Polly.Retry;
using Rms.Server.Core.Utility;
using System;
using System.Threading.Tasks;

namespace Rms.Server.Core.Abstraction.Pollies
{
    /// <summary>
    /// Blob接続用のPollyクラス
    /// </summary>
    public class BlobPolly
    {
        /// <summary>アプリケーション設定</summary>
        private readonly AppSettings settings;

        /// <summary>リトライポリシー</summary>
        ////private RetryPolicy retryPolicy;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">アプリケーション設定</param>
        public BlobPolly(AppSettings settings)
        {
            this.settings = settings;
            //// AzureBlobSDKは自前でリトライ機能を持っているため、本項目はサーキットブレーカー対応する場合に修正する
            // https://docs.microsoft.com/ja-jp/azure/architecture/best-practices/retry-service-specific#azure-storage
            ////this.retryPolicy =
            //    Policy.Handle<SqlException>()
            //    .Or<AggregateException>(e => e.InnerExceptions.Any(i => i is SqlException))
            //    .WaitAndRetry(BlobAccessMaxAttempts, retryCount => TimeSpan.FromSeconds(retryCount * BlobAccessDelayDeltaSeconds));
        }

        /// <summary>
        /// 引数に指定したActionを実行する
        /// </summary>
        /// <param name="action">実行するAction</param>
        public void Execute(Action action)
        {
            // Azure Blobは基本的にSDKでリトライが実行されているため、retryはこちらでは実行しない。
            // 本クラスは、サーキットブレーカー対応時のための仕込みである。

            // また、BlobClentは基本的にAsyncしかないため、
            // Pollyを正しく使用するならば、StorageExceptionでなくAggregateExceptionに対して処理する必要がある。
            action();
        }

        ///// <summary>
        ///// 引数に指定したActionを実行する
        ///// </summary>
        ///// <param name="action">実行するAction</param>
        ////public async Task ExecuteAsync(Func<Task> funcAsync)
        ////{
        //    // Azure Blobは基本的にSDKでリトライが実行されているため、retryはこちらでは実行しない。
        //    // 本クラスは、サーキットブレーカー対応時のための仕込みである。
        //    await funcAsync();

        ////    //Policy.Handle().RetryAsync().ExecuteAsync(funcAsync);

        ////}
    }
}
