using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rms.Server.Core.Utility;
using System;
using System.Collections.Generic;

namespace Rms.Server.Test
{
    public class TestDiProviderBuilder<TAppSettings> where TAppSettings : AppSettings
    {
        /// <summary>
        /// ServiceCollection. [HowTo] new TestDiProviderBuilder().ServiceCollection.AddTransient<DeliveryFilesController>();
        /// </summary>
        public ServiceCollection ServiceCollection { get; private set; }
        private ConfigurationBuilder ConfigurationBuilder { get; }
            = new ConfigurationBuilder();
        private Dictionary<string, string> myConfiguration { get; }
            = new Dictionary<string, string>();

        public TestDiProviderBuilder(Action<IServiceCollection> addUtilityFunc)
        {
            ServiceCollection = new ServiceCollection();
            addUtilityFunc(ServiceCollection);
            ServiceCollection.AddLogging();
        }

        /// <summary>
        /// AppSettingsに設定を追加する。
        /// DB接続文字列などの"Values"以外のグループは以下のようにkeyを指定すること。
        /// "「グループ名」:「キー名」"
        /// 例："ConnectionStrings:PrimaryDbConnectionString"
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="value">値</param>
        /// <returns>値を追加したインスタンス</returns>
        public TestDiProviderBuilder<TAppSettings> AddConfigure(string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                myConfiguration.Add(key, value);
            }
            return this;
        }

        /// <summary>
        /// AppSettingsに設定をDictionary形式で一括追加する。
        /// DB接続文字列などの"Values"以外のグループは以下のようにkeyを指定すること。
        /// "「グループ名」:「キー名」"
        /// 例："ConnectionStrings:PrimaryDbConnectionString"
        /// </summary>
        /// <param name="configures">キーと値がセットになったDictionaryインスタンス</param>
        /// <returns>値を追加したインスタンス</returns>
        public TestDiProviderBuilder<TAppSettings> AddConfigures(Dictionary<string, string> configures)
        {
            if (configures != null)
            {
                foreach (KeyValuePair<string, string> pair in configures)
                {
                    AddConfigure(pair.Key, pair.Value);
                }
            }

            return this;
        }

        /// <summary>
        /// Settings\test\local.settings.jsonの内容を使ってServiceProviderを取得する
        /// </summary>
        /// <returns>ServiceProvider</returns>
        public ServiceProvider Build()
        {
            AddAppSettings();
            return ServiceCollection.BuildServiceProvider();
        }

        /// <summary>
        /// 明示的にlocal.settings.jsonファイルのパスを指定し、ServiceProviderを取得する
        /// </summary>
        /// <returns>ServiceProvider</returns>
        public ServiceProvider Build(string localSettingsJsonFilePath)
        {
            AddAppSettings(localSettingsJsonFilePath);
            return ServiceCollection.BuildServiceProvider();
        }

        /// <summary>
        /// Settings\test\local.settings.jsonファイルの内容を設定値に追加する
        /// </summary>
        /// <param name="localSettingsJsonFilePath"></param>
        private void AddAppSettings()
        {
            AddAppSettings("local.settings.json");
        }

        /// <summary>
        /// 明示的に指定したlocal.settings.jsonファイルの内容を設定値に追加する
        /// </summary>
        /// <param name="localSettingsJsonFilePath"></param>
        private void AddAppSettings(string localSettingsJsonFilePath)
        {
            var configuration = ConfigurationBuilder.AddJsonFile(localSettingsJsonFilePath, true);
            if (myConfiguration.Count > 0)
            {
                configuration = ConfigurationBuilder.AddInMemoryCollection(myConfiguration);
            }
            var settings = Activator.CreateInstance(typeof(TAppSettings), new object[] { configuration });
            ServiceCollection.AddSingleton(_ => (TAppSettings)Activator.CreateInstance(typeof(TAppSettings), new object[] { configuration }));
            if (typeof(TAppSettings) != typeof(AppSettings))
            {
                ServiceCollection.AddSingleton<AppSettings>(s => s.GetService<TAppSettings>());
            }
        }
    }
}
