using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Blob;
using Rms.Server.Core.Abstraction.Models;
using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Core.Abstraction.Repositories.Blobs;
using Rms.Server.Core.Utility;
using Rms.Server.Test;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TestHelper;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace AbstractionTest.Repositories
{
    /// <summary>
    /// PrimaryBlobRepositoryのテスト
    /// </summary>
    /// <remarks>
    /// 前提として設定ファイルに以下の値が必要
    /// ・PrimaryBlobの接続文字列
    /// </remarks>
    [TestClass]
    public class PrimaryBlobRepositoryTest
    {
        /// <summary>テスト対象のコンテナ名</summary>
        private static readonly string TargetContainerName1 = "server-unit-test1";

        /// <summary>テスト結果格納先のルートフォルダ</summary>
        private static readonly string TestResultRootDir = string.Format(@"TestResult.{0:yyyyMMddHHmmss}\{1}", DateTime.Now, typeof(PrimaryBlobRepository).Name);

        /// <summary>PrimaryBlob</summary>
        private PrimaryBlob primaryBlob;

        /// <summary>テスト対象</summary>
        private IPrimaryRepository target;

        /// <summary>
        /// ClassInit
        /// </summary>
        /// <param name="context">TestContext</param>
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            if (Directory.Exists(TestResultRootDir))
            {
                Directory.Delete(TestResultRootDir, true);
            }
        }

        /// <summary>
        /// ClassCleanup
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanup()
        {
            PrimaryBlob primaryBlob;

            // DI
            {
                TestDiProviderBuilder builder = new TestDiProviderBuilder();
                builder.ServiceCollection.AddTransient<PrimaryBlob>();

                builder.ServiceCollection.AddSingleton<ITimeProvider>(UnitTestHelper.CreateTimeProvider(new DateTime(2030, 1, 1)));
                ServiceProvider provider = builder.Build();
                primaryBlob = provider.GetService<PrimaryBlob>();
            }

            var container1 = primaryBlob.Client.GetContainerReference(TargetContainerName1);
            container1.DeleteIfExistsAsync().Wait();
        }

        /// <summary>
        /// TestInitialize
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            DependencyInjection();
            foreach (CloudBlockBlob blockBlob in primaryBlob.Client.GetBlockBlobs(TargetContainerName1))
            {
                blockBlob.DeleteIfExistsAsync().Wait();
            }
        }

        /// <summary>
        /// TestCleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            DependencyInjection();
            foreach (CloudBlockBlob blockBlob in primaryBlob.Client.GetBlockBlobs(TargetContainerName1))
            {
                blockBlob.DeleteIfExistsAsync().Wait();
            }
        }

        /// <summary>
        /// Deleteメソッドのテスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InitialBlobFileSet">Blobのファイル・フォルダ構成の初期状態</param>
        /// <param name="in_TargetBlobs">Blobから削除するファイルのパス（複数削除する場合はカンマ区切り）</param>
        /// <param name="expected_BlobFileSet">Blobのファイル・フォルダ構成の期待値</param>
        /// <param name="expected_ExceptionType">例外が発生する場合の期待値</param>
        /// <param name="expected_ExceptionMessage">例外が発生する場合の期待値</param>
        /// <param name="remarks">備考欄</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\Repositories_BlobRepository_Delete.csv")]
        public void DeleteTest(string no, string in_InitialBlobFileSet, string in_TargetBlobs, string expected_BlobFileSet, string expected_ExceptionType, string expected_ExceptionMessage, string remarks)
        {
            // DI
            DependencyInjection();

            // テストデータ準備
            {
                FileInfo[] initial_files = new DirectoryInfo(in_InitialBlobFileSet).GetFiles("*", SearchOption.AllDirectories);
                foreach (FileInfo file in initial_files)
                {
                    primaryBlob.Client.Upload(TargetContainerName1, new DirectoryInfo(in_InitialBlobFileSet), file);
                }
            }

            // 期待値
            DirectoryInfo expectedDir = new DirectoryInfo(expected_BlobFileSet);
            string[] expectedFiles = expectedDir.Exists ? expectedDir.GetFiles("*", SearchOption.AllDirectories).Select(x => x.FullName).OrderBy(x => x).ToArray() : new string[] { };
            string[] expected_filenames = expectedFiles.Select(x => x.Replace(expectedDir.FullName, string.Empty)).ToArray();
            string[] expected_filecontents = expectedFiles.Select(x => File.ReadAllText(x)).ToArray();
            string expected_exceptiontype = string.IsNullOrEmpty(expected_ExceptionType) ? null : expected_ExceptionType;
            string expected_exceptionmessage = string.IsNullOrEmpty(expected_ExceptionMessage) ? null : expected_ExceptionMessage;

            // 結果格納先
            Exception actualException = null;
            string actualBlobFileSet = Path.Combine(TestResultRootDir, string.Format("{0}", no), "Delete");

            // テスト実行
            try
            {
                foreach (string targetBlob in in_TargetBlobs.Split(","))
                {
                    target.Delete(new ArchiveFile() { ContainerName = TargetContainerName1, FilePath = targetBlob });
                }
            }
            catch (AggregateException ex)
            {
                actualException = ex.InnerException;
            }
            catch (Exception ex)
            {
                actualException = ex;
            }

            // テスト結果
            DirectoryInfo actualDir = new DirectoryInfo(actualBlobFileSet);
            string[] actualFiles = primaryBlob.Client.GetFiles(TargetContainerName1, actualDir).OrderBy(x => x).ToArray();
            string[] actual_filenames = actualFiles.Select(x => x.Replace(actualDir.FullName, string.Empty)).ToArray();
            string[] actual_filecontents = actualFiles.Select(x => File.ReadAllText(x)).ToArray();
            string actual_exceptiontype = actualException?.GetType()?.FullName;
            string actual_exceptionmessage = actualException?.Message;

            // 確認
            CollectionAssert.AreEqual(expected_filenames, actual_filenames);
            CollectionAssert.AreEqual(expected_filecontents, actual_filecontents);
            Assert.AreEqual(expected_exceptiontype, actual_exceptiontype);
            Assert.AreEqual(expected_exceptionmessage, actual_exceptionmessage);
        }


        /// <summary>
        /// 長い名前、あるいは、大容量のファイルのテスト
        /// </summary>
        [DataTestMethod]
        [DataRow(1024, 10)]
#if 大きいので何度も実施したくない // TODO: テスト実施時に復活
        [DataRow(3, 300 * 1024 * 1024)]
#endif
        public void LengthAndSizeTest(int filenameLength, int fileSize)
        {
            // DI
            DependencyInjection();

            string target_filename = new string('1', filenameLength);
            string target_filecontent = new string('1', fileSize);

            // テストデータ準備
            primaryBlob.Client.Upload(TargetContainerName1, target_filename, target_filecontent);

            // 期待値
            string[] expected_filenames = new string[] { };

            // テスト実行
            target.Delete(new ArchiveFile() { ContainerName = TargetContainerName1, FilePath = target_filename });

            // テスト結果
            string[] actual_filenames = primaryBlob.Client.GetBlockBlobs(TargetContainerName1).Select(x => x.Name).ToArray();

            // 確認
            CollectionAssert.AreEqual(expected_filenames, actual_filenames);
        }

        /// <summary>
        /// DIを実行する
        /// </summary>
        /// <param name="appSettings">アプリケーション設定を上書きする場合は指定する</param>
        private void DependencyInjection(Dictionary<string, string> appSettings = null)
        {
            TestDiProviderBuilder builder = new TestDiProviderBuilder();
            builder.ServiceCollection.AddTransient<PrimaryBlob>();

            if (appSettings != null)
            {
                foreach (KeyValuePair<string, string> pair in appSettings)
                {
                    builder.AddConfigure(pair.Key, pair.Value);
                }
            }

            builder.ServiceCollection.AddSingleton<ITimeProvider>(UnitTestHelper.CreateTimeProvider(new DateTime(2030, 1, 1)));
            ServiceProvider provider = builder.Build();
            primaryBlob = provider.GetService<PrimaryBlob>();
            target = provider.GetService<IPrimaryRepository>();
        }
    }
}
