using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Rms.Server.Core.Abstraction.Repositories.Blobs;
using Rms.Server.Core.Azure.Functions.Dispatcher;
using Rms.Server.Core.Azure.Functions.Startup;
using Rms.Server.Core.Service.Services;
using Rms.Server.Core.Utility;
using Rms.Server.Test;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using TestHelper;

namespace Azure.Functions.DispatcherTest
{
    /// <summary>
    /// DispatchDiskDriveTest
    /// </summary>
    [TestClass]
    public class DispatchDiskDriveTest
    {
        /// <summary>テスト結果格納先のルートフォルダ</summary>
        private static readonly string TestResultRootDir = string.Format(@"TestResult.{0:yyyyMMddHHmmss}\{1}", DateTime.Now, nameof(DispatchController.DispatchDiskDrive));

        /// <summary>評価対象外のカラム名</summary>
        private static readonly string[] IgnoreColumns = new string[]
        {
        };

        /// <summary>テスト実行時間</summary>
        private static readonly DateTime TestTime = new DateTime(2030, 1, 1);

        /// <summary>FailureBlob置換対象パス</summary>
        private static readonly string FailureBlobReplacePath = @"\ms-027\";

        /// <summary>FailureBlob</summary>
        private FailureBlob _failureBlob;

        /// <summary>アプリケーション設定(local.settings.json)</summary>
        private AppSettings _localAppSettings = new Rms.Server.Core.Utility.AppSettings();

        /// <summary>アプリケーション設定(DI後)</summary>
        private AppSettings _diAppSettings;

        /// <summary>テスト対象</summary>
        private DispatchController _target;

        /// <summary>
        /// ClassInit
        /// </summary>
        /// <param name="context">TestContext</param>
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            // テスト結果格納先削除
            if (Directory.Exists(TestResultRootDir))
            {
                Directory.Delete(TestResultRootDir, true);
            }

            DispatcherTestCommon.DeleteDbData();

            // マスタテーブルデータを削除
            DispatcherTestCommon.DeleteMasterTableData();

            // DB設定
            // マスタテーブルデータを作成する
            DispatcherTestCommon.MakeMasterTableData();
        }

        /// <summary>
        /// ClassCleanup
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanup()
        {
            // マスタテーブルデータを削除
            DispatcherTestCommon.DeleteMasterTableData();
        }

        /// <summary>
        /// TestInitialize
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            DispatcherTestCommon.DeleteDbData();
        }

        /// <summary>
        /// TestCleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            DispatcherTestCommon.DeleteDbData();

            if (_diAppSettings != null)
            {
                DispatcherTestCommon.DeleteFailureBlobFile(_failureBlob, _diAppSettings.FailureBlobContainerNameDispatcher);
            }
        }

        /// <summary>
        /// DiskDriveTest
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InitializeSqlSet">DBの初期状態を生成するためのsqlファイルが格納されているフォルダパス</param>
        /// <param name="in_Message">インプットEventHubメッセージ</param>
        /// <param name="expected_TableData">DBの期待値</param>
        /// <param name="expected_FailureBlobFileSet">FailureBlobのファイル・フォルダ構成の期待値</param>
        /// <param name="expected_LogMessages">ログ出力の期待値</param>
        /// <param name="expected_WithoutError">trueの場合にはエラーメッセージがないことを確認する</param>
        /// <param name="in_AppSettings">AppSettingsに追加登録する項目をまとめたJSON文字列</param>
        /// <param name="error_Method">例外を発生させるメソッド名</param>
        /// <param name="remarks">備考</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\DispatchController_DispatchDiskDrive.csv")]
        public void DiskDriveTest(string no, string in_InitializeSqlSet, string in_Message, string expected_TableData, string expected_FailureBlobFileSet, string expected_LogMessages, string expected_WithoutError, string in_AppSettings, string error_Method, string remarks)
        {
            List<TestLog> actual_logs = new List<TestLog>();

            // AppSettings
            Dictionary<string, string> appSettingsConfigures = null;
            if (!string.IsNullOrEmpty(in_AppSettings))
            {
                using (var sr = new StreamReader(in_AppSettings))
                {
                    JsonTextReader reader = new JsonTextReader(sr);
                    var se = new JsonSerializer();
                    appSettingsConfigures = se.Deserialize<Dictionary<string, string>>(reader);
                }
            }

            // DI
            DependencyInjection(appSettingsConfigures, actual_logs, error_Method);

            // FailureBlobのファイルはDI後に削除する(コンテナ名を変更しての試験があるため)
            DispatcherTestCommon.DeleteFailureBlobFile(_failureBlob, _diAppSettings.FailureBlobContainerNameDispatcher);

            // テストデータ準備
            {
                if (Directory.Exists(in_InitializeSqlSet))
                {
                    using (SqlConnection connection = new SqlConnection(_localAppSettings.PrimaryDbConnectionString))
                    {
                        try
                        {
                            connection.Open();

                            // 各試験で必要となるデータを設定
                            foreach (string path in Directory.GetFiles(in_InitializeSqlSet, "*.sql", SearchOption.AllDirectories))
                            {
                                var cmdText = File.ReadAllText(path);
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

            // テスト実行後、テーブルを評価するかどうか
            bool isCheckTable = !string.IsNullOrEmpty(expected_TableData) && File.Exists(expected_TableData);

            // 評価対象テーブル名
            string targetTableName = isCheckTable ? Path.GetFileNameWithoutExtension(expected_TableData) : null;

            // 期待値
            DataTable expected_table = isCheckTable ? DbTestHelper.SelectCsv(expected_TableData, "SELECT * FROM " + targetTableName) : null;
            DirectoryInfo expectedFailureDir = new DirectoryInfo(expected_FailureBlobFileSet);
            string[] expectedFailureFiles = expectedFailureDir.Exists ? expectedFailureDir.GetFiles("*", SearchOption.AllDirectories).Select(x => x.FullName).OrderBy(x => x).ToArray() : new string[] { };
            string[] expected_failure_names = expectedFailureFiles.Select(x => x.Replace(expectedFailureDir.FullName, string.Empty)).ToArray();
            string[] expected_failure_contents = expectedFailureFiles.Select(x => File.ReadAllText(x)).ToArray();
            List<string> expected_log_messages = (!string.IsNullOrEmpty(expected_LogMessages) && File.Exists(expected_LogMessages)) ? File.ReadLines(expected_LogMessages).ToList() : new List<string>();

            // テスト実行
            _target.DispatchDiskDrive(in_Message, new TestLogger<DispatchController>(actual_logs));

            // テスト結果
            DataTable actual_table = isCheckTable ? DbTestHelper.SelectTable("SELECT * FROM " + targetTableName) : null;
            DirectoryInfo actualDir = new DirectoryInfo(Path.Combine(TestResultRootDir, no));
            string[] actualFailureFiles = _failureBlob.Client.GetFiles(_diAppSettings.FailureBlobContainerNameDispatcher, actualDir).OrderBy(x => x).ToArray();
            string[] actual_failure_names = actualFailureFiles.Select(x => x.Replace(actualDir.FullName, string.Empty)).ToArray();
            string[] actual_failure_contents = actualFailureFiles.Select(x => File.ReadAllText(x)).ToArray();
            List<string> actual_log_messages = actual_logs.Select(x => x.GetSimpleText()).ToList();

            // 確認
            if (isCheckTable)
            {
                // 評価対象外のカラムを除外
                Array.ForEach(IgnoreColumns, x => expected_table.Columns.Remove(x));
                Array.ForEach(IgnoreColumns, x => actual_table.Columns.Remove(x));

                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected_table.Rows.Count, actual_table.Rows.Count);
                for (int i = 0; i < expected_table.Rows.Count; i++)
                {
                    CollectionAssert.AreEqual(expected_table.Rows[i].ItemArray, actual_table.Rows[i].ItemArray);
                }
            }

            // フォルダ名は小文字で取得されるため大文字に変換
            for (int i = 0; i < expected_failure_names.Length; i++)
            {
                expected_failure_names[i] = expected_failure_names[i].Replace(FailureBlobReplacePath, FailureBlobReplacePath.ToUpper());
            }
            CollectionAssert.AreEqual(expected_failure_names, actual_failure_names);
            CollectionAssert.AreEqual(expected_failure_contents, actual_failure_contents);

            bool.TryParse(expected_WithoutError, out bool isWithoutError);
            if (isWithoutError)
            {
                // エラーログが含まれない事を確認
                // actual_logsはexpected_log_messageとの比較時に要素が削除されるため本処理を先に実行すること
                var error_logs = actual_logs.Where(x => x.LogLevel == Microsoft.Extensions.Logging.LogLevel.Error);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(false, error_logs.Any());
            }

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
        /// <param name="exceptionMethodName">例外を発生させるサービスのメソッド名</param>
        private void DependencyInjection(Dictionary<string, string> appSettings = null, List<TestLog> testLogs = null, string exceptionMethodName = null)
        {
            // DI前のFailureBlobを取得
            var locaSettingsBuilder = new TestDiProviderBuilder();
            var locaSettingsProvider = locaSettingsBuilder.Build();
            _failureBlob = locaSettingsProvider.GetService<FailureBlob>();

            var builder = new TestDiProviderBuilder<AppSettings>(FunctionsHostBuilderExtend.AddUtility);

            builder.ServiceCollection.AddTransient<DispatchController>();
            builder.ServiceCollection.AddTransient<FailureBlob>();

            if (appSettings != null)
            {
                builder.AddConfigures(appSettings);
            }

            Mock<DateTimeProvider> timeProviderMock = new Mock<DateTimeProvider>();
            timeProviderMock.SetupGet(tp => tp.UtcNow).Returns(TestTime);
            builder.ServiceCollection.AddSingleton<ITimeProvider>(timeProviderMock.Object);

            if (testLogs != null)
            {
                builder.ServiceCollection.AddSingleton<ILogger<DispatchService>>(new TestLogger<DispatchService>(testLogs));
            }

            if (!string.IsNullOrEmpty(exceptionMethodName))
            {
                builder.ServiceCollection.AddTransient<IDispatchService, DispatchServiceMock>();
            }

            ServiceProvider provider = builder.Build();
            _diAppSettings = provider.GetService<AppSettings>();
            _target = provider.GetService<DispatchController>();

            var dispatchServiceMock = provider.GetService<IDispatchService>() as DispatchServiceMock;
            dispatchServiceMock?.Init(exceptionMethodName);
        }
    }
}
