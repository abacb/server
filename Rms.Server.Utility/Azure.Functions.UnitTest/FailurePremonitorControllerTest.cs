using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;
using Rms.Server.Core.Abstraction.Repositories.Blobs;
using Rms.Server.Core.Utility;
using Rms.Server.Operation.Utility;
using Rms.Server.Test;
using Rms.Server.Utility.Azure.Functions.FailurePremonitor;
using Rms.Server.Utility.Azure.Functions.StartUp;
using Rms.Server.Utility.Service.Services;
using Rms.Server.Utility.Utility;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace Azure.Functions.UnitTest
{
    /// <summary>
    /// FailurePremonitorControllerTest
    /// </summary>
    [TestClass]
    public class FailurePremonitorControllerTest
    {
        /// <summary>テスト結果格納先のルートフォルダ</summary>
        private static readonly string TestResultRootDir = string.Format(@"TestResult.{0:yyyyMMddHHmmss}\{1}", DateTime.Now, typeof(FailurePremonitorController).Name);

        /// <summary>テスト設定</summary>
        private static readonly Dictionary<string, string> DefaultAppSettingValues = new Dictionary<string, string>()
        {
            { "ConnectionString", "Endpoint=sb://meta-tril-eventhubs-ms-02.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ichu8quUipHizHY6RIpLCWSjXkHV3HK5MYGk6fA3MTI=" },
            { "AlarmQueueName", "alarm-queue" },
            { "SystemName", "rms" },
            { "SubSystemName", "failurepremonitor" },
            { "FailureBlobContainerName", "utility-unit-test" },
        };

        /// <summary>テスト実行時間</summary>
        private static readonly DateTime TestTime = new DateTime(2030, 1, 1);

        /// <summary>FailureBlob</summary>
        private FailureBlob failureBlob;

        /// <summary>アプリケーション設定</summary>
        private UtilityAppSettings settings;

        /// <summary>テスト対象</summary>
        private FailurePremonitorController target;

        /// <summary>
        /// ClassInit
        /// </summary>
        /// <param name="context">TestContext</param>
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            if (Directory.Exists(TestResultRootDir))
            {
                Directory.Delete(TestResultRootDir, true);
            }
        }

        /// <summary>
        /// ClassCleanup
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        /// <summary>
        /// TestInitialize
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            DependencyInjection();

            foreach (CloudBlockBlob blockBlob in failureBlob.Client.GetBlockBlobs(DefaultAppSettingValues["FailureBlobContainerName"]))
            {
                blockBlob.DeleteIfExistsAsync().Wait();
            }

            using (SqlConnection connection = new SqlConnection(settings.PrimaryDbConnectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("TRUNCATE TABLE utility.DT_ALARM_DEF_FAILURE_PREMONITOR;", connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// TestCleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            DependencyInjection();
            foreach (CloudBlockBlob blockBlob in failureBlob.Client.GetBlockBlobs(DefaultAppSettingValues["FailureBlobContainerName"]))
            {
                blockBlob.DeleteIfExistsAsync().Wait();
            }

            using (SqlConnection connection = new SqlConnection(settings.PrimaryDbConnectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("TRUNCATE TABLE utility.DT_ALARM_DEF_FAILURE_PREMONITOR;", connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// ReceiveFailurePredictiveResultLogTest
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InitializeSqlSet">DBの初期状態を生成するためのsqlファイルが格納されているフォルダパス</param>
        /// <param name="in_Message">インプットEventHubメッセージ</param>
        /// <param name="expected_AlarmQueueSet">アラームキューの期待値</param>
        /// <param name="expected_FailureBlobFileSet">FailureBlobのファイル・フォルダ構成の期待値</param>
        /// <param name="expected_LogMessages">ログ出力の期待値</param>
        /// <param name="remarks">備考欄</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\Functions_FailurePremonitor_ReceiveFailurePredictiveResultLog.csv")]
        public void ReceiveFailurePredictiveResultLogTest(string no, string in_InitializeSqlSet, string in_Message, string expected_AlarmQueueSet, string expected_FailureBlobFileSet, string expected_LogMessages, string remarks)
        {
            List<TestLog> actual_logs = new List<TestLog>();

            // DI
            DependencyInjection(DefaultAppSettingValues, actual_logs);

            // テストデータ準備
            {
                if (Directory.Exists(in_InitializeSqlSet))
                {
                    using (SqlConnection connection = new SqlConnection(settings.PrimaryDbConnectionString))
                    {
                        try
                        {
                            connection.Open();
                            foreach (string path in Directory.GetFiles(in_InitializeSqlSet, "*.sql", SearchOption.AllDirectories))
                            {
                                string cmdText = File.ReadAllText(path);
                                using (SqlCommand command = new SqlCommand(cmdText, connection))
                                {
                                    command.ExecuteNonQuery();
                                }
                            }
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }

                in_Message = (in_Message != null && File.Exists(in_Message)) ? File.ReadAllText(in_Message) : in_Message;
            }

            // 期待値
            List<string> expected_queues = Directory.Exists(expected_AlarmQueueSet) ? Directory.GetFiles(expected_AlarmQueueSet, "*.json").Select(x => File.ReadAllText(x)).ToList() : new List<string>();
            DirectoryInfo expectedFailureDir = new DirectoryInfo(expected_FailureBlobFileSet);
            string[] expectedFailureFiles = expectedFailureDir.Exists ? expectedFailureDir.GetFiles("*", SearchOption.AllDirectories).Select(x => x.FullName).OrderBy(x => x).ToArray() : new string[] { };
            string[] expected_failure_names = expectedFailureFiles.Select(x => x.Replace(expectedFailureDir.FullName, string.Empty)).ToArray();
            string[] expected_failure_contents = expectedFailureFiles.Select(x => File.ReadAllText(x)).ToArray();
            List<string> expected_log_messages = (expected_LogMessages != null && File.Exists(expected_LogMessages)) ? File.ReadLines(expected_LogMessages).ToList() : new List<string>();

            // テスト実行
            target.ReceiveFailurePredictiveResultLog(in_Message, new TestLogger<FailurePremonitorController>(actual_logs));

            // テスト結果
            List<string> actual_queses = QueueOperation.GetMessages(settings.AlarmQueueName, settings.QueueStorageConnectionString);
            DirectoryInfo actualDir = new DirectoryInfo(Path.Combine(TestResultRootDir, no));
            string[] actualFailureFiles = failureBlob.Client.GetFiles(settings.FailureBlobContainerName, actualDir).OrderBy(x => x).ToArray();
            string[] actual_failure_names = actualFailureFiles.Select(x => x.Replace(actualDir.FullName, string.Empty)).ToArray();
            string[] actual_failure_contents = actualFailureFiles.Select(x => File.ReadAllText(x)).ToArray();
            List<string> actual_log_messages = actual_logs.Select(x => x.GetSimpleText()).ToList();

            // 確認
            CollectionAssert.AreEqual(expected_queues, actual_queses);
            CollectionAssert.AreEqual(expected_failure_names, actual_failure_names);
            CollectionAssert.AreEqual(expected_failure_contents, actual_failure_contents);
            foreach (var expected_log_message in expected_log_messages)
            {
                var matching_element = actual_log_messages.FirstOrDefault(actual => actual.Contains(expected_log_message));
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(matching_element, string.Format("「{0}」に一致する要素が見つかりません", expected_log_message));
                if (matching_element != null)
                {
                    actual_log_messages.Remove(matching_element);
                }
            }
        }

        /// <summary>
        /// DIを実行する
        /// </summary>
        /// <param name="appSettings">アプリケーション設定を上書きする場合は指定する</param>
        /// <param name="testLogs">ログの格納先</param>
        private void DependencyInjection(Dictionary<string, string> appSettings = null, List<TestLog> testLogs = null)
        {
            var builder = new TestDiProviderBuilder<UtilityAppSettings>(FunctionsHostBuilderExtend.AddUtility);
            builder.ServiceCollection.AddTransient<FailurePremonitorController>();
            builder.ServiceCollection.AddTransient<FailureBlob>();
            builder.AddConfigures(appSettings);

            Mock<DateTimeProvider> timeProviderMock = new Mock<DateTimeProvider>();
            timeProviderMock.SetupGet(tp => tp.UtcNow).Returns(TestTime);
            builder.ServiceCollection.AddSingleton<ITimeProvider>(timeProviderMock.Object);

            if (testLogs != null)
            {
                builder.ServiceCollection.AddSingleton<ILogger<FailurePremonitorService>>(new TestLogger<FailurePremonitorService>(testLogs));
            }

            ServiceProvider provider = builder.Build();
            failureBlob = provider.GetService<FailureBlob>();
            settings = provider.GetService<AppSettings>() as UtilityAppSettings;
            target = provider.GetService<FailurePremonitorController>();
        }
    }
}
