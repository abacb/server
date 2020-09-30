using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;
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
    /// DispatchPlusServiceBillLogTest
    /// </summary>
    [TestClass]
    public class DispatchPlusServiceBillLogTest
    {
        /// <summary>�e�X�g���ʊi�[��̃��[�g�t�H���_</summary>
        private static readonly string TestResultRootDir = string.Format(@"TestResult.{0:yyyyMMddHHmmss}\{1}", DateTime.Now, nameof(DispatchController.DispatchPlusServiceBillLog));

        /// <summary>�e�X�g�ݒ�</summary>
        private static readonly Dictionary<string, string> DefaultAppSettingValues = new Dictionary<string, string>()
        {
        };

        /// <summary>�]���ΏۊO�̃J������</summary>
        private static readonly string[] IgnoreColumns = new string[]
        {
        };

        /// <summary>�e�X�g���s����</summary>
        private static readonly DateTime TestTime = new DateTime(2030, 1, 1);

        /// <summary>FailureBlob</summary>
        private FailureBlob failureBlob;

        /// <summary>�A�v���P�[�V�����ݒ�</summary>
        private AppSettings settings;

        /// <summary>�e�X�g�Ώ�</summary>
        private DispatchController target;

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
            InitializeTestData();

            // �}�X�^�e�[�u���f�[�^���쐬����
            DbTestHelper.ExecSqlFromFilePath(@"TestData\MakeMasterTableData.sql");
        }

        /// <summary>
        /// TestCleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            InitializeTestData();
        }

        /// <summary>
        /// ServiceBillLogTest
        /// </summary>
        /// <param name="no">�e�X�g�ԍ�</param>
        /// <param name="in_InitializeSqlSet">DB�̏�����Ԃ𐶐����邽�߂�sql�t�@�C�����i�[����Ă���t�H���_�p�X</param>
        /// <param name="in_Message">�C���v�b�gEventHub���b�Z�[�W</param>
        /// <param name="expected_TableData">DB�̊��Ғl</param>
        /// <param name="expected_FailureBlobFileSet">FailureBlob�̃t�@�C���E�t�H���_�\���̊��Ғl</param>
        /// <param name="expected_LogMessages">���O�o�͂̊��Ғl</param>
        /// <param name="expected_WithoutError">true�̏ꍇ�ɂ̓G���[���b�Z�[�W���Ȃ����Ƃ��m�F����</param>
        /// <param name="remarks">���l��</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\DispatchController_DispatchPlusServiceBillLog.csv")]
        public void ServiceBillLogTest(string no, string in_InitializeSqlSet, string in_Message, string expected_TableData, string expected_FailureBlobFileSet, string expected_LogMessages, string expected_WithoutError, string remarks)
        {
            List<TestLog> actual_logs = new List<TestLog>();

            // DI
            DependencyInjection(DefaultAppSettingValues, actual_logs);

            // �e�X�g�f�[�^����
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

            // �e�X�g���s��A�e�[�u����]�����邩�ǂ���
            bool isCheckTable = !string.IsNullOrEmpty(expected_TableData) && File.Exists(expected_TableData);

            // �]���Ώۃe�[�u����
            string targetTableName = isCheckTable ? Path.GetFileNameWithoutExtension(expected_TableData) : null;

            // ���Ғl
            DataTable expected_table = isCheckTable ? DbTestHelper.SelectCsv(expected_TableData, "SELECT * FROM " + targetTableName) : null;
            DirectoryInfo expectedFailureDir = new DirectoryInfo(expected_FailureBlobFileSet);
            string[] expectedFailureFiles = expectedFailureDir.Exists ? expectedFailureDir.GetFiles("*", SearchOption.AllDirectories).Select(x => x.FullName).OrderBy(x => x).ToArray() : new string[] { };
            string[] expected_failure_names = expectedFailureFiles.Select(x => x.Replace(expectedFailureDir.FullName, string.Empty)).ToArray();
            string[] expected_failure_contents = expectedFailureFiles.Select(x => File.ReadAllText(x)).ToArray();
            List<string> expected_log_messages = (!string.IsNullOrEmpty(expected_LogMessages) && File.Exists(expected_LogMessages)) ? File.ReadLines(expected_LogMessages).ToList() : new List<string>();

            // �e�X�g���s
            target.DispatchPlusServiceBillLog(in_Message, new TestLogger<DispatchController>(actual_logs));

            // �e�X�g����
            DataTable actual_table = isCheckTable ? DbTestHelper.SelectTable("SELECT * FROM " + targetTableName) : null;
            DirectoryInfo actualDir = new DirectoryInfo(Path.Combine(TestResultRootDir, no));
            string[] actualFailureFiles = failureBlob.Client.GetFiles(settings.FailureBlobContainerNameDispatcher, actualDir).OrderBy(x => x).ToArray();
            string[] actual_failure_names = actualFailureFiles.Select(x => x.Replace(actualDir.FullName, string.Empty)).ToArray();
            string[] actual_failure_contents = actualFailureFiles.Select(x => File.ReadAllText(x)).ToArray();
            List<string> actual_log_messages = actual_logs.Select(x => x.GetSimpleText()).ToList();

            // �m�F
            if (isCheckTable)
            {
                // �]���ΏۊO�̃J���������O
                Array.ForEach(IgnoreColumns, x => expected_table.Columns.Remove(x));
                Array.ForEach(IgnoreColumns, x => actual_table.Columns.Remove(x));

                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected_table.Rows.Count, actual_table.Rows.Count);
                for (int i = 0; i < expected_table.Rows.Count; i++)
                {
                    CollectionAssert.AreEqual(expected_table.Rows[i].ItemArray, actual_table.Rows[i].ItemArray);
                }
            }

            CollectionAssert.AreEqual(expected_failure_names, actual_failure_names);
            CollectionAssert.AreEqual(expected_failure_contents, actual_failure_contents);

            bool.TryParse(expected_WithoutError, out bool isWithoutError);
            if (isWithoutError)
            {
                var error_logs = actual_logs.Where(x => x.LogLevel == Microsoft.Extensions.Logging.LogLevel.Error);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(false, error_logs.Any());
            }

            foreach (var expected_log_message in expected_log_messages)
            {
                var matching_element = actual_log_messages.FirstOrDefault(actual => actual.Contains(expected_log_message));
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(matching_element, string.Format("�u{0}�v�Ɉ�v����v�f��������܂���", expected_log_message));
                if (matching_element != null)
                {
                    actual_log_messages.Remove(matching_element);
                }
            }
        }

        /// <summary>
        /// DI�����s����
        /// </summary>
        /// <param name="appSettings">�A�v���P�[�V�����ݒ���㏑������ꍇ�͎w�肷��</param>
        /// <param name="testLogs">���O�̊i�[��</param>
        private void DependencyInjection(Dictionary<string, string> appSettings = null, List<TestLog> testLogs = null)
        {
            var builder = new TestDiProviderBuilder<AppSettings>(FunctionsHostBuilderExtend.AddUtility);
            builder.ServiceCollection.AddTransient<DispatchController>();
            builder.ServiceCollection.AddTransient<FailureBlob>();
            builder.AddConfigures(appSettings);

            Mock<DateTimeProvider> timeProviderMock = new Mock<DateTimeProvider>();
            timeProviderMock.SetupGet(tp => tp.UtcNow).Returns(TestTime);
            builder.ServiceCollection.AddSingleton<ITimeProvider>(timeProviderMock.Object);

            if (testLogs != null)
            {
                builder.ServiceCollection.AddSingleton<ILogger<DispatchService>>(new TestLogger<DispatchService>(testLogs));
            }

            ServiceProvider provider = builder.Build();
            failureBlob = provider.GetService<FailureBlob>();
            settings = provider.GetService<AppSettings>() as AppSettings;
            target = provider.GetService<DispatchController>();
        }

        /// <summary>
        /// �e�X�g�f�[�^�̏��������s��
        /// </summary>
        private void InitializeTestData()
        {
            DependencyInjection();

            foreach (CloudBlockBlob blockBlob in failureBlob.Client.GetBlockBlobs(DefaultAppSettingValues["FailureBlobContainerNameDispatcher"]))
            {
                blockBlob.DeleteIfExistsAsync().Wait();
            }

            DbTestHelper.DeleteAllReseed();

            using (SqlConnection connection = new SqlConnection(settings.PrimaryDbConnectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("TRUNCATE TABLE core.DT_PLUS_SERVICE_BILL_LOG;", connection))
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
