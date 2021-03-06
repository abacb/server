﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;
using Rms.Server.Core.Abstraction.Repositories.Blobs;
using Rms.Server.Core.Utility;
using Rms.Server.Operation.Utility;
using Rms.Server.Test;
using Rms.Server.Utility.Azure.Functions.ConnectionMonitor;
using Rms.Server.Utility.Azure.Functions.StartUp;
using Rms.Server.Utility.Service.Services;
using Rms.Server.Utility.Utility;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace Azure.Functions.UnitTest
{
    /// <summary>
    /// ParentChildrenConnectionMonitorControllerTest
    /// </summary>
    [TestClass]
    public class ParentChildrenConnectionMonitorControllerTest
    {
        /// <summary>テスト結果格納先のルートフォルダ</summary>
        private static readonly string TestResultRootDir = string.Format(@"TestResult.{0:yyyyMMddHHmmss}\{1}", DateTime.Now, typeof(DeviceConnectionMonitorController).Name);

        /// <summary>テスト設定</summary>
        private static readonly Dictionary<string, string> DefaultAppSettingValues = new Dictionary<string, string>()
        {
            { "AlarmQueueName", "alarm-queue" },
            { "SystemName", "rms" },
            { "SubSystemName", "connectionmonitor" },
        };

        /// <summary>テスト実行時間</summary>
        private static readonly DateTime TestTime = new DateTime(2030, 1, 1);

        /// <summary>アプリケーション設定</summary>
        private UtilityAppSettings settings;

        /// <summary>テスト対象</summary>
        private ParentChildrenConnectionMonitorController target;

        /// <summary>
        /// テーブルクリアのSQLを取得する
        /// </summary>
        public string ClearTableCommand
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("TRUNCATE TABLE utility.DT_ALARM_DEF_CONNECTION_MONITOR;");
                sb.AppendLine("DELETE FROM core.DT_INVENTORY;");
                sb.AppendLine("DBCC CHECKIDENT ('core.DT_INVENTORY', RESEED, 0);");
                sb.AppendLine("DELETE FROM core.DT_DEVICE;");
                sb.AppendLine("DBCC CHECKIDENT ('core.DT_DEVICE', RESEED, 0);");
                sb.AppendLine("DELETE FROM core.DT_PARENT_CHILD_CONNECT;");
                sb.AppendLine("DBCC CHECKIDENT ('core.DT_PARENT_CHILD_CONNECT', RESEED, 0);");
                sb.AppendLine("DELETE FROM core.MT_INSTALL_TYPE;");
                sb.AppendLine("DBCC CHECKIDENT ('core.MT_INSTALL_TYPE', RESEED, 0);");
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
            DependencyInjection();

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
        }

        /// <summary>
        /// ReceiveErrorLogTest
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InitializeSqlSet">DBの初期状態を生成するためのsqlファイルが格納されているフォルダパス</param>
        /// <param name="expected_AlarmQueueSet">アラームキューの期待値</param>
        /// <param name="expected_LogMessages">ログ出力の期待値</param>
        /// <param name="remarks">備考欄</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\Functions_ParentChildrenConnectionMonitor_MonitorParentChildConnectTable.csv")]
        public void MonitorParentChildConnectTableTest(string no, string in_InitializeSqlSet, string expected_AlarmQueueSet, string expected_LogMessages, string remarks)
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
            }

            // 期待値
            List<string> expected_queues = Directory.Exists(expected_AlarmQueueSet) ? Directory.GetFiles(expected_AlarmQueueSet, "*.json").Select(x => File.ReadAllText(x)).ToList() : new List<string>();
            List<string> expected_log_messages = (expected_LogMessages != null && File.Exists(expected_LogMessages)) ? File.ReadLines(expected_LogMessages).ToList() : new List<string>();

            // テスト実行
            target.MonitorParentChildConnectTable(null, new TestLogger<ParentChildrenConnectionMonitorController>(actual_logs));

            // テスト結果
            List<string> actual_queses = QueueOperation.GetMessages(settings.AlarmQueueName, settings.QueueStorageConnectionString);
            List<string> actual_log_messages = actual_logs.Select(x => x.GetSimpleText()).ToList();

            // 確認
            CollectionAssert.AreEqual(expected_queues, actual_queses);
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
            builder.ServiceCollection.AddTransient<ParentChildrenConnectionMonitorController>();
            builder.AddConfigures(appSettings);

            Mock<DateTimeProvider> timeProviderMock = new Mock<DateTimeProvider>();
            timeProviderMock.SetupGet(tp => tp.UtcNow).Returns(TestTime);
            builder.ServiceCollection.AddSingleton<ITimeProvider>(timeProviderMock.Object);

            if (testLogs != null)
            {
                builder.ServiceCollection.AddSingleton<ILogger<ParentChildrenConnectionMonitorService>>(new TestLogger<ParentChildrenConnectionMonitorService>(testLogs));
            }

            ServiceProvider provider = builder.Build();
            settings = provider.GetService<UtilityAppSettings>();
            target = provider.GetService<ParentChildrenConnectionMonitorController>();
        }
    }
}
