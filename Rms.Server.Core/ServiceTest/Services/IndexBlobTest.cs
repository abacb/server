using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;
using Newtonsoft.Json;
using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Core.Service.Models;
using Rms.Server.Core.Service.Services;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Utility.Models.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestHelper;
using static Rms.Server.Core.Service.Services.DeviceService;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ServiceTest.Services
{
    [TestClass]
    public class IndexBlobTest
    {
        // テスト設計
        // ・正常系
        //      ・ファイルコピー処理を行う
        // ・異常系
        //      ・ファイル

        [TestMethod]
        // [DynamicData("aa",DynamicDataSourceType.Method)]
        public async Task Index_Success_CopyedFile()
        {
            // 前提条件
            // ・ファイルがCollectingBlob上に存在する。
            // あと条件
            // ・ファイルがPrimaryBlob上にコピーされる。

            // TestInitialize
            DbTestHelper.DeleteAll();
            var dataOnDb = DbTestHelper.CreateMasterTables();
            dataOnDb = DbTestHelper.CreateDevices(dataOnDb);
            var owner = dataOnDb.Get<DtDevice>().First().EdgeId.ToString();

            // 設定値が空
            var diBuilder = new TestDiProviderBuilder();
            diBuilder.ServiceCollection.AddTransient<ITimeProvider>(x => UnitTestHelper.CreateTimeProvider(new DateTime(2020, 3, 1)));
            var provider = diBuilder.Build();

            // テストデータ作成
            // 準備として、Primary,Collectingのファイルを全部消す。
            var settings = provider.GetService<AppSettings>();
            await DeleteContainerOnBlob(settings.CollectingBlobConnectionString, "collect");
            await DeleteContainerOnBlob(settings.CollectingBlobConnectionString, "unknown");
            await DeleteContainerOnBlob(settings.PrimaryBlobConnectionString, "log");
            await DeleteContainerOnBlob(settings.PrimaryBlobConnectionString, "error");
            await DeleteContainerOnBlob(settings.PrimaryBlobConnectionString, "device");

            await CreateTestFilesOnCollectingBlob(settings, owner);

            // テスト対象
            var target = provider.GetService<IIndexBlobService>();
            target.Index();

            // 結果確認
            //var _account = CloudStorageAccount.Parse(settings.CoreStorageCollectingConnectionString);
            //var client = _account.CreateCloudBlobClient();
            //var container = client.GetContainerReference(settings.CoreStorageCollectingCollectContainerName);
            //var blob1 = container.GetBlockBlobReference(filePath);



            // CollectingBlobにファイルがないことを確認。


            // PrimaryBlobにそのファイルがあることを確認。

            // DBにファイルがあることを確認。

        }

        //[DataTestMethod]
        //[DynamicData("aa", DynamicDataSourceType.Method)]
        //public async Task Index_Success_CopyedFile(
        //    (string text, int number) actual,
        //    (string text, int number) 
            
        //    )
        //{
        //    Assert.AreEqual(actual.text, text);
        //    Assert.AreEqual(actual.number, number);
        //}
        public static IEnumerable<object[]> aa()
        {
            yield return new object[] { ("text1", 1), ("text1", 1) };
            yield return new object[] { ("text2", 2), "text2", 2 };
        }


        private static async Task DeleteContainerOnBlob(string connectionString, string containerName)
        {
            var _account = CloudStorageAccount.Parse(connectionString);
            var client = _account.CreateCloudBlobClient();

            var container = client.GetContainerReference(containerName);

            if(!await container.ExistsAsync())
            {
                return;
            }

            // Blob一覧を取得
            var token = default(BlobContinuationToken);
            do
            {
                // Blob一覧を取得
                var segment = await container.ListBlobsSegmentedAsync(token);

                token = segment.ContinuationToken;

                // Blobの名前と中身をConsoleに表示
                foreach (var blob in segment.Results.OfType<CloudBlockBlob>())
                {
                    await blob.DeleteIfExistsAsync();
                }
            } while (token != null);

            // Blobコンテナを削除はしない。
            // Blobの制約で、同名のコンテナを作成するには、最低でも30秒空ける必要があるため。
            // await container.DeleteIfExistsAsync();
        }


        // private static CloudBlockBlob Create(string filepath, string owner, string container,DateTime datetime, string content)

        private static async Task CreateTestFilesOnCollectingBlob(AppSettings settings, string owner)
        {
            var connectionString = settings.CollectingBlobConnectionString;

            // これはArchiveFile
            var containerName = settings.CollectingBlobContainerNameCollect;
            var filePath = $"{owner}/subdirectory/originalfilename.txt";

            var _account = CloudStorageAccount.Parse(connectionString);
            var client = _account.CreateCloudBlobClient();
            var container = client.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            // Blobを追加
            {
                // "file1.txt"を作成
                CloudBlockBlob blob1 = container.GetBlockBlobReference(filePath);
                blob1.Metadata.Add("sys_owner", owner);
                blob1.Metadata.Add("sys_container", "log");
                blob1.Metadata.Add("sys_sub_directory", "subdirectory");
                blob1.Metadata.Add("sys_file_datetime", DateTime.UtcNow.ToString());

                await blob1.UploadTextAsync("file1 content");
            }
        }
    }
}
