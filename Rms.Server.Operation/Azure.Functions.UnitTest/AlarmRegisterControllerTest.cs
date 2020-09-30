using CsvHelper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;
using Rms.Server.Core.Abstraction.Repositories.Blobs;
using Rms.Server.Core.Utility;
using Rms.Server.Operation.Azure.Functions.AlarmRegister;
using Rms.Server.Operation.Azure.Functions.StartUp;
using Rms.Server.Operation.Service.Services;
using Rms.Server.Operation.Utility;
using Rms.Server.Test;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Azure.Functions.UnitTest
{
    /// <summary>
    /// AlarmRegisterControllerTest
    /// </summary>
    [TestClass]
    public class AlarmRegisterControllerTest
    {
        /// <summary>テスト結果格納先のルートフォルダ</summary>
        private static readonly string TestResultRootDir = string.Format(@"TestResult.{0:yyyyMMddHHmmss}\{1}", DateTime.Now, typeof(AlarmRegisterController).Name);

        /// <summary>DBの事前設定を行うためのSQLファイル</summary>
        private static readonly string DbPresetSqlFile = @"TestData\Azure.Functions\AlarmRegister\DbPreset.sql";

        /// <summary>テスト設定</summary>
        private static readonly Dictionary<string, string> DefaultAppSettingValues = new Dictionary<string, string>()
        {
            { "ConnectionString", "BlobEndpoint=https://rmsopemujpemain01.blob.core.windows.net/;QueueEndpoint=https://rmsopemujpemain01.queue.core.windows.net/;FileEndpoint=https://rmsopemujpemain01.file.core.windows.net/;TableEndpoint=https://rmsopemujpemain01.table.core.windows.net/;SharedAccessSignature=sv=2019-02-02&ss=bfqt&srt=sco&sp=rwdlacup&se=9999-12-31T14:59:59Z&st=2019-12-31T15:00:00Z&spr=https&sig=df0HxgGoFo3k%2Fk8gIsfYxOPVolY7xCcP%2Be5G%2BpVYJT0%3D" },
            { "MailQueueName", "mail" },
            { "AlarmMailAddressFrom", "test@test.com" },
            { "FailureBlobContainerName", "operation-unit-test" },
            { "DbAccessMaxAttempts", "3" },
            { "DbAccessDelayDeltaSeconds", "3" },
            { "BlobAccessMaxAttempts", "3" },
            { "BlobAccessDelayDeltaSeconds", "3" },
            { "MailQueueAccessMaxAttempts", "3" },
            { "MailQueueDelayDeltaSeconds", "3" },
        };

        /// <summary>評価対象外のカラム名</summary>
        private static readonly string[] IgnoreColumns = new string[]
        {
        };

        /// <summary>日付形式のカラム名</summary>
        private static readonly string[] DateTimeColumns = new string[]
        {
            "ALARM_DATETIME",
            "EVENT_DATETIME",
            "CREATE_DATETIME"
        };

        /// <summary>テスト実行時間</summary>
        private static readonly DateTime TestTime = new DateTime(2030, 1, 1);

        /// <summary>FailureBlob</summary>
        private FailureBlob failureBlob;

        /// <summary>アプリケーション設定</summary>
        private OperationAppSettings settings;

        /// <summary>テスト対象</summary>
        private AlarmRegisterController target;

        /// <summary>
        /// テーブルクリアのSQLを取得する
        /// </summary>
        public string ClearTableCommand
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("TRUNCATE TABLE operation.DT_ALARM_CONFIG;");
                sb.AppendLine("DELETE FROM operation.DT_ALARM;");
                sb.AppendLine("DBCC CHECKIDENT ('operation.DT_ALARM', RESEED, 0);");
                sb.AppendLine("DELETE FROM operation.DT_EQUIPMENT;");
                sb.AppendLine("DBCC CHECKIDENT ('operation.DT_EQUIPMENT', RESEED, 0);");
                sb.AppendLine("DELETE FROM operation.DT_INSTALL_BASE;");
                sb.AppendLine("DBCC CHECKIDENT ('operation.DT_INSTALL_BASE', RESEED, 0);");
                sb.AppendLine("DELETE FROM operation.MT_NETWORK_ROUTE;");
                sb.AppendLine("DBCC CHECKIDENT ('operation.MT_NETWORK_ROUTE', RESEED, 0);");
                return sb.ToString();
            }
        }

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
            DependencyInjection(DefaultAppSettingValues);

            QueueOperation.GetMessages(settings.MailQueueName, settings.MailQueueConnectionString);

            using (SqlConnection connection = new SqlConnection(settings.PrimaryDbConnectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(ClearTableCommand, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                finally
                {
                    connection.Close();
                }
            }

            foreach (CloudBlockBlob blockBlob in failureBlob.Client.GetBlockBlobs(DefaultAppSettingValues["FailureBlobContainerName"]))
            {
                blockBlob.DeleteIfExistsAsync().Wait();
            }
        }

        /// <summary>
        /// TestCleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            DependencyInjection(DefaultAppSettingValues);

            QueueOperation.GetMessages(settings.MailQueueName, settings.MailQueueConnectionString);

            using (SqlConnection connection = new SqlConnection(settings.PrimaryDbConnectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(ClearTableCommand, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                finally
                {
                    connection.Close();
                }
            }

            foreach (CloudBlockBlob blockBlob in failureBlob.Client.GetBlockBlobs(DefaultAppSettingValues["FailureBlobContainerName"]))
            {
                blockBlob.DeleteIfExistsAsync().Wait();
            }
        }

        /// <summary>
        /// DequeueAlarmInfoTest
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InitializeSqlSet">DBの初期状態を生成するためのsqlファイルが格納されているフォルダパス</param>
        /// <param name="in_QueueItem">インプットアラームキューを記載したjsonファイルのパス</param>
        /// <param name="expected_MailQueueSet">メールキューの期待値を記載したjsonファイルが格納されているフォルダパス</param>
        /// <param name="expected_TableData">DT_ALARMテーブルの期待値を記載したcsvファイルのパス</param>
        /// <param name="expected_FailureBlobFileSet">FailureBlobのファイル・フォルダ構成の期待値</param>
        /// <param name="expected_LogMessages">ログ出力の期待値を記載したファイルのパス</param>
        /// <param name="remarks">備考欄</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\Functions_AlarmRegister_DequeueAlarmInfo.csv")]
        public void DequeueAlarmInfoTest(string no, string in_InitializeSqlSet, string in_QueueItem, string expected_MailQueueSet, string expected_TableData, string expected_FailureBlobFileSet, string expected_LogMessages, string remarks)
        {
            List<TestLog> actual_logs = new List<TestLog>();

            // DI
            DependencyInjection(DefaultAppSettingValues, actual_logs);

            // テストデータ準備
            {
                if (Directory.Exists(in_InitializeSqlSet))
                {
                    // 設置ベースデータテーブル、機器データテーブルを先に設定する
                    PresetDb();

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

                in_QueueItem = (in_QueueItem != null && File.Exists(in_QueueItem)) ? File.ReadAllText(in_QueueItem) : in_QueueItem;
            }

            // テスト実行後、テーブルを評価するかどうか
            bool isCheckTable = !string.IsNullOrEmpty(expected_TableData) && File.Exists(expected_TableData);

            // 評価対象テーブル名
            string targetTableName = isCheckTable ? Path.GetFileNameWithoutExtension(expected_TableData) : null;

            // 期待値
            List<string> expected_queues = Directory.Exists(expected_MailQueueSet) ? Directory.GetFiles(expected_MailQueueSet, "*.json").Select(x => File.ReadAllText(x)).ToList() : new List<string>();
            DataTable expected_table = isCheckTable ? DataTableHelper.SelectCsv(settings.PrimaryDbConnectionString, expected_TableData, targetTableName) : null;
            DirectoryInfo expectedFailureDir = new DirectoryInfo(expected_FailureBlobFileSet);
            string[] expectedFailureFiles = expectedFailureDir.Exists ? expectedFailureDir.GetFiles("*", SearchOption.AllDirectories).Select(x => x.FullName).OrderBy(x => x).ToArray() : new string[] { };
            string[] expected_failure_names = expectedFailureFiles.Select(x => x.Replace(expectedFailureDir.FullName, string.Empty)).ToArray();
            string[] expected_failure_contents = expectedFailureFiles.Select(x => File.ReadAllText(x)).ToArray();
            List<string> expected_log_messages = (expected_LogMessages != null && File.Exists(expected_LogMessages)) ? File.ReadLines(expected_LogMessages).ToList() : new List<string>();

            // テスト実行
            target.DequeueAlarmInfo(in_QueueItem, new TestLogger<AlarmRegisterController>(actual_logs));

            // テスト結果
            List<string> actual_queses = QueueOperation.GetMessages(settings.MailQueueName, settings.MailQueueConnectionString);
            DataTable actual_table = isCheckTable ? DataTableHelper.SelectTable(settings.PrimaryDbConnectionString, "SELECT * FROM " + targetTableName) : null;
            DirectoryInfo actualDir = new DirectoryInfo(Path.Combine(TestResultRootDir, no));
            string[] actualFailureFiles = failureBlob.Client.GetFiles(settings.FailureBlobContainerName, actualDir).OrderBy(x => x).ToArray();
            string[] actual_failure_names = actualFailureFiles.Select(x => x.Replace(actualDir.FullName, string.Empty)).ToArray();
            string[] actual_failure_contents = actualFailureFiles.Select(x => File.ReadAllText(x)).ToArray();
            List<string> actual_log_messages = actual_logs.Select(x => x.GetSimpleText()).ToList();

            // 確認（メールキュー）
            CollectionAssert.AreEqual(expected_queues, actual_queses);

            // 確認（DT_ALARM）
            if (isCheckTable)
            {
                // 評価対象外のカラムを除外
                Array.ForEach(IgnoreColumns, x => expected_table.Columns.Remove(x));
                Array.ForEach(IgnoreColumns, x => actual_table.Columns.Remove(x));

                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected_table.Rows.Count, actual_table.Rows.Count);

                // 日付は期待値読み込み時に日本時刻で取得されてしまうため別に比較する
                foreach (var columnName in DateTimeColumns)
                {
                    var exptected_datetime_table = expected_table.DefaultView.ToTable(false, columnName);
                    var actual_datetime_table = actual_table.DefaultView.ToTable(false, columnName);

                    for (int i = 0; i < exptected_datetime_table.Rows.Count; i++)
                    {
                        var expected_datetime = ((DateTime)exptected_datetime_table.Rows[i].ItemArray[0]).ToUniversalTime();
                        var actual_datetime = (DateTime)actual_datetime_table.Rows[i].ItemArray[0];

                        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected_datetime, actual_datetime);
                    }

                    // 評価したカラムを除外
                    expected_table.Columns.Remove(columnName);
                    actual_table.Columns.Remove(columnName);
                }

                for (int i = 0; i < expected_table.Rows.Count; i++)
                {
                    CollectionAssert.AreEqual(expected_table.Rows[i].ItemArray, actual_table.Rows[i].ItemArray);
                }
            }

            // 確認（Failureブロブ）
            CollectionAssert.AreEqual(expected_failure_names, actual_failure_names);
            CollectionAssert.AreEqual(expected_failure_contents, actual_failure_contents);

            // 確認（ログ）
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
            var builder = new TestDiProviderBuilder<OperationAppSettings>(FunctionsHostBuilderExtend.AddUtility);
            builder.ServiceCollection.AddTransient<AlarmRegisterController>();
            builder.ServiceCollection.AddTransient<FailureBlob>();
            builder.AddConfigures(appSettings);

            Mock<DateTimeProvider> timeProviderMock = new Mock<DateTimeProvider>();
            timeProviderMock.SetupGet(tp => tp.UtcNow).Returns(TestTime);
            builder.ServiceCollection.AddSingleton<ITimeProvider>(timeProviderMock.Object);

            if (testLogs != null)
            {
                builder.ServiceCollection.AddSingleton<ILogger<AlarmRegisterService>>(new TestLogger<AlarmRegisterService>(testLogs));
            }

            ServiceProvider provider = builder.Build();
            failureBlob = provider.GetService<FailureBlob>();
            settings = provider.GetService<AppSettings>() as OperationAppSettings;
            target = provider.GetService<AlarmRegisterController>();
        }

        /// <summary>
        /// 事前に設定が必要なDBにデータを登録する
        /// </summary>
        private void PresetDb()
        {
            using (SqlConnection connection = new SqlConnection(settings.PrimaryDbConnectionString))
            {
                try
                {
                    connection.Open();

                    // 設置ベースデータテーブル、機器データテーブルを先に設定する
                    string cmdText = File.ReadAllText(DbPresetSqlFile);

                    using (SqlCommand command = new SqlCommand(cmdText, connection))
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
    }
}
