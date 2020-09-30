using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Rms.Server.Core.Abstraction.Models;
using Rms.Server.Core.Abstraction.Repositories.Blobs;
using Rms.Server.Core.Azure.Functions.WebApi.Controllers;
using Rms.Server.Core.Azure.Functions.WebApi.Dto;
using Rms.Server.Core.Service.Models;
using Rms.Server.Core.Service.Services;
using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Utility.Models.Entites;
using Rms.Server.Test;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TestHelper;
using myUtility = Rms.Server.Core.Utility;

namespace Azure.Functions.WebApiTest.Controllers
{
    /// <summary>
    /// 配信グループWebApiのテストクラス
    /// </summary>
    [TestClass]
    public class DevicesControllerTest
    {
        /// <summary>
        /// DevicesController
        /// </summary>
        private static DevicesController _controller = null;

        /// <summary>
        /// DeliveringBlob
        /// </summary>
        private static DeliveringBlob _deliveringBlob = null;

        /// <summary>
        /// AppSettings
        /// </summary>
        private static myUtility.AppSettings _appSettings = null;

        /// <summary>
        /// Serviceで出力されるログを格納するリスト
        /// </summary>
        private static List<TestLog> _serviceLogs = null;

        /// <summary>
        /// Service内で用いられるロガー
        /// </summary>
        private static TestLogger<DeviceService> _serviceLogger = null;

        /// <summary>
        /// 事前アップロードで使用するコンテナ名
        /// </summary>
        private const string _initialBlobContainerName = "config";

        /// <summary>
        /// 事前アップロードで使用するファイルパス
        /// </summary>
        private const string _initialBlobFilePath = "installbase/B/installbase.config";

        /// <summary>
        /// ClassInit
        /// </summary>
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
            // 端末データを全削除して次回作成時のSIDを1にする
            DbTestHelper.ExecSqlFromFilePath(@"TestData\ReseedDevice.sql");
        }

        /// <summary>
        /// PostDeviceメソッドのテスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InsertNewDataSqlPath">挿入するデータのSQLファイルパス</param>
        /// <param name="in_DeleteNewDataSqlPath">削除するデータのSQLファイルパス</param>
        /// <param name="in_InputJsonDataPath">投入するデータのJSONファイルパス</param>
        /// <param name="in_InitialBlobFilePath">事前にBlobにアップロードしておくファイルのパス</param>
        /// <param name="in_DeliveringBlobContainerNameInstallbase">AppSettingsに登録するDeliveringBlobのコンテナ名</param>
        /// <param name="in_DeliveringBlobInstallbaseFilePath">AppSettingsに登録するDeliveringBlobのファイルパス</param>
        /// <param name="in_InvalidDbConnectionString">DB接続文字列を不正地で設定するかどうか</param>
        /// <param name="in_InvalidBlobConnectionString">Blob接続文字列を不正地で設定するかどうか</param>
        /// <param name="in_ServiceException">Serviceで強制的に例外を発生させるかどうか</param>
        /// <param name="expected_DataJsonPath">期待するJsonデータファイルパス</param>
        /// <param name="expected_LogMessagesPath">期待するログメッセージが記載されているファイルパス</param>
        /// <param name="expected_UploadedDeliveringBlobPath">期待するアップロードしたBlobの内容が記載されているファイルのパス</param>
        /// <param name="expected_DtDevice_TableDataPath">期待する端末データのDBテーブル内容</param>
        /// <param name="remarks">備考</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\DevicesContoroller\Controllers_DevicesContoroller_PostDevice.csv")]
        public void PostDeviceTest(
            string no,
            string in_InsertNewDataSqlPath,
            string in_DeleteNewDataSqlPath,
            string in_InputJsonDataPath,
            string in_InitialBlobFilePath,
            string in_DeliveringBlobContainerNameInstallbase,
            string in_DeliveringBlobInstallbaseFilePath,
            string in_InvalidDbConnectionString,
            string in_InvalidBlobConnectionString,
            string in_ServiceException,
            string expected_DataJsonPath,
            string expected_LogMessagesPath,
            string expected_UploadedDeliveringBlobPath,
            string expected_DtDevice_TableDataPath,
            string remarks)
        {
            string configContents;

            try
            {
                bool didUploadInitialBlob = CommonPreset(
                    DateTime.Parse("2020/04/01 09:00:00.0000000"),
                    in_InvalidDbConnectionString,
                    in_InvalidBlobConnectionString,
                    in_ServiceException,
                    in_DeliveringBlobContainerNameInstallbase,
                    in_DeliveringBlobInstallbaseFilePath,
                    in_InsertNewDataSqlPath,
                    in_InitialBlobFilePath,
                    out configContents);

                // 投入JSONをDTOに変換する
                string inputDtoJson = (!string.IsNullOrEmpty(in_InputJsonDataPath) && File.Exists(in_InputJsonDataPath)) ?
                    File.ReadAllText(in_InputJsonDataPath) :
                    "";

                // DTO変換できない場合は初期値とする(Azure Functionsと同様の挙動)
                DeviceAddRequestDto inputDto;
                try
                {
                    inputDto = JsonConvert.DeserializeObject<DeviceAddRequestDto>(inputDtoJson);
                }
                catch (Exception)
                {
                    inputDto = new DeviceAddRequestDto();
                }

                // アップロード予定の場所にあるBlobを削除する
                TryFormatDeliveringBlobInstallbaseFilePath(_appSettings.DeliveringBlobInstallbaseFilePath, inputDto.Device?.EquipmentUid, out string filePath);
                TryDeleteBlob(in_DeliveringBlobContainerNameInstallbase, filePath);

                // ログ格納用ロガーの用意
                var actualLogs = new List<TestLog>();
                var logger = new TestLogger<DevicesController>(actualLogs);

                // WebApi実行
                string edgeId = string.Empty;
                var postResult = _controller.PostDevice(inputDto, logger);
                if (postResult is OkObjectResult)
                {
                    // 結果が流動的になる値は比較できないので固定値を入れる
                    var okDto = ((postResult as OkObjectResult).Value as DeviceResponseDto);
                    Assert.IsNotNull(okDto);
                    edgeId = okDto.EdgeId.ToString();
                }

                // レスポンスデータをチェックする
                CheckResponseDataEquals(postResult, expected_DataJsonPath, edgeId);

                // ログの出力をチェックする
                CheckLogMessagesContains(expected_LogMessagesPath, actualLogs, edgeId);

                // DB内容をチェックする
                CheckDbDataEquals(expected_DtDevice_TableDataPath);

                // アップロードしたBlobの内容をチェックし、削除する
                CheckBlobsEqualsAndCleanBlobs(
                    didUploadInitialBlob,
                    edgeId,
                    configContents,
                    _appSettings.DeliveringBlobContainerNameInstallbase,
                    filePath,
                    expected_UploadedDeliveringBlobPath);

                // 後処理
                DbTestHelper.ExecSqlFromFilePath(in_DeleteNewDataSqlPath);
            }
            catch
            {
                // デバッグでエラー番号見る用
                throw;
            }
        }

        /// <summary>
        /// PutDeviceメソッドのテスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InsertNewDataSqlPath">挿入するデータのSQLファイルパス</param>
        /// <param name="in_DeleteNewDataSqlPath">削除するデータのSQLファイルパス</param>
        /// <param name="in_InputJsonDataPath">投入するデータのJSONファイルパス</param>
        /// <param name="in_InitialBlobFilePath">事前にアップロードするBlobファイルが置いてあるパス</param>
        /// <param name="in_DeliveringBlobContainerNameInstallbase">AppSettingsに登録するDeliveringBlobのコンテナ名</param>
        /// <param name="in_DeliveringBlobInstallbaseFilePath">AppSettingsに登録するDeliveringBlobのファイルパス</param>
        /// <param name="in_SidFormat">投入するSIDフォーマット</param>
        /// <param name="in_InvalidDbConnectionString">DB接続文字列を不正地で設定するかどうか</param>
        /// <param name="in_InvalidBlobConnectionString">Blob接続文字列を不正地で設定するかどうか</param>
        /// <param name="in_ServiceException">Serviceで強制的に例外を発生させるかどうか</param>
        /// <param name="expected_DataJsonPath">期待するJsonデータファイルパス</param>
        /// <param name="expected_LogMessagesPath">期待するログメッセージが記載されているファイルパス</param>
        /// <param name="expected_UploadedDeliveringBlobPath">期待するアップロードしたBlobの内容が記載されているファイルのパス</param>
        /// <param name="expected_DtDevice_TableDataPath">期待する端末データのDBテーブル内容</param>
        /// <param name="remarks">備考</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\DevicesContoroller\Controllers_DevicesContoroller_PutDevice.csv")]
        public void PutDeviceTest(
            string no,
            string in_InsertNewDataSqlPath,
            string in_DeleteNewDataSqlPath,
            string in_InputJsonDataPath,
            string in_InitialBlobFilePath,
            string in_DeliveringBlobContainerNameInstallbase,
            string in_DeliveringBlobInstallbaseFilePath,
            string in_SidFormat,
            string in_InvalidDbConnectionString,
            string in_InvalidBlobConnectionString,
            string in_ServiceException,
            string expected_DataJsonPath,
            string expected_LogMessagesPath,
            string expected_UploadedDeliveringBlobPath,
            string expected_DtDevice_TableDataPath,
            string remarks)
        {
            string configContents;

            try
            {
                bool didUploadInitialBlob = CommonPreset(
                DateTime.Parse("2020/04/30 09:00:00.0000000"),
                in_InvalidDbConnectionString,
                in_InvalidBlobConnectionString,
                in_ServiceException,
                in_DeliveringBlobContainerNameInstallbase,
                in_DeliveringBlobInstallbaseFilePath,
                in_InsertNewDataSqlPath,
                in_InitialBlobFilePath,
                out configContents);

                // 投入JSONをDTOに変換する
                string inputDtoJson = (!string.IsNullOrEmpty(in_InputJsonDataPath) && File.Exists(in_InputJsonDataPath)) ?
                    File.ReadAllText(in_InputJsonDataPath) :
                    "";

                // SID, RowVersionの取得
                DataTable inputDataTable = DbTestHelper.SelectTable("SELECT SID, EDGE_ID, EQUIPMENT_UID FROM core.DT_DEVICE");
                long sid = 1;
                string edgeId = string.Empty;
                string equipmentUid = string.Empty;
                if (inputDataTable.Rows.Count > 0)
                {
                    sid = (long)inputDataTable.Rows[0].ItemArray[0];
                    edgeId = ((Guid)inputDataTable.Rows[0].ItemArray[1]).ToString();
                    equipmentUid = (string)inputDataTable.Rows[0].ItemArray[2];
                }

                // 投入するSIDを設定
                long inputSid = long.Parse(string.Format(in_SidFormat, sid));

                // DTO変換できない場合は初期値とする(Azure Functionsと同様の挙動)
                DeviceUpdateRequestDto inputDto;
                try
                {
                    inputDto = JsonConvert.DeserializeObject<DeviceUpdateRequestDto>(inputDtoJson);
                }
                catch (Exception)
                {
                    inputDto = new DeviceUpdateRequestDto();
                }

                // アップロード予定の場所にあるBlobを削除する
                TryFormatDeliveringBlobInstallbaseFilePath(_appSettings.DeliveringBlobInstallbaseFilePath, equipmentUid, out string filePath);
                if (didUploadInitialBlob &&
                    (!_appSettings.DeliveringBlobContainerNameInstallbase.Equals(_initialBlobContainerName) || !filePath.Equals(_initialBlobFilePath)))
                {
                    // 事前Blobがアップロードされている場合は違うパスの時のみ削除する
                    TryDeleteBlob(in_DeliveringBlobContainerNameInstallbase, filePath);
                }

                // ログ格納用ロガーの用意
                var actualLogs = new List<TestLog>();
                var logger = new TestLogger<DevicesController>(actualLogs);

                // WebApi実行
                var putResult = _controller.PutDevice(inputDto, inputSid, logger);

                // レスポンスデータをチェックする
                CheckResponseDataEquals(putResult, expected_DataJsonPath, edgeId);

                // ログの出力をチェックする
                CheckLogMessagesContains(expected_LogMessagesPath, actualLogs, edgeId);

                // DB内容をチェックする
                CheckDbDataEquals(expected_DtDevice_TableDataPath);

                // アップロードしたBlobの内容をチェックし、削除する
                CheckBlobsEqualsAndCleanBlobs(
                    didUploadInitialBlob,
                    edgeId,
                    configContents,
                    _appSettings.DeliveringBlobContainerNameInstallbase,
                    filePath,
                    expected_UploadedDeliveringBlobPath);

                // 後処理
                DbTestHelper.ExecSqlFromFilePath(in_DeleteNewDataSqlPath);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 共通の事前設定項目
        /// </summary>
        /// <param name="timeProviderUtcNow">DIするTimeProviderのUtcNowで返却させる日時</param>
        /// <param name="in_InvalidDbConnectionString">投入するAppSettingsが正常値かどうか</param>
        /// <param name="in_InvalidBlobConnectionString">Blob接続文字列を不正地で設定するかどうか</param>
        /// <param name="in_ServiceException">Serviceで強制的に例外を発生させるかどうか</param>
        /// <param name="in_DeliveringBlobContainerNameInstallbase">AppSettingsに登録するDeliveringBlobのコンテナ名</param>
        /// <param name="in_DeliveringBlobInstallbaseFilePath">AppSettingsに登録するDeliveringBlobのファイルパス</param>
        /// <param name="in_InsertNewDataSqlPath">挿入するデータのSQLファイルパス</param>
        /// <param name="in_InitialBlobFilePath">事前にBlobにアップロードしておくファイルのパス</param>
        /// <param name="configContents">事前アップロードしたBlobファイル内容</param>
        /// <returns></returns>
        private bool CommonPreset(
            DateTime timeProviderUtcNow,
            string in_InvalidDbConnectionString,
            string in_InvalidBlobConnectionString,
            string in_ServiceException,
            string in_DeliveringBlobContainerNameInstallbase,
            string in_DeliveringBlobInstallbaseFilePath,
            string in_InsertNewDataSqlPath,
            string in_InitialBlobFilePath,
            out string configContents)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (bool.TryParse(in_InvalidDbConnectionString, out bool isDbSettingsInvalid) && isDbSettingsInvalid)
            {
                // DB接続文字列が不正なAppSettingsとする
                dic.Add("ConnectionStrings:PrimaryDbConnectionString", null);
            }
            if (bool.TryParse(in_InvalidBlobConnectionString, out bool isBlobSettingsInvalid) && isBlobSettingsInvalid)
            {
                // Blob接続文字列が不正なAppSettingsとする
                dic.Add("ConnectionStrings:DeliveringBlobConnectionString", "BlobEndpoint=https://rmscoemujpedeliver01.blob.core.windows.net/;QueueEndpoint=https://rmscoemujpedeliver01.queue.core.windows.net/;FileEndpoint=https://rmscoemujpedeliver01.file.core.windows.net/;TableEndpoint=https://rmscoemujpedeliver01.table.core.windows.net/;SharedAccessSignature=sv=2019-02-02&ss=bfqt&srt=sco&sp=rwdlacup&se=9999-12-31T14:59:59Z&st=2019-12-31T15:00:00Z&spr=https&sig=hogehogehogehogehogehogehogehogehogehogehogehoge");
            }

            // Blobのコンテナ名・ファイルパスの設定
            dic.Add("DeliveringBlobContainerNameInstallbase", in_DeliveringBlobContainerNameInstallbase);
            dic.Add("DeliveringBlobInstallbaseFilePath", in_DeliveringBlobInstallbaseFilePath);

            // DI実施
            DependencyInjection(timeProviderUtcNow, dic, (bool.TryParse(in_ServiceException, out bool isServiceException) && isServiceException));

            // 初期データ挿入
            DbTestHelper.ExecSqlFromFilePath(in_InsertNewDataSqlPath);

            // アップロード予定の場所にあるBlobを削除する
            TryDeleteBlob(_initialBlobContainerName, _initialBlobFilePath);

            // 初期Blobを入れる場合はアップロードする
            configContents = string.Empty;
            bool didUploadInitialBlob = false;
            if (!string.IsNullOrEmpty(in_InitialBlobFilePath) && File.Exists(in_InitialBlobFilePath))
            {
                configContents = File.ReadAllText(in_InitialBlobFilePath);

                // テストでは固定のコンテナ名・ファイルパスで設定する
                TryUploadBlob(_initialBlobContainerName, _initialBlobFilePath, configContents);
                didUploadInitialBlob = true;
            }

            // WebAPIでアップロードする先のコンテナを作成する
            _ = TryCreateContainer(in_DeliveringBlobContainerNameInstallbase);

            return didUploadInitialBlob;
        }

        /// <summary>
        /// レスポンスデータが期待値と等しいかチェックする
        /// </summary>
        /// <param name="actionResult">レスポンスデータ</param>
        /// <param name="expected_DataJsonPath">期待するJsonデータファイルパス</param>
        /// <param name="edgeId">エッジID</param>
        private void CheckResponseDataEquals(
            IActionResult actionResult,
            string expected_DataJsonPath,
            string edgeId)
        {
            string resultJson = myUtility.ObjectExtensions.ToStringJsonIndented(actionResult);
            string expectJson = string.Empty;
            if (File.Exists(expected_DataJsonPath))
            {
                expectJson = File.ReadAllText(expected_DataJsonPath);

                // エッジIDは流動的になるので期待値を設定する
                expectJson = expectJson.Replace("\"edgeId\": {0}", "\"edgeId\": \"" + edgeId + "\"");
            }

            // データの比較
            Assert.AreEqual(expectJson, resultJson);
        }

        /// <summary>
        /// 期待するログメッセージが記載されているファイルパスが表示されたかチェックする
        /// </summary>
        /// <param name="expected_LogMessagesPath">期待するログメッセージが記載されているファイルパス</param>
        /// <param name="controllerLogs">Controllerで出力されたログ</param>
        /// <param name="edgeId">エッジID</param>
        private void CheckLogMessagesContains(
            string expected_LogMessagesPath,
            List<TestLog> controllerLogs,
            string edgeId)
        {
            // 期待するログ、実際のログを取得
            List<string> expectedLogMessages = (!string.IsNullOrEmpty(expected_LogMessagesPath) && File.Exists(expected_LogMessagesPath)) ?
                File.ReadLines(expected_LogMessagesPath).ToList() :
                new List<string>();
            List<string> actualControllerLogMessages = controllerLogs.Select(x => x.GetSimpleText()).ToList();
            List<string> actualServiceLogMessages = _serviceLogs.Select(x => x.GetSimpleText()).ToList();

            // ControllerかServiceで期待するログが出力されたか確認
            foreach (var expectedLogMessage in expectedLogMessages)
            {
                string expectedLogMessageEdgeId = expectedLogMessage.Replace("エッジID = {0}", "エッジID = " + edgeId);

                var matchingElementController = actualControllerLogMessages.FirstOrDefault(actual => actual.Contains(expectedLogMessageEdgeId));
                var matchingElementService = actualServiceLogMessages.FirstOrDefault(actual => actual.Contains(expectedLogMessageEdgeId));
                Assert.IsTrue((matchingElementController != null || matchingElementService != null), string.Format("「{0}」に一致する要素が見つかりません", expectedLogMessageEdgeId));
                actualControllerLogMessages.Remove(matchingElementController);
                actualControllerLogMessages.Remove(matchingElementService);
            }
        }

        /// <summary>
        /// DBデータが期待値と等しいかチェックする
        /// </summary>
        /// <param name="expected_DtDevice_TableDataPath">期待する端末データのDBテーブル内容</param>
        private void CheckDbDataEquals(string expected_DtDevice_TableDataPath)
        {
            // テーブルデータの実際の値・期待値を取得(EdgeId以外)
            DataTable deviceActualTable = DbTestHelper.SelectTable(
                @"SELECT 
                    SID, EQUIPMENT_MODEL_SID, INSTALL_TYPE_SID, CONNECT_STATUS_SID, EQUIPMENT_UID, REMOTE_CONNECT_UID, RMS_SOFT_VERSION, CONNECT_START_DATETIME, CONNECT_UPDATE_DATETIME, CREATE_DATETIME, UPDATE_DATETIME 
                FROM core.DT_DEVICE");
            DataTable deviceExpectedTable = DbTestHelper.SelectCsv(expected_DtDevice_TableDataPath,
                @"SELECT 
                    SID, EQUIPMENT_MODEL_SID, INSTALL_TYPE_SID, CONNECT_STATUS_SID, EQUIPMENT_UID, REMOTE_CONNECT_UID, RMS_SOFT_VERSION, CONNECT_START_DATETIME, CONNECT_UPDATE_DATETIME, CREATE_DATETIME, UPDATE_DATETIME
                FROM core.DT_DEVICE");

            // テーブルデータの比較
            CheckDataTableEquals(deviceExpectedTable, deviceActualTable);
        }

        /// <summary>
        /// アップロードされているBlobが期待値と等しいかチェックし、その後削除する
        /// </summary>
        /// <param name="didUploadInitialBlob">Blobを事前アップロードしたか</param>
        /// <param name="edgeId">エッジID</param>
        /// <param name="initialBlobContents">事前アップロードしたBlob内容</param>
        /// <param name="actualUploadedContainerName">アップロードに使用したコンテナ名</param>
        /// <param name="actualUploadedFilePath">アップロードに使用したファイルパス</param>
        /// <param name="expectedUploadedDeliveringBlobPath">期待するアップロードしたBlobの内容が記載されているファイルのパス</param>
        private void CheckBlobsEqualsAndCleanBlobs(
            bool didUploadInitialBlob,
            string edgeId,
            string initialBlobContents,
            string actualUploadedContainerName,
            string actualUploadedFilePath,
            string expectedUploadedDeliveringBlobPath)
        {
            // Blobの状況の取得して確認する
            string actualConfigContents = string.Empty;
            string actualUploadedContents;
            string expectedUploadedContents = !string.IsNullOrEmpty(expectedUploadedDeliveringBlobPath) && File.Exists(expectedUploadedDeliveringBlobPath) ?
                File.ReadAllText(expectedUploadedDeliveringBlobPath) :
                string.Empty;
            string expectedConfigContents = string.Empty;

            // エッジIDを期待値に埋め込む(JSONだと中括弧がFormatに影響を与えて面倒なので、Replaceを使用する)
            expectedUploadedContents = expectedUploadedContents.Replace("\"edgeId\": {0}", "\"edgeId\": \"" + edgeId + "\"");

            TryDownloadBlob(actualUploadedContainerName, actualUploadedFilePath, out actualUploadedContents);
            if (didUploadInitialBlob &&
                (!_initialBlobContainerName.Equals(actualUploadedContainerName) || !_initialBlobFilePath.Equals(actualUploadedFilePath)))
            {
                // 別にconfigにアップロードされているファイルがあるので、期待値を設定する
                expectedConfigContents = initialBlobContents;

                // 事前アップロード済みで、今回アップロードしたファイルとコンテナ名・ファイルパスが被っていない場合は、
                // 事前アップロードファイルの内容を取得する
                TryDownloadBlob(_initialBlobContainerName, _initialBlobFilePath, out actualConfigContents);
            }

            // Blobの内容確認
            Assert.AreEqual(expectedConfigContents, actualConfigContents);
            Assert.AreEqual(expectedUploadedContents, actualUploadedContents);

            // アップロードしたBlobの削除
            TryDeleteBlob(_initialBlobContainerName, _initialBlobFilePath);
            TryDeleteBlob(actualUploadedContainerName, actualUploadedFilePath);

            // WebAPIでアップロードした先のコンテナを削除する
            _ = TryDeleteContainer(actualUploadedContainerName);
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
                for (int item_i = 0; item_i < csvExpected.Rows[row_i].ItemArray.Length; item_i++)
                {
                    if (dbActual.Rows[row_i].ItemArray[item_i] == null)
                    {
                        // DBとデータを合わせるためにnullを代入
                        csvExpected.Rows[row_i].ItemArray[item_i] = null;
                    }
                }

                CollectionAssert.AreEqual(csvExpected.Rows[row_i].ItemArray, dbActual.Rows[row_i].ItemArray);
            }
        }

        /// <summary>
        /// ファイルパスのフォーマット設定を行う
        /// </summary>
        /// <param name="filePathFormat">フォーマット付きのファイルパス</param>
        /// <param name="equipmentUid">機器UID</param>
        /// <param name="formatedFilePath">フォーマット後のファイルパス</param>
        /// <returns>フォーマット化成功/失敗</returns>
        private bool TryFormatDeliveringBlobInstallbaseFilePath(string filePathFormat, string equipmentUid, out string formatedFilePath)
        {
            bool result;

            try
            {
                formatedFilePath = string.Format(filePathFormat, equipmentUid);
                result = true;
            }
            catch
            {
                formatedFilePath = string.Empty;
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Blobをアップロードする
        /// </summary>
        /// <param name="containerName">コンテナ名</param>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="contents">アップロード内容</param>
        /// <returns>アップロード成功/失敗</returns>
        private bool TryUploadBlob(string containerName, string filePath, string contents)
        {
            bool result;

            try
            {
                _deliveringBlob.Client.Upload(containerName, filePath, contents);
                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Blob内容を取得する
        /// </summary>
        /// <param name="containerName">コンテナ名</param>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="contents">取得内容</param>
        /// <returns>取得成功/失敗</returns>
        private bool TryDownloadBlob(string containerName, string filePath, out string contents)
        {
            bool result;

            try
            {
                _deliveringBlob.Client.Download(containerName, filePath, out contents);
                result = true;
            }
            catch
            {
                contents = string.Empty;
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Blobを削除する
        /// </summary>
        /// <param name="containerName">コンテナ名</param>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>削除成功/失敗</returns>
        private bool TryDeleteBlob(string containerName, string filePath)
        {
            bool result;

            try
            {
                _deliveringBlob.Delete(new ArchiveFile()
                {
                    ContainerName = containerName,
                    FilePath = filePath
                });
                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// コンテナを作成する
        /// </summary>
        /// <param name="containerName">コンテナ名</param>
        /// <returns>作成成功/失敗</returns>
        async private Task<bool> TryCreateContainer(string containerName)
        {
            if (containerName.Equals(_initialBlobContainerName))
            {
                // デフォルトのコンテナ名の場合は作成しない
                return true;
            }

            bool result;
            try
            {
                CloudStorageAccount _account = CloudStorageAccount.Parse(_appSettings.DeliveringBlobConnectionString);
                CloudBlobClient Client = _account.CreateCloudBlobClient();
                CloudBlobContainer container = Client.GetContainerReference(containerName);
                await container.CreateIfNotExistsAsync();
                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// コンテナを削除する
        /// </summary>
        /// <param name="containerName">コンテナ名</param>
        /// <returns>削除成功/失敗</returns>
        async private Task<bool> TryDeleteContainer(string containerName)
        {
            if (containerName.Equals(_initialBlobContainerName))
            {
                // デフォルトのコンテナ名の場合は削除しない
                return true;
            }

            bool result;
            try
            {
                CloudStorageAccount _account = CloudStorageAccount.Parse(_appSettings.DeliveringBlobConnectionString);
                CloudBlobClient Client = _account.CreateCloudBlobClient();
                CloudBlobContainer container = Client.GetContainerReference(containerName);
                await container.DeleteIfExistsAsync();
                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
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
            builder.ServiceCollection.AddTransient<DevicesController>();
            if (isSeriviceError)
            {
                // Serviceの各メソッドで例外を発生させるクラスに置き換える
                builder.ServiceCollection.AddTransient<IDeviceService, TestDeviceService>();
            }
            builder.ServiceCollection.AddTransient<DeliveringBlob>();
            builder.ServiceCollection.AddSingleton<myUtility.ITimeProvider>(UnitTestHelper.CreateTimeProvider(utcNow));
            builder.AddConfigures(appSettings);

            _serviceLogs = new List<TestLog>();
            _serviceLogger = new TestLogger<DeviceService>(_serviceLogs);
            builder.ServiceCollection.AddSingleton<ILogger<DeviceService>>(_serviceLogger);

            // DI後したクラスを作成する
            ServiceProvider provider = builder.Build();
            _controller = provider.GetService<DevicesController>();
            _deliveringBlob = provider.GetService<DeliveringBlob>();
            _appSettings = provider.GetService<myUtility.AppSettings>();
        }
    }

    /// <summary>
    /// 強制的に例外を発生させるServiceのテストクラス
    /// </summary>
    public class TestDeviceService : IDeviceService
    {
        /// <summary>
        /// 機器情報を保存する
        /// </summary>
        /// <remarks>sd 04-01.機器登録</remarks>
        /// <param name="utilParam">Utility型機器情報</param>
        /// <param name="baseConfig">設置設定情報</param>
        /// <returns>結果付きDB保存済み機器情報</returns>
        public Result<DtDevice> Create(DtDevice utilParam, InstallBaseConfig baseConfig)
        {
            throw new Exception();
        }

        /// <summary>
        /// 機器情報を更新する
        /// </summary>
        /// <remarks>sd 04-01.機器登録</remarks>
        /// <param name="utilParam">>Utility型機器情報</param>
        /// <param name="baseConfig">設置設定情報</param>
        /// <returns>結果付きDB保存済み機器情報</returns>
        public Result<DtDevice> Update(DtDevice utilParam, InstallBaseConfig baseConfig)
        {
            throw new Exception();
        }

        /// <summary>
        /// デバイスにリモート接続をリクエストする
        /// </summary>
        /// <param name="request">リクエスト</param>
        /// <returns>結果</returns>
        public Task<Result> RequestRemoteAsync(RequestRemote request)
        {
            throw new Exception();
        }
    }
}
