using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Rms.Server.Core.Azure.Functions.DbCleaner;
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

namespace Azure.Functions.DbCleanerTest
{
    /// <summary>
    /// DbCleanControllerTest
    /// </summary>
    [TestClass]
    public class DbCleanControllerTest
    {
        /// <summary>�폜�Ώۃe�[�u���̃e�[�u����</summary>
        /// <remarks>�폜����邱�Ƃ��m�F����e�[�u���Q�B�ˑ��֌W���i�ˑ����������j�B</remarks>
        private static readonly string[] TargetTableNames = new string[]
        {
            "core.DT_SOFT_VERSION",
            "core.DT_DISK_DRIVE",
            "core.DT_DIRECTORY_USAGE",
            "core.DT_INVENTORY",
            "core.DT_EQUIPMENT_USAGE",
            "core.DT_DRIVE",
            "core.DT_PLUS_SERVICE_BILL_LOG",
            "core.DT_DXA_BILL_LOG",
            "core.DT_DXA_QC_LOG",
            "core.DT_INSTALL_RESULT",
        };

        /// <summary>�폜�ΏۊO�e�[�u���̃e�[�u����</summary>
        /// <remarks>�폜����Ȃ����Ƃ��m�F����e�[�u���Q�B�폜�Ώۃe�[�u���ƃ����[�V����������e�[�u����I�o����B�ˑ��֌W���i�ˑ����������j�B</remarks>
        private static readonly string[] NonTargetTableNames = new string[]
        {
            "core.MT_EQUIPMENT_TYPE",
            "core.MT_INSTALL_TYPE",
            "core.MT_EQUIPMENT_MODEL",
            "core.MT_CONNECT_STATUS",
            "core.MT_DELIVERY_FILE_TYPE",
            "core.MT_DELIVERY_GROUP_STATUS",
            "core.MT_INSTALL_RESULT_STATUS",
            "core.DT_DEVICE",
            "core.DT_DELIVERY_FILE",
            "core.DT_DELIVERY_GROUP",
            "core.DT_DELIVERY_RESULT",
        };

        /// <summary>�e�X�g�Ŏg�p����S�e�[�u���̃e�[�u����</summary>
        /// <remarks>�ˑ��֌W���i�ˑ����������j</remarks>
        private static readonly string[] AllTableNames = NonTargetTableNames.Concat(TargetTableNames).ToArray();

        /// <summary>�e�X�g���s����</summary>
        private static readonly DateTime TestTime = new DateTime(2020, 5, 7);

        /// <summary>�A�v���P�[�V�����ݒ�</summary>
        private AppSettings settings;

        /// <summary>�e�X�g�Ώ�</summary>
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
        /// <param name="no">�e�X�g�ԍ�</param>
        /// <param name="mock_Exception">DbCleanerService���X���[����Exception�N���X��</param>
        /// <param name="in_AppSettingsJson">�A�v���P�[�V�����ݒ肪�L�ڂ���Ă���json�t�@�C���̃t�@�C���p�X</param>
        /// <param name="in_InitializeSqlList">DB�̏�����Ԃ𐶐����邽�߂�sql�t�@�C���̃t�@�C���p�X�̃��X�g�i�J���}��؂�j</param>
        /// <param name="expected_TargetTableDataSet">�폜�Ώۃe�[�u���̊��Ғl���L�ڂ���csv�t�@�C�����i�[����Ă���t�H���_�p�X</param>
        /// <param name="expected_NonTargetTableDataSet">�폜�ΏۊO�e�[�u���̊��Ғl���L�ڂ���csv�t�@�C�����i�[����Ă���t�H���_�p�X</param>
        /// <param name="expected_ExistLogMessages">�o�͂���郍�O�̃��X�g���L�ڂ���txt�t�@�C���̃p�X</param>
        /// <param name="expected_NotExistLogMessages">�o�͂���Ȃ����O�̃��X�g���L�ڂ���txt�t�@�C���̃p�X</param>
        /// <param name="remarks">���l��</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\DbCleanController_CleanDb.csv")]
        public void CleanDbTest(string no, string mock_Exception, string in_AppSettingsJson, string in_InitializeSqlList, string expected_TargetTableDataSet, string expected_NonTargetTableDataSet, string expected_ExistLogMessages, string expected_NotExistLogMessages, string remarks)
        {
            List<TestLog> actual_logs = new List<TestLog>();

            // �A�v���P�[�V�����ݒ�ilocal.settings.json�ɏ㏑������ݒ�j
            Dictionary<string, string> appSettings = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(in_AppSettingsJson));

            // DI
            DependencyInjection(appSettings, actual_logs, Type.GetType(mock_Exception));

            // �e�X�g�f�[�^����
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

            // ���Ғl
            Dictionary<string, DataTable> expected_target_tables = Directory.GetFiles(expected_TargetTableDataSet, "*.csv", SearchOption.AllDirectories)
                .ToDictionary(
                    x => Path.GetFileNameWithoutExtension(x),
                    y => DbTestHelper.SelectCsv(y, "SELECT SID, COLLECT_DATETIME FROM " + Path.GetFileNameWithoutExtension(y)));
            Dictionary<string, DataTable> expected_nontarget_tables = Directory.GetFiles(expected_NonTargetTableDataSet, "*.csv", SearchOption.AllDirectories)
                .ToDictionary(
                    x => Path.GetFileNameWithoutExtension(x),
                    y => DbTestHelper.SelectCsv(y, "SELECT SID FROM " + Path.GetFileNameWithoutExtension(y)));
            List<string> expected_exist_log_messages = File.ReadLines(expected_ExistLogMessages).ToList();
            List<string> expected_notexist_log_messages = File.ReadLines(expected_NotExistLogMessages).ToList();

            // �e�X�g���s
            target.CleanDb(null, new TestLogger<DbCleanController>(actual_logs));

            // �e�X�g����
            Dictionary<string, DataTable> actual_target_tables = TargetTableNames.ToDictionary(x => x, y => DbTestHelper.SelectTable("SELECT SID, COLLECT_DATETIME FROM " + y));
            Dictionary<string, DataTable> actual_nontarget_tables = NonTargetTableNames.ToDictionary(x => x, y => DbTestHelper.SelectTable("SELECT SID FROM " + y));
            List<string> actual_log_messages = actual_logs.Select(x => x.GetSimpleText()).ToList();

            // �m�F�i�폜�Ώۃe�[�u���j
            foreach (string tableName in TargetTableNames)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected_target_tables[tableName].Rows.Count, actual_target_tables[tableName].Rows.Count);
                for (int i = 0; i < expected_target_tables[tableName].Rows.Count; i++)
                {
                    CollectionAssert.AreEqual(expected_target_tables[tableName].Rows[i].ItemArray, actual_target_tables[tableName].Rows[i].ItemArray);
                }
            }

            // �m�F�i�폜�ΏۊO�e�[�u���j
            foreach (string tableName in NonTargetTableNames)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected_nontarget_tables[tableName].Rows.Count, actual_nontarget_tables[tableName].Rows.Count);
                for (int i = 0; i < expected_nontarget_tables[tableName].Rows.Count; i++)
                {
                    CollectionAssert.AreEqual(expected_nontarget_tables[tableName].Rows[i].ItemArray, actual_nontarget_tables[tableName].Rows[i].ItemArray);
                }
            }

            // �m�F�i��o�̓��O�̊m�F�j
            foreach (var expected_log in expected_notexist_log_messages)
            {
                var matching_element = actual_log_messages.FirstOrDefault(actual => actual.Contains(expected_log));
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNull(matching_element, string.Format("�u{0}�v�Ɉ�v����v�f�����݂��܂�", expected_log));
            }

            // �m�F�i�o�̓��O�̊m�F�j
            foreach (var expected_log in expected_exist_log_messages)
            {
                var matching_element = actual_log_messages.FirstOrDefault(actual => actual.Contains(expected_log));
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(matching_element, string.Format("�u{0}�v�Ɉ�v����v�f��������܂���", expected_log));
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
        /// <param name="exceptionType">DbCleanerService�̃��b�N�ɗ�O���X���[������ꍇ���̃^�C�v���w�肷��BDbCleanerService�̃��b�N���g�p���Ȃ��ꍇ��null���w�肷��</param>
        private void DependencyInjection(Dictionary<string, string> appSettings = null, List<TestLog> testLogs = null, Type exceptionType = null)
        {
            var builder = new TestDiProviderBuilder<AppSettings>(FunctionsHostBuilderExtend.AddUtility);
            builder.ServiceCollection.AddTransient<DbCleanController>();
            if (exceptionType != null)
            {
                Exception exception = Activator.CreateInstance(exceptionType) as Exception;
                builder.ServiceCollection.AddTransient<ICleanDbService>(x => new ErrorCleanDbService(exception));
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
            settings = provider.GetService<AppSettings>() as AppSettings;
            target = provider.GetService<DbCleanController>();
        }

        /// <summary>
        /// ��ɃG���[����������CleanDbService�N���X
        /// </summary>
        public class ErrorCleanDbService : ICleanDbService
        {
            /// <summary>�����������O</summary>
            private Exception exception = null;

            /// <summary>
            /// �R���X�g���N�^
            /// </summary>
            /// <param name="e">�����������O</param>
            public ErrorCleanDbService(Exception e)
            {
                exception = e;
            }

            /// <summary>
            /// �s�v�f�[�^�̍폜
            /// </summary>
            public void Clean()
            {
                throw exception;
            }
        }
    }
}
