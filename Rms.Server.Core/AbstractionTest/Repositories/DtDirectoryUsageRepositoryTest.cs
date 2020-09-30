using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Core.Utility.Models.Entites;
using Rms.Server.Test;
using System;
using System.IO;
using TestHelper;

namespace Rms.Server.Core.AbstractionTest.Repositories
{
    /// <summary>
    /// ディレクトリ使用量の単体テストクラス
    /// </summary>
    /// <remarks>Create以外はauto-generatedと一緒なので割愛。他のDispatcher関連でも同様の処理をしているため、DtDirectoryUsageRepositoryを代表としてテストする。</remarks>
    [TestClass]
    public class DtDirectoryUsageRepositoryTest
    {
        /// <summary>
        /// DtDirectoryUsageRepository
        /// </summary>
        private static DtDirectoryUsageRepository _directoryUsageRepository = null;

        /// <summary>
        /// InstancesOnDb
        /// </summary>
        private InstancesOnDb _dataOnDb = null;

        /// <summary>
        /// ClassInit
        /// </summary>
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            // 関連DBデータを全削除
            DbTestHelper.DeleteAllReseed();

            // Repository生成
            TestDiProviderBuilder builder = new TestDiProviderBuilder();
            builder.ServiceCollection.AddTransient<DtDirectoryUsageRepository>();
            ServiceProvider provider = builder.Build();
            _directoryUsageRepository = provider.GetService<DtDirectoryUsageRepository>();
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
            // 関連DBデータを作成
            _dataOnDb = DbTestHelper.CreateMasterTables();
            _dataOnDb = DbTestHelper.CreateDeliveries(_dataOnDb);
            _dataOnDb = DbTestHelper.CreateDevices(_dataOnDb);
        }

        /// <summary>
        /// TestCleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            // 関連DBデータを全削除
            DbTestHelper.DeleteAllReseed();
        }

        /// <summary>
        /// CreateDtDirectoryUsageIfAlreadyMessageThrowExメソッドのテスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InsertNewDataSqlPath">挿入するデータのSQLファイルパス</param>
        /// <param name="in_DeleteNewDataSqlPath">削除するデータのSQLファイルパス</param>
        /// <param name="expected_DataJsonPath">期待するJsonデータファイルパス</param>
        /// <param name="expected_ExceptionType">期待する例外タイプ</param>
        /// <param name="expected_ExceptionMessage">期待する例外メッセージ</param>
        /// <param name="remarks">備考</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\Dispatcher\Repositories_DtDirectoryUsage_CreateDtDirectoryUsageIfAlreadyMessageThrowEx.csv")]
        public void CreateDtDirectoryUsageIfAlreadyMessageThrowExTest(
            string no,
            string in_InsertNewDataSqlPath,
            string in_DeleteNewDataSqlPath,
            string expected_DataJsonPath,
            string expected_ExceptionType,
            string expected_ExceptionMessage,
            string remarks)
        {
            // 初期データ挿入
            RepositoryTestHelper.ExecInsertSql(in_InsertNewDataSqlPath);

            // データを作成する
            var baseDateTime = DateTime.UtcNow;
            var newDirectoryUsageData = new DtDirectoryUsage()
            {
                DeviceSid = 1,
                SourceEquipmentUid = RepositoryTestHelper.CreateSpecifiedNumberString(30), // 上限値いっぱい
                MessageId = RepositoryTestHelper.CreateSpecifiedNumberString(64), // 上限値いっぱい、既存でないメッセージID
                CreateDatetime = baseDateTime
            };

            if (expected_ExceptionType == typeof(RmsException).FullName)
            {
                // 存在しないSidを指定する
                newDirectoryUsageData.DeviceSid = 999;
            }
            else if (expected_ExceptionType == typeof(ArgumentNullException).FullName)
            {
                newDirectoryUsageData = null;
            }
            else if (expected_ExceptionType == typeof(RmsAlreadyExistException).FullName)
            {
                newDirectoryUsageData.MessageId = "hoge"; // 既存のメッセージID
            }
            else if (expected_ExceptionType == typeof(RmsParameterException).FullName)
            {
                if (expected_ExceptionMessage.Contains("SourceEquipmentUid"))
                {
                    newDirectoryUsageData.SourceEquipmentUid = RepositoryTestHelper.CreateSpecifiedNumberString(65); // 上限値+1
                }
                else if (expected_ExceptionMessage.Contains("MessageId"))
                {
                    newDirectoryUsageData.MessageId = RepositoryTestHelper.CreateSpecifiedNumberString(65); // 上限値+1、既存でないメッセージ
                }
            }

            string exceptionName = "";
            string exceptionMessage = "";
            try
            {
                var createdDirectoryUsageData = _directoryUsageRepository.CreateDtDirectoryUsageIfAlreadyMessageThrowEx(newDirectoryUsageData);
                if (createdDirectoryUsageData != null)
                {
                    // 作成日時はDB側で設定されることを確認する
                    Assert.AreNotEqual(baseDateTime, createdDirectoryUsageData.CreateDatetime);

                    // 比較に使用しない値はnull or 固定値とする
                    createdDirectoryUsageData.CreateDatetime = DateTime.Parse("2020/4/1 0:00:00");
                }

                // データのjson化
                string readJson = Utility.ObjectExtensions.ToStringJson(createdDirectoryUsageData);
                string expectJson = null;
                if (File.Exists(expected_DataJsonPath))
                {
                    expectJson = File.ReadAllText(expected_DataJsonPath);
                }

                // データの比較
                Assert.AreEqual(expectJson, readJson);

                // TODO DBデータ内容をチェックする
            }
            catch (RmsAlreadyExistException e)
            {
                exceptionName = e.GetType().FullName;
                exceptionMessage = e.Message;
            }
            catch (RmsParameterException e)
            {
                exceptionName = e.GetType().FullName;
                exceptionMessage = e.Message;
            }
            catch (RmsException e)
            {
                exceptionName = e.GetType().FullName;
                exceptionMessage = e.Message;
            }
            catch (ArgumentNullException e)
            {
                exceptionName = e.GetType().FullName;
                exceptionMessage = typeof(DtDirectoryUsage).FullName + " is null."; // HACK ←の部分をメッセージから抽出できれば...
            }
            // 例外発生チェック
            Assert.AreEqual(expected_ExceptionType, exceptionName);
            Assert.AreEqual(expected_ExceptionMessage, exceptionMessage);

            // 後処理
            RepositoryTestHelper.ExecDeleteSql(in_DeleteNewDataSqlPath);
        }
    }
}