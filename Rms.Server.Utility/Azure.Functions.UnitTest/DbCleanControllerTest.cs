using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Rms.Server.Core.Utility;
using Rms.Server.Test;
using Rms.Server.Utility.Azure.Functions.DbCleaner;
using Rms.Server.Utility.Azure.Functions.StartUp;
using Rms.Server.Utility.Service.Services;
using Rms.Server.Utility.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace Azure.Functions.DbCleanerTest
{
    /// <summary>
    /// DbCleanControllerTest
    /// </summary>
    [TestClass]
    public class DbCleanControllerTest
    {
        /// <summary>削除対象テーブルのテーブル名</summary>
        /// <remarks>削除されることを確認するテーブル群。依存関係順（依存する方が先）。</remarks>
        private static readonly string[] TargetTableNames = new string[]
        {
            "utility.DT_ALMILOG_ANALYSIS_RESULT",
            "utility.DT_BLOCLOG_ANALYSIS_RESULT",
        };

        /// <summary>削除対象外テーブルのテーブル名</summary>
        /// <remarks>削除されないことを確認するテーブル群。削除対象テーブルと類似名のテーブルを選出する。依存関係順（依存する方が先）。</remarks>
        private static readonly string[] NonTargetTableNames = new string[]
        {
            "utility.DT_ALMILOG_ANALYSIS_CONFIG",
            "utility.DT_BLOCLOG_ANALYSIS_CONFIG",
        };

        /// <summary>テストで使用する全テーブルのテーブル名</summary>
        /// <remarks>依存関係順（依存する方が先）</remarks>
        private static readonly string[] AllTableNames = NonTargetTableNames.Concat(TargetTableNames).ToArray();

        /// <summary>テスト実行時間</summary>
        private static readonly DateTime TestTime = new DateTime(2020, 5, 7);

        /// <summary>アプリケーション設定</summary>
        private UtilityAppSettings settings;

        /// <summary>テスト対象</summary>
        private DbCleanController target;

        /// <summary>
        /// ClassInit
        /// </summary>
        /// <param name="context">TestContext</param>
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
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

            using (SqlConnection connection = new SqlConnection(settings.PrimaryDbConnectionString))
            {
                try
                {
                    connection.Open();
                    foreach (string tableName in AllTableNames.Reverse())
                    {
                        using (SqlCommand command = new SqlCommand(string.Format("DELETE FROM {0}; DBCC CHECKIDENT ('{0}', RESEED, 0);", tableName), connection))
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

        /// <summary>
        /// TestCleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            DependencyInjection();

            using (SqlConnection connection = new SqlConnection(settings.PrimaryDbConnectionString))
            {
                try
                {
                    connection.Open();
                    foreach (string tableName in AllTableNames.Reverse())
                    {
                        using (SqlCommand command = new SqlCommand(string.Format("DELETE FROM {0}; DBCC CHECKIDENT ('{0}', RESEED, 0);", tableName), connection))
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

        /// <summary>
        /// CleanDbTest
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="mock_Exception">DbCleanerServiceがスローするExceptionクラス名</param>
        /// <param name="in_AppSettingsJson">アプリケーション設定が記載されているjsonファイルのファイルパス</param>
        /// <param name="in_InitializeSqlList">DBの初期状態を生成するためのsqlファイルのファイルパスのリスト（カンマ区切り）</param>
        /// <param name="expected_TargetTableDataSet">削除対象テーブルの期待値を記載したcsvファイルが格納されているフォルダパス</param>
        /// <param name="expected_NonTargetTableDataSet">削除対象外テーブルの期待値を記載したcsvファイルが格納されているフォルダパス</param>
        /// <param name="expected_ExistLogMessages">出力されるログのリストを記載したtxtファイルのパス</param>
        /// <param name="expected_NotExistLogMessages">出力されないログのリストを記載したtxtファイルのパス</param>
        /// <param name="remarks">備考欄</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\Functions_DbCleaner_CleanDb.csv")]
        public void CleanDbTest(string no, string mock_Exception, string in_AppSettingsJson, string in_InitializeSqlList, string expected_TargetTableDataSet, string expected_NonTargetTableDataSet, string expected_ExistLogMessages, string expected_NotExistLogMessages, string remarks)
        {
            List<TestLog> actual_logs = new List<TestLog>();

            // アプリケーション設定（local.settings.jsonに上書きする設定）
            Dictionary<string, string> appSettings = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(in_AppSettingsJson));

            // DI
            DependencyInjection(appSettings, actual_logs, Type.GetType(mock_Exception));

            // テストデータ準備
            {
                using (SqlConnection connection = new SqlConnection(settings.PrimaryDbConnectionString))
                {
                    try
                    {
                        connection.Open();
                        foreach (string path in in_InitializeSqlList.Split(",", StringSplitOptions.RemoveEmptyEntries))
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

            // 期待値
            Dictionary<string, DataTable> expected_target_tables = Directory.GetFiles(expected_TargetTableDataSet, "*.csv", SearchOption.AllDirectories)
                .ToDictionary(
                    x => Path.GetFileNameWithoutExtension(x),
                    y => DataTableHelper.SelectCsv(settings.PrimaryDbConnectionString, y, Path.GetFileNameWithoutExtension(y), "SID, CREATE_DATETIME"));
            Dictionary<string, DataTable> expected_nontarget_tables = Directory.GetFiles(expected_NonTargetTableDataSet, "*.csv", SearchOption.AllDirectories)
                .ToDictionary(
                    x => Path.GetFileNameWithoutExtension(x),
                    y => DataTableHelper.SelectCsv(settings.PrimaryDbConnectionString, y, Path.GetFileNameWithoutExtension(y), "SID"));
            List<string> expected_exist_log_messages = File.ReadLines(expected_ExistLogMessages).ToList();
            List<string> expected_notexist_log_messages = File.ReadLines(expected_NotExistLogMessages).ToList();

            // テスト実行
            target.CleanDb(null, new TestLogger<DbCleanController>(actual_logs));

            // テスト結果
            Dictionary<string, DataTable> actual_target_tables = TargetTableNames.ToDictionary(x => x, y => DataTableHelper.SelectTable(settings.PrimaryDbConnectionString, "SELECT SID, CREATE_DATETIME FROM " + y));
            Dictionary<string, DataTable> actual_nontarget_tables = NonTargetTableNames.ToDictionary(x => x, y => DataTableHelper.SelectTable(settings.PrimaryDbConnectionString, "SELECT SID FROM " + y));
            List<string> actual_log_messages = actual_logs.Select(x => x.GetSimpleText()).ToList();

            // 確認（削除対象テーブル）
            foreach (string tableName in TargetTableNames)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected_target_tables[tableName].Rows.Count, actual_target_tables[tableName].Rows.Count);
                for (int i = 0; i < expected_target_tables[tableName].Rows.Count; i++)
                {
                    CollectionAssert.AreEqual(expected_target_tables[tableName].Rows[i].ItemArray, actual_target_tables[tableName].Rows[i].ItemArray);
                }
            }

            // 確認（削除対象外テーブル）
            foreach (string tableName in NonTargetTableNames)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected_nontarget_tables[tableName].Rows.Count, actual_nontarget_tables[tableName].Rows.Count);
                for (int i = 0; i < expected_nontarget_tables[tableName].Rows.Count; i++)
                {
                    CollectionAssert.AreEqual(expected_nontarget_tables[tableName].Rows[i].ItemArray, actual_nontarget_tables[tableName].Rows[i].ItemArray);
                }
            }

            // 確認（非出力ログの確認）
            foreach (var expected_log in expected_notexist_log_messages)
            {
                var matching_element = actual_log_messages.FirstOrDefault(actual => actual.Contains(expected_log));
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNull(matching_element, string.Format("「{0}」に一致する要素が存在します", expected_log));
            }

            // 確認（出力ログの確認）
            foreach (var expected_log in expected_exist_log_messages)
            {
                var matching_element = actual_log_messages.FirstOrDefault(actual => actual.Contains(expected_log));
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(matching_element, string.Format("「{0}」に一致する要素が見つかりません", expected_log));
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
        /// <param name="exceptionType">DbCleanerServiceのモックに例外をスローさせる場合そのタイプを指定する。DbCleanerServiceのモックを使用しない場合はnullを指定する</param>
        private void DependencyInjection(Dictionary<string, string> appSettings = null, List<TestLog> testLogs = null, Type exceptionType = null)
        {
            var builder = new TestDiProviderBuilder<UtilityAppSettings>(FunctionsHostBuilderExtend.AddUtility);
            builder.ServiceCollection.AddTransient<DbCleanController>();
            if (exceptionType != null)
            {
                Exception exception = Activator.CreateInstance(exceptionType) as Exception;
                builder.ServiceCollection.AddTransient<Rms.Server.Core.Service.Services.ICleanDbService>(x => new ErrorCleanDbService(exception));
            }

            builder.AddConfigures(appSettings);

            Mock<DateTimeProvider> timeProviderMock = new Mock<DateTimeProvider>();
            timeProviderMock.SetupGet(tp => tp.UtcNow).Returns(TestTime);
            builder.ServiceCollection.AddSingleton<ITimeProvider>(timeProviderMock.Object);

            if (testLogs != null)
            {
                builder.ServiceCollection.AddSingleton<ILogger<CleanDbService>>(new TestLogger<CleanDbService>(testLogs));
            }

            ServiceProvider provider = builder.Build();
            settings = provider.GetService<UtilityAppSettings>() as UtilityAppSettings;
            target = provider.GetService<DbCleanController>();
        }

        /// <summary>
        /// 常にエラーが発生するCleanDbServiceクラス
        /// </summary>
        public class ErrorCleanDbService : Rms.Server.Core.Service.Services.ICleanDbService
        {
            /// <summary>発生させる例外</summary>
            private Exception exception = null;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="e">発生させる例外</param>
            public ErrorCleanDbService(Exception e)
            {
                exception = e;
            }

            /// <summary>
            /// 不要データの削除
            /// </summary>
            public void Clean()
            {
                throw exception;
            }
        }
    }
}
