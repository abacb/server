using Microsoft.Extensions.Configuration;
using Rms.Server.Core.Utility;

namespace Rms.Server.Utility.Utility
{
    /// <summary>
    /// アプリケーション設定
    /// </summary>
    public class UtilityAppSettings : AppSettings
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="builder">ビルダー</param>
        public UtilityAppSettings(IConfigurationBuilder builder)
        {
            this._configuration = builder.Build();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UtilityAppSettings()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", true)
                .AddEnvironmentVariables();
            this._configuration = builder.Build();
        }

        /// <summary>
        /// QueueStorage(Operation)接続文字列
        /// </summary>
        public string QueueStorageConnectionString => this._configuration.GetConnectionString(nameof(this.QueueStorageConnectionString));

        /// <summary>
        /// アルミスロープログ解析結果保持期間
        /// </summary>
        ////[TODO]

        /// <summary>
        /// ムラログ解析結果保持期間
        /// </summary>
        ////[TODO]

        /// <summary>
        /// アラームキュー(Operation)名称
        /// </summary>
        public string AlarmQueueName => this._configuration[nameof(this.AlarmQueueName)];

        /// <summary>
        /// アラームキュー操作リトライ回数
        /// </summary>
        public int AlarmQueueAccessMaxAttempts
        {
            get
            {
                int accessMaxAttempts;
                return int.TryParse(this._configuration[nameof(this.AlarmQueueAccessMaxAttempts)], out accessMaxAttempts) ? accessMaxAttempts : 3;
            }
        }

        /// <summary>
        /// アラームキュー操作再試行までの秒数
        /// </summary>
        public int AlarmQueueDelayDeltaSeconds
        {
            get
            {
                int delayDeltaSeconds;
                return int.TryParse(this._configuration[nameof(this.AlarmQueueDelayDeltaSeconds)], out delayDeltaSeconds) ? delayDeltaSeconds : 3;
            }
        }

        /// <summary>
        /// システム名
        /// </summary>
        public string SystemName => this._configuration[nameof(this.SystemName)];

        /// <summary>
        /// サブシステム名
        /// </summary>
        public string SubSystemName => this._configuration[nameof(this.SubSystemName)];

        /// <summary>
        /// アラームカウント閾値(アルミスロープログ予兆監視のみ使用)
        /// </summary>
        public string AlarmCountThreshold => this._configuration[nameof(this.AlarmCountThreshold)];

        /// <summary>
        /// 再送ファイル集積コンテナ名
        /// </summary>
        public string FailureBlobContainerName => this._configuration[nameof(this.FailureBlobContainerName)];
    }
}
