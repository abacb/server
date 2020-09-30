using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Core.Utility.Models.Entites;
using Rms.Server.Test;
using System;
using System.IO;
using System.Linq;
using TestHelper;

namespace Rms.Server.Core.AbstractionTest.Repositories
{
    /// <summary>
    /// 配信グループ(追加)の単体テストクラス
    /// </summary>
    [TestClass]
    public class DtDeliveryGroupRepositoryTest
    {
        /// <summary>
        /// DtDeliveryGroupRepository
        /// </summary>
        private static DtDeliveryGroupRepository _deliveryGroupRepository = null;

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
            builder.ServiceCollection.AddTransient<DtDeliveryGroupRepository>();
            ServiceProvider provider = builder.Build();
            _deliveryGroupRepository = provider.GetService<DtDeliveryGroupRepository>();
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
        /// ReadDtDeliveryGroupメソッドのテスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InsertNewDataSqlPath">挿入するデータのSQLファイルパス</param>
        /// <param name="in_DeleteNewDataSqlPath">削除するデータのSQLファイルパス</param>
        /// <param name="expected_DataJsonPath">期待するJsonデータファイルパス</param>
        /// <param name="expected_ExceptionType">期待する例外タイプ</param>
        /// <param name="expected_ExceptionMessage">期待する例外メッセージ</param>
        /// <param name="remarks">備考</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\DtDeliveryGroupRepository\Repositories_DtDeliveryGroupRepository_ReadDtDeliveryGroup.csv")]
        public void ReadDtDeliveryGroupTest(
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

            // データを取得する(配信結果付き)
            var readModel = _deliveryGroupRepository.ReadDtDeliveryGroup(1);
            if (readModel != null)
            {
                // 比較に使用しないパラメータはnullにする
                readModel.RowVersion = null;
            }

            // データのjson化
            string readJson = Utility.ObjectExtensions.ToStringJson(readModel);
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
        /// DeleteDtDeliveryGroupTestメソッドのテスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InsertNewDataSqlPath">挿入するデータのSQLファイルパス</param>
        /// <param name="in_DeleteNewDataSqlPath">削除するデータのSQLファイルパス</param>
        /// <param name="expected_DataJsonPath">期待するJsonデータファイルパス</param>
        /// <param name="expected_ExceptionType">期待する例外タイプ</param>
        /// <param name="expected_ExceptionMessage">期待する例外メッセージ</param>
        /// <param name="remarks">備考</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\DtDeliveryGroupRepository\Repositories_DtDeliveryGroupRepository_DeleteDtDeliveryGroup.csv")]
        public void DeleteDtDeliveryGroupTest(
            string no,
            string in_InsertNewDataSqlPath,
            string in_DeleteNewDataSqlPath,
            string expected_DataJsonPath,
            string expected_ExceptionType,
            string expected_ExceptionMessage,
            string remarks)
        {
            byte[] rowVersion = new byte[] { };
            DtDeliveryGroup readModel = null;
            bool canDelete = false;

            // 初期データ挿入
            if (RepositoryTestHelper.ExecInsertSql(in_InsertNewDataSqlPath))
            {
                canDelete = true;

                // RowVersion取得のため読み出す
                readModel = _deliveryGroupRepository.ReadDtDeliveryGroup(1);
                Assert.IsNotNull(readModel);
                rowVersion = readModel.RowVersion;
            }

            string exceptionName = "";
            string exceptionMessage = "";
            try
            {
                // データを削除する
                var deletedModel = _deliveryGroupRepository.DeleteDtDeliveryGroup(1, rowVersion);
                Assert.IsTrue((deletedModel != null) == canDelete);

                // 削除データを読み出せないことを確認する
                readModel = _deliveryGroupRepository.ReadDtDeliveryGroup(1);
                Assert.IsNull(readModel);
            }
            catch(RmsCannotChangeDeliveredFileException e)
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
        /// UpdateDtDeliveryGroupIfDeliveryNotStartメソッドのテスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InsertNewDataSqlPath">挿入するデータのSQLファイルパス</param>
        /// <param name="in_DeleteNewDataSqlPath">削除するデータのSQLファイルパス</param>
        /// <param name="expected_DataJsonPath">期待するJsonデータファイルパス</param>
        /// <param name="expected_ExceptionType">期待する例外タイプ</param>
        /// <param name="expected_ExceptionMessage">期待する例外メッセージ</param>
        /// <param name="remarks">備考</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\DtDeliveryGroupRepository\Repositories_DtDeliveryGroupRepository_UpdateDtDeliveryGroupIfDeliveryNotStart.csv")]
        public void UpdateDtDeliveryGroupIfDeliveryNotStartTest(
            string no,
            string in_InsertNewDataSqlPath,
            string in_DeleteNewDataSqlPath,
            string expected_DataJsonPath,
            string expected_ExceptionType,
            string expected_ExceptionMessage,
            string remarks)
        {
            DtDeliveryGroup changedModel = new DtDeliveryGroup()
            {
                Sid = 0
            };
            bool canDelete = false;

            // 初期データ挿入
            if (RepositoryTestHelper.ExecInsertSql(in_InsertNewDataSqlPath))
            {
                canDelete = true;

                // RowVersion取得のため読み出す
                var readModel = _deliveryGroupRepository.ReadDtDeliveryGroup(1);
                Assert.IsNotNull(readModel);

                // データを更新する(SIDは変更すると動作しないので変更しない)
                changedModel.Sid = readModel.Sid;
                changedModel.DeliveryGroupStatusSid = 999;
                changedModel.MtDeliveryGroupStatus = null;
                changedModel.DeliveryFileSid = 999;
                changedModel.Name = RepositoryTestHelper.CreateSpecifiedNumberString(100);
                changedModel.StartDatetime = DateTime.Parse("2020/4/2 0:00:00");
                changedModel.DownloadDelayTime = 10;
                changedModel.CreateDatetime = DateTime.UtcNow;

                if (expected_ExceptionType.Equals(typeof(RmsParameterException).FullName))
                {
                    // 異常値を設定する
                    changedModel.Name = RepositoryTestHelper.CreateSpecifiedNumberString(101);
                }
            }

            string exceptionName = "";
            string exceptionMessage = "";
            try
            {
                // データを更新する
                var updatedModel = _deliveryGroupRepository.UpdateDtDeliveryGroupIfDeliveryNotStart(changedModel);
                Assert.IsTrue((updatedModel != null) == canDelete);

                if (updatedModel != null)
                {
                    // 比較に使用しないパラメータはnull or 固定値にする
                    updatedModel.UpdateDatetime = DateTime.Parse("2020/4/1 0:00:00");
                    updatedModel.RowVersion = null;
                    updatedModel.MtDeliveryGroupStatus.CreateDatetime = DateTime.Parse("2020/4/1 0:00:00");
                }

                // データのjson化
                string readJson = Utility.ObjectExtensions.ToStringJson(updatedModel);
                string expectJson = null;
                if (File.Exists(expected_DataJsonPath))
                {
                    expectJson = File.ReadAllText(expected_DataJsonPath);
                }

                // データの比較
                Assert.AreEqual(expectJson, readJson);

                // TODO DBデータ内容をチェックする
            }
            catch (RmsCannotChangeDeliveredFileException e)
            {
                exceptionName = e.GetType().FullName;
                exceptionMessage = e.Message;
            }
            catch (RmsParameterException e)
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
        /// ReadStartableDtDeliveryGroupメソッドのテスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InsertNewDataSqlPath">挿入するデータのSQLファイルパス</param>
        /// <param name="in_DeleteNewDataSqlPath">削除するデータのSQLファイルパス</param>
        /// <param name="expected_DataJsonPath">期待するJsonデータファイルパス</param>
        /// <param name="expected_ExceptionType">期待する例外タイプ</param>
        /// <param name="expected_ExceptionMessage">期待する例外メッセージ</param>
        /// <param name="remarks">備考</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\DtDeliveryGroupRepository\Repositories_DtDeliveryGroupRepository_ReadStartableDtDeliveryGroup.csv")]
        public void ReadStartableDtDeliveryGroupTest(
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

            // 配信グループデータを読み出す
            var readModels = _deliveryGroupRepository.ReadStartableDtDeliveryGroup().ToList();

            if (readModels.Count != 0)
            {
                // 比較に使用しないパラメータはnull or 固定値にする
                readModels.Where(x => x.Sid == 1).First().RowVersion = null;
                readModels.Where(x => x.Sid == 1).First().DtDeliveryFile.RowVersion = null;
                readModels.Where(x => x.Sid == 1).First().MtDeliveryGroupStatus.CreateDatetime = DateTime.Parse("2020/4/1 0:00:00");
            }

            // データのjson化
            string readJson = Utility.ObjectExtensions.ToStringJson(readModels);
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
        /// ReadDeliveryIncludedDtDeliveryGroupメソッドのテスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InsertNewDataSqlPath">挿入するデータのSQLファイルパス</param>
        /// <param name="in_DeleteNewDataSqlPath">削除するデータのSQLファイルパス</param>
        /// <param name="expected_DataJsonPath">期待するJsonデータファイルパス</param>
        /// <param name="expected_ExceptionType">期待する例外タイプ</param>
        /// <param name="expected_ExceptionMessage">期待する例外メッセージ</param>
        /// <param name="remarks">備考</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\DtDeliveryGroupRepository\Repositories_DtDeliveryGroupRepository_ReadDeliveryIncludedDtDeliveryGroup.csv")]
        public void ReadDeliveryIncludedDtDeliveryGroupTest(
            string no,
            string in_InsertNewDataSqlPath,
            string in_DeleteNewDataSqlPath,
            string expected_DataJsonPath,
            string expected_ExceptionType,
            string expected_ExceptionMessage,
            string remarks)
        {
            bool canRead = false;

            // 初期データ挿入
            if (RepositoryTestHelper.ExecInsertSql(in_InsertNewDataSqlPath))
            {
                canRead = true;
            }

            // 配信グループデータを読み出す
            var readModel = _deliveryGroupRepository.ReadDeliveryIncludedDtDeliveryGroup(1);
            Assert.IsTrue((readModel != null) == canRead);

            if (readModel != null)
            {
                // 比較に使用しないパラメータはnull or 固定値にする
                readModel.RowVersion = null;
                readModel.DtDeliveryFile.RowVersion = null;
                readModel.DtDeliveryFile.MtInstallType.CreateDatetime = DateTime.Parse("2020/4/1 0:00:00");
                readModel.DtDeliveryFile.MtDeliveryFileType.CreateDatetime = DateTime.Parse("2020/4/1 0:00:00");
                readModel.DtDeliveryFile.DtDeliveryModel.ToList()[0].MtEquipmentModel.CreateDatetime = DateTime.Parse("2020/4/1 0:00:00");
                readModel.DtDeliveryResult.ToList()[0].DtDevice.EdgeId = Guid.Empty;
                readModel.DtDeliveryResult.ToList()[0].DtDevice1.EdgeId = Guid.Empty;
                readModel.DtDeliveryResult.ToList()[0].DtDevice.MtConnectStatus.CreateDatetime = DateTime.Parse("2020/4/1 0:00:00");
                readModel.DtDeliveryResult.ToList()[0].DtDevice1.MtConnectStatus.CreateDatetime = DateTime.Parse("2020/4/1 0:00:00");
                readModel.DtDeliveryResult.ToList()[0].DtDevice.MtEquipmentModel.CreateDatetime = DateTime.Parse("2020/4/1 0:00:00");
                readModel.DtDeliveryResult.ToList()[0].DtDevice1.MtEquipmentModel.CreateDatetime = DateTime.Parse("2020/4/1 0:00:00");
                readModel.DtDeliveryResult.ToList()[0].DtDevice.MtInstallType.CreateDatetime = DateTime.Parse("2020/4/1 0:00:00");
                readModel.DtDeliveryResult.ToList()[0].DtDevice1.MtInstallType.CreateDatetime = DateTime.Parse("2020/4/1 0:00:00");
            }

            // データのjson化
            string readJson = Utility.ObjectExtensions.ToStringJson(readModel);
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
        /// UpdateDtDeliveryGroupStatusStartedメソッドのテスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InsertNewDataSqlPath">挿入するデータのSQLファイルパス</param>
        /// <param name="in_DeleteNewDataSqlPath">削除するデータのSQLファイルパス</param>
        /// <param name="expected_DataJsonPath">期待するJsonデータファイルパス</param>
        /// <param name="expected_ExceptionType">期待する例外タイプ</param>
        /// <param name="expected_ExceptionMessage">期待する例外メッセージ</param>
        /// <param name="remarks">備考</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\DtDeliveryGroupRepository\Repositories_DtDeliveryGroupRepository_UpdateDtDeliveryGroupStatusStarted.csv")]
        public void UpdateDtDeliveryGroupStatusStartedTest(
            string no,
            string in_InsertNewDataSqlPath,
            string in_DeleteNewDataSqlPath,
            string expected_DataJsonPath,
            string expected_ExceptionType,
            string expected_ExceptionMessage,
            string remarks)
        {
            bool canUpdate = false;

            // 初期データ挿入
            if (RepositoryTestHelper.ExecInsertSql(in_InsertNewDataSqlPath))
            {
                canUpdate = true;
            }

            string exceptionName = "";
            string exceptionMessage = "";
            try
            {
                // データを更新する
                var updatedModel = _deliveryGroupRepository.UpdateDtDeliveryGroupStatusStarted(1);
                Assert.IsTrue((updatedModel != null) == canUpdate);

                if (updatedModel != null)
                {
                    // 比較に使用しないパラメータはnull or 固定値にする
                    updatedModel.UpdateDatetime = DateTime.Parse("2020/4/1 0:00:00");
                    updatedModel.RowVersion = null;
                    updatedModel.MtDeliveryGroupStatus.CreateDatetime = DateTime.Parse("2020/4/1 0:00:00");
                }

                // データのjson化
                string readJson = Utility.ObjectExtensions.ToStringJson(updatedModel);
                string expectJson = null;
                if (File.Exists(expected_DataJsonPath))
                {
                    expectJson = File.ReadAllText(expected_DataJsonPath);
                }

                // データの比較
                Assert.AreEqual(expectJson, readJson);

                // TODO DBデータ内容をチェックする
            }
            catch (RmsCannotChangeDeliveredFileException e)
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
        /// GetDevicesByGatewaySidNotCompletedDownloadメソッドのテスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InsertNewDataSqlPath">挿入するデータのSQLファイルパス</param>
        /// <param name="in_DeleteNewDataSqlPath">削除するデータのSQLファイルパス</param>
        /// <param name="expected_DataJsonPath">期待するJsonデータファイルパス</param>
        /// <param name="expected_ExceptionType">期待する例外タイプ</param>
        /// <param name="expected_ExceptionMessage">期待する例外メッセージ</param>
        /// <param name="remarks">備考</param>
        [Ignore]
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\DtDeliveryGroupRepository\Repositories_DtDeliveryGroupRepository_GetDevicesByGatewaySidNotCompletedDownload.csv")]
        public void GetDevicesByGatewaySidNotCompletedDownloadTest(
            string no,
            string in_InsertNewDataSqlPath,
            string in_DeleteNewDataSqlPath,
            string expected_DataJsonPath,
            string expected_ExceptionType,
            string expected_ExceptionMessage,
            string remarks)
        {
            // TODO
        }
    }
}