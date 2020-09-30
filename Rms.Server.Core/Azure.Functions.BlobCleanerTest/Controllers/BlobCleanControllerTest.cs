using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Rms.Server.Core.Abstraction.Models;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Core.Abstraction.Repositories.Blobs;
using Rms.Server.Core.Azure.Functions.BlobCleaner;
using Rms.Server.Core.Service.Services;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Core.Utility.Models.Entites;
using Rms.Server.Test;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TestHelper;

namespace Azure.Functions.BlobCleanerTest
{
    /// <summary>
    /// BlobCleanerテストコード
    /// </summary>
    [TestClass]
    public class BlobCleanControllerTest
    {
        /// <summary>
        /// BlobCleanController
        /// </summary>
        private static BlobCleanController _controller = null;

        /// <summary>
        /// ServiceProvider
        /// </summary>
        private static ServiceProvider _provider = null;

        /// <summary>
        /// DtDeviceFileRepositoryMock
        /// </summary>
        private static DtDeviceFileRepositoryMock _dtDeviceFileRepositoryMock = null;

        /// <summary>
        /// PrimaryBlobRepositoryMock
        /// </summary>
        private static PrimaryBlobRepositoryMock _primaryBlobRepositoryMock = null;

        /// <summary>
        /// AppSettings
        /// </summary>
        private static AppSettings _appSettings = null;

        /// <summary>
        /// DBPolly
        /// </summary>
        private static DBPolly _dBPolly = null;

        /// <summary>
        /// BlobPolly
        /// </summary>
        private static BlobPolly _blobPolly = null;

        /// <summary>
        /// PrimaryBlob
        /// </summary>
        private static PrimaryBlob _primaryBlob;

        /// <summary>
        /// DateTimeProvider
        /// </summary>
        private static DateTimeProvider _dateTimeProvider = null;

        /// <summary>
        /// Logリスト
        /// </summary>
        private readonly static List<TestLog> _dtDeviceFileActualLogs = new List<TestLog>();

        /// <summary>
        /// ロガー
        /// </summary>
        private static TestLogger<CleanBlobService> _serviceLogger = null;

        /// <summary>
        /// 日時データ文字列のフォーマット定義
        /// </summary>
        private readonly static string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fff";

        /// <summary>
        /// テスト対象コンテナ名一覧
        /// </summary>
        private readonly static string[] ContainerNameList = new string[] { "log", "error", "device" };

        #region テスト準備

        /// <summary>
        /// ClassInit
        /// </summary>
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            // 関連DBデータを全削除
            DbTestHelper.DeleteAllReseed();

            // DB設定
            // マスタテーブルデータを作成する
            DbTestHelper.ExecSqlFromFilePath(@"TestData\MakeMasterTableData.sql");
        }

        #endregion

        #region テスト後処理

        /// <summary>
        /// ClassCleanup
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanup()
        {
            // 関連DBデータを全削除
            DbTestHelper.DeleteAllReseed();
        }

        #endregion

        /// <summary>
        /// BlobCleanerの例外処理をテストする
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="TestDateTime">テストの基準となる時刻</param>
        /// <param name="LocalFileRoot">コンテナにアップロードするローカルファイルのパス</param>
        /// <param name="in_AppSettings">AppSettingsに追加登録する項目をまとめたJSON文字列</param>
        /// <param name="in_InsertNewDataSqlPath">DBに挿入するデータを記述したSQL文のパス</param>
        /// <param name="expected_BlobStatusPath">BlobCleaner実行後のBlobの状態を記述したCSVファイルのパス</param>
        /// <param name="expected_FileAttributesCheck">ファイル属性データテーブルをテストするか?</param>
        /// <param name="expected_FileAttributesCount">ファイル属性テータテーブルのレコード数</param>
        /// <param name="expected_LogLevel">期待するログレベル</param>
        /// <param name="expected_LogResource">エラーコード文字列</param>
        /// <param name="expected_LogInfo">ログに出力する情報を格納したCSVファイルパス</param>
        /// <param name="expected_WithoutError">trueの場合にはエラーメッセージがないことを確認する。falseの場合にはログの確認を行う</param>
        /// <param name="expected_ErrorDescription">trueの場合にはエラーメッセージの詳細を確認する</param>
        /// <param name="error_DeleteDtDeviceFile">DeleteDtDeviceFileで例外を発生させるか?</param>
        /// <param name="error_Delete">Deleteで例外を発生させるか?</param>
        /// <param name="remarks">テストに関する備考（CLにおけるテスト番号）</param>
        /// <returns>非同期処理タスク</returns>
        /// <remarks>
        /// ・例外処理フラグはtrueのときに例外を発生させる
        /// ・例外処理フラグは排他的に設定すること
        /// </remarks>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\Controllers_BlobCleanContoroller.csv")]
        public async Task BlobCleanTest(
            string no,
            string TestDateTime,
            string LocalFileRoot,
            string in_AppSettings,
            string in_InsertNewDataSqlPath,
            string expected_BlobStatusPath,
            string expected_FileAttributesCheck,
            string expected_FileAttributesCount,
            string expected_LogLevel,
            string expected_LogResource,
            string expected_LogInfo,
            string expected_WithoutError,
            string expected_ErrorDescription,
            string error_DeleteDtDeviceFile,
            string error_Delete,
            string remarks)
        {
            // AppSettings
            Dictionary<string, string> appSettingsConfigures = null;
            if (!string.IsNullOrEmpty(in_AppSettings))
            {
                appSettingsConfigures = JsonConvert.DeserializeObject<Dictionary<string, string>>(in_AppSettings);
            }

            // 例外処理を発生させるメソッドを示すフラグ
            bool.TryParse(error_DeleteDtDeviceFile, out bool isErrorOnDeleteDtDeviceFile);
            bool.TryParse(error_Delete, out bool isErrorOnDelete);

            // エラーログが出ていないことを確認するかどうかを示すフラグ
            bool.TryParse(expected_WithoutError, out bool isWithoutError);

            // エラーログの一致性は確認しないがエラーメッセージの有無はチェックするかどうかを示すフラグ
            bool.TryParse(expected_ErrorDescription, out bool isErrorDescriptionNeeded);

            // 端末ファイル属性データテーブルの状態をテストするかどうかを示すフラグ
            bool.TryParse(expected_FileAttributesCheck, out bool isFileAttributesChecked);

            // 端末ファイル属性データテーブルの状態をテストする場合に、期待するレコード数
            // 全削除を期待するのであれば0
            int.TryParse(expected_FileAttributesCount, out int expectedFileAttributesCount);

            // ログ格納用ロガーの用意
            // Controller用
            var actualControllerLogs = new List<TestLog>();
            var logger = new TestLogger<BlobCleanController>(actualControllerLogs);

            // Service用
            var actualServiceLogs = new List<TestLog>();
            _serviceLogger = new TestLogger<CleanBlobService>(actualServiceLogs);

            // テスト実行日時（ファイル削除閾値の基準となる値）
            DateTime testDateTime = default(DateTime);
            if (!string.IsNullOrEmpty(TestDateTime))
            {
                testDateTime = DateTime.ParseExact(TestDateTime, DateTimeFormat, null);
            }
                               
            // テスト用実行日時情報、Logger、AppSettingsを渡してDI
            DependencyInjection(testDateTime, _serviceLogger, appSettingsConfigures);

            // 例外処理を実行フラグをモックに設定する
            // 第1引数は削除対象ファイルのうち何番目のファイル処理時に例外を起こすかを指定するインデックスだが、
            // 基本的に1番目のファイルを対象とするので1固定とする
            _dtDeviceFileRepositoryMock.Init(1, isErrorOnDeleteDtDeviceFile);
            _primaryBlobRepositoryMock.Init(1, isErrorOnDelete);

            // テスト用データをDBに挿入する
            if (!string.IsNullOrEmpty(in_InsertNewDataSqlPath))
            {
                DbTestHelper.ExecSqlFromFilePath(in_InsertNewDataSqlPath);
            }

            // PrimaryBlobの接続文字列
            var connectionString = _appSettings.PrimaryBlobConnectionString;

            // ローカルファイルパスの記述がある場合にはBlobにファイルをアップロードする
            int dbMaxCount = 0;

            if (!string.IsNullOrEmpty(LocalFileRoot))
            {
                // ファイルリスト取得
                string[] fileList = Directory.GetFiles(LocalFileRoot, "*", SearchOption.AllDirectories);
                dbMaxCount = fileList.Length;

                // 端末ファイルの数だけDBからファイルパス情報を取得する
                List<BlobInfo> blobInfoList = GetBlobInfoListFromDB(dbMaxCount);

                // Blobアップロード処理
                foreach (string name in fileList)
                {
                    // ローカルのテスト用ファイルのファイル名のみを取得
                    string filename = Path.GetFileName(name);

                    foreach (var blobInfo in blobInfoList)
                    {
                        // Blobコンテナを取得する
                        var container = GetContainer(connectionString, blobInfo.ContainerName);

                        // ローカルのテスト用ファイル名が、Blobのパスに含まれていた場合にそのファイルをBlobとして追加する
                        // ローカルファイルとBlobのアップロード先のファイル名を1対1に対応させる
                        if (blobInfo.FilePath.Contains(filename))
                        {
                            // 指定したローカルファイルパスのファイルをアップロード
                            await Upload(container, blobInfo.FilePath, name);
                            break;
                        }
                    }
                }
            }

            // BlobCleaner実行
            {
                Rms.Server.Core.Utility.Assert.IfNull(_controller);
                _controller.CleanBlob(null, logger);
            }

            // テスト結果フラグ
            bool isDatabaseStatusMatched = true;
            bool isBlobStatusMatched = true;
            bool isExpectedLogContained = false;

            // DBとBlobの状態確認
            if (!string.IsNullOrEmpty(expected_BlobStatusPath))
            {
                var expectedBlobStatusList = GetBlobExpectedStatusList(expected_BlobStatusPath);
                var actualDBStatusList = GetBlobInfoListFromDB(dbMaxCount).Select(x => x.FilePath);

                foreach (var eachStatus in expectedBlobStatusList)
                {
                    // DBの確認
                    if (eachStatus.IsDBCheckNeeded)
                    {
                        if (eachStatus.IsDeletedFromDB)
                        {
                            if (actualDBStatusList.Contains(eachStatus.Path))
                            {
                                isDatabaseStatusMatched = false;
                                break;
                            }
                        }
                        else
                        {
                            if (!actualDBStatusList.Contains(eachStatus.Path))
                            {
                                isDatabaseStatusMatched = false;
                                break;
                            }
                        }
                    }

                    // Containerの確認
                    if (eachStatus.IsContainerCheckNeeded)
                    {
                        var actualBlobList = await GetBlobList(connectionString, eachStatus.ContainerName);
                        if (eachStatus.IsDeletedFromContainer)
                        {
                            if (actualBlobList.Contains(eachStatus.Path))
                            {
                                isBlobStatusMatched = false;
                                break;
                            }
                        }
                        else
                        {
                            if (!actualBlobList.Contains(eachStatus.Path))
                            {
                                isBlobStatusMatched = false;
                                break;
                            }
                        }
                    }
                }
            }

            // DB（FileAttributes）のチェック
            if (isFileAttributesChecked)
            {
                int actualFileAttributesCount = GetFileAttributesCount(dbMaxCount);
                if (actualFileAttributesCount != expectedFileAttributesCount)
                {
                    isDatabaseStatusMatched = false;
                }
            }

            // ログの確認
            if (isWithoutError)
            {
                // エラーログが出ていないことを確認する
                bool isControllerError = false;
                bool isServiceError = false;

                // Controllerログ
                foreach (TestLog eachLog in actualControllerLogs)
                {
                    if (eachLog.LogLevel == Microsoft.Extensions.Logging.LogLevel.Error)
                    {
                        isControllerError = true;
                        break;
                    }
                }

                // Serviceログ
                foreach (TestLog eachLog in actualServiceLogs)
                {
                    if (eachLog.LogLevel == Microsoft.Extensions.Logging.LogLevel.Error)
                    {
                        isServiceError = true;
                        break;
                    }
                }

                isExpectedLogContained = !(isControllerError || isServiceError);
            }
            else if (isErrorDescriptionNeeded)
            {
                // エラーログの詳細が含まれているか確認する
                // Serviceログ（Controllerのログはチェックしない）
                foreach (TestLog eachLog in actualServiceLogs)
                {
                    if (eachLog.LogLevel == Microsoft.Extensions.Logging.LogLevel.Error)
                    {
                        string message = eachLog.GetSimpleText();
                        if (!message.Contains("()"))
                        {
                            isExpectedLogContained = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                //
                // ログの詳細を確認する
                //

                // ログに出力する情報
                var logInfo = GetLogInfo(expected_LogInfo);

                // 期待するログメッセージ
                var expectedLogMessages = GetLogMessages(expected_LogResource, logInfo);

                // ログレベル
                int expectedLogLevel = -1;
                if (!string.IsNullOrEmpty(expected_LogLevel))
                {
                    int.TryParse(expected_LogLevel, out expectedLogLevel);
                }

                int expectedLogMessagesCount = expectedLogMessages.Count;
                int countMatchLogs = 0;

                foreach (string expectedLogMessage in expectedLogMessages)
                {
                    // Controllerログ
                    foreach (TestLog eachLog in actualControllerLogs)
                    {
                        string actualLog = eachLog.GetSimpleText();
                        if (actualLog.Contains(expectedLogMessage) 
                            && eachLog.LogLevel == (Microsoft.Extensions.Logging.LogLevel) expectedLogLevel)
                        {
                            // 期待していたログが見つかった
                            countMatchLogs += 1;
                            break;
                        }
                    }

                    // Serviceログ
                    foreach (TestLog eachLog in actualServiceLogs)
                    {
                        string actualLog = eachLog.GetSimpleText();
                        if (actualLog.Contains(expectedLogMessage))
                        {
                            // 期待していたログが見つかった
                            countMatchLogs += 1;
                            break;
                        }
                    }
                }

                // 期待するログがすべて見つかればフラグを立てる
                if (countMatchLogs == expectedLogMessagesCount)
                {
                    isExpectedLogContained = true;
                }
            }

            // Blob削除（コンテナは削除しない）: テストケース1回ごとにBlobを削除する
            // 不正な接続文字列を指定していた場合には例外が発生するが、テストとは関係ないため例外は無視する
            try
            {
                foreach (string containerName in ContainerNameList)
                {
                    await DeleteBlobInContainer(connectionString, containerName);
                }
            } 
            catch (Exception)
            {
                ;
            }

            // Blob削除後にテスト
            // 期待するログがService用のロガーに記録されていればテストOK
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(true, isDatabaseStatusMatched);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(true, isBlobStatusMatched);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(true, isExpectedLogContained);
        }

        /// <summary>
        /// テキストファイルから1行ずつ読み出した結果をリストに変換して取得する
        /// 先頭行はヘッダと見なすためリストには追加しない
        /// </summary>
        /// <param name="filePath">テキストファイルパス</param>
        /// <returns>読み出した結果を1行ずつ格納したリスト</returns>
        private List<string> GetLinesFromTextFile(string filePath)
        {
            List<string> result = new List<string>();

            StreamReader sr = new StreamReader(filePath);
            _ = sr.ReadLine();  // 先頭行はヘッダなので読み飛ばす

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    result.Add(line);
                }
            }

            sr.Close();

            return result;
        }

        /// <summary>
        /// 端末ファイルデータテーブルに格納されているファイルのパスをリスト形式で取得する
        /// </summary>
        /// <param name="maxCount">取得したい数の上限（実際の数より多くてもよい）</param>
        /// <returns>ファイルパスリスト</returns>
        private List<BlobInfo> GetBlobInfoListFromDB(int maxCount)
        {
            List<BlobInfo> list = new List<BlobInfo>();

            for (int i = 0; i < maxCount; i++)
            {
                var deviceFile = _dtDeviceFileRepositoryMock.ReadDtDeviceFile(i + 1); // SIDは1開始を前提とする
                if (deviceFile != null)
                {
                    list.Add(new BlobInfo
                    {
                        FilePath = deviceFile.FilePath,
                        ContainerName = deviceFile.Container
                    });
                }
            }

            return list;
        }

        private List<ExpectedStatus> GetBlobExpectedStatusList(string expectedStatusCsvFilePath)
        {
            List<ExpectedStatus> result = new List<ExpectedStatus>();

            if (string.IsNullOrEmpty(expectedStatusCsvFilePath))
            {
                return result;
            }

            // CSVファイル読み込み
            List<string> lines = GetLinesFromTextFile(expectedStatusCsvFilePath);
            foreach (string line in lines)
            {
                string[] columns = line.Split(",");
                if (columns.Length == 6)
                {
                    string containerName = columns[0];
                    string path = columns[1];
                    string checkDB = columns[2];
                    string deletedFromDB = columns[3];
                    string checkContainer = columns[4];
                    string deletedFromContainer = columns[5];

                    bool.TryParse(checkDB, out bool isDBCheckNeeded);
                    bool.TryParse(deletedFromDB, out bool isDeletedFromDB);
                    bool.TryParse(checkContainer, out bool isContainerCheckNeeded);
                    bool.TryParse(deletedFromContainer, out bool isDeletedFromContainer);

                    ExpectedStatus status = new ExpectedStatus
                    {
                        ContainerName = containerName,
                        Path = path,
                        IsDBCheckNeeded = isDBCheckNeeded,
                        IsDeletedFromDB = isDeletedFromDB,
                        IsContainerCheckNeeded = isContainerCheckNeeded,
                        IsDeletedFromContainer = isDeletedFromContainer
                    };

                    result.Add(status);
                }
            }

            return result;
        }

        /// <summary>
        /// 端末ファイル属性データテーブルのデータがすべて削除されているかどうかを確認する
        /// </summary>
        /// <param name="maxCount">端末データテーブルに登録したレコード数以上の整数値</param>
        /// <returns>テーブルに存在するレコード数</returns>
        private int GetFileAttributesCount(int maxCount)
        {
            int recordCount = 0;

            for (int i = 0; i < maxCount; i++)
            {
                var deviceFile = _dtDeviceFileRepositoryMock.ReadDtDeviceFile(i + 1); // SIDは1開始を前提とする
                if (deviceFile != null)
                {
                    int count = deviceFile.DtDeviceFileAttribute.Select(x => x.DeviceFileSid == (i + 1)).Count();
                    recordCount += count;
                }               
            }

            return recordCount;
        }

        #region ログ出力

        /// <summary>
        /// CSVファイルに格納されたデータからログに出力するデータリストを作成する
        /// </summary>
        /// <param name="logInfoCsvFilePath">ログ情報を格納したCSVファイルのパス</param>
        /// <returns>ログ出力情報リスト</returns>
        private List<LogInfo> GetLogInfo(string logInfoCsvFilePath)
        {
            List<LogInfo> result = new List<LogInfo>();

            if (string.IsNullOrEmpty(logInfoCsvFilePath))
            {
                return result;
            }

            // CSVファイル読み込み
            List<string> lines = GetLinesFromTextFile(logInfoCsvFilePath);
            foreach (string line in lines)
            {
                string[] columns = line.Split(",");
                if (columns.Length == 3)
                {
                    string containerName = columns[0];
                    string targetFilePath = columns[1];
                    string logDateTimeString = columns[2];

                    DateTime logDateTime = default(DateTime);
                    if (!string.IsNullOrEmpty(logDateTimeString))
                    {
                        logDateTime = DateTime.ParseExact(logDateTimeString, DateTimeFormat, null);
                    }

                    LogInfo info = new LogInfo
                    {
                        ContainerName = containerName,
                        TargetFilePath = targetFilePath,
                        LogDateTime = logDateTime
                    };

                    result.Add(info);
                }
            }

            return result;
        }

        /// <summary>
        /// 期待するログメッセージリストを取得する
        /// </summary>
        /// <param name="errorCode">エラーコード文字列</param>
        /// <param name="logInfo">ログに格納する情報のリスト</param>
        /// <returns>Loggerに記録されたエラーメッセージ</returns>
        private List<string> GetLogMessages(string errorCode, List<LogInfo> logInfo)
        {
            List<string> result = new List<string>();

            var actualLogs = new List<TestLog>();
            var logger = new TestLogger<CleanBlobService>(actualLogs);

            if (logInfo.Count <= 0)
            {
                result.Add(errorCode);
            }
            else
            {
                foreach (LogInfo info in logInfo)
                {
                    // ログ出力
                    Rms.Server.Core.Utility.Extensions.LoggerExtensions.Error(logger,
                            new RmsException(errorCode), errorCode, new object[] { info.ContainerName, info.TargetFilePath, info.LogDateTime });
                }

                foreach (var log in actualLogs)
                {
                    result.Add(log.GetSimpleText());
                }
            }

            return result;
        }

        #endregion

        #region Blob操作

        /// <summary>
        /// 指定した名称のBlobコンテナを取得する
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="containerName"></param>
        /// <returns>コンテナ</returns>
        private static CloudBlobContainer GetContainer(string connectionString, string containerName)
        {
            var account = CloudStorageAccount.Parse(connectionString);
            var client = account.CreateCloudBlobClient();
            return client.GetContainerReference(containerName);
        }

        /// <summary>
        /// 指定したローカルパスのファイルをBlobストレージにアップロードする
        /// </summary>
        /// <param name="container">コンテナ</param>
        /// <param name="path">ローカルファイルパス</param>
        /// <param name="name">Blob名</param>
        /// <returns>非同期処理タスク</returns>
        private static async Task Upload(CloudBlobContainer container, string path, string name)
        {
            // ローカルのテスト用ファイルをBlobに変換する
            CloudBlockBlob blob = container.GetBlockBlobReference(path);

            // 指定したローカルファイルパスのファイルをアップロード
            await blob.UploadFromFileAsync(name);
        }

        /// <summary>
        /// 指定したコンテナ内のBlobリストを取得する
        /// </summary>
        /// <param name="connectionString">接続文字列</param>
        /// <param name="containerName">コンテナ名</param>
        /// <returns>Blobリスト</returns>
        private static async Task<List<string>> GetBlobList(string connectionString, string containerName)
        {
            List<string> blobList = new List<string>();

            var _account = CloudStorageAccount.Parse(connectionString);
            var client = _account.CreateCloudBlobClient();

            var container = client.GetContainerReference(containerName);

            if (!await container.ExistsAsync())
            {
                return blobList;
            }

            // Blob一覧を取得
            BlobContinuationToken token = null;
            do
            {
                // Blob一覧を取得
                var segment = await container.ListBlobsSegmentedAsync(string.Empty, true, BlobListingDetails.Metadata, null, token, null, null);

                token = segment.ContinuationToken;

                var blobs = segment.Results.OfType<CloudBlockBlob>();

                // Blobをリストに追加
                foreach (var blob in blobs)
                {
                    blobList.Add(blob.Name);
                }
            } while (token != null);

            return blobList;
        }

        /// <summary>
        /// 指定したコンテナ内のBlobを削除する
        /// </summary>
        /// <param name="connectionString">接続文字列</param>
        /// <param name="containerName">コンテナ名</param>
        /// <returns>非同期処理タスク</returns>
        private static async Task DeleteBlobInContainer(string connectionString, string containerName)
        {
            var _account = CloudStorageAccount.Parse(connectionString);
            var client = _account.CreateCloudBlobClient();

            var container = client.GetContainerReference(containerName);

            if (!await container.ExistsAsync())
            {
                return;
            }

            // Blob一覧を取得
            BlobContinuationToken token = null;
            do
            {
                // Blob一覧を取得
                var segment = await container.ListBlobsSegmentedAsync(string.Empty, true, BlobListingDetails.Metadata, null, token, null, null);

                token = segment.ContinuationToken;

                var blobs = segment.Results.OfType<CloudBlockBlob>();

                // Blobを削除
                foreach (var blob in blobs)
                {
                    await blob.DeleteIfExistsAsync();
                }
            } while (token != null);
        }

        #endregion

        /// <summary>
        /// DIを実行する
        /// </summary>
        /// <param name="utcNow">TimeProviderに渡す日時</param>
        /// <param name="serviceLogger">Service用のロガー</param>
        /// <param name="configures">AppSettingsに追加する項目</param>
        private void DependencyInjection(DateTime utcNow, TestLogger<CleanBlobService> serviceLogger, Dictionary<string, string> configures = null)
        {
            TestDiProviderBuilder builder = new TestDiProviderBuilder();

            // Blob
            builder.ServiceCollection.AddTransient<PrimaryBlob>();

            // Polly
            builder.ServiceCollection.AddSingleton(s => new BlobPolly(s.GetService<AppSettings>()));
            builder.ServiceCollection.AddSingleton(s => new DBPolly(s.GetService<AppSettings>()));

            // Logger
            builder.ServiceCollection.AddSingleton<ILogger<CleanBlobService>>(serviceLogger);

            // Controller生成
            builder.ServiceCollection.AddTransient<BlobCleanController>();

            // Repository生成
            builder.ServiceCollection.AddTransient<IDtDeviceFileRepository, DtDeviceFileRepositoryMock>();
            builder.ServiceCollection.AddTransient<IPrimaryRepository, PrimaryBlobRepositoryMock>();

            builder.ServiceCollection.AddSingleton<ITimeProvider>(UnitTestHelper.CreateTimeProvider(utcNow));

            // 追加の設定項目
            if (configures != null)
            {
                builder.AddConfigures(configures);
            }

            _provider = builder.Build();

            // AppSettings
            _appSettings = _provider.GetService<AppSettings>();

            // DateTimeProvider
            _dateTimeProvider = _provider.GetService<ITimeProvider>() as DateTimeProvider;

            // PrimaryBlob
            _primaryBlob = _provider.GetService<PrimaryBlob>();

            // BlobPolly
            _blobPolly = _provider.GetService<BlobPolly>();

            // DBPolly
            _dBPolly = _provider.GetService<DBPolly>();

            // BlobCleanController
            _controller = _provider.GetService<BlobCleanController>();

            // DtDeviceFileRepository 
            _dtDeviceFileRepositoryMock = _provider.GetService<IDtDeviceFileRepository>() as DtDeviceFileRepositoryMock;

            // PrimaryBlobRepository
            _primaryBlobRepositoryMock = _provider.GetService<IPrimaryRepository>() as PrimaryBlobRepositoryMock;
        }

        /// <summary>
        /// ログに出力する情報をまとめたクラス
        /// </summary>
        public class LogInfo
        {
            /// <summary>
            /// ログに出力するコンテナ名
            /// </summary>
            public string ContainerName;

            /// <summary>
            /// ログに出力するファイルパス
            /// </summary>
            public string TargetFilePath;

            /// <summary>
            /// ログに出力する日時情報
            /// </summary>
            public DateTime LogDateTime;
        }

        /// <summary>
        /// Blob情報クラス
        /// </summary>
        public class BlobInfo
        {
            /// <summary>
            /// ファイルパス
            /// </summary>
            public string FilePath;

            /// <summary>
            /// コンテナ名
            /// </summary>
            public string ContainerName;
        }
    
        /// <summary>
        /// BlobCleaner実行後の期待するBlob状態を管理するクラス
        /// </summary>
        public class ExpectedStatus
        {
            /// <summary>
            /// コンテナ名
            /// </summary>
            public string ContainerName;

            /// <summary>
            /// パス
            /// </summary>
            public string Path;

            /// <summary>
            /// DBの状態を確認する必要があるか?
            /// </summary>
            public bool IsDBCheckNeeded;

            /// <summary>
            /// DBから当該Blobの情報が削除されたか?
            /// </summary>
            public bool IsDeletedFromDB;

            /// <summary>
            /// コンテナの状態を確認する必要があるか?
            /// </summary>
            public bool IsContainerCheckNeeded;

            /// <summary>
            /// コンテナから当該Blobが削除されたか?
            /// </summary>
            public bool IsDeletedFromContainer;
        }

        #region Mockクラス

        /// <summary>
        /// DtDeviceFileRepository Mockクラス
        /// </summary>
        public class DtDeviceFileRepositoryMock : IDtDeviceFileRepository
        {
            /// <summary>
            /// 例外処理を特定のタイミングで行うメソッドについて、メソッドの何回目の呼び出しかを示すインデックス
            /// </summary>
            private static int _currentIndex = 1;

            /// <summary>
            /// 例外処理を特定のタイミングで行うメソッドについて、
            /// 何回目の呼び出しで例外処理を起こすか示すインデックス
            /// </summary>
            private static int _index = 1;

            /// <summary>
            /// 例外処理実行フラグ
            /// DeleteDtDeviceFile用
            /// </summary>
            /// <remarks>
            /// 例外処理を起こす場合にはtrue、正規の挙動を起こす場合にはfalse
            /// </remarks>
            private static bool _failedToDeleteDtDeviceFile = false;

            /// <summary>
            /// 正規の処理を実行する場合に参照するリポジトリ
            /// </summary>
            private static DtDeviceFileRepository _repo = null;

            public void Init(int index, bool failedToDeleteDtDeviceFile)
            {
                // 例外発生させるか?
                _failedToDeleteDtDeviceFile = failedToDeleteDtDeviceFile;

                // 現在のインデックスを初期化
                _currentIndex = 1;

                // 例外を発生させるインデックス（何回目にメソッドが呼ばれたときに例外を発生させるか）
                _index = index;

                // リポジトリDI
                var logger = new TestLogger<DtDeviceFileRepository>(_dtDeviceFileActualLogs);
                _repo = new DtDeviceFileRepository(logger, _dateTimeProvider, _dBPolly, _appSettings);
            }

            /// <summary>
            /// 端末ファイルデータをテーブルに登録する
            /// </summary>
            /// <param name="inData">登録するデータ</param>
            /// <returns>処理結果</returns>

            public DtDeviceFile CreateOrUpdateDtDeviceFile(DtDeviceFile inData)
            {
                return _repo.CreateOrUpdateDtDeviceFile(inData);
            }

            /// <summary>
            /// 端末ファイルデータを取得する
            /// </summary>
            /// <param name="sid">端末SID</param>
            /// <returns>処理結果</returns>
            public DtDeviceFile ReadDtDeviceFile(long sid)
            {
                return _repo.ReadDtDeviceFile(sid);
            }

            /// <summary>
            /// 端末ファイルデータを削除する
            /// </summary>
            /// <param name="sid">SID</param>
            /// <returns>削除結果</returns>
            /// <remarks>
            /// 指定されたインデックス回目の呼び出し時のみ例外を投げる
            /// </remarks>
            public DtDeviceFile DeleteDtDeviceFile(long sid)
            {
                if (_failedToDeleteDtDeviceFile && _currentIndex == _index)
                {
                    _currentIndex += 1;
                    throw new RmsException("CO_BLC_BLC_005");
                }

                _currentIndex += 1;
                return _repo.DeleteDtDeviceFile(sid);
            }

            /// <summary>
            /// 削除対象ファイル一覧を取得する
            /// </summary>
            /// <param name="containerName">コンテナ名</param>
            /// <param name="path">ファイルパス</param>
            /// <param name="endDateTime">削除対象となるBlobの最終更新日時の閾値</param>
            /// <returns>削除対象ファイルリスト</returns>
            public IEnumerable<DtDeviceFile> FindByFilePathStartingWithAndUpdateDatetimeLessThan(string containerName, string path, DateTime endDateTime)
            {
                return _repo.FindByFilePathStartingWithAndUpdateDatetimeLessThan(containerName, path, endDateTime);
            }
        }

        /// <summary>
        /// PrimaryBlobRepository Mockクラス
        /// </summary>
        public class PrimaryBlobRepositoryMock : IPrimaryRepository
        {
            /// <summary>
            /// 例外処理を特定のタイミングで行うメソッドについて、メソッドの何回目の呼び出しかを示すインデックス
            /// </summary>
            private static int _currentIndex = 1;

            /// <summary>
            /// 例外処理を特定のタイミングで行うメソッドについて、
            /// 何回目の呼び出しで例外処理を起こすか示すインデックス
            /// </summary>
            private static int _index = 1;

            /// <summary>
            /// 例外処理実行フラグ
            /// </summary>
            /// <remarks>
            /// 例外処理を起こす場合にはtrue、正規の挙動を起こす場合にはfalse
            /// </remarks>
            private static bool _failedToDelete = false;

            /// <summary>
            /// 正規の処理を実行する場合に参照するリポジトリ
            /// </summary>
            private static PrimaryBlobRepository _repo = null;

            /// <summary>
            /// 初期化処理
            /// 例外処理を実行するメソッドのフラグと何回目の呼び出しで例外処理を実行するかを設定する
            /// </summary>
            /// <param name="index">例外処理実行インデックス</param>
            /// <param name="failedToDelete">例外処理を起こすか（例外処理を起こす場合にはtrue）</param>
            public void Init(int index, bool failedToDelete)
            {
                // 例外を発生させるか?
                _failedToDelete = failedToDelete;

                // 現在のインデックスを初期化
                _currentIndex = 1;

                // 例外を発生させるインデックス（何回目にメソッドが呼ばれたときに例外を発生させるか）
                _index = index;

                // リポジトリDI
                DBPolly dbPolly = new DBPolly(_appSettings);
                var logger = new TestLogger<PrimaryBlobRepository>(_dtDeviceFileActualLogs);

                _repo = new PrimaryBlobRepository(_appSettings, _primaryBlob, _blobPolly, logger);
            }

            /// <summary>
            /// Blobを削除する
            /// </summary>
            /// <remarks>
            /// 例外処理フラグがtrueの場合には、_index回目の呼び出しの場合のみ例外を投げる
            /// </remarks>
            /// <param name="file">削除するファイル</param>
            public void Delete(ArchiveFile file)
            {
                if (_failedToDelete && _currentIndex == _index)
                {
                    _currentIndex += 1;
                    throw new RmsException("CO_BLC_BLC_006");
                }

                _currentIndex += 1;
                _repo.Delete(file);
            }

            #endregion
        }
    }
}
