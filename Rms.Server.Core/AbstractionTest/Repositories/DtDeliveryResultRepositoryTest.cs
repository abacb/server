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
    /// 配信結果の単体テストクラス
    /// </summary>
    /// <remarks>auto-generatedの処理内容はどれも同一なので、DtDeliveryResultを代表としてテストする。</remarks>
    [TestClass]
    public class DtDeliveryResultRepositoryTest
    {
        /// <summary>
        /// DtDeliveryResultRepository
        /// </summary>
        private static DtDeliveryResultRepository _deliveryResultRepository = null;

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
            builder.ServiceCollection.AddTransient<DtDeliveryResultRepository>();
            ServiceProvider provider = builder.Build();
            _deliveryResultRepository = provider.GetService<DtDeliveryResultRepository>();
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
        /// CreateDtDeliveryResultメソッドのテスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InsertNewDataSqlPath">挿入するデータのSQLファイルパス</param>
        /// <param name="in_DeleteNewDataSqlPath">削除するデータのSQLファイルパス</param>
        /// <param name="expected_DataJsonPath">期待するJsonデータファイルパス</param>
        /// <param name="expected_ExceptionType">期待する例外タイプ</param>
        /// <param name="expected_ExceptionMessage">期待する例外メッセージ</param>
        /// <param name="remarks">備考</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\auto-generated\Repositories_DtDeliveryResult_CreateDtDeliveryResult.csv")]
        public void CreateDtDeliveryResultTest(
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
            var newResultData = new DtDeliveryResult()
            {
                DeviceSid = 1,
                GwDeviceSid = 1,
                DeliveryGroupSid = 1,
                CreateDatetime = baseDateTime
            };

            if (expected_ExceptionType == typeof(RmsException).FullName)
            {
                // 存在しないSidを指定する
                newResultData.DeviceSid = 999;
            }
            if (expected_ExceptionType == typeof(ArgumentNullException).FullName)
            {
                newResultData = null;
            }

            string exceptionName = "";
            string exceptionMessage = "";
            try
            {
                var createdResultData = _deliveryResultRepository.CreateDtDeliveryResult(newResultData);
                if (createdResultData != null)
                {
                    // 作成日時はDB側で設定されることを確認する
                    Assert.AreNotEqual(baseDateTime, createdResultData.CreateDatetime);

                    // 比較に使用しない値はnull or 固定値とする
                    createdResultData.CreateDatetime = DateTime.Parse("2020/4/1 0:00:00");
                }

                // データのjson化
                string readJson = Utility.ObjectExtensions.ToStringJson(createdResultData);
                string expectJson = null;
                if (File.Exists(expected_DataJsonPath))
                {
                    expectJson = File.ReadAllText(expected_DataJsonPath);
                }

                // データの比較
                Assert.AreEqual(expectJson, readJson);

                // TODO DBデータ内容をチェックする
            }
            catch (RmsException e)
            {
                exceptionName = e.GetType().FullName;
                exceptionMessage = e.Message;
            }
            catch (ArgumentNullException e)
            {
                exceptionName = e.GetType().FullName;
                exceptionMessage = typeof(DtDeliveryGroup).FullName + " is null."; // HACK ←の部分をメッセージから抽出できれば...
            }
            // 例外発生チェック
            Assert.AreEqual(expected_ExceptionType, exceptionName);
            Assert.AreEqual(expected_ExceptionMessage, exceptionMessage);

            // 後処理
            RepositoryTestHelper.ExecDeleteSql(in_DeleteNewDataSqlPath);
        }

        /// <summary>
        /// ReadDtDeliveryResultメソッドのテスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InsertNewDataSqlPath">挿入するデータのSQLファイルパス</param>
        /// <param name="in_DeleteNewDataSqlPath">削除するデータのSQLファイルパス</param>
        /// <param name="expected_DataJsonPath">期待するJsonデータファイルパス</param>
        /// <param name="expected_ExceptionType">期待する例外タイプ</param>
        /// <param name="expected_ExceptionMessage">期待する例外メッセージ</param>
        /// <param name="remarks">備考</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\auto-generated\Repositories_DtDeliveryResult_ReadDtDeliveryResult.csv")]
        public void ReadDtDeliveryResultTest(
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

            var readResultData = _deliveryResultRepository.ReadDtDeliveryResult(1);

            // データのjson化
            string readJson = Utility.ObjectExtensions.ToStringJson(readResultData);
            string expectJson = null;
            if (File.Exists(expected_DataJsonPath))
            {
                expectJson = File.ReadAllText(expected_DataJsonPath);
            }

            // データの比較
            Assert.AreEqual(expectJson, readJson);

            // 後処理
            RepositoryTestHelper.ExecDeleteSql(in_DeleteNewDataSqlPath);
        }

        /// <summary>
        /// UpdateDtDeliveryResultメソッドのテスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InsertNewDataSqlPath">挿入するデータのSQLファイルパス</param>
        /// <param name="in_DeleteNewDataSqlPath">削除するデータのSQLファイルパス</param>
        /// <param name="expected_DataJsonPath">期待するJsonデータファイルパス</param>
        /// <param name="expected_ExceptionType">期待する例外タイプ</param>
        /// <param name="expected_ExceptionMessage">期待する例外メッセージ</param>
        /// <param name="remarks">備考</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\auto-generated\Repositories_DtDeliveryResult_UpdateDtDeliveryResult.csv")]
        public void UpdateDtDeliveryResultTest(
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

            // 更新データを作成する
            var baseDateTime = DateTime.UtcNow;
            var changeResultData = new DtDeliveryResult()
            {
                Sid = 1,
                DeviceSid = 2,
                GwDeviceSid = 2,
                DeliveryGroupSid = 2,
                CreateDatetime = baseDateTime
            };

            if (expected_ExceptionType == typeof(RmsException).FullName)
            {
                // 存在しないSidを指定する
                changeResultData.DeviceSid = 999;
            }
            if (expected_ExceptionType == typeof(ArgumentNullException).FullName)
            {
                changeResultData = null;
            }

            string exceptionName = "";
            string exceptionMessage = "";
            try
            {
                var updatedResultData = _deliveryResultRepository.UpdateDtDeliveryResult(changeResultData);
                if (updatedResultData != null)
                {
                    // 作成日時まで更新されていることを確認する
                    Assert.AreEqual(baseDateTime, updatedResultData.CreateDatetime);

                    // 比較に使用しない値はnull or 固定値とする
                    updatedResultData.CreateDatetime = DateTime.Parse("2020/4/1 0:00:00");
                }

                // データのjson化
                string readJson = Utility.ObjectExtensions.ToStringJson(updatedResultData);
                string expectJson = null;
                if (File.Exists(expected_DataJsonPath))
                {
                    expectJson = File.ReadAllText(expected_DataJsonPath);
                }

                // データの比較
                Assert.AreEqual(expectJson, readJson);

                // TODO DBデータ内容をチェックする
            }
            catch (RmsException e)
            {
                exceptionName = e.GetType().FullName;
                exceptionMessage = e.Message;
            }
            catch (ArgumentNullException e)
            {
                exceptionName = e.GetType().FullName;
                exceptionMessage = typeof(DtDeliveryGroup).FullName + " is null."; // HACK ←の部分をメッセージから抽出できれば...
            }
            // 例外発生チェック
            Assert.AreEqual(expected_ExceptionType, exceptionName);
            Assert.AreEqual(expected_ExceptionMessage, exceptionMessage);

            // 後処理
            RepositoryTestHelper.ExecDeleteSql(in_DeleteNewDataSqlPath);
        }

        /// <summary>
        /// DeleteDtDeliveryResultメソッドのテスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InsertNewDataSqlPath">挿入するデータのSQLファイルパス</param>
        /// <param name="in_DeleteNewDataSqlPath">削除するデータのSQLファイルパス</param>
        /// <param name="expected_DataJsonPath">期待するJsonデータファイルパス</param>
        /// <param name="expected_ExceptionType">期待する例外タイプ</param>
        /// <param name="expected_ExceptionMessage">期待する例外メッセージ</param>
        /// <param name="remarks">備考</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\auto-generated\Repositories_DtDeliveryResult_DeleteDtDeliveryResult.csv")]
        public void DeleteDtDeliveryResultTest(
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

            var deletedResultData = _deliveryResultRepository.DeleteDtDeliveryResult(1);

            // データのjson化
            string readJson = Utility.ObjectExtensions.ToStringJson(deletedResultData);
            string expectJson = null;
            if (File.Exists(expected_DataJsonPath))
            {
                expectJson = File.ReadAllText(expected_DataJsonPath);
            }

            // データの比較
            Assert.AreEqual(expectJson, readJson);

            // 後処理
            RepositoryTestHelper.ExecDeleteSql(in_DeleteNewDataSqlPath);
        }
    }
}