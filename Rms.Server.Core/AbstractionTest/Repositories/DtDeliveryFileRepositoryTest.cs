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
    public class DtDeliveryFileRepositoryTest
    {
        /// <summary>
        /// DtDeliveryGroupRepository
        /// </summary>
        private static DtDeliveryGroupRepository _deliveryGroupRepository = null;

        /// <summary>
        /// DtDeliveryResultRepository
        /// </summary>
        private static DtDeliveryFileRepository _deliveryFileRepository = null;

        /// <summary>
        /// DtDeliveryModelRepository
        /// </summary>
        private static DtDeliveryModelRepository _deliveryModelRepository = null;

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
            builder.ServiceCollection.AddTransient<DtDeliveryFileRepository>();
            builder.ServiceCollection.AddTransient<DtDeliveryModelRepository>();
            ServiceProvider provider = builder.Build();
            _deliveryGroupRepository = provider.GetService<DtDeliveryGroupRepository>();
            _deliveryFileRepository = provider.GetService<DtDeliveryFileRepository>();
            _deliveryModelRepository = provider.GetService<DtDeliveryModelRepository>();
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
        /// ReadDtDeliveryFileメソッドのテスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InsertNewDataSqlPath">挿入するデータのSQLファイルパス</param>
        /// <param name="in_DeleteNewDataSqlPath">削除するデータのSQLファイルパス</param>
        /// <param name="expected_DataJsonPath">期待するJsonデータファイルパス</param>
        /// <param name="expected_ExceptionType">期待する例外タイプ</param>
        /// <param name="expected_ExceptionMessage">期待する例外メッセージ</param>
        /// <param name="remarks">備考</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\DtDeliveryFileRepository\Repositories_DtDeliveryFileRepository_ReadDtDeliveryFile.csv")]
        public void ReadDtDeliveryFileTest(
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
            var readModel = _deliveryFileRepository.ReadDtDeliveryFile(2);
            if (readModel != null)
            {
                // 比較に使用しないパラメータはnullにする
                readModel.RowVersion = null;
                readModel.MtDeliveryFileType.CreateDatetime = DateTime.Parse("2020/4/1 0:00:00");
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
        /// UpdateDtDeliveryFileIfNoGroupStartedメソッドのテスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InsertNewDataSqlPath">挿入するデータのSQLファイルパス</param>
        /// <param name="in_DeleteNewDataSqlPath">削除するデータのSQLファイルパス</param>
        /// <param name="expected_DataJsonPath">期待するJsonデータファイルパス</param>
        /// <param name="expected_ExceptionType">期待する例外タイプ</param>
        /// <param name="expected_ExceptionMessage">期待する例外メッセージ</param>
        /// <param name="remarks">備考</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\DtDeliveryFileRepository\Repositories_DtDeliveryFileRepository_UpdateDtDeliveryFileIfNoGroupStarted.csv")]
        public void UpdateDtDeliveryFileIfNoGroupStartedTest(
            string no,
            string in_InsertNewDataSqlPath,
            string in_DeleteNewDataSqlPath,
            string expected_DataJsonPath,
            string expected_ExceptionType,
            string expected_ExceptionMessage,
            string remarks)
        {
            bool canUpdate = false;
            DtDeliveryFile changedModel = new DtDeliveryFile()
            {
                Sid = 0
            };

            // 初期データ挿入
            if (RepositoryTestHelper.ExecInsertSql(in_InsertNewDataSqlPath))
            {
                canUpdate = true;

                // RowVersion取得のため読み出す
                var readModel = _deliveryFileRepository.ReadDtDeliveryFile(2);
                Assert.IsNotNull(readModel);

                // 更新データ設定
                changedModel.Sid = readModel.Sid;
                changedModel.DeliveryFileTypeSid = 2;
                changedModel.InstallTypeSid = 2;
                changedModel.FilePath = RepositoryTestHelper.CreateSpecifiedNumberString(301); // 変更されないので上限値+1
                changedModel.Version = RepositoryTestHelper.CreateSpecifiedNumberString(30);
                changedModel.InstallableVersion = RepositoryTestHelper.CreateSpecifiedNumberString(300);
                changedModel.Description = RepositoryTestHelper.CreateSpecifiedNumberString(200);
                changedModel.InformationId = RepositoryTestHelper.CreateSpecifiedNumberString(45);
                changedModel.IsCanceled = true;
                changedModel.CreateDatetime = DateTime.Parse("2099/12/31 0:00:00");
                changedModel.RowVersion = readModel.RowVersion;

                // 子エンティティの設定
                changedModel.DtDeliveryModel.Add(new DtDeliveryModel()
                {
                    EquipmentModelSid = 2,
                    CreateDatetime = DateTime.Parse("2020/4/1 0:00:00")
                });

                if (expected_ExceptionType == typeof(RmsParameterException).FullName)
                {
                    if (expected_ExceptionMessage.Contains("InstallableVersion"))
                    {
                        changedModel.InstallableVersion = RepositoryTestHelper.CreateSpecifiedNumberString(301); // 上限値+1
                    }
                    else if (expected_ExceptionMessage.Contains("Version"))
                    {
                        changedModel.Version = RepositoryTestHelper.CreateSpecifiedNumberString(31); // 上限値+1
                    }
                    else if (expected_ExceptionMessage.Contains("Description"))
                    {
                        changedModel.Description = RepositoryTestHelper.CreateSpecifiedNumberString(201); // 上限値+1
                    }
                    else if (expected_ExceptionMessage.Contains("InformationId"))
                    {
                        changedModel.InformationId = RepositoryTestHelper.CreateSpecifiedNumberString(46); // 上限値+1
                    }
                }
            }

            string exceptionName = "";
            string exceptionMessage = "";
            try
            {
                if (expected_ExceptionType == typeof(ArgumentNullException).FullName)
                {
                    changedModel = null;
                }

                // データを更新する
                var updatedModel = _deliveryFileRepository.UpdateDtDeliveryFileIfNoGroupStarted(changedModel);
                Assert.IsTrue((updatedModel != null) == canUpdate);

                if (updatedModel != null)
                {
                    // 比較に使用しないパラメータはnull or 固定値にする
                    updatedModel.UpdateDatetime = DateTime.Parse("2020/4/1 0:00:00");
                    updatedModel.RowVersion = null;
                    updatedModel.DtDeliveryModel.ToList()[0].CreateDatetime = DateTime.Parse("2020/4/1 0:00:00");
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

                // 挿入SQLで登録した配信ファイル型式は削除されていることを確認する
                var readDeliveryModel = _deliveryModelRepository.ReadDtDeliveryModel(1);
                Assert.IsNull(readDeliveryModel);

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
            catch (ArgumentNullException e)
            {
                exceptionName = e.GetType().FullName;
                exceptionMessage = typeof(DtDeliveryFile).FullName + " is null."; // HACK ←の部分をメッセージから抽出できれば...
            }
            // 例外発生チェック
            Assert.AreEqual(expected_ExceptionType, exceptionName);
            Assert.AreEqual(expected_ExceptionMessage, exceptionMessage);

            // 後処理
            RepositoryTestHelper.ExecDeleteSql(in_DeleteNewDataSqlPath);
        }

        /// <summary>
        /// UpdateCancelFlagメソッドのテスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InsertNewDataSqlPath">挿入するデータのSQLファイルパス</param>
        /// <param name="in_DeleteNewDataSqlPath">削除するデータのSQLファイルパス</param>
        /// <param name="expected_DataJsonPath">期待するJsonデータファイルパス</param>
        /// <param name="expected_ExceptionType">期待する例外タイプ</param>
        /// <param name="expected_ExceptionMessage">期待する例外メッセージ</param>
        /// <param name="remarks">備考</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\DtDeliveryFileRepository\Repositories_DtDeliveryFileRepository_UpdateCancelFlag.csv")]
        public void UpdateCancelFlagTest(
            string no,
            string in_InsertNewDataSqlPath,
            string in_DeleteNewDataSqlPath,
            string expected_DataJsonPath,
            string expected_ExceptionType,
            string expected_ExceptionMessage,
            string remarks)
        {
            bool canUpdate = false;
            DtDeliveryFile changedModel = new DtDeliveryFile()
            {
                Sid = 0
            };

            // 初期データ挿入
            if (RepositoryTestHelper.ExecInsertSql(in_InsertNewDataSqlPath))
            {
                canUpdate = true;

                // RowVersion取得のため読み出す
                var readModel = _deliveryFileRepository.ReadDtDeliveryFile(2);
                Assert.IsNotNull(readModel);

                // 更新データ設定
                changedModel.Sid = readModel.Sid;
                changedModel.DeliveryFileTypeSid = 2;
                changedModel.InstallTypeSid = 2;
                changedModel.FilePath = RepositoryTestHelper.CreateSpecifiedNumberString(301); // 変更されないので上限値+1
                changedModel.Version = RepositoryTestHelper.CreateSpecifiedNumberString(31); // 変更されないので上限値+1
                changedModel.InstallableVersion = RepositoryTestHelper.CreateSpecifiedNumberString(301); // 変更されないので上限値+1
                changedModel.Description = RepositoryTestHelper.CreateSpecifiedNumberString(201); // 変更されないので上限値+1
                changedModel.InformationId = RepositoryTestHelper.CreateSpecifiedNumberString(46); // 変更されないので上限値+1
                changedModel.CreateDatetime = DateTime.Parse("2099/12/31 0:00:00");
                changedModel.RowVersion = readModel.RowVersion;

                switch (no)
                {
                    case "1_001":
                    default:
                        changedModel.IsCanceled = null;
                        break;
                    case "1_002":
                        changedModel.IsCanceled = true;
                        break;
                    case "1_003":
                        // 挿入SQLと同じ値
                        changedModel.IsCanceled = false;
                        break;
                }
            }

            string exceptionName = "";
            string exceptionMessage = "";
            try
            {
                if (expected_ExceptionType == typeof(ArgumentNullException).FullName)
                {
                    changedModel = null;
                }

                // データを更新する
                var updatedModel = _deliveryFileRepository.UpdateCancelFlag(changedModel);
                Assert.IsTrue((updatedModel != null) == canUpdate);

                if (changedModel.IsCanceled == false)
                {
                    // 中止フラグが更新前と同一の場合は更新されていない(RowVersionに変化がない)ことを確認する
                    Assert.IsTrue(changedModel.RowVersion.SequenceEqual(updatedModel.RowVersion));
                }

                if (updatedModel != null)
                {
                    // 比較に使用しないパラメータはnull or 固定値にする
                    updatedModel.UpdateDatetime = DateTime.Parse("2020/4/1 0:00:00");
                    updatedModel.RowVersion = null;
                    updatedModel.DtDeliveryModel.ToList()[0].CreateDatetime = DateTime.Parse("2020/4/1 0:00:00");
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
            catch (RmsParameterException e)
            {
                exceptionName = e.GetType().FullName;
                exceptionMessage = e.Message;
            }
            catch (ArgumentNullException e)
            {
                exceptionName = e.GetType().FullName;
                exceptionMessage = typeof(DtDeliveryFile).FullName + " is null."; // HACK ←の部分をメッセージから抽出できれば...
            }
            // 例外発生チェック
            Assert.AreEqual(expected_ExceptionType, exceptionName);
            Assert.AreEqual(expected_ExceptionMessage, exceptionMessage);

            // 後処理
            RepositoryTestHelper.ExecDeleteSql(in_DeleteNewDataSqlPath);
        }

        /// <summary>
        /// DeleteDtDeliveryFileメソッドのテスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InsertNewDataSqlPath">挿入するデータのSQLファイルパス</param>
        /// <param name="in_DeleteNewDataSqlPath">削除するデータのSQLファイルパス</param>
        /// <param name="expected_DataJsonPath">期待するJsonデータファイルパス</param>
        /// <param name="expected_ExceptionType">期待する例外タイプ</param>
        /// <param name="expected_ExceptionMessage">期待する例外メッセージ</param>
        /// <param name="remarks">備考</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\DtDeliveryFileRepository\Repositories_DtDeliveryFileRepository_DeleteDtDeliveryFile.csv")]
        public void DeleteDtDeliveryFileTest(
            string no,
            string in_InsertNewDataSqlPath,
            string in_DeleteNewDataSqlPath,
            string expected_DataJsonPath,
            string expected_ExceptionType,
            string expected_ExceptionMessage,
            string remarks)
        {
            byte[] rowVersion = new byte[] { };
            DtDeliveryFile readModel = null;
            bool canDelete = false;

            // 初期データ挿入
            if (RepositoryTestHelper.ExecInsertSql(in_InsertNewDataSqlPath))
            {
                canDelete = true;

                // RowVersion取得のため読み出す
                readModel = _deliveryFileRepository.ReadDtDeliveryFile(2);
                Assert.IsNotNull(readModel);
                rowVersion = readModel.RowVersion;
            }

            string exceptionName = "";
            string exceptionMessage = "";
            try
            {
                // データを削除する
                var deletedModel = _deliveryFileRepository.DeleteDtDeliveryFile(2, rowVersion);
                Assert.IsTrue((deletedModel != null) == canDelete);

                // 削除データを読み出せないことを確認する
                readModel = _deliveryFileRepository.ReadDtDeliveryFile(2);
                Assert.IsNull(readModel);

                // 子エンティティのデータも削除されたことを確認する
                var group = _deliveryGroupRepository.ReadDtDeliveryGroup(1);
                Assert.IsNull(group);
                var model = _deliveryModelRepository.ReadDtDeliveryModel(1);
                Assert.IsNull(model);
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
    }
}