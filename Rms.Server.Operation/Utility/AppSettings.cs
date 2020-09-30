using Microsoft.Extensions.Configuration;
using Rms.Server.Core.Utility;

namespace Rms.Server.Operation.Utility
{
    /// <summary>
    /// アプリケーション設定
    /// </summary>
    public class OperationAppSettings : AppSettings
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="builder">ビルダー</param>
        public OperationAppSettings(IConfigurationBuilder builder)
        {
            this._configuration = builder.Build();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OperationAppSettings()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", true)
                .AddEnvironmentVariables();
            this._configuration = builder.Build();
        }

        /// <summary>
        /// メールキュー接続文字列
        /// </summary>
        public string MailQueueConnectionString => this._configuration.GetConnectionString(nameof(this.MailQueueConnectionString));

        /// <summary>
        /// アラームデータ保持期間
        /// </summary>
        //// [TODO]

        /// <summary>
        /// メールキュー名称
        /// </summary>
        public string MailQueueName => this._configuration[nameof(this.MailQueueName)];

        /// <summary>
        /// メールキュー操作リトライ回数
        /// </summary>
        public int MailQueueAccessMaxAttempts
        {
            get
            {
                int accessMaxAttempts;
                return int.TryParse(this._configuration[nameof(this.MailQueueAccessMaxAttempts)], out accessMaxAttempts) ? accessMaxAttempts : 3;
            }
        }

        /// <summary>
        /// メールキュー操作再試行までの秒数
        /// </summary>
        public int MailQueueDelayDeltaSeconds
        {
            get
            {
                int delayDeltaSeconds;
                return int.TryParse(this._configuration[nameof(this.MailQueueDelayDeltaSeconds)], out delayDeltaSeconds) ? delayDeltaSeconds : 3;
            }
        }

        /// <summary>
        /// 送信元メールアドレス
        /// </summary>
        public string AlarmMailAddressFrom => this._configuration[nameof(this.AlarmMailAddressFrom)];

        /// <summary>
        /// メール本文フォーマット
        /// </summary>
        public string MailTextFormat => this._configuration[nameof(this.MailTextFormat)];

        /// <summary>
        /// SendGrid APIキー
        /// </summary>
        public string SendGridApiKey => this._configuration[nameof(this.SendGridApiKey)];

        /// <summary>
        /// SendGrid操作リトライ回数
        /// </summary>
        public int SendGridAccessMaxAttempts
        {
            get
            {
                int accessMaxAttempts;
                return int.TryParse(this._configuration[nameof(this.SendGridAccessMaxAttempts)], out accessMaxAttempts) ? accessMaxAttempts : 3;
            }
        }

        /// <summary>
        /// SendGrid操作再試行までの秒数
        /// </summary>
        public int SendGridDelayDeltaSeconds
        {
            get
            {
                int delayDeltaSeconds;
                return int.TryParse(this._configuration[nameof(this.SendGridDelayDeltaSeconds)], out delayDeltaSeconds) ? delayDeltaSeconds : 3;
            }
        }

        /// <summary>
        /// 再送ファイル集積コンテナ名
        /// </summary>
        public string FailureBlobContainerName => this._configuration[nameof(this.FailureBlobContainerName)];
    }
}
