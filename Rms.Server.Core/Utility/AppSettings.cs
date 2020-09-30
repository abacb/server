using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rms.Server.Core.Utility
{
    /// <summary>
    /// アプリケーション設定
    /// </summary>
    /// <remarks>
    /// 方針として、本クラスには各システムで共通で使う項目を定義する。
    /// </remarks>
    public class AppSettings
    {
        /// <summary>
        /// アプリケーション設定
        /// </summary>
        /// <param name="builder">アプリケーション構成</param>
        public AppSettings(IConfigurationBuilder builder)
        {
            _configuration = builder.Build();
        }

        /// <summary>
        /// アプリケーション設定
        /// </summary>
        public AppSettings()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", true)
                .AddEnvironmentVariables();
            _configuration = builder.Build();
        }

        #region DB

        /// <summary>
        /// プライマリDBの接続文字列
        /// </summary>
        public string PrimaryDbConnectionString => _configuration.GetConnectionString(nameof(PrimaryDbConnectionString));

        /// <summary>
        /// DBの試行回数
        /// </summary>
        public int DbAccessMaxAttempts => int.TryParse(_configuration[nameof(DbAccessMaxAttempts)], out int AccessmaxAttempts) ? AccessmaxAttempts : 3;

        /// <summary>
        /// SQL DB操作再試行までの秒数
        /// </summary>
        public int DbAccessDelayDeltaSeconds => int.TryParse(_configuration[nameof(DbAccessDelayDeltaSeconds)], out int DelayDeltaSeconds) ? DelayDeltaSeconds : 3;

        #endregion

        #region Blob

        /// <summary>
        /// PrimaryBlobの接続文字列
        /// </summary>
        public string PrimaryBlobConnectionString => _configuration.GetConnectionString(nameof(PrimaryBlobConnectionString));

        /// <summary>
        /// CollectingBlobの接続文字列
        /// </summary>
        public string CollectingBlobConnectionString => _configuration.GetConnectionString(nameof(CollectingBlobConnectionString));

        /// <summary>
        /// DeliveringBlobの接続文字列
        /// </summary>
        public string DeliveringBlobConnectionString => _configuration.GetConnectionString(nameof(DeliveringBlobConnectionString));

        /// <summary>
        /// FailureBlobの接続文字列
        /// </summary>
        public string FailureBlobConnectionString => _configuration.GetConnectionString(nameof(FailureBlobConnectionString));

        /// <summary>
        /// Blobクライアントの試行回数
        /// </summary>
        public int BlobAccessMaxAttempts => int.TryParse(_configuration[nameof(BlobAccessMaxAttempts)], out int maxAttempts) ? maxAttempts : 3;

        /// <summary>
        /// Blobクライアント操作再試行までの秒数
        /// </summary>
        public int BlobAccessDelayDeltaSeconds => int.TryParse(_configuration[nameof(BlobAccessDelayDeltaSeconds)], out int delayDeltaSeconds) ? delayDeltaSeconds : 3;

        /// <summary>
        /// 収集用コンテナ名
        /// </summary>
        public string CollectingBlobContainerNameCollect => _configuration[nameof(CollectingBlobContainerNameCollect)] ?? "collect";

        /// <summary>
        /// BlobIndexer異常ファイル集積コンテナ名
        /// </summary>
        public string CollectingBlobContainerNameUnknown => _configuration[nameof(CollectingBlobContainerNameUnknown)] ?? "unknown";

        /// <summary>
        /// Dispatcher再送ファイル集積コンテナ名
        /// </summary>
        public string FailureBlobContainerNameDispatcher => _configuration[nameof(FailureBlobContainerNameDispatcher)] ?? "dispatcher";

        /// <summary>
        /// DeliveringBlobのコンテナ名
        /// </summary>
        public string DeliveringBlobContainerNameInstallbase => _configuration[nameof(DeliveringBlobContainerNameInstallbase)] ?? "config";

        /// <summary>
        /// DeliveringBlobの保存先ファイルパス
        /// </summary>
        public string DeliveringBlobInstallbaseFilePath => _configuration[nameof(DeliveringBlobInstallbaseFilePath)] ?? "installbase/{0}/installbase.config";

        #endregion

        #region IoTHubs

        /// <summary>
        /// DPS(Device provisioning service)接続文字列
        /// </summary>
        public string DpsConnectionString => _configuration.GetConnectionString(nameof(DpsConnectionString));

        /// <summary>
        /// IoTHub(Core)操作リトライ回数
        /// </summary>
        public int IotHubMaxRetryAttempts => int.TryParse(_configuration[nameof(IotHubMaxRetryAttempts)], out int maxRetryAttempt) ? maxRetryAttempt : 3;

        /// <summary>
        /// IoTHub(Core)操作再試行までの秒数
        /// </summary>
        public int IotHubDelayDeltaSeconds => int.TryParse(_configuration[nameof(IotHubDelayDeltaSeconds)], out int delayDeltaSeconds) ? delayDeltaSeconds : 3;

        /// <summary>
        /// IoTHub(Core) CloudToDeviceMethodのレスポンスタイムアウト時間（秒）
        /// </summary>
        public int IotHubDirectMessageResponseTimeoutSeconds => int.TryParse(_configuration[nameof(IotHubDirectMessageResponseTimeoutSeconds)], out int messageResponseTimeoutSeconds) ? messageResponseTimeoutSeconds : 0;

        /// <summary>
        /// IoTHub(Core) CloudToDeviceMethodの接続タイムアウト時間（秒）
        /// </summary>
        public int IotHubDirectMessageConnectionTimeoutSeconds => int.TryParse(_configuration[nameof(IotHubDirectMessageConnectionTimeoutSeconds)], out int messageConnectionTimeoutSeconds) ? messageConnectionTimeoutSeconds : 0;

        /// <summary>
        /// DPS(Core)操作リトライ回数
        /// </summary>
        public int DpsMaxRetryAttempts => int.TryParse(_configuration[nameof(DpsMaxRetryAttempts)], out int maxRetryAttempts) ? maxRetryAttempts : 3;

        /// <summary>
        /// DPS(Core)操作再試行までの秒数
        /// </summary>
        public int DpsDelayDeltaSeconds => int.TryParse(_configuration[nameof(DpsDelayDeltaSeconds)], out int delayDeltaSeconds) ? delayDeltaSeconds : 3;

        #endregion

        #region サービス固有

        /// <summary>
        /// BlobIndexerで正常とみなすファイルの保持期間(日)
        /// </summary>
        public int BlobIndexerKeepFileDays => int.TryParse(_configuration[nameof(BlobIndexerKeepFileDays)], out int days) ? days : 7;

        /// <summary>
        /// BlobCleanerの対象パスのPrefix
        /// </summary>
        ////public string PrefixForBlobCleanerTarget => configuration[nameof(PrefixForBlobCleanerTarget)];

        /// <summary>
        /// WebAPIリモート接続パラメータ
        /// </summary>
        /// <remarks>{0}：セッションコード</remarks>
        public string RemoteParameter => _configuration[nameof(RemoteParameter)] ?? "--on-load \"customization? preselect_allow_remote_desktop_control = false\" --connect {0}";

        #endregion

        /// <summary>
        /// configuration
        /// </summary>
        protected IConfigurationRoot _configuration { get; set; }

        /// <summary>
        /// 直接の子孫構成のサブセクションを取得
        /// </summary>
        /// <param name="prefix">prefix</param>
        /// <returns>prefixがnullまたは空白の場合：子孫構成のサブセクションを全て返す。それ以外の場合：子孫構成のサブセクションのキー名の先頭がprefixのものをすべて返す</returns>
        /// <remarks>プレフィックスの大文字小文字は区別しない</remarks>
        public IEnumerable<IConfigurationSection> GetConfigs(string prefix = null)
        {
            return string.IsNullOrWhiteSpace(prefix) ?
                _configuration.GetChildren() :
                _configuration.GetChildren().Where(x => x.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToArray();
        }

        /// <summary>
        /// 接続文字列の取得
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>接続文字列</returns>
        /// <remarks>あんまり作りたくないけどconfigurationをprivateとしているため仕方なく。</remarks>
        public string GetConnectionString(string key) => _configuration.GetConnectionString(key);
    }
}
