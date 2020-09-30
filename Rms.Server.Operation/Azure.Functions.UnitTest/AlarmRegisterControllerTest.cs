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
        /// <summary>�e�X�g���ʊi�[��̃��[�g�t�H���_</summary>
        private static readonly string TestResultRootDir = string.Format(@"TestResult.{0:yyyyMMddHHmmss}\{1}", DateTime.Now, typeof(AlarmRegisterController).Name);

        /// <summary>DB�̎��O�ݒ���s�����߂�SQL�t�@�C��</summary>
        private static readonly string DbPresetSqlFile = @"TestData\Azure.Functions\AlarmRegister\DbPreset.sql";

        /// <summary>�e�X�g�ݒ�</summary>
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

        /// <summary>�]���ΏۊO�̃J������</summary>
        private static readonly string[] IgnoreColumns = new string[]
        {
        };

        /// <summary>���t�`���̃J������</summary>
        private static readonly string[] DateTimeColumns = new string[]
        {
            "ALARM_DATETIME",
            "EVENT_DATETIME",
            "CREATE_DATETIME"
        };

        /// <summary>�e�X�g���s����</summary>
        private static readonly DateTime TestTime = new DateTime(2030, 1, 1);

        /// <summary>FailureBlob</summary>
        private FailureBlob failureBlob;

        /// <summary>�A�v���P�[�V�����ݒ�</summary>
        private OperationAppSettings settings;

        /// <summary>�e�X�g�Ώ�</summary>
        private AlarmRegisterController target;

        /// <summary>
        /// �e�[�u���N���A��SQL���擾����
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
        /// <param name="no">�e�X�g�ԍ�</param>
        /// <param name="in_InitializeSqlSet">DB�̏�����Ԃ𐶐����邽�߂�sql�t�@�C�����i�[����Ă���t�H���_�p�X</param>
        /// <param name="in_QueueItem">�C���v�b�g�A���[���L���[���L�ڂ���json�t�@�C���̃p�X</param>
        /// <param name="expected_MailQueueSet">���[���L���[�̊��Ғl���L�ڂ���json�t�@�C�����i�[����Ă���t�H���_�p�X</param>
        /// <param name="expected_TableData">DT_ALARM�e�[�u���̊��Ғl���L�ڂ���csv�t�@�C���̃p�X</param>
        /// <param name="expected_FailureBlobFileSet">FailureBlob�̃t�@�C���E�t�H���_�\���̊��Ғl</param>
        /// <param name="expected_LogMessages">���O�o�͂̊��Ғl���L�ڂ����t�@�C���̃p�X</param>
        /// <param name="remarks">���l��</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\Functions_AlarmRegister_DequeueAlarmInfo.csv")]
        public void DequeueAlarmInfoTest(string no, string in_InitializeSqlSet, string in_QueueItem, string expected_MailQueueSet, string expected_TableData, string expected_FailureBlobFileSet, string expected_LogMessages, string remarks)
        {
            List<TestLog> actual_logs = new List<TestLog>();

            // DI
            DependencyInjection(DefaultAppSettingValues, actual_logs);

            // �e�X�g�f�[�^����
            {
                if (Directory.Exists(in_InitializeSqlSet))
                {
                    // �ݒu�x�[�X�f�[�^�e�[�u���A�@��f�[�^�e�[�u�����ɐݒ肷��
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

            // �e�X�g���s��A�e�[�u����]�����邩�ǂ���
            bool isCheckTable = !string.IsNullOrEmpty(expected_TableData) && File.Exists(expected_TableData);

            // �]���Ώۃe�[�u����
            string targetTableName = isCheckTable ? Path.GetFileNameWithoutExtension(expected_TableData) : null;

            // ���Ғl
            List<string> expected_queues = Directory.Exists(expected_MailQueueSet) ? Directory.GetFiles(expected_MailQueueSet, "*.json").Select(x => File.ReadAllText(x)).ToList() : new List<string>();
            DataTable expected_table = isCheckTable ? DataTableHelper.SelectCsv(settings.PrimaryDbConnectionString, expected_TableData, targetTableName) : null;
            DirectoryInfo expectedFailureDir = new DirectoryInfo(expected_FailureBlobFileSet);
            string[] expectedFailureFiles = expectedFailureDir.Exists ? expectedFailureDir.GetFiles("*", SearchOption.AllDirectories).Select(x => x.FullName).OrderBy(x => x).ToArray() : new string[] { };
            string[] expected_failure_names = expectedFailureFiles.Select(x => x.Replace(expectedFailureDir.FullName, string.Empty)).ToArray();
            string[] expected_failure_contents = expectedFailureFiles.Select(x => File.ReadAllText(x)).ToArray();
            List<string> expected_log_messages = (expected_LogMessages != null && File.Exists(expected_LogMessages)) ? File.ReadLines(expected_LogMessages).ToList() : new List<string>();

            // �e�X�g���s
            target.DequeueAlarmInfo(in_QueueItem, new TestLogger<AlarmRegisterController>(actual_logs));

            // �e�X�g����
            List<string> actual_queses = QueueOperation.GetMessages(settings.MailQueueName, settings.MailQueueConnectionString);
            DataTable actual_table = isCheckTable ? DataTableHelper.SelectTable(settings.PrimaryDbConnectionString, "SELECT * FROM " + targetTableName) : null;
            DirectoryInfo actualDir = new DirectoryInfo(Path.Combine(TestResultRootDir, no));
            string[] actualFailureFiles = failureBlob.Client.GetFiles(settings.FailureBlobContainerName, actualDir).OrderBy(x => x).ToArray();
            string[] actual_failure_names = actualFailureFiles.Select(x => x.Replace(actualDir.FullName, string.Empty)).ToArray();
            string[] actual_failure_contents = actualFailureFiles.Select(x => File.ReadAllText(x)).ToArray();
            List<string> actual_log_messages = actual_logs.Select(x => x.GetSimpleText()).ToList();

            // �m�F�i���[���L���[�j
            CollectionAssert.AreEqual(expected_queues, actual_queses);

            // �m�F�iDT_ALARM�j
            if (isCheckTable)
            {
                // �]���ΏۊO�̃J���������O
                Array.ForEach(IgnoreColumns, x => expected_table.Columns.Remove(x));
                Array.ForEach(IgnoreColumns, x => actual_table.Columns.Remove(x));

                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected_table.Rows.Count, actual_table.Rows.Count);

                // ���t�͊��Ғl�ǂݍ��ݎ��ɓ��{�����Ŏ擾����Ă��܂����ߕʂɔ�r����
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

                    // �]�������J���������O
                    expected_table.Columns.Remove(columnName);
                    actual_table.Columns.Remove(columnName);
                }

                for (int i = 0; i < expected_table.Rows.Count; i++)
                {
                    CollectionAssert.AreEqual(expected_table.Rows[i].ItemArray, actual_table.Rows[i].ItemArray);
                }
            }

            // �m�F�iFailure�u���u�j
            CollectionAssert.AreEqual(expected_failure_names, actual_failure_names);
            CollectionAssert.AreEqual(expected_failure_contents, actual_failure_contents);

            // �m�F�i���O�j
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
        /// ���O�ɐݒ肪�K�v��DB�Ƀf�[�^��o�^����
        /// </summary>
        private void PresetDb()
        {
            using (SqlConnection connection = new SqlConnection(settings.PrimaryDbConnectionString))
            {
                try
                {
                    connection.Open();

                    // �ݒu�x�[�X�f�[�^�e�[�u���A�@��f�[�^�e�[�u�����ɐݒ肷��
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
