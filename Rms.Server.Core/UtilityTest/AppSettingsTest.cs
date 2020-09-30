using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rms.Server.Core.Azure.Utility.Validations;
using Rms.Server.Core.Utility;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TestHelper;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace UtilityTest.Validations
{
    /// <summary>
    /// AppSettingsTest
    /// </summary>
    [TestClass()]
    public class AppSettingsTest
    {
        /// <summary>
        /// string型のプロパティにおいて、
        /// 設定されていないときは初期値を取得し、設定されているときは設定値を読み込むことを確認する
        /// </summary>
        /// <remarks>
        /// [DataRow("プロパティ名", "数値", "期待する結果")]
        /// ・文字列が入っていないとき初期値が取得できるか
        /// ・文字列が入っているとき、その値が読み込めるか
        /// </remarks>
        [DataTestMethod]
        [DataRow("CollectingBlobContainerNameCollect", "", "collect")]
        [DataRow("CollectingBlobContainerNameCollect", "100", "100")]
        [DataRow("CollectingBlobContainerNameUnknown", "", "unknown")]
        [DataRow("CollectingBlobContainerNameUnknown", "100", "100")]
        [DataRow("FailureBlobContainerNameDispatcher", "", "dispatcher")]
        [DataRow("FailureBlobContainerNameDispatcher", "100", "100")]
        public void GetStringValueTest(string targetPropertyName, string actualValue, string expected)
        {
            // テスト対象の準備
            //NULLか空欄ではなかったら
            var diBuilder = new TestDiProviderBuilder();
            if (!string.IsNullOrEmpty(actualValue))
            {
                ///keyと値の追加
                diBuilder.AddConfigure(targetPropertyName, actualValue);
            }
            var provider = diBuilder.Build();
            AppSettings testTarget = provider.GetService(typeof(AppSettings)) as AppSettings;

            // テスト
            //プロパティを取ってくる
            //actual 取ってきた値
            var property = typeof(AppSettings).GetProperty(targetPropertyName);
            string actual = property.GetValue(testTarget) as string;

            //結果
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// int型のプロパティにおいて、
        /// 設定されていないか、数値以外の文字列が入っているときは初期値を取得し、数値が入っているときはその数値を読み込むことを確認する
        /// </summary>
        /// <remarks>
        /// [DataRow("プロパティ名", "数値", "期待する結果")]
        /// ・NULLもしくは空欄が入っているとき初期値が取得できるか
        /// ・文字列が入っているとき初期値が取得できるか
        /// ・数値が入っているときにその値が読み込めるか
        /// </remarks>
        [DataTestMethod]
        [DataRow("BlobIndexerKeepFileDays", "", 7)] // BlobIndexerKeepFileDays
        [DataRow("BlobIndexerKeepFileDays", "hoge", 7)]
        [DataRow("BlobIndexerKeepFileDays", "100", 100)]
        [DataRow("BlobAccessMaxAttempts", "", 3)] // BlobAccessMaxAttempts
        [DataRow("BlobAccessMaxAttempts", "hoge", 3)]
        [DataRow("BlobAccessMaxAttempts", "100", 100)]
        [DataRow("BlobAccessDelayDeltaSeconds", "", 3)] // BlobAccessDelayDeltaSeconds
        [DataRow("BlobAccessDelayDeltaSeconds", "hoge", 3)]
        [DataRow("BlobAccessDelayDeltaSeconds", "100", 100)]
        [DataRow("IotHubMaxRetryAttempts", "", 3)] // IotHubMaxRetryAttempts
        [DataRow("IotHubMaxRetryAttempts", "hoge", 3)]
        [DataRow("IotHubMaxRetryAttempts", "100", 100)]
        [DataRow("IotHubDelayDeltaSeconds", "", 3)] // IotHubDelayDeltaSeconds
        [DataRow("IotHubDelayDeltaSeconds", "hoge", 3)]
        [DataRow("IotHubDelayDeltaSeconds", "100", 100)]
        [DataRow("IotHubDirectMessageConnectionTimeoutSeconds", "", 0)] // IotHubDirectMessageConnectionTimeoutSeconds
        [DataRow("IotHubDirectMessageConnectionTimeoutSeconds", "hoge", 0)]
        [DataRow("IotHubDirectMessageConnectionTimeoutSeconds", "100", 100)]
        [DataRow("IotHubDirectMessageResponseTimeoutSeconds", "", 0)] // IotHubDirectMessageResponseTimeoutSeconds
        [DataRow("IotHubDirectMessageResponseTimeoutSeconds", "hoge", 0)]
        [DataRow("IotHubDirectMessageResponseTimeoutSeconds", "100", 100)]
        [DataRow("DpsMaxRetryAttempts", "", 3)]  // DpsMaxRetryAttempts
        [DataRow("DpsMaxRetryAttempts", "hoge", 3)]
        [DataRow("DpsMaxRetryAttempts", "100", 100)]
        [DataRow("DpsDelayDeltaSeconds", "", 3)] // DpsDelayDeltaSeconds
        [DataRow("DpsDelayDeltaSeconds", "hoge", 3)]
        [DataRow("DpsDelayDeltaSeconds", "100", 100)]
        public void GetIntValueTest(string targetPropertyName, string actualValue, int expected)
        {
            // テスト対象の準備
            //NULLか空欄ではなかったら
            var diBuilder = new TestDiProviderBuilder();
            if (int.TryParse(actualValue, out int result) == true)
            {
                diBuilder.AddConfigure(targetPropertyName, actualValue);
            }
            var provider = diBuilder.Build();
            AppSettings testTarget = provider.GetService(typeof(AppSettings)) as AppSettings;

            // テスト
            var property = typeof(AppSettings).GetProperty(targetPropertyName);
            int? actual = property.GetValue(testTarget) as int?;

            //結果
            Assert.AreEqual(expected, actual);
        }
    }
}