using Polly;
using Polly.Retry;
using Rms.Server.Operation.Utility;
using SendGrid;
using System;
using System.Threading.Tasks;

namespace Rms.Server.Operation.Abstraction.Pollies
{
    /// <summary>
    /// SendGrid接続用のPollyクラス
    /// </summary>
    public class SendGridPolly
    {
        /// <summary>
        /// アプリケーション設定
        /// </summary>
        private readonly OperationAppSettings settings;

        /// <summary>
        /// 非同期リトライポリシー
        /// </summary>
        private readonly AsyncRetryPolicy<Response> asyncRetryPolicy;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">アプリケーション設定</param>
        public SendGridPolly(OperationAppSettings settings)
        {
            this.settings = settings;

            asyncRetryPolicy =
                Policy.Handle<Exception>()
                .OrResult<Response>(x => IsNeedRetry(x))
                .WaitAndRetryAsync(settings.SendGridAccessMaxAttempts, retryCount => TimeSpan.FromSeconds(retryCount * settings.SendGridDelayDeltaSeconds));
        }

        /// <summary>
        /// 引数に指定したActionを実行する
        /// </summary>
        /// <param name="funcAsync">実行するAction</param>
        /// <returns>Task</returns>
        public async Task ExecuteAsync(Func<Task<Response>> funcAsync)
        {
            await asyncRetryPolicy.ExecuteAsync(funcAsync);
        }

        /// <summary>
        /// リトライが必要かどうか判定する
        /// </summary>
        /// <remarks>参考：https://sendgrid.kke.co.jp/docs/API_Reference/Web_API_v3/Mail/errors.html </remarks>
        /// <param name="response">レスポンス</param>
        /// <returns>リトライが必要な場合trueを、不要な場合falseを返す。</returns>
        public bool IsNeedRetry(Response response)
        {
            int code = (int)response.StatusCode;

            if (code >= 200 && code < 300)
            {
                // 2xxの応答は要求が成功したことを意味します
                return false;
            }
            else if (code >= 400 && code < 500)
            {
                switch (code)
                {
                    case 404:
                        // 指定したリソースが存在しないか見つかりません
                        return true;

                    case 429:
                        // 要求数がSendGridの制限を超えました。
                        // （次の要求は通る可能性はあるが、要求制限に達しているためリトライはしない）
                        return false;

                    default:
                        // 4xxの応答は要求が失敗したことを意味します
                        // （リトライしても失敗することが予想されるためリトライしない）
                        return false;
                }
            }
            else if (code >= 500 && code < 600)
            {
                // 5xxの応答はSendGrid側でエラーが発生したことを意味します
                return true;
            }
            else
            {
                // リファレンスに記載がないためここには入らないはずだが念のため
                return true;
            }
        }
    }
}
