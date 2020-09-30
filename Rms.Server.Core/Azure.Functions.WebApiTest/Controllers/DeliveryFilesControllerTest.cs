using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Rms.Server.Core.Azure.Functions.WebApi.Controllers;
using Rms.Server.Core.Azure.Functions.WebApi.Dto;
using Rms.Server.Core.Azure.Functions.WebApi.Utility;
using Rms.Server.Core.Service.Services;
using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Utility.Models.Entites;
using Rms.Server.Test;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using TestHelper;

namespace Azure.Functions.WebApiTest.Controllers
{
    /// <summary>
    /// 配信グループWebApiのテストクラス
    /// </summary>
    [TestClass]
    public class DeliveryFilesControllerTest
    {
        /// <summary>
        /// DeliveryFilesController
        /// </summary>
        private static DeliveryFilesController _controller = null;

        /// <summary>
        /// Serviceで出力されるログを格納するリスト
        /// </summary>
        private static List<TestLog> _serviceLogs = null;

        /// <summary>
        /// Service内で用いられるロガー
        /// </summary>
        private static TestLogger<DeliveryFileService> _serviceLogger = null;

        /// <summary>
        /// ClassInit
        /// </summary>
        /// <param name="context">コンテキスト</param>
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            // 関連DBデータを全削除
            DbTestHelper.DeleteAllReseed();

            // マスタテーブルデータを作成する
            DbTestHelper.ExecSqlFromFilePath(@"TestData\MakeMasterTableData.sql");
        }

        /// <summary>
        /// ClassCleanup
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanup()
        {
            // 関連DBデータを全削除
            DbTestHelper.DeleteAllReseed();
        }

        /// <summary>
        /// TestInitialize
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
        }

        /// <summary>
        /// TestCleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            // 配信グループ・配信結果・適用結果テーブルのデータを削除して次回作成時のSIDを1にする
            DbTestHelper.ExecSqlFromFilePath(@"TestData\ReseedDeliveryFileChildren.sql");
        }

        /// <summary>
        /// PutDeliveryFileStatusメソッドのテスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InsertNewDataSqlPath">挿入するデータのSQLファイルパス</param>
        /// <param name="in_DeleteNewDataSqlPath">削除するデータのSQLファイルパス</param>
        /// <param name="in_InputJsonDataPath">投入するデータのJSONファイルパス</param>
        /// <param name="in_SidFormat">投入するSIDのstringフォーマット</param>
        /// <param name="in_AppSettingsInvalid">投入するAppSettingsが正常値かどうか</param>
        /// <param name="in_ServiceException">Serviceで強制的に例外を発生させるかどうか</param>
        /// <param name="expected_DataJsonPath">期待するJsonデータファイルパス</param>
        /// <param name="expected_LogMessagesPath">期待するログメッセージ</param>
        /// <param name="expected_DtDeliveryFile_TableDataPath">期待する配信ファイルのDBテーブル内容</param>
        /// <param name="remarks">備考</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\DeliveryFilesContoroller\Controllers_DeliveryFilesContoroller_PutDeliveryFileStatus.csv")]
        public void PutDeliveryFileStatusTest(
            string no,
            string in_InsertNewDataSqlPath,
            string in_DeleteNewDataSqlPath,
            string in_InputJsonDataPath,
            string in_SidFormat,
            string in_AppSettingsInvalid,
            string in_ServiceException,
            string expected_DataJsonPath,
            string expected_LogMessagesPath,
            string expected_DtDeliveryFile_TableDataPath,
            string remarks)
        {
            Dictionary<string, string> dir = null;
            if (bool.TryParse(in_AppSettingsInvalid, out bool isAppSettingsInvalid) && isAppSettingsInvalid)
            {
                // DB接続文字列が不正なAppSettingsとする
                dir = new Dictionary<string, string>()
                {
                    {"ConnectionStrings:PrimaryDbConnectionString", "TestConnectionString" }
                };
            }

            // DI実施
            DependencyInjection(DateTime.Parse("2020/04/30 09:00:00.0000000"), dir, (bool.TryParse(in_ServiceException, out bool isServiceException) && isServiceException));

            // 初期データ挿入
            DbTestHelper.ExecSqlFromFilePath(in_InsertNewDataSqlPath);

            // 投入JSONをDTOに変換する
            string inputDtoJson = (!string.IsNullOrEmpty(in_InputJsonDataPath) && File.Exists(in_InputJsonDataPath)) ?
                File.ReadAllText(in_InputJsonDataPath) :
                "";

            // SID, RowVersionの取得
            DataTable inputDataTable = DbTestHelper.SelectTable("SELECT SID, ROW_VERSION FROM core.DT_DELIVERY_FILE");
            long sid = 1;
            long rowVersion = 0;
            if (inputDataTable.Rows.Count > 0)
            {
                sid = (long)inputDataTable.Rows[0].ItemArray[0];
                rowVersion = WebApiHelper.ConvertByteArrayToLong((byte[])inputDataTable.Rows[0].ItemArray[1]);
            }

            // 投入するSIDを設定
            long inputSid = long.Parse(string.Format(in_SidFormat, sid));

            // JSONにRowVersionを埋め込む(JSONだと中括弧がFormatに影響を与えて面倒なので、Replaceを使用する)
            inputDtoJson = inputDtoJson.Replace("\"rowVersion\": {0}", "\"rowVersion\": " + rowVersion);

            // DTO変換できない場合は初期値とする(Azure Functionsと同様の挙動)
            DeliveryFileStatusUpdateRequestDto inputDto = null;
            try
            {
                inputDto = JsonConvert.DeserializeObject<DeliveryFileStatusUpdateRequestDto>(inputDtoJson);
            }
            catch (Exception)
            {
                inputDto = new DeliveryFileStatusUpdateRequestDto();
            }

            // ログ格納用ロガーの用意
            var actualLogs = new List<TestLog>();
            var logger = new TestLogger<DeliveryFilesController>(actualLogs);

            // WebApi実行
            var postResult = _controller.PutDeliveryFileStatus(inputDto, inputSid, logger);
            
            // データのjson化
            string resultJson = Rms.Server.Core.Utility.ObjectExtensions.ToStringJsonIndented(postResult);
            string expectJson = null;
            if (File.Exists(expected_DataJsonPath))
            {
                expectJson = File.ReadAllText(expected_DataJsonPath);

                // 期待値は投入RowVersion値+1(JSONだと中括弧がFormatに影響を与えて面倒なので、Replaceを使用する)
                expectJson = expectJson.Replace("\"rowVersion\": {0}", "\"rowVersion\": " + (rowVersion + 1));
            }

            // データの比較
            Assert.AreEqual(expectJson, resultJson);

            // 期待するログ、実際のログを取得
            List<string> expectedLogMessages = (!string.IsNullOrEmpty(expected_LogMessagesPath) && File.Exists(expected_LogMessagesPath)) ?
                File.ReadLines(expected_LogMessagesPath).ToList() :
                new List<string>();
            List<string> actualControllerLogMessages = actualLogs.Select(x => x.GetSimpleText()).ToList();
            List<string> actualServiceLogMessages = _serviceLogs.Select(x => x.GetSimpleText()).ToList();

            // ControllerかServiceで期待するログが出力されたか確認
            foreach (var expectedLogMessage in expectedLogMessages)
            {
                var matchingElementController = actualControllerLogMessages.FirstOrDefault(actual => actual.Contains(expectedLogMessage));
                var matchingElementService = actualServiceLogMessages.FirstOrDefault(actual => actual.Contains(expectedLogMessage));
                Assert.IsTrue((matchingElementController != null || matchingElementService != null), string.Format("「{0}」に一致する要素が見つかりません", expectedLogMessage));
                actualControllerLogMessages.Remove(matchingElementController);
                actualControllerLogMessages.Remove(matchingElementService);
            }

            // テーブルデータの実際の値・期待値を取得(RowVersion以外)
            DataTable DeliveryFileActualTable = DbTestHelper.SelectTable(
                @"SELECT 
                    SID, DELIVERY_FILE_TYPE_SID, INSTALL_TYPE_SID, FILE_PATH, VERSION, INSTALLABLE_VERSION, DESCRIPTION, INFORMATION_ID, IS_CANCELED, CREATE_DATETIME, UPDATE_DATETIME
                FROM core.DT_DELIVERY_FILE");
            DataTable DeliveryFileExpectedTable = DbTestHelper.SelectCsv(expected_DtDeliveryFile_TableDataPath,
                @"SELECT 
                    SID, DELIVERY_FILE_TYPE_SID, INSTALL_TYPE_SID, FILE_PATH, VERSION, INSTALLABLE_VERSION, DESCRIPTION, INFORMATION_ID, IS_CANCELED, CREATE_DATETIME, UPDATE_DATETIME
                FROM core.DT_DELIVERY_FILE");

            // テーブルデータの比較
            CheckDataTableEquals(DeliveryFileExpectedTable, DeliveryFileActualTable);

            // 後処理
            DbTestHelper.ExecSqlFromFilePath(in_DeleteNewDataSqlPath);
        }

        /// <summary>
        /// テーブルデータが等しいか比較する。CSVデータとDBデータを比較する際に、
        /// DBデータのnullとDBデータの空文字を"等しい"とするために、
        /// CSVデータをnullに変更する。
        /// また、RowVersionは毎回変更される値で比較できないため、DBデータをnullに変更する。
        /// これらは副作用となるため、以降の処理に注意すること
        /// </summary>
        /// <param name="csvExpected"></param>
        /// <param name="dbActual"></param>
        private void CheckDataTableEquals(DataTable csvExpected, DataTable dbActual)
        {
            Assert.AreEqual(csvExpected.Rows.Count, dbActual.Rows.Count);

            // RowVersionは流動的な値なので、比較対象から外す(nullにする)
            var rowVersionColumn = dbActual.Columns["ROW_VERSION"];
            if (rowVersionColumn != null)
            {
                var rowVersionIndex = rowVersionColumn.Ordinal;
                foreach (DataRow row in dbActual.Rows)
                {
                    row.ItemArray[rowVersionIndex] = null;
                }
            }

            // データテーブルの比較
            for (int row_i = 0; row_i < csvExpected.Rows.Count; row_i++)
            {
                CollectionAssert.AreEqual(csvExpected.Rows[row_i].ItemArray, dbActual.Rows[row_i].ItemArray);
            }
        }

        /// <summary>
        /// DIを実行する
        /// </summary>
        /// <param name="utcNow">TimeProviderに渡す日時</param>
        /// <param name="appSettings">アプリケーション設定を上書きする場合は指定する</param>
        /// <param name="isSeriviceError">サービスで例外を強制的に発生させる場合はtrueを指定する</param>
        private void DependencyInjection(DateTime utcNow, Dictionary<string, string> appSettings = null, bool isSeriviceError = false)
        {
            // Repository生成
            TestDiProviderBuilder builder = new TestDiProviderBuilder();
            builder.ServiceCollection.AddTransient<DeliveryFilesController>();
            if (isSeriviceError)
            {
                // Serviceの各メソッドで例外を発生させるクラスに置き換える
                builder.ServiceCollection.AddTransient<IDeliveryFileService, TestDeliveryFileService>();
            }
            builder.ServiceCollection.AddSingleton<Rms.Server.Core.Utility.ITimeProvider>(UnitTestHelper.CreateTimeProvider(utcNow));
            builder.AddConfigures(appSettings);

            _serviceLogs = new List<TestLog>();
            _serviceLogger = new TestLogger<DeliveryFileService>(_serviceLogs);
            builder.ServiceCollection.AddSingleton<ILogger<DeliveryFileService>>(_serviceLogger);

            // DIしたControllerを作成する
            ServiceProvider provider = builder.Build();
            _controller = provider.GetService<DeliveryFilesController>();
        }
    }

    /// <summary>
    /// 強制的に例外を発生させるServiceのテストクラス
    /// </summary>
    public class TestDeliveryFileService : IDeliveryFileService
    {
        /// <summary>
        /// 配信ファイルを追加する
        /// </summary>
        /// <param name="utilParam">配信ファイルパラメータ</param>
        /// <returns>DBに追加したパラメータ(Result付き)</returns>
        public Result<DtDeliveryFile> Create(DtDeliveryFile utilParam)
        {
            throw new Exception();
        }

        /// <summary>
        /// 配信ファイルを更新する
        /// </summary>
        /// <param name="utilParam">配信ファイルパラメータ</param>
        /// <returns>DB更新したパラメータ(Result付き)</returns>
        public Result<DtDeliveryFile> Update(DtDeliveryFile utilParam)
        {
            throw new Exception();
        }

        /// <summary>
        /// 配信ファイルを削除する
        /// </summary>
        /// <param name="sid">削除する配信ファイルのSID</param>
        /// <param name="rowVersion">削除する配信ファイルのRowVersion</param>
        /// <returns>DB削除したパラメータ(Result付き)</returns>
        public Result<DtDeliveryFile> Delete(long sid, byte[] rowVersion)
        {
            throw new Exception();
        }

        /// <summary>
        /// 中止フラグを更新する
        /// </summary>
        /// <param name="utilParam">更新する配信ファイルデータ</param>
        /// <returns>DB更新したデータ(Result付き)</returns>
        public Result<DtDeliveryFile> PutCancelFlag(DtDeliveryFile utilParam)
        {
            throw new Exception();
        }
    }
}
