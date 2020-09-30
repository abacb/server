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
    /// 親子間通信データの単体テストクラス
    /// </summary>
    /// <remarks>CRUD処理はauto-generatedと一緒なので割愛。</remarks>
    [TestClass]
    public class DtParentChildConnectRepositoryTest
    {
        /// <summary>
        /// DtParentChildConnectRepository
        /// </summary>
        private static DtParentChildConnectRepository _parentChildConnectRepository = null;

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
            builder.ServiceCollection.AddTransient<DtParentChildConnectRepository>();
            ServiceProvider provider = builder.Build();
            _parentChildConnectRepository = provider.GetService<DtParentChildConnectRepository>();
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
        /// Saveメソッド(DtParentChildConnectFromParent)のテスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InsertNewDataSqlPath">挿入するデータのSQLファイルパス</param>
        /// <param name="in_DeleteNewDataSqlPath">削除するデータのSQLファイルパス</param>
        /// <param name="expected_DataJsonPath">期待するJsonデータファイルパス</param>
        /// <param name="expected_ExceptionType">期待する例外タイプ</param>
        /// <param name="expected_ExceptionMessage">期待する例外メッセージ</param>
        /// <param name="remarks">備考</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\Dispatcher\Repositories_DtParentChildConnectRepository_SaveFromParent.csv")]
        public void SaveFromParentTest(
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
            var newParentData = new DtParentChildConnectFromParent()
            {
                ParentDeviceUid = "ParentDeviceUid",
                ChildDeviceUid = "ChildDeviceUid",
                ParentResult = false,
                ParentConfirmDatetime = DateTime.Parse("2020/4/2 0:00:00")
            };

            if (expected_ExceptionType == typeof(RmsException).FullName)
            {
                // 各パラメータでnull非許容の場合の確認
                if (remarks.Contains("ParentResult"))
                {
                    newParentData.ParentResult = null;
                }
                else if (remarks.Contains("ParentConfirmDatetime"))
                {
                    newParentData.ParentConfirmDatetime = null;
                }
                else if (remarks.Contains("All"))
                {
                    newParentData = null;
                }
                else
                {
                    // UIDを存在しないものにする
                    newParentData.ParentDeviceUid = null;
                    newParentData.ChildDeviceUid = null;
                }
            }

            string exceptionName = "";
            string exceptionMessage = "";
            try
            {
                var savedParentData = _parentChildConnectRepository.Save(newParentData);
                if (savedParentData != null)
                {
                    // 比較に使用しない値はnull or 固定値とする
                    savedParentData.CreateDatetime = DateTime.Parse("2020/4/1 0:00:00");
                    savedParentData.UpdateDatetime = DateTime.Parse("2020/4/1 0:00:00");
                }

                // データのjson化
                string readJson = Utility.ObjectExtensions.ToStringJson(savedParentData);
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
            catch (RmsException e)
            {
                exceptionName = e.GetType().FullName;
                exceptionMessage = e.Message;
            }
            // 例外発生チェック
            Assert.AreEqual(expected_ExceptionType, exceptionName);
            Assert.AreEqual(expected_ExceptionMessage, exceptionMessage);

            // 後処理
            RepositoryTestHelper.ExecDeleteSql(in_DeleteNewDataSqlPath);
        }

        /// <summary>
        /// Saveメソッド(DtParentChildConnectFromChild)のテスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InsertNewDataSqlPath">挿入するデータのSQLファイルパス</param>
        /// <param name="in_DeleteNewDataSqlPath">削除するデータのSQLファイルパス</param>
        /// <param name="expected_DataJsonPath">期待するJsonデータファイルパス</param>
        /// <param name="expected_ExceptionType">期待する例外タイプ</param>
        /// <param name="expected_ExceptionMessage">期待する例外メッセージ</param>
        /// <param name="remarks">備考</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\Dispatcher\Repositories_DtParentChildConnectRepository_SaveFromChild.csv")]
        public void SaveFromChildTest(
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
            var newChildData = new DtParentChildConnectFromChild()
            {
                ParentDeviceUid = "ParentDeviceUid",
                ChildDeviceUid = "ChildDeviceUid",
                ChildResult = false,
                ChildConfirmDatetime = DateTime.Parse("2020/4/2 0:00:00")
            };

            if (expected_ExceptionType == typeof(RmsException).FullName)
            {
                // 各パラメータでnull非許容の場合の確認
                if (remarks.Contains("ChildResult"))
                {
                    newChildData.ChildResult = null;
                }
                else if (remarks.Contains("ChildConfirmDatetime"))
                {
                    newChildData.ChildConfirmDatetime = null;
                }
                else if (remarks.Contains("All"))
                {
                    newChildData = null;
                }
                else
                {
                    // UIDを存在しないものにする
                    newChildData.ParentDeviceUid = null;
                    newChildData.ChildDeviceUid = null;
                }
            }

            string exceptionName = "";
            string exceptionMessage = "";
            try
            {
                var savedCHildData = _parentChildConnectRepository.Save(newChildData);
                if (savedCHildData != null)
                {
                    // 比較に使用しない値はnull or 固定値とする
                    savedCHildData.CreateDatetime = DateTime.Parse("2020/4/1 0:00:00");
                    savedCHildData.UpdateDatetime = DateTime.Parse("2020/4/1 0:00:00");
                }

                // データのjson化
                string readJson = Utility.ObjectExtensions.ToStringJson(savedCHildData);
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
            catch (RmsException e)
            {
                exceptionName = e.GetType().FullName;
                exceptionMessage = e.Message;
            }
            // 例外発生チェック
            Assert.AreEqual(expected_ExceptionType, exceptionName);
            Assert.AreEqual(expected_ExceptionMessage, exceptionMessage);

            // 後処理
            RepositoryTestHelper.ExecDeleteSql(in_DeleteNewDataSqlPath);
        }
    }
}