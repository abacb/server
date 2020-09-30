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
using Rms.Server.Core.Azure.Functions.BlobIndexer;
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

namespace Azure.Functions.BlobIndexerTest
{
    /// <summary>
    /// BlobIndexer自動テスト
    /// </summary>
    [TestClass]
    public class IndexBlobControllerTest
    {
        /// <summary>
        /// 日時データ文字列のフォーマット定義
        /// </summary>
        private static readonly string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fff";

        /// <summary>
        /// テスト対象コンテナ名一覧（PrimaryBlob）
        /// </summary>
        private static readonly string[] PrimaryBlobContainerNameList = new string[] { "log", "error", "device" };

        /// <summary>
        /// テスト対象コンテナ名一覧（CollectingBlob）
        /// </summary>
        private static readonly string[] CollectingBlobContainerNameList = new string[]
        {
            "collect", "unknown", "changedcollect", "changedunknown"
        };

        /// <summary>
        /// BlobCleanController
        /// </summary>
        private static IndexBlobController _controller = null;

        /// <summary>
        /// ServiceProvider
        /// </summary>
        private static ServiceProvider _provider = null;

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
        /// CollectingBlob
        /// </summary>
        private static CollectingBlob _collectingBlob;

        /// <summary>
        /// PrimaryBlobRepositoryMock
        /// </summary>
        private static PrimaryBlobRepositoryMock _primaryBlobRepositoryMock = null;

        /// <summary>
        /// CollectingBlobRepositoryMock
        /// </summary>
        private static CollectingBlobRepositoryMock _collectingBlobRepositoryMock;

        /// <summary>
        /// DtDeviceFileRepositoryMock
        /// </summary>
        private static DtDeviceFileRepositoryMock _dtDeviceFileRepositoryMock;

        /// <summary>
        /// DateTimeProvider
        /// </summary>
        private static DateTimeProvider _dateTimeProvider = null;

        /// <summary>
        /// ロガー
        /// </summary>
        private static TestLogger<IndexBlobService> _serviceLogger = null;

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="context">コンテキスト</param>
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            // 関連DBデータを全削除
            DbTestHelper.DeleteAllReseed();

            // DB設定
            // マスタテーブルデータを作成する
            DbTestHelper.ExecSqlFromFilePath(@"TestData\Sqls\MakeMasterTableData.sql");
        }

        /// <summary>
        /// 後処理
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanup()
        {
            // 関連DBデータを全削除
            DbTestHelper.DeleteAllReseed();
        }

        /// <summary>
        /// BlobIndexer自動テスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_TestDateTime">テスト実施日：この値を基準にDBに設定される作成・更新日時が決まるので注意</param>
        /// <param name="in_AppSettings">AppSettings（追加または既存の設定値を更新）</param>
        /// <param name="in_InsertNewDataSqlPath">SQL文を記載したファイルパス（追加でDBに設定が必要な場合に使用）</param>
        /// <param name="in_UploadedBlobInfo">コンテナにアップロードするファイルの情報をまとめたCSVファイルのパス</param>
        /// <param name="expected_BlobStatusPath">BlobIndexer実行後のコンテナ内Blob状態を記述したCSVファイルのパス</param>
        /// <param name="expected_DBStatusPath">BlobIndexer実行後のDB状態を記述したJSONファイルのパス</param>
        /// <param name="expected_BlobContents">期待するBlobファイルの内容</param>
        /// <param name="expected_LogResource">期待するログリソース（エラー番号）</param>
        /// <param name="expected_LogInfo">期待するログに出力される情報をまとめたCSVファイルのパス</param>
        /// <param name="expected_CheckNotAddedDeviceFile">端末ファイル属性データテーブルにデータ追加がなかったことを確認する場合にはtrueにする</param>
        /// <param name="error_Copy">CollectingBlobRepositoryMockクラスのCopyメソッドで例外を発生させる場合はtrueにする</param>
        /// <param name="error_PrimaryDelete">PrimaryBlobRepositoryMockクラスのDeleteメソッドで例外を発生させる場合はtrueにする</param>
        /// <param name="error_CollectingDelete">CollectingBlobRepositoryMockクラスのDeleteメソッドで例外を発生させる場合はtrueにする</param>
        /// <param name="error_CreateDtDeviceFile">DtDeviceFileRepositoryMockクラスのCreateDtDeviceFileメソッドで例外を発生させる場合はtrueにする</param>
        /// <param name="error_Index">IndexBlobServiceMockクラスのIndexメソッドで例外を発生させる場合にはtrueにする</param>
        /// <param name="remark">備考</param>
        /// <returns>非同期処理タスク</returns>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\Controllers_IndexBlobContoroller.csv")]
        public async Task IndexBlobTest(
            string no,
            string in_TestDateTime,
            string in_AppSettings,
            string in_InsertNewDataSqlPath,
            string in_UploadedBlobInfo,
            string expected_BlobStatusPath,
            string expected_DBStatusPath,
            string expected_BlobContents,
            string expected_LogResource,
            string expected_LogInfo,
            string expected_CheckNotAddedDeviceFile,
            string error_Copy,
            string error_PrimaryDelete,
            string error_CollectingDelete,
            string error_CreateDtDeviceFile,
            string error_Index,
            string remark)
        {
            // AppSettings
            Dictionary<string, string> appSettingsConfigures = null;
            if (!string.IsNullOrEmpty(in_AppSettings))
            {
                appSettingsConfigures = JsonConvert.DeserializeObject<Dictionary<string, string>>(in_AppSettings);
            }

            // DBに端末ファイル情報が追加されていないことを確認する
            bool.TryParse(expected_CheckNotAddedDeviceFile, out bool isCheckedDeviceFileNotAdded);

            // 例外処理を発生させるメソッドを示すフラグ
            bool.TryParse(error_Copy, out bool isErrorOnCopy);
            bool.TryParse(error_PrimaryDelete, out bool isErrorOnPrimaryDelete);
            bool.TryParse(error_CollectingDelete, out bool isErrorOnCollectingDelete);
            bool.TryParse(error_CreateDtDeviceFile, out bool isErrorCreateDtDeviceFile);
            bool.TryParse(error_Index, out bool isErrorOnIndex);

            // ログ格納用ロガーの用意
            // Controller用
            var actualControllerLogs = new List<TestLog>();
            var logger = new TestLogger<IndexBlobController>(actualControllerLogs);

            // Service用
            var actualServiceLogs = new List<TestLog>();
            _serviceLogger = new TestLogger<IndexBlobService>(actualServiceLogs);

            // テスト実行日時（ファイル削除閾値の基準となる値）
            DateTime testDateTime = ConvertStringToDateTime(in_TestDateTime);

            // DI
            DependencyInjection(testDateTime, _serviceLogger, isErrorOnIndex, appSettingsConfigures);

            // DI後にRepositoryの例外発生設定を行う
            _dtDeviceFileRepositoryMock.Init(isErrorCreateDtDeviceFile);
            _primaryBlobRepositoryMock.Init(isErrorOnPrimaryDelete);
            _collectingBlobRepositoryMock.Init(isErrorOnCopy, isErrorOnCollectingDelete);

            // テスト用データをDBに挿入する
            // 端末データテーブルデータを作成する（前回のデータをクリアする機能も兼ねている）
            DbTestHelper.ExecSqlFromFilePath(@"TestData\Sqls\MakeDeviceData.sql");

            // 端末ファイルデータは特定のテストケースでのみ追加する
            if (!string.IsNullOrEmpty(in_InsertNewDataSqlPath))
            {
                DbTestHelper.ExecSqlFromFilePath(in_InsertNewDataSqlPath);
            }

            // PrimaryBlobの接続文字列
            var primaryBlobConnectionString = _appSettings.PrimaryBlobConnectionString;
            var collectingBlobConnectionString = _appSettings.CollectingBlobConnectionString;

            // Blobファイルアップロード
            // テスト用のファイルをCollectingBlobにアップロードする
            if (!string.IsNullOrEmpty(in_UploadedBlobInfo))
            {
                List<BlobInfo> uploadedBlobInfo = GetUploadedBlobInfo(in_UploadedBlobInfo);

                // Blobアップロード処理
                foreach (BlobInfo info in uploadedBlobInfo)
                {
                    // 接続文字列
                    string connectionString = string.Empty;
                    if (PrimaryBlobContainerNameList.Contains(info.ContainerName))
                    {
                        connectionString = primaryBlobConnectionString;
                    }
                    else if (CollectingBlobContainerNameList.Contains(info.ContainerName))
                    {
                        connectionString = collectingBlobConnectionString;
                    }

                    // コンテナ取得
                    CloudBlobContainer container = GetContainer(connectionString, info.ContainerName);

                    // 指定したローカルファイルパスのファイルをアップロード
                    await Upload(container, info.BlobPath, info.LocalFilePath, info.MetaData);
                }
            }

            // BlobIndexer実行
            {
                Rms.Server.Core.Utility.Assert.IfNull(_controller);
                _controller.IndexBlob(null, logger);
            }

            // テスト結果フラグ
            bool isDatabaseStatusMatched = true;
            bool isDeviceFileNotAdded = true;
            bool isBlobStatusMatched = true;
            bool isBlobUpdated = true;
            bool isExpectedLogContained = false;

            // DB状態確認
            if (!string.IsNullOrEmpty(expected_DBStatusPath))
            {
                // 期待するDBの状態を記述したJSONファイルを読み出してデシリアライズ
                string inputJson;

                StreamReader sr = new StreamReader(expected_DBStatusPath);
                inputJson = sr.ReadToEnd();
                sr.Close();

                ExpectedDeviceFile[] expectedDeviceFiles = JsonConvert.DeserializeObject<ExpectedDeviceFile[]>(inputJson);

                // 端末ファイル数
                int countDeviceFiles = expectedDeviceFiles.Length;

                // DBに格納されている結果をリスト化する
                DtDeviceFile[] actualDeviceFiles = new DtDeviceFile[countDeviceFiles];

                // DBの内容を読み出す
                for (int i = 0; i < countDeviceFiles; i++)
                {
                    var model = _dtDeviceFileRepositoryMock.ReadDtDeviceFile(i + 1);    // SIDは1から始まる
                    actualDeviceFiles[i] = model;
                }

                // 期待する結果だったレコードの数をカウントする
                int expectedRecordFoundCount = 0;

                // DBと比較する
                foreach (var expected in expectedDeviceFiles)
                {
                    foreach (var actual in actualDeviceFiles)
                    {
                        if (actual == null)
                        {
                            continue;
                        }

                        // ファイルパスとコンテナ名で対象レコードかどうか確認する
                        // 端末ファイル情報はコンテナ名とパスの組み合わせで一意性が決まる
                        if (!expected.Path.Equals(actual.FilePath) || !expected.ContainerName.Equals(actual.Container))
                        {
                            continue;
                        }

                        if (expected.CreatedTime == actual.CreateDatetime)
                        {
                            // 端末ファイル属性情報の比較を行う
                            var actualAttributes = actual.DtDeviceFileAttribute;
                            var expectedAttributes = expected.DeviceFileAttributes;

                            int attributesMatchCount = 0;

                            // Name-Valueのペア数が一致するか確認する
                            if (expectedAttributes.Length != actualAttributes.Count())
                            {
                                break;
                            }

                            // Name-Valueがすべて一致するか確認する
                            foreach (var expectedAttribute in expectedAttributes)
                            {
                                foreach (var actualAttribute in actualAttributes)
                                {
                                    if (actualAttribute.Name.Equals(expectedAttribute.Name)
                                        && actualAttribute.Value.Equals(expectedAttribute.Value))
                                    {
                                        attributesMatchCount += 1;
                                        break;
                                    }
                                }
                            }

                            // 端末ファイル情報の比較結果がtrueかつ端末ファイル属性情報がすべてDBに格納されていた
                            if (attributesMatchCount == expectedAttributes.Length)
                            {
                                expectedRecordFoundCount += 1;
                                break;
                            }
                        }
                    }
                }

                // 一致するレコードの数が期待するレコードの数と一致すればDBの状態は正しい
                if (expectedRecordFoundCount != expectedDeviceFiles.Length)
                {
                    isDatabaseStatusMatched = false;
                }
            }

            // DB確認（端末ファイルデータが追加されていないことのみ確認）
            if (isCheckedDeviceFileNotAdded)
            {
                var model = _dtDeviceFileRepositoryMock.ReadDtDeviceFile(1);

                // 端末ファイルデータテーブルの先頭にレコードが存在する場合、
                // データが追加されたことになるのでNG
                if (model != null)
                {
                    isDeviceFileNotAdded = false;
                }
            }

            // Blob状態確認
            if (!string.IsNullOrEmpty(expected_BlobStatusPath))
            {
                var expectedBlobStatus = GetExpectedBlobInfoList(expected_BlobStatusPath);

                foreach (var status in expectedBlobStatus)
                {
                    string connectionString;
                    if (PrimaryBlobContainerNameList.Contains(status.ContainerName))
                    {
                        connectionString = primaryBlobConnectionString;
                    }
                    else
                    {
                        connectionString = collectingBlobConnectionString;
                    }

                    var actualBlobs = await GetBlobList(connectionString, status.ContainerName);

                    bool contains = ContainsExpectedBlobInfo(actualBlobs, status);
                    if (status.Deleted == contains)
                    {
                        // 削除フラグtrueであればBlobがコンテナに存在しないことが期待される
                        isBlobStatusMatched = false;
                        break;
                    }
                }
            }

            // Blob内容確認
            if (!string.IsNullOrEmpty(expected_BlobContents))
            {
                // 情報を管理クラスに格納
                ExpectedBlobContentsInfo info = JsonConvert.DeserializeObject<ExpectedBlobContentsInfo>(expected_BlobContents);

                // コンテナからBlob取得
                string connectionString = PrimaryBlobContainerNameList.Contains(info.ContainerName) ? primaryBlobConnectionString : collectingBlobConnectionString;
                var container = GetContainer(connectionString, info.ContainerName);
                var blob = container.GetBlockBlobReference(info.Path);
                ExpectedBlobContents actualBlobContent = await GetBlobContents(blob);

                // 期待した内容になっているかどうかを確認する
                if (!info.Contents.FileName.Equals(actualBlobContent.FileName) || !info.Contents.No.Equals(actualBlobContent.No))
                {
                    // 内容が一致してなければNG
                    isBlobUpdated = false;
                }
            }
            
            // ログの確認
            if (!string.IsNullOrEmpty(expected_LogInfo))
            {
                // 期待するログ
                var logInfo = GetLogInfo(expected_LogInfo);
                List<string> expectedLogMessages = GetLogMessages(expected_LogResource, logInfo);

                int expectedLogMessagesCount = expectedLogMessages.Count;
                int countMatchLogs = 0;

                foreach (string expectedLog in expectedLogMessages)
                {
                    // Controller
                    foreach (TestLog eachLog in actualControllerLogs)
                    {
                        string actualLog = eachLog.GetSimpleText();
                        if (actualLog.Contains(expectedLog))
                        {
                            // 期待していたログが見つかった
                            countMatchLogs += 1;
                            break;
                        }
                    }

                    // Service
                    foreach (TestLog eachLog in actualServiceLogs)
                    {
                        string actualLog = eachLog.GetSimpleText();
                        if (actualLog.Contains(expectedLog))
                        {
                            // 期待していたログが見つかった
                            countMatchLogs += 1;
                            break;
                        }
                    }

                    // 期待するログがすべて見つかればフラグを立てる
                    if (countMatchLogs == expectedLogMessagesCount)
                    {
                        isExpectedLogContained = true;
                    }
                }
            }
            else
            {
                // ログを確認しないテストの場合は強制的にtrueにする
                isExpectedLogContained = true;
            }

            // Blob削除（コンテナは削除しない）: テストケース1回ごとにBlobを削除する
            // 期待するBlob状態リストに記載されているBlobのみを選択的に削除する
            if (!string.IsNullOrEmpty(expected_BlobStatusPath))
            {
                var expectedBlobList = GetExpectedBlobInfoList(expected_BlobStatusPath);
                
                foreach (var expectedBlob in expectedBlobList)
                {
                    // 削除対象Blobを取得
                    string connectionString
                        = PrimaryBlobContainerNameList.Contains(expectedBlob.ContainerName) 
                            ? primaryBlobConnectionString : collectingBlobConnectionString;

                    var container = GetContainer(connectionString, expectedBlob.ContainerName);
                    var blob = container.GetBlockBlobReference(expectedBlob.BlobPath);

                    try
                    {
                        await blob.DeleteIfExistsAsync();
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            // 自動生成される異常集積コンテナを削除する
            // テストとは関係ないため例外は無視する
            try
            {
                await DeleteContainer(collectingBlobConnectionString, "changedunknown");
            }
            catch (Exception)
            {
            }

            // テスト結果：Blob削除後に確認する
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(true, isDatabaseStatusMatched);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(true, isDeviceFileNotAdded);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(true, isBlobStatusMatched);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(true, isBlobUpdated);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(true, isExpectedLogContained);
        }

        #region Blob操作

        /// <summary>
        /// 指定したストレージアカウントの指定した名称のBlobコンテナを取得する
        /// </summary>
        /// <param name="connectionString">接続文字列</param>
        /// <param name="containerName">コンテナ名</param>
        /// <returns>コンテナ</returns>
        private static CloudBlobContainer GetContainer(string connectionString, string containerName)
        {
            var account = CloudStorageAccount.Parse(connectionString);
            var client = account.CreateCloudBlobClient();
            return client.GetContainerReference(containerName);
        }

        /// <summary>
        /// 指定したコンテナを削除する
        /// </summary>
        /// <param name="connectionString">接続文字列</param>
        /// <param name="containerName">削除対象コンテナ名</param>
        /// <returns>コンテナ削除非同期タスク</returns>
        private static async Task DeleteContainer(string connectionString, string containerName)
        {
            var account = CloudStorageAccount.Parse(connectionString);
            var client = account.CreateCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference(containerName);

            // Delete the specified container and handle the exception.
            await container.DeleteAsync();
        }

        /// <summary>
        /// 指定したローカルパスのファイルをBlobストレージにアップロードする
        /// </summary>
        /// <param name="container">コンテナ</param>
        /// <param name="blobName">Blob名</param>
        /// <param name="localFilePath">ローカルファイルパス</param>
        /// <param name="metaData">Blobに設定するメタデータ</param>
        /// <returns>非同期処理タスク</returns>
        private static async Task Upload(CloudBlobContainer container, string blobName, string localFilePath, Dictionary<string, string> metaData)
        {
            // ローカルのテスト用ファイルをBlobに変換する
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            // メタデータ追加
            foreach (var (key, value) in metaData)
            {
                blob.Metadata.Add(key, Uri.EscapeDataString(value));
            }

            // 指定したローカルファイルパスのファイルをアップロード
            await blob.UploadFromFileAsync(localFilePath);
        }

        /// <summary>
        /// 指定したBlobの内容を取得する
        /// </summary>
        /// <param name="blob">Blob</param>
        /// <returns>Blobファイルの内容（Jsonテキスト）</returns>
        private static async Task<ExpectedBlobContents> GetBlobContents(CloudBlockBlob blob)
        {
            string jsonText;
            using (var memoryStream = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(memoryStream);
                jsonText = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            return JsonConvert.DeserializeObject<ExpectedBlobContents>(jsonText);
        }

        /// <summary>
        /// 指定したストレージアカウントの指定したコンテナ内のBlobリストを取得する
        /// </summary>
        /// <param name="connectionString">接続文字列</param>
        /// <param name="containerName">コンテナ名</param>
        /// <returns>Blobリスト</returns>
        private static async Task<List<CloudBlob>> GetBlobList(string connectionString, string containerName)
        {
            List<CloudBlob> blobList = new List<CloudBlob>();

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
                    blobList.Add(blob);
                }
            }
            while (token != null);

            return blobList;
        }

        #endregion

        /// <summary>
        /// DI
        /// </summary>
        /// <param name="utcNow">テスト実施日時</param>
        /// <param name="serviceLogger">DIするService用のロガー</param>
        /// <param name="isErrorOnIndex">Serviceのメソッドで例外を発生させるか?</param>
        /// <param name="configures">AppSettingsに追加する設定</param>
        private void DependencyInjection(
            DateTime utcNow, 
            TestLogger<IndexBlobService> serviceLogger, 
            bool isErrorOnIndex = false,
            Dictionary<string, string> configures = null)
        {
            TestDiProviderBuilder builder = new TestDiProviderBuilder();

            // Blob
            builder.ServiceCollection.AddTransient<PrimaryBlob>();
            builder.ServiceCollection.AddTransient<CollectingBlob>();

            // Polly
            builder.ServiceCollection.AddSingleton(s => new BlobPolly(s.GetService<AppSettings>()));
            builder.ServiceCollection.AddSingleton(s => new DBPolly(s.GetService<AppSettings>()));

            // Logger
            builder.ServiceCollection.AddSingleton<ILogger<IndexBlobService>>(serviceLogger);

            // Service
            if (isErrorOnIndex)
            {
                builder.ServiceCollection.AddTransient<IIndexBlobService, IndexBlobServiceMock>();
            }

            // Controller生成
            builder.ServiceCollection.AddTransient<IndexBlobController>();

            // Repository生成
            builder.ServiceCollection.AddTransient<IPrimaryRepository, PrimaryBlobRepositoryMock>();
            builder.ServiceCollection.AddTransient<ICollectingRepository, CollectingBlobRepositoryMock>();
            builder.ServiceCollection.AddTransient<IDtDeviceFileRepository, DtDeviceFileRepositoryMock>();

            // TimeProvider
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

            // Primary & CollectingBlob
            _primaryBlob = _provider.GetService<PrimaryBlob>();
            _collectingBlob = _provider.GetService<CollectingBlob>();

            // DBPolly
            _dBPolly = _provider.GetService<DBPolly>();

            // BlobPolly
            _blobPolly = _provider.GetService<BlobPolly>();

            // BlobCleanController
            _controller = _provider.GetService<IndexBlobController>();

            // PrimaryBlobRepositoryMock
            _primaryBlobRepositoryMock = _provider.GetService<IPrimaryRepository>() as PrimaryBlobRepositoryMock;

            // CollectingBlobRepositoryMock
            _collectingBlobRepositoryMock = _provider.GetService<ICollectingRepository>() as CollectingBlobRepositoryMock;

            // DtDeviceFileRepositoryMock
            _dtDeviceFileRepositoryMock = _provider.GetService<IDtDeviceFileRepository>() as DtDeviceFileRepositoryMock;
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
        /// 文字列をDateTime型に変換する
        /// </summary>
        /// <param name="dateTime">文字列</param>
        /// <returns>DateTime</returns>
        private DateTime ConvertStringToDateTime(string dateTime)
        {
            return string.IsNullOrEmpty(dateTime) ? default(DateTime) : DateTime.ParseExact(dateTime, DateTimeFormat, null);
        }

        #region Log

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
                if (columns.Length == 2)
                {
                    string containerName = columns[0];
                    string targetFilePath = columns[1];

                    LogInfo info = new LogInfo
                    {
                        ContainerName = containerName,
                        Path = targetFilePath
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
                    Rms.Server.Core.Utility.Extensions.LoggerExtensions.Error(
                        logger,
                        new RmsException(errorCode), 
                        errorCode, 
                        new object[] { info.ContainerName, info.Path });
                }

                foreach (var log in actualLogs)
                {
                    result.Add(log.GetSimpleText());
                }
            }

            return result;
        }

        #endregion

        /// <summary>
        /// 期待するBlob状態がBlobリスト内に存在するか確認する
        /// </summary>
        /// <param name="actualBlobs">コンテナに格納されたBlobリスト</param>
        /// <param name="expectedBlobInfo">期待するBlob状態</param>
        /// <returns>期待するBlob状態がBlobリスト内に存在すればtrueを返す</returns>
        private bool ContainsExpectedBlobInfo(List<CloudBlob> actualBlobs, ExpectedBlobInfo expectedBlobInfo)
        {
            bool result = false;

            foreach (var blob in actualBlobs)
            {
                string path = blob.Name;
                var metadata = blob.Metadata;

                // パス確認
                if (!path.Equals(expectedBlobInfo.BlobPath))
                {
                    // パスが一致しないのであれば次の要素をチェック
                    continue;
                }

                // パスが一致した要素についてメタデータを確認
                bool allKeyValuePairMatches = true;

                foreach (var (expectedKey, expectedValue) in expectedBlobInfo.Metadata)
                {
                    if (!metadata.ContainsKey(expectedKey))
                    {
                        // メタデータのキーが一致しないのでNG
                        allKeyValuePairMatches = false;
                        break;
                    }

                    string actualValue = Uri.UnescapeDataString(metadata[expectedKey]);

                    if (!actualValue.Equals(expectedValue))
                    {
                        // メタデータのキーと値が一致しないのでNG
                        allKeyValuePairMatches = false;
                        break;
                    }
                }

                if (allKeyValuePairMatches)
                {
                    // NGが無かった場合には、パスとメタデータが完全に一致したので
                    // 指定したBlobがコンテナに存在したと判定
                    result = true;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// アップロードするBlobの情報を設定ファイルから取得する
        /// </summary>
        /// <param name="csvPath">設定ファイル</param>
        /// <returns>アップロードするBlob情報リスト</returns>
        private List<BlobInfo> GetUploadedBlobInfo(string csvPath)
        {
            List<BlobInfo> result = new List<BlobInfo>();

            if (string.IsNullOrEmpty(csvPath))
            {
                return result;
            }

            // CSVファイル読み込み
            List<string> lines = GetLinesFromTextFile(csvPath);
            foreach (string line in lines)
            {
                string[] columns = line.Split(",");
                if (columns.Length == 9)
                {
                    string containerName = columns[0];
                    string localPath = columns[1];
                    string blobPath = columns[2];

                    string sys_container = columns[3];
                    string sys_owner = columns[4];
                    string sys_sub_directory = columns[5];
                    string sys_file_datetime = columns[6];
                    string user = columns[7];
                    string user2 = columns[8];

                    BlobInfo status = new BlobInfo();
                    Dictionary<string, string> metaData = new Dictionary<string, string>();

                    if (!string.IsNullOrEmpty(sys_container))
                    {
                        metaData["sys_container"] = sys_container;
                    }

                    if (!string.IsNullOrEmpty(sys_owner))
                    {
                        metaData["sys_owner"] = sys_owner;
                    }

                    if (!string.IsNullOrEmpty(sys_sub_directory))
                    {
                        metaData["sys_sub_directory"] = sys_sub_directory;
                    }

                    if (!string.IsNullOrEmpty(sys_file_datetime))
                    {
                        metaData["sys_file_datetime"] = sys_file_datetime;
                    }

                    if (!string.IsNullOrEmpty(user))
                    {
                        metaData["user"] = user;
                    }

                    if (!string.IsNullOrEmpty(user2))
                    {
                        metaData["user2"] = user2;
                    }

                    status.ContainerName = containerName;
                    status.LocalFilePath = localPath;
                    status.BlobPath = blobPath;
                    status.MetaData = metaData;

                    result.Add(status);
                }
            }

            return result;
        }

        /// <summary>
        /// 期待するBlob情報のリストを設定ファイルから取得する
        /// </summary>
        /// <param name="expectedBlobStatusCsvPath">設定ファイル</param>
        /// <returns>期待するBlob情報リスト</returns>
        private List<ExpectedBlobInfo> GetExpectedBlobInfoList(string expectedBlobStatusCsvPath)
        {
            List<ExpectedBlobInfo> infoList = new List<ExpectedBlobInfo>();

            if (string.IsNullOrEmpty(expectedBlobStatusCsvPath))
            {
                return infoList;
            }

            // CSVファイル読み込み
            List<string> lines = GetLinesFromTextFile(expectedBlobStatusCsvPath);
            foreach (string line in lines)
            {
                string[] columns = line.Split(",");
                if (columns.Length == 9)
                {
                    string containerName = columns[0];
                    string path = columns[1];
                    string deleted = columns[2];
                    string sys_container = columns[3];
                    string sys_owner = columns[4];
                    string sys_sub_directory = columns[5];
                    string sys_file_datetime = columns[6];
                    string user = columns[7];
                    string user2 = columns[8];

                    Dictionary<string, string> metadata = new Dictionary<string, string>();
                    if (!string.IsNullOrEmpty(sys_container))
                    {
                        metadata.Add("sys_container", sys_container);
                    }

                    if (!string.IsNullOrEmpty(sys_owner))
                    {
                        metadata.Add("sys_owner", sys_owner);
                    }

                    if (!string.IsNullOrEmpty(sys_sub_directory))
                    {
                        metadata.Add("sys_sub_directory", sys_sub_directory);
                    }

                    if (!string.IsNullOrEmpty(sys_file_datetime))
                    {
                        metadata.Add("sys_file_datetime", sys_file_datetime);
                    }

                    if (!string.IsNullOrEmpty(user))
                    {
                        metadata.Add("user", user);
                    }

                    if (!string.IsNullOrEmpty(user2))
                    {
                        metadata.Add("user2", user2);
                    }

                    bool.TryParse(deleted, out bool isDeleted);

                    ExpectedBlobInfo info = new ExpectedBlobInfo
                    {
                        ContainerName = containerName,
                        BlobPath = path,
                        Deleted = isDeleted,
                        Metadata = metadata
                    };

                    infoList.Add(info);
                }
            }

            return infoList;
        }

        #region テスト用情報管理クラス

        /// <summary>
        /// アップロードするBlobの情報を管理するクラス
        /// </summary>
        public class BlobInfo
        {
            /// <summary>
            /// コンテナ名
            /// </summary>
            public string ContainerName { get; set; }

            /// <summary>
            /// アップロード用テストデータのローカルファイルパス
            /// </summary>
            public string LocalFilePath { get; set; }

            /// <summary>
            /// アップロード先コンテナ上のBlobパス
            /// </summary>
            public string BlobPath { get; set; }

            /// <summary>
            /// メタデータ
            /// </summary>
            public Dictionary<string, string> MetaData { get; set; }
        }

        /// <summary>
        /// コンテナに格納されていることが期待されるBlobの状態を管理するクラス
        /// </summary>
        public class ExpectedBlobInfo
        {
            /// <summary>
            /// 対象Blobが削除されていることを確認するか?
            /// </summary>
            public bool Deleted { get; set; }

            /// <summary>
            /// コンテナ名
            /// </summary>
            public string ContainerName { get; set; }

            /// <summary>
            /// コンテナ上のBlobパス
            /// </summary>
            public string BlobPath { get; set; }

            /// <summary>
            /// メタデータ
            /// </summary>
            public Dictionary<string, string> Metadata { get; set; }
        }

        /// <summary>
        /// Blobの内容
        /// </summary>
        public class ExpectedBlobContents
        {
            /// <summary>
            /// テスト番号
            /// </summary>
            public string No { get; set; }

            /// <summary>
            /// ファイル名
            /// </summary>
            public string FileName { get; set; }
        }

        /// <summary>
        /// 更新が行われたことを確認するBlobファイルの情報とファイルの内容をまとめたクラス
        /// </summary>
        public class ExpectedBlobContentsInfo
        {
            /// <summary>
            /// Blobの内容
            /// </summary>
            public ExpectedBlobContents Contents { get; set; }

            /// <summary>
            /// コンテナ名
            /// </summary>
            public string ContainerName { get; set; }

            /// <summary>
            /// Blobパス
            /// </summary>
            public string Path { get; set; }
        }

        /// <summary>
        /// 期待する端末ファイルデータテーブルの状態を管理するクラス
        /// </summary>
        public class ExpectedDeviceFile
        {
            /// <summary>
            /// コンテナ名
            /// </summary>
            public string ContainerName { get; set; }

            /// <summary>
            /// Blobパス
            /// </summary>
            public string Path { get; set; }

            /// <summary>
            /// 更新日時
            /// </summary>
            public DateTime? UpdateTime { get; set; }

            /// <summary>
            /// 作成日時
            /// </summary>
            public DateTime? CreatedTime { get; set; }

            /// <summary>
            /// ファイル属性データ
            /// </summary>
            public ExpectedDeviceFileAttribute[] DeviceFileAttributes { get; set; }
        }

        /// <summary>
        /// 期待する端末ファイル属性データテーブルの状態を管理するクラス
        /// </summary>
        public class ExpectedDeviceFileAttribute
        {
            /// <summary>
            /// Name
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Value
            /// </summary>
            public string Value { get; set; }

            /// <summary>
            /// 更新日時
            /// </summary>
            public DateTime? UpdateTime { get; set; }

            /// <summary>
            /// 作成日時
            /// </summary>
            public DateTime? CreatedTime { get; set; }
        }

        /// <summary>
        /// ログに出力する情報をまとめたクラス
        /// </summary>
        public class LogInfo
        {
            /// <summary>
            /// ログに出力するコンテナ名
            /// </summary>
            public string ContainerName { get; set; }

            /// <summary>
            /// ログに出力するファイルパス
            /// </summary>
            public string Path { get; set; }
        }

        #endregion

        #region Mockクラス

        /// <summary>
        /// IndexBlobServiceのモッククラス（例外発生用）
        /// </summary>
        public class IndexBlobServiceMock : IIndexBlobService
        {
            /// <summary>
            /// Serviceのメソッドを強制的に例外にする
            /// </summary>
            public void Index()
            {
                throw new RmsException("IndexBlobControllerTest");
            }
        }

        /// <summary>
        /// DtDeviceFileRepository Mockクラス
        /// </summary>
        public class DtDeviceFileRepositoryMock : IDtDeviceFileRepository
        {
            /// <summary>
            /// Logリスト
            /// </summary>
            private static readonly List<TestLog> DtDeviceFileActualLogs = new List<TestLog>();

            /// <summary>
            /// 例外処理実行フラグ
            /// CreateDtDeviceFile用
            /// </summary>
            /// <remarks>
            /// 例外処理を起こす場合にはtrue、正規の挙動を起こす場合にはfalse
            /// </remarks>
            private static bool _failedToCreateDtDeviceFile = false;

            /// <summary>
            /// 正規の処理を実行する場合に参照するリポジトリ
            /// </summary>
            private static DtDeviceFileRepository _repo = null;

            /// <summary>
            /// 初期化処理 : 例外発生設定を行う
            /// </summary>
            /// <param name="failedToCreateDtDeviceFile">例外発生フラグ</param>
            public void Init(bool failedToCreateDtDeviceFile)
            {
                // 例外発生させるか?
                _failedToCreateDtDeviceFile = failedToCreateDtDeviceFile;

                // リポジトリDI
                var logger = new TestLogger<DtDeviceFileRepository>(DtDeviceFileActualLogs);
                _repo = new DtDeviceFileRepository(logger, _dateTimeProvider, _dBPolly, _appSettings);
            }

            /// <summary>
            /// 引数に指定したDtDeviceFileをDT_DEVICE_FILEテーブルへ登録する
            /// </summary>
            /// <param name="inData">登録するデータ</param>
            /// <returns>処理結果</returns>
            public DtDeviceFile CreateOrUpdateDtDeviceFile(DtDeviceFile inData)
            {
                if (_failedToCreateDtDeviceFile)
                {
                    throw new RmsException("IndexBlobControllerTest");
                }

                return _repo.CreateOrUpdateDtDeviceFile(inData);
            }

            /// <summary>
            /// DT_DEVICE_FILEテーブルからDtDeviceFileを取得する
            /// </summary>
            /// <param name="sid">取得するデータのSID</param>
            /// <returns>取得したデータ</returns>
            public DtDeviceFile ReadDtDeviceFile(long sid)
            {
                return _repo.ReadDtDeviceFile(sid);
            }

            /// <summary>
            /// テーブルからDtDeviceFileを削除する
            /// </summary>
            /// <param name="sid">削除するデータのSID</param>
            /// <returns>削除したデータ</returns>
            public DtDeviceFile DeleteDtDeviceFile(long sid)
            {
                return _repo.DeleteDtDeviceFile(sid);
            }

            /// <summary>
            /// 引数に指定したパスに、ファイルパスが先頭一致するDtDeviceFileを取得する
            /// </summary>
            /// <param name="containerName">コンテナ名</param>
            /// <param name="path">パス。指定したパスに先頭一致するDtDeviceFileを取得する。</param>
            /// <param name="endDateTime">期間(終了)</param>
            /// <returns>DtDeviceFileのリスト</returns>
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
            /// ロガー
            /// </summary>
            private static readonly List<TestLog> TestLogList = new List<TestLog>();

            /// <summary>
            /// 例外処理実行フラグ
            /// </summary>
            private static bool _failedToDelete = false;

            /// <summary>
            /// 正規の処理を実行する場合に参照するリポジトリ
            /// </summary>
            private static PrimaryBlobRepository _repo = null;

            /// <summary>
            /// 初期化処理
            /// 例外処理を実行するメソッドのフラグを設定する
            /// </summary>
            /// <param name="failedToDelete">例外処理を起こすか（例外処理を起こす場合にはtrue）</param>
            public void Init(bool failedToDelete)
            {
                // 例外を発生させるか?
                _failedToDelete = failedToDelete;

                // リポジトリDI
                DBPolly dbPolly = new DBPolly(_appSettings);
                var logger = new TestLogger<PrimaryBlobRepository>(TestLogList);

                _repo = new PrimaryBlobRepository(_appSettings, _primaryBlob, _blobPolly, logger);
            }

            /// <summary>
            /// Blobを削除する
            /// </summary>
            /// <param name="file">削除するファイル</param>
            public void Delete(ArchiveFile file)
            {
                if (_failedToDelete)
                {
                    throw new RmsException("IndexBlobControllerTest");
                }

                _repo.Delete(file);
            }
        }

        /// <summary>
        /// CollectingBlobRepository Mockクラス
        /// </summary>
        public class CollectingBlobRepositoryMock : ICollectingRepository
        {
            /// <summary>
            /// ロガー
            /// </summary>
            private static readonly List<TestLog> TestLogList = new List<TestLog>();

            /// <summary>
            /// Copyメソッドで例外を発生させるか?
            /// </summary>
            private static bool _isErrorOnCopy = false;

            /// <summary>
            /// Deleteメソッドで例外を発生させるか?
            /// </summary>
            private static bool _isErrorOnDelete = false;

            /// <summary>
            /// 正規処理実行用のリポジトリインスタンス
            /// </summary>
            private static CollectingBlobRepository _repo = null;

            /// <summary>
            /// 初期化処理 : 例外発生設定を行う
            /// </summary>
            /// <param name="isErrorOnCopy">Copyメソッドで例外を発生させるか?</param>
            /// <param name="isErrorOnDelete">Deleteメソッドで例外を発生させるか?</param>
            public void Init(bool isErrorOnCopy, bool isErrorOnDelete)
            {
                // 例外発生させるか?
                _isErrorOnCopy = isErrorOnCopy;
                _isErrorOnDelete = isErrorOnDelete;

                // リポジトリDI
                var logger = new TestLogger<PrimaryBlobRepository>(TestLogList);
                _repo = new CollectingBlobRepository(_appSettings, _primaryBlob, _collectingBlob, _blobPolly, logger);
            }

            /// <summary>
            /// 対象のファイルを削除する。
            /// </summary>
            /// <param name="file">ファイル</param>
            public void Delete(ArchiveFile file)
            {
                if (_isErrorOnDelete)
                {
                    throw new RmsException("IndexBlobControllerTest");
                }

                _repo.Delete(file);
            }

            /// <summary>
            /// 指定directory下のファイルを取得する
            /// </summary>
            /// <param name="directory">ディレクトリ</param>
            /// <returns>ファイル</returns>
            public IEnumerable<ArchiveFile> GetArchiveFiles(ArchiveDirectory directory)
            {
                return _repo.GetArchiveFiles(directory);
            }

            /// <summary>
            /// コレクティングBlobからプライマリBlobにコピーする
            /// </summary>
            /// <param name="source">コピー元</param>
            /// <param name="destination">コピー先</param>
            public void CopyToPrimary(ArchiveFile source, ArchiveFile destination)
            {
                _repo.CopyToPrimary(source, destination);
            }

            /// <summary>
            /// 同一Blob上のコンテナにファイルをコピーする。
            /// </summary>
            /// <param name="source">CollectingBlobのコピー元</param>
            /// <param name="destination">CollectingBlobのコピー先</param>
            public void Copy(ArchiveFile source, ArchiveFile destination)
            {
                if (_isErrorOnCopy)
                {
                    throw new RmsException("IndexBlobControllerTest");
                }

                _repo.Copy(source, destination);
            }
        }

        #endregion
    }
}
