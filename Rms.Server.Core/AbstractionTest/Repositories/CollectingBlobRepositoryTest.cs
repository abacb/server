using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
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
    /// CollectingBlobRepositoryのテスト
    /// </summary>
    /// <remarks>
    /// 前提として設定ファイルに以下の値が必要
    /// ・PrimaryBlobの接続文字列
    /// ・CollectingBlobの接続文字列
    /// </remarks>
    [TestClass]
    public class CollectingBlobRepositoryTest
    {
        /// <summary>テスト対象のコンテナ名</summary>
        private static readonly string TargetContainerName1 = "server-unit-test1";

        /// <summary>テスト対象のコンテナ名</summary>
        private static readonly string TargetContainerName2 = "server-unit-test2";

        /// <summary>テスト結果格納先のルートフォルダ</summary>
        private static readonly string TestResultRootDir = string.Format(@"TestResult.{0:yyyyMMddHHmmss}\{1}", DateTime.Now, typeof(CollectingBlobRepository).Name);

        /// <summary>PrimaryBlob</summary>
        private PrimaryBlob primaryBlob;

        /// <summary>CollectingBlob</summary>
        private CollectingBlob collectingBlob;

        /// <summary>テスト対象</summary>
        private ICollectingRepository target;

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
            CollectingBlob collectingBlob;

            // DI
            {
                TestDiProviderBuilder builder = new TestDiProviderBuilder();
                builder.ServiceCollection.AddTransient<CollectingBlob>();

                builder.ServiceCollection.AddSingleton<ITimeProvider>(UnitTestHelper.CreateTimeProvider(new DateTime(2030, 1, 1)));
                ServiceProvider provider = builder.Build();
                primaryBlob = provider.GetService<PrimaryBlob>();
                collectingBlob = provider.GetService<CollectingBlob>();
            }

            {
                var container1 = primaryBlob.Client.GetContainerReference(TargetContainerName1);
                container1.DeleteIfExistsAsync().Wait();
                var container2 = primaryBlob.Client.GetContainerReference(TargetContainerName2);
                container2.DeleteIfExistsAsync().Wait();
            }
            {
                var container1 = collectingBlob.Client.GetContainerReference(TargetContainerName1);
                container1.DeleteIfExistsAsync().Wait();
                var container2 = collectingBlob.Client.GetContainerReference(TargetContainerName2);
                container2.DeleteIfExistsAsync().Wait();
            }
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
            foreach (CloudBlockBlob blockBlob in primaryBlob.Client.GetBlockBlobs(TargetContainerName2))
            {
                blockBlob.DeleteIfExistsAsync().Wait();
            }
            foreach (CloudBlockBlob blockBlob in collectingBlob.Client.GetBlockBlobs(TargetContainerName1))
            {
                blockBlob.DeleteIfExistsAsync().Wait();
            }
            foreach (CloudBlockBlob blockBlob in collectingBlob.Client.GetBlockBlobs(TargetContainerName2))
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
            foreach (CloudBlockBlob blockBlob in primaryBlob.Client.GetBlockBlobs(TargetContainerName2))
            {
                blockBlob.DeleteIfExistsAsync().Wait();
            }
            foreach (CloudBlockBlob blockBlob in collectingBlob.Client.GetBlockBlobs(TargetContainerName1))
            {
                blockBlob.DeleteIfExistsAsync().Wait();
            }
            foreach (CloudBlockBlob blockBlob in collectingBlob.Client.GetBlockBlobs(TargetContainerName2))
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
                    collectingBlob.Client.Upload(TargetContainerName1, new DirectoryInfo(in_InitialBlobFileSet), file);
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
            string[] actualFiles = collectingBlob.Client.GetFiles(TargetContainerName1, actualDir).OrderBy(x => x).ToArray();
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
        /// CopyToPrimaryメソッドのテスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InitialBlobFileSet">Blobのファイル・フォルダ構成の初期状態</param>
        /// <param name="in_TargetBlobs">コピーするファイルのパス（複数コピーする場合はカンマ区切り）</param>
        /// <param name="expected_BlobFileSet">Blobのファイル・フォルダ構成の期待値</param>
        /// <param name="expected_ExceptionType">例外が発生する場合の期待値</param>
        /// <param name="expected_ExceptionMessage">例外が発生する場合の期待値</param>
        /// <param name="remarks">備考欄</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\Repositories_BlobRepository_Copy.csv")]
        public void CopyToPrimaryTest(string no, string in_InitialBlobFileSet, string in_TargetBlobs, string expected_BlobFileSet, string expected_ExceptionType, string expected_ExceptionMessage, string remarks)
        {
            // DI
            DependencyInjection();

            // テストデータ準備
            {
                FileInfo[] initial_files = new DirectoryInfo(in_InitialBlobFileSet).GetFiles("*", SearchOption.AllDirectories);
                foreach (FileInfo file in initial_files)
                {
                    collectingBlob.Client.Upload(TargetContainerName1, new DirectoryInfo(in_InitialBlobFileSet), file);
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
            string actualBlobFileSet = Path.Combine(TestResultRootDir, string.Format("{0}", no), "CopyToPrimary");

            // テスト実行
            try
            {
                foreach (string targetBlob in in_TargetBlobs.Split(","))
                {
                    ArchiveFile src = new ArchiveFile() { ContainerName = TargetContainerName1, FilePath = targetBlob };
                    ArchiveFile dst = new ArchiveFile() { ContainerName = TargetContainerName2, FilePath = targetBlob };
                    target.CopyToPrimary(src, dst);
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
            string[] actualFiles = primaryBlob.Client.GetFiles(TargetContainerName2, actualDir).OrderBy(x => x).ToArray();
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
        /// Copyメソッドのテスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InitialBlobFileSet">Blobのファイル・フォルダ構成の初期状態</param>
        /// <param name="in_TargetBlobs">コピーするファイルのパス（複数コピーする場合はカンマ区切り）</param>
        /// <param name="expected_BlobFileSet">Blobのファイル・フォルダ構成の期待値</param>
        /// <param name="expected_ExceptionType">例外が発生する場合の期待値</param>
        /// <param name="expected_ExceptionMessage">例外が発生する場合の期待値</param>
        /// <param name="remarks">備考欄</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\Repositories_BlobRepository_Copy.csv")]
        public void CopyTest(string no, string in_InitialBlobFileSet, string in_TargetBlobs, string expected_BlobFileSet, string expected_ExceptionType, string expected_ExceptionMessage, string remarks)
        {
            // DI
            DependencyInjection();

            // テストデータ準備
            {
                FileInfo[] initial_files = new DirectoryInfo(in_InitialBlobFileSet).GetFiles("*", SearchOption.AllDirectories);
                foreach (FileInfo file in initial_files)
                {
                    collectingBlob.Client.Upload(TargetContainerName1, new DirectoryInfo(in_InitialBlobFileSet), file);
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
            string actualBlobFileSet = Path.Combine(TestResultRootDir, string.Format("{0}", no), "Copy");

            // テスト実行
            try
            {
                foreach (string targetBlob in in_TargetBlobs.Split(","))
                {
                    ArchiveFile src = new ArchiveFile() { ContainerName = TargetContainerName1, FilePath = targetBlob };
                    ArchiveFile dst = new ArchiveFile() { ContainerName = TargetContainerName2, FilePath = targetBlob };
                    target.Copy(src, dst);
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
            string[] actualFiles = collectingBlob.Client.GetFiles(TargetContainerName2, actualDir).OrderBy(x => x).ToArray();
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
        /// GetArchiveFilesメソッドのテスト
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="in_InitialBlobFileSet">Blobのファイル・フォルダ構成の初期状態</param>
        /// <param name="in_DirectoryPath">対象となるディレクトリのパス</param>
        /// <param name="expected_ArchiveFiles">ArchiveFileの期待値</param>
        /// <param name="expected_ExceptionType">例外が発生する場合の期待値</param>
        /// <param name="expected_ExceptionMessage">例外が発生する場合の期待値</param>
        /// <param name="remarks">備考欄</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\Repositories_BlobRepository_GetArchiveFiles.csv")]
        public void GetArchiveFilesTest(string no, string in_InitialBlobFileSet, string in_DirectoryPath, string expected_ArchiveFiles, string expected_ExceptionType, string expected_ExceptionMessage, string remarks)
        {
            // DI
            DependencyInjection();

            // テストデータ準備
            {
                FileInfo[] initial_files = new DirectoryInfo(in_InitialBlobFileSet).GetFiles("*", SearchOption.AllDirectories);
                foreach (FileInfo file in initial_files)
                {
                    collectingBlob.Client.Upload(TargetContainerName1, new DirectoryInfo(in_InitialBlobFileSet), file);
                }
            }

            // 期待値
            ArchiveFile[] expected_files = JsonConvert.DeserializeObject<ArchiveFile[]>(File.ReadAllText(expected_ArchiveFiles));
            string expected_exceptiontype = string.IsNullOrEmpty(expected_ExceptionType) ? null : expected_ExceptionType;
            string expected_exceptionmessage = string.IsNullOrEmpty(expected_ExceptionMessage) ? null : expected_ExceptionMessage;

            // 結果格納先
            IEnumerable<ArchiveFile> actualFiles = null;
            Exception actualException = null;
            string actualBlobFileSet = Path.Combine(TestResultRootDir, string.Format("{0}", no), "GetArchiveFiles");

            // テスト実行
            try
            {
                actualFiles = target.GetArchiveFiles(new ArchiveDirectory() { ContainerName = TargetContainerName1, DirectoryPath = in_DirectoryPath });
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
            ArchiveFile[] actual_files = actualFiles.ToArray();
            string actual_exceptiontype = actualException?.GetType()?.FullName;
            string actual_exceptionmessage = actualException?.Message;

            // 確認
            CollectionAssert.AreEqual(expected_files.Select(x => x.FilePath).ToArray(), actual_files.Select(x => x.FilePath).ToArray());
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

            ArchiveFile src = new ArchiveFile() { ContainerName = TargetContainerName1, FilePath = target_filename };
            ArchiveFile dst = new ArchiveFile() { ContainerName = TargetContainerName2, FilePath = target_filename };

            // テストデータ準備
            collectingBlob.Client.Upload(TargetContainerName1, target_filename, target_filecontent);

            // Copyテスト
            {
                // 期待値
                string[] expected_filenames = new string[] { target_filename };

                // テスト実行
                target.Copy(src, dst);

                // テスト結果
                string[] actual_filenames = collectingBlob.Client.GetBlockBlobs(TargetContainerName1).Select(x => x.Name).ToArray();

                // 確認
                CollectionAssert.AreEqual(expected_filenames, actual_filenames);
            }

            // CopyToPrimaryテスト
            {
                // 期待値
                string[] expected_filenames = new string[] { target_filename };

                // テスト実行
                target.CopyToPrimary(src, dst);

                // テスト結果
                string[] actual_filenames = primaryBlob.Client.GetBlockBlobs(TargetContainerName2).Select(x => x.Name).ToArray();

                // 確認
                CollectionAssert.AreEqual(expected_filenames, actual_filenames);
            }

            // Deleteテスト
            {
                // 期待値
                string[] expected_filenames = new string[] { };

                // テスト実行
                target.Delete(new ArchiveFile() { ContainerName = TargetContainerName2, FilePath = target_filename });

                // テスト結果
                string[] actual_filenames = collectingBlob.Client.GetBlockBlobs(TargetContainerName2).Select(x => x.Name).ToArray();

                // 確認
                CollectionAssert.AreEqual(expected_filenames, actual_filenames);
            }
        }

        /// <summary>
        /// DIを実行する
        /// </summary>
        /// <param name="appSettings">アプリケーション設定を上書きする場合は指定する</param>
        private void DependencyInjection(Dictionary<string, string> appSettings = null)
        {
            TestDiProviderBuilder builder = new TestDiProviderBuilder();
            builder.ServiceCollection.AddTransient<CollectingBlob>();

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
            collectingBlob = provider.GetService<CollectingBlob>();
            target = provider.GetService<ICollectingRepository>();
        }
    }
}
