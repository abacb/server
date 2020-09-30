using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rms.Server.Core.Azure.Functions.Startup;
using Rms.Server.Core.Utility;
using Rms.Server.Test;
using System;
using System.Collections.Generic;

namespace TestHelper
{
    public class TestDiProviderBuilder : TestDiProviderBuilder<AppSettings>
    {
        public TestDiProviderBuilder() : base(FunctionsHostBuilderExtend.AddUtility)
        {
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
        public new TestDiProviderBuilder AddConfigure(string key, string value)
        {
            base.AddConfigure(key, value);
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
        public new TestDiProviderBuilder AddConfigures(Dictionary<string, string> configures)
        {
            base.AddConfigures(configures);
            return this;
        }
    }
}
