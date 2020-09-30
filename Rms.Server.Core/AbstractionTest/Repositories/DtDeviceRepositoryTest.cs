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
    public class DtDeviceRepositoryTest
    {
        /// <summary>
        /// DtDeliveryGroupRepository
        /// </summary>
        private static DtDeliveryGroupRepository _deliveryGroupRepository = null;

        /// <summary>
        /// DtDeviceRepository
        /// </summary>
        private static DtDeviceRepository _deviceRepository = null;

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
            builder.ServiceCollection.AddTransient<DtDeviceRepository>();
            ServiceProvider provider = builder.Build();
            _deliveryGroupRepository = provider.GetService<DtDeliveryGroupRepository>();
            _deviceRepository = provider.GetService<DtDeviceRepository>();
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
        /// ReadDtDeviceメソッドのテスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InsertNewDataSqlPath">挿入するデータのSQLファイルパス</param>
        /// <param name="in_DeleteNewDataSqlPath">削除するデータのSQLファイルパス</param>
        /// <param name="expected_DataJsonPath">期待するJsonデータファイルパス</param>
        /// <param name="expected_ExceptionType">期待する例外タイプ</param>
        /// <param name="expected_ExceptionMessage">期待する例外メッセージ</param>
        /// <param name="remarks">備考</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\DtDeviceRepository\Repositories_DtDeviceRepository_ReadDtDevice.csv")]
        public void ReadDtDeviceTest(
            string no,
            string in_InsertNewDataSqlPath,
            string in_DeleteNewDataSqlPath,
            string expected_DataJsonPath,
            string expected_ExceptionType,
            string expected_ExceptionMessage,
            string remarks)
        {
            string uidOrEdgeId = null;

            // 初期データ挿入
            if (RepositoryTestHelper.ExecInsertSql(in_InsertNewDataSqlPath))
            {
                // UidとEdgeID取得のため読み出す
                var readDevice = _deviceRepository.ReadDtDevice(2);
                Assert.IsNotNull(readDevice);

                switch (no)
                {
                    case "1_001":
                        // UIDで検索する
                        uidOrEdgeId = readDevice.EquipmentUid;
                        break;
                    case "1_002":
                        // エッジIDで検索する
                        uidOrEdgeId = readDevice.EdgeId.ToString();
                        break;
                    case "1_003":
                        // 空文字
                        uidOrEdgeId = "";
                        break;
                    default:
                        // null投入
                        break;
                }
            }

            // データを取得する(配信結果付き)
            var readModel = _deviceRepository.ReadDtDevice(uidOrEdgeId);

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
        /// ReadDtDeviceOnlineGatewayメソッドのテスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InsertNewDataSqlPath">挿入するデータのSQLファイルパス</param>
        /// <param name="in_DeleteNewDataSqlPath">削除するデータのSQLファイルパス</param>
        /// <param name="expected_DataJsonPath">期待するJsonデータファイルパス</param>
        /// <param name="expected_ExceptionType">期待する例外タイプ</param>
        /// <param name="expected_ExceptionMessage">期待する例外メッセージ</param>
        /// <param name="remarks">備考</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\DtDeviceRepository\Repositories_DtDeviceRepository_ReadDtDeviceOnlineGateway.csv")]
        public void ReadDtDeviceOnlineGatewayTest(
            string no,
            string in_InsertNewDataSqlPath,
            string in_DeleteNewDataSqlPath,
            string expected_DataJsonPath,
            string expected_ExceptionType,
            string expected_ExceptionMessage,
            string remarks)
        {
            DtDeliveryGroup readDeliveryGroup = new DtDeliveryGroup()
            {
                Sid = 0
            };

            // 初期データ挿入
            if (RepositoryTestHelper.ExecInsertSql(in_InsertNewDataSqlPath))
            {
                // 配信グループを取得する
                readDeliveryGroup = _deliveryGroupRepository.ReadDtDeliveryGroup(1);
                Assert.IsNotNull(readDeliveryGroup);
            }

            string exceptionName = "";
            string exceptionMessage = "";
            try
            {
                if (expected_ExceptionType == typeof(ArgumentNullException).FullName)
                {
                    readDeliveryGroup = null;
                }

                // データを取得する(配信結果付き)
                var readModel = _deviceRepository.ReadDtDeviceOnlineGateway(readDeliveryGroup);
                if (readModel.Count() != 0)
                {
                    // 比較に使用しないパラメータはnull or 固定値にする
                    readModel.ToList()[0].MtConnectStatus.CreateDatetime = DateTime.Parse("2020/4/1 0:00:00");
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
            }
            catch (ArgumentNullException e)
            {
                exceptionName = e.GetType().FullName;
                exceptionMessage = typeof(DtDevice).FullName + " is null."; // HACK ←の部分をメッセージから抽出できれば...
            }
            // 例外発生チェック
            Assert.AreEqual(expected_ExceptionType, exceptionName);
            Assert.AreEqual(expected_ExceptionMessage, exceptionMessage);

            // 後処理
            RepositoryTestHelper.ExecDeleteSql(in_DeleteNewDataSqlPath);
        }

        /// <summary>
        /// UpdateDeviceConnectionStatusメソッドのテスト
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
        [CsvDataSourece(@"TestCases\DtDeviceRepository\Repositories_DtDeviceRepository_UpdateDeviceConnectionStatus.csv")]
        public void UpdateDeviceConnectionStatusTest(
            string no,
            string in_InsertNewDataSqlPath,
            string in_DeleteNewDataSqlPath,
            string expected_DataJsonPath,
            string expected_ExceptionType,
            string expected_ExceptionMessage,
            string remarks)
        {
            // TODO 処理内容が正しいか問い合わせ中
        }

        /// <summary>
        /// UpdateDeviceInfoByTwinChangedメソッドのテスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InsertNewDataSqlPath">挿入するデータのSQLファイルパス</param>
        /// <param name="in_DeleteNewDataSqlPath">削除するデータのSQLファイルパス</param>
        /// <param name="expected_DataJsonPath">期待するJsonデータファイルパス</param>
        /// <param name="expected_ExceptionType">期待する例外タイプ</param>
        /// <param name="expected_ExceptionMessage">期待する例外メッセージ</param>
        /// <param name="remarks">備考</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\DtDeviceRepository\Repositories_DtDeviceRepository_UpdateDeviceInfoByTwinChanged.csv")]
        public void UpdateDeviceInfoByTwinChangedTest(
            string no,
            string in_InsertNewDataSqlPath,
            string in_DeleteNewDataSqlPath,
            string expected_DataJsonPath,
            string expected_ExceptionType,
            string expected_ExceptionMessage,
            string remarks)
        {
            DtDeliveryGroup readDeliveryGroup = new DtDeliveryGroup()
            {
                Sid = 0
            };
            DtTwinChanged twinChangedData = new DtTwinChanged()
            {
                RemoteConnectionUid = RepositoryTestHelper.CreateSpecifiedNumberString(64),
                SoftVersion = RepositoryTestHelper.CreateSpecifiedNumberString(30),
            };

            if (expected_ExceptionType == typeof(RmsParameterException).FullName)
            {
                if (expected_ExceptionMessage.Contains("RemoteConnectUid"))
                {
                    twinChangedData.RemoteConnectionUid = RepositoryTestHelper.CreateSpecifiedNumberString(65); // 上限値+1
                }
                else if (expected_ExceptionMessage.Contains("RmsSoftVersion"))
                {
                    twinChangedData.SoftVersion = RepositoryTestHelper.CreateSpecifiedNumberString(31); // 上限値+1
                }
            }

            // 初期データ挿入
            RepositoryTestHelper.ExecInsertSql(in_InsertNewDataSqlPath);

            string exceptionName = "";
            string exceptionMessage = "";
            try
            {
                if (expected_ExceptionType == typeof(ArgumentNullException).FullName)
                {
                    twinChangedData = null;
                }

                // データを取得する(配信結果付き)
                var readModel = _deviceRepository.UpdateDeviceInfoByTwinChanged(2, twinChangedData);
                if (readModel != null)
                {
                    readModel.UpdateDatetime = DateTime.Parse("2020/4/1 0:00:00");
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
                exceptionMessage = typeof(DtDevice).FullName + " is null."; // HACK ←の部分をメッセージから抽出できれば...
            }
            // 例外発生チェック
            Assert.AreEqual(expected_ExceptionType, exceptionName);
            Assert.AreEqual(expected_ExceptionMessage, exceptionMessage);

            // 後処理
            RepositoryTestHelper.ExecDeleteSql(in_DeleteNewDataSqlPath);
        }
    }
}