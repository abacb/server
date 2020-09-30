using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Rms.Server.Core.Abstraction.Models;
using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Core.Service.Services;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Utility.Models.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TestHelper;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ServiceTest
{
    // このテストはちょっと正直やりすぎというか多分実際のBlobとDBを作ってテストしたほうが早い

    [TestClass]
    public class CleanBlobServiceTest
    {
        [TestMethod]
        public void NoChangeInRepositoryIfNoSettings()
        {
            // 設定値が空
            var diBuilder = new TestDiProviderBuilder();
            diBuilder.ServiceCollection.AddTransient<ITimeProvider>(x => UnitTestHelper.CreateTimeProvider(new DateTime(2020, 3, 1)));
            diBuilder.ServiceCollection.AddSingleton<IPrimaryRepository, PrimaryRepositoryMock>();
            diBuilder.ServiceCollection.AddSingleton<IDtDeviceFileRepository, DtDeviceFileRepositoryMock>();
            var provider = diBuilder.Build();

            // テスト対象
            var target = provider.GetService<ICleanBlobService>();
            target.Clean();

            // 結果確認のため、状態を持つリポジトリを取得する。このために上記の設定でシングルトンにしている。
            var primaryRepositoryMock = provider.GetService<IPrimaryRepository>() as PrimaryRepositoryMock;
            var deviceFileRepositoryMock = provider.GetService<IDtDeviceFileRepository>() as DtDeviceFileRepositoryMock;

            // Delete が特に変更ないことを確認。
            var list = PrimaryRepositoryMock.GetDefaultList();
            Assert.AreEqual(
                PrimaryRepositoryMock.GetDefaultList().ToStringJson(),
                primaryRepositoryMock.ArchiveFiles.ToStringJson());
            Assert.AreEqual(
                DtDeviceFileRepositoryMock.GetDefaultList().ToStringJson(),
                deviceFileRepositoryMock.files.ToStringJson());
        }

        // テストに使用するデータをTestContainerに押し込めています。
        // 入力値と期待する結果をマッチングできるようにテストを作成します。
        [DataTestMethod]
        [DataRow("BlobCleanTarget_Container1_path/folder1", "1", "BlobCleanTarget_Container1_path/folder3", "1", 3, "フォルダ1の1月分のデータが削除済み")]
        [DataRow("BlobCleanTarget_Container1_path/folder1", "1", "BlobCleanTarget_Container1_path/folder2", "1", 3, "フォルダ1_2の1月分のデータが削除済み")]
        [DataRow("BlobCleanTarget_Container1_path/folder", "1", "", "", 3, "フォルダ1_2の1月分のデータが削除済み")]
        public void NormarizeTest(
            string SettingKey1, string SettingValue1,
            string SettingKey2, string SettingValue2,
            int nowMonth,
            string expectedGetterName)
        {
            // 設定値
            var diBuilder = new TestDiProviderBuilder();
            diBuilder.AddConfigure(SettingKey1, SettingValue1);
            diBuilder.AddConfigure(SettingKey2, SettingValue2);

            // DI設定
            diBuilder.ServiceCollection.AddTransient<ITimeProvider>(x => UnitTestHelper.CreateTimeProvider(new DateTime(2020, nowMonth, 1)));
            diBuilder.ServiceCollection.AddSingleton<IPrimaryRepository, PrimaryRepositoryMock>();
            diBuilder.ServiceCollection.AddSingleton<IDtDeviceFileRepository, DtDeviceFileRepositoryMock>();
            var provider = diBuilder.Build();

            // テスト対象実行
            var target = provider.GetService<ICleanBlobService>();
            target.Clean();

            var expected = GetExpected(expectedGetterName);

            // 結果確認のため、状態を持つリポジトリを取得する。このために上記の設定でシングルトンにしている。
            var primaryRepository = provider.GetService<IPrimaryRepository>() as PrimaryRepositoryMock;
            var dtDeviceFileRepository = provider.GetService<IDtDeviceFileRepository>() as DtDeviceFileRepositoryMock;

            Assert.AreEqual(
                expected.Item1.ToStringJson(),
                dtDeviceFileRepository.files.ToStringJson());
            Assert.AreEqual(
                expected.Item2.ToStringJson(),
                primaryRepository.ArchiveFiles.ToStringJson());
        }

        // リポジトリが失敗したとき、
        [DataTestMethod]
        [DataRow(true, false, false, false, false, false)]
        [DataRow(false, true, false, false, false, false)]
        [DataRow(false, false, true, false, false, false)]
        [DataRow(false, false, false, true, false, false)]
        [DataRow(false, false, false, false, true, false)]
        [DataRow(false, false, false, false, false, true)]
        public void ExceptionTest(
            bool IsFailed_CreateDtDeviceFile,
            bool IsFailed_GetFileDatasBelowFilePath,
            bool IsFailed_DeleteDtDeviceFile,
            bool IsThrowEx_CreateDtDeviceFile,
            bool IsThrowEx_GetFileDatasBelowFilePath,
            bool IsThrowEx_DeleteDtDeviceFile)
        {
            // 設定値
            var diBuilder = new TestDiProviderBuilder();
            diBuilder.AddConfigure("BlobCleanTarget_container_path/folder1", "1");

            // DI設定
            diBuilder.ServiceCollection.AddTransient<ITimeProvider>(x => UnitTestHelper.CreateTimeProvider(new DateTime(2020, 3, 1)));

            var primaryRepository = new PrimaryRepositoryMock();
            diBuilder.ServiceCollection.AddSingleton<IPrimaryRepository>(x => primaryRepository);

            // 失敗設定する
            var dtDeviceFileRepository = new DtDeviceFileRepositoryMock()
                .FailedIfCallCreateDtDeviceFile(IsFailed_CreateDtDeviceFile)
                .FailedIfCallDeleteDtDeviceFile(IsFailed_DeleteDtDeviceFile)
                .FailedIfCallGetFileDatasBelowFilePath(IsFailed_GetFileDatasBelowFilePath)
                .ThrowIfCallCreateDtDeviceFile(IsThrowEx_CreateDtDeviceFile)
                .ThrowIfCallDeleteDtDeviceFile(IsThrowEx_DeleteDtDeviceFile)
                .ThrowIfCallGetFileDatasBelowFilePath(IsThrowEx_GetFileDatasBelowFilePath);

            diBuilder.ServiceCollection.AddSingleton<IDtDeviceFileRepository>(x => dtDeviceFileRepository);
            var provider = diBuilder.Build();

            // テスト対象実行
            var target = provider.GetService<ICleanBlobService>();
            target.Clean();

            var expected = GetExpected("CreateAllData");

            Assert.AreEqual(
                expected.Item1.ToStringJson(),
                dtDeviceFileRepository.files.ToStringJson());
            Assert.AreEqual(
                expected.Item2.ToStringJson(),
                primaryRepository.ArchiveFiles.ToStringJson());
        }

        [Ignore]
        [TestMethod]
        public void Onepass()
        {
            // 準備
            var diBuilder = new TestDiProviderBuilder();
            diBuilder.AddConfigure("BlobCleanTarget_container_filepath", "30");

            // init
            // DBにファイルパス、データを追加

            // コンテナがなければ作成
            // Blobにそれと同じファイルを追加
            var file = new ArchiveFile()
            {
                ContainerName = "",
                FilePath = ""
            };

            // プロバイダ生成
            var provider = diBuilder.Build();

            // ■テストの本筋
            var actual = provider.GetService<CleanBlobService>();
            actual.Clean();

            // 削除対象がGetできばOK
            // 非削除対象がGetできなければNG

            // 後始末
        }

        #region Test Helper

        public Tuple<List<DtDeviceFile>, List<ArchiveFile>> GetExpected(string methodName)
        {
            var instance = new TestDataContainer();
            MethodInfo mi = typeof(TestDataContainer).GetMethod(methodName);
            return (Tuple<List<DtDeviceFile>, List<ArchiveFile>>)mi.Invoke(instance, null);
        }

        public class PrimaryRepositoryMock : IPrimaryRepository
        {
            private bool _isThrowExDeleteInBackground = false;

            public List<ArchiveFile> ArchiveFiles;

            public PrimaryRepositoryMock()
            {
                ArchiveFiles = new TestDataContainer().CreateAllData().Item2;
            }

            public PrimaryRepositoryMock ThrowIfCallDeleteInBackground(bool isThrow)
            {
                _isThrowExDeleteInBackground = isThrow;
                return this;
            }

            public static List<ArchiveFile> GetDefaultList()
            {
                return new TestDataContainer().CreateAllData().Item2;
            }
            public void Delete(ArchiveFile file)
            {
                if (_isThrowExDeleteInBackground)
                {
                    throw new Exception();
                }

                ArchiveFiles.RemoveAll(x => x.ContainerName == file.ContainerName && file.FilePath == x.FilePath);
                return;
            }
        }

        public class DtDeviceFileRepositoryMock : IDtDeviceFileRepository
        {
            private bool _isThrowEx_CreateDtDeviceFile = false;
            private bool _isThrowEx_GetFileDatasBelowFilePath = false;
            private bool _isThrowEx_DeleteDtDeviceFile = false;
            private bool _iFailed_CreateDtDeviceFile = false;
            private bool _iFailed_GetFileDatasBelowFilePath = false;
            private bool _iFailed_DeleteDtDeviceFile = false;
            public List<DtDeviceFile> files;

            public DtDeviceFileRepositoryMock()
            {
                files = GetDefaultList();
            }
            public DtDeviceFileRepositoryMock ThrowIfCallCreateDtDeviceFile(bool isThrow)
            {
                _isThrowEx_CreateDtDeviceFile = isThrow;
                return this;
            }
            public DtDeviceFileRepositoryMock ThrowIfCallGetFileDatasBelowFilePath(bool isThrow)
            {
                _isThrowEx_GetFileDatasBelowFilePath = isThrow;
                return this;
            }
            public DtDeviceFileRepositoryMock ThrowIfCallDeleteDtDeviceFile(bool isThrow)
            {
                _isThrowEx_DeleteDtDeviceFile = isThrow;
                return this;
            }
            public DtDeviceFileRepositoryMock FailedIfCallCreateDtDeviceFile(bool isThrow)
            {
                _iFailed_CreateDtDeviceFile = isThrow;
                return this;
            }
            public DtDeviceFileRepositoryMock FailedIfCallGetFileDatasBelowFilePath(bool isThrow)
            {
                _iFailed_GetFileDatasBelowFilePath = isThrow;
                return this;
            }
            public DtDeviceFileRepositoryMock FailedIfCallDeleteDtDeviceFile(bool isThrow)
            {
                _iFailed_DeleteDtDeviceFile = isThrow;
                return this;
            }

            public static List<DtDeviceFile> GetDefaultList()
            {
                return new TestDataContainer().CreateAllData().Item1;
            }

            public DtDeviceFile CreateOrUpdateDtDeviceFile(DtDeviceFile inData)
            {
                if (_isThrowEx_CreateDtDeviceFile)
                {
                    throw new RmsException("Error!");
                }
                if (_iFailed_CreateDtDeviceFile)
                {
                    return null;
                }

                files.Add(inData);
                return inData;
            }

            /// <summary>
            /// Serviceのテストでは不要だが、インターフェースは存在するので実装する
            /// </summary>
            /// <param name="sid">端末SID</param>
            /// <returns>SIDだけを設定したDtDeviceFileインスタンス</returns>
            /// <remarks>
            /// BlobCleanControllerの自動テストの都合上、DBから端末ファイルデータを取得するインターフェースが必要。
            /// CleanBlobServiceTest用のMockには最低限の実装のみを行い、本テストにおいては使用しない
            /// </remarks>
            public DtDeviceFile ReadDtDeviceFile(long sid)
            {
                return new DtDeviceFile() { Sid = sid };
            }

            public IEnumerable<DtDeviceFile> FindByFilePathStartingWithAndUpdateDatetimeLessThan(
                string containerName,
                string path,
                DateTime endDateTime
                )
            {
                if (_isThrowEx_GetFileDatasBelowFilePath)
                {
                    throw new RmsException("Error!");
                }
                if (_iFailed_GetFileDatasBelowFilePath)
                {
                    return null;
                }

                var list =
                    files
                    .Where(x => x.FilePath.StartsWith(path))
                    .Where(x => x.Container == containerName)
                    .Where(x => x.UpdateDatetime < endDateTime);

                // そのまま渡すと外からこの配列を操作したときにInvalidOperationException: Collection was modified; 
                // になるため、同じ値のコピーを渡す。
                var copied = JsonConvert.DeserializeObject<IEnumerable<DtDeviceFile>>(JsonConvert.SerializeObject(list));

                return copied;
            }

            public DtDeviceFile DeleteDtDeviceFile(long sid)
            {
                if (_isThrowEx_DeleteDtDeviceFile)
                {
                    throw new RmsException("Error!");
                }
                if (_iFailed_DeleteDtDeviceFile)
                {
                    return null;
                }

                files.RemoveAll(x => x.Sid == sid);
                return new DtDeviceFile();
            }
        }

        public class TestDataContainer
        {
            public Tuple<List<DtDeviceFile>, List<ArchiveFile>> CreateAllData()
            {
                return
                    new Tuple<List<DtDeviceFile>, List<ArchiveFile>>(
                    new List<DtDeviceFile>()
                        {
                        // こういうデータが初期値であるという前提
                        new DtDeviceFile(){Sid = 1, DeviceSid = 1, FilePath = "path/folder1/hoge1.txt", UpdateDatetime = new DateTime(2020, 1,  1, 0, 0, 0 ), Container = "Container1"},
                        new DtDeviceFile(){Sid = 2, DeviceSid = 1, FilePath = "path/folder1/hoge2.txt", UpdateDatetime = new DateTime(2020, 1, 31, 23, 59, 59 ), Container = "Container1"},
                        new DtDeviceFile(){Sid = 3, DeviceSid = 1, FilePath = "path/folder1/hoge3.txt", UpdateDatetime = new DateTime(2020, 2,  1, 0, 0, 0 ), Container = "Container1"},
                        new DtDeviceFile(){Sid = 4, DeviceSid = 1, FilePath = "path/folder1/hoge4.txt", UpdateDatetime = new DateTime(2020, 2, 29, 23, 59, 59 ), Container = "Container1"},
                        new DtDeviceFile(){Sid = 5, DeviceSid = 1, FilePath = "path/folder1/hoge5.txt", UpdateDatetime = new DateTime(2020, 3,  1, 0, 0, 0 ), Container = "Container1"},
                        new DtDeviceFile(){Sid = 6, DeviceSid = 1, FilePath = "path/folder1/hoge6.txt", UpdateDatetime = new DateTime(2020, 3, 31, 23, 59, 59 ), Container = "Container1"},

                        new DtDeviceFile(){Sid = 11, DeviceSid = 2, FilePath = "path/folder2/hoge1.txt", UpdateDatetime = new DateTime(2020, 1,  1, 0, 0, 0 ), Container = "Container1"},
                        new DtDeviceFile(){Sid = 12, DeviceSid = 2, FilePath = "path/folder2/hoge2.txt", UpdateDatetime = new DateTime(2020, 1, 31, 23, 59, 59 ), Container = "Container1"},
                        new DtDeviceFile(){Sid = 13, DeviceSid = 2, FilePath = "path/folder2/hoge3.txt", UpdateDatetime = new DateTime(2020, 2,  1, 0, 0, 0 ), Container = "Container1"},
                        new DtDeviceFile(){Sid = 14, DeviceSid = 2, FilePath = "path/folder2/hoge4.txt", UpdateDatetime = new DateTime(2020, 2, 29, 23, 59, 59 ), Container = "Container1"},
                        new DtDeviceFile(){Sid = 15, DeviceSid = 2, FilePath = "path/folder2/hoge5.txt", UpdateDatetime = new DateTime(2020, 3,  1, 0, 0, 0 ), Container = "Container1"},
                        new DtDeviceFile(){Sid = 16, DeviceSid = 2, FilePath = "path/folder2/hoge6.txt", UpdateDatetime = new DateTime(2020, 3, 31, 23, 59, 59 ), Container = "Container1"},
                        },
                    new List<ArchiveFile>()
                        {
                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder1/hoge1.txt" },
                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder1/hoge2.txt" },
                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder1/hoge3.txt" },
                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder1/hoge4.txt" },
                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder1/hoge5.txt" },
                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder1/hoge6.txt" },

                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder2/hoge1.txt" },
                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder2/hoge2.txt" },
                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder2/hoge3.txt" },
                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder2/hoge4.txt" },
                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder2/hoge5.txt" },
                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder2/hoge6.txt" },
                        });
            }

            public Tuple<List<DtDeviceFile>, List<ArchiveFile>> フォルダ1の1月分のデータが削除済み()
            {
                return
                    new Tuple<List<DtDeviceFile>, List<ArchiveFile>>(
                    new List<DtDeviceFile>()
                        {
                        //new DtDeviceFile(){Sid = 1, DeviceSid = 1, FilePath = "path/folder1/hoge1.txt", UpdateDatetime = new DateTime(2020, 1,  1, 0, 0, 0 ), Container = "Container1"},
                        //new DtDeviceFile(){Sid = 2, DeviceSid = 1, FilePath = "path/folder1/hoge2.txt", UpdateDatetime = new DateTime(2020, 1, 31, 23, 59, 59 ), Container = "Container1"},
                        new DtDeviceFile(){Sid = 3, DeviceSid = 1, FilePath = "path/folder1/hoge3.txt", UpdateDatetime = new DateTime(2020, 2,  1, 0, 0, 0 ), Container = "Container1"},
                        new DtDeviceFile(){Sid = 4, DeviceSid = 1, FilePath = "path/folder1/hoge4.txt", UpdateDatetime = new DateTime(2020, 2, 29, 23, 59, 59 ), Container = "Container1"},
                        new DtDeviceFile(){Sid = 5, DeviceSid = 1, FilePath = "path/folder1/hoge5.txt", UpdateDatetime = new DateTime(2020, 3,  1, 0, 0, 0 ), Container = "Container1"},
                        new DtDeviceFile(){Sid = 6, DeviceSid = 1, FilePath = "path/folder1/hoge6.txt", UpdateDatetime = new DateTime(2020, 3, 31, 23, 59, 59 ), Container = "Container1"},

                        new DtDeviceFile(){Sid = 11, DeviceSid = 2, FilePath = "path/folder2/hoge1.txt", UpdateDatetime = new DateTime(2020, 1,  1, 0, 0, 0 ), Container = "Container1"},
                        new DtDeviceFile(){Sid = 12, DeviceSid = 2, FilePath = "path/folder2/hoge2.txt", UpdateDatetime = new DateTime(2020, 1, 31, 23, 59, 59 ), Container = "Container1"},
                        new DtDeviceFile(){Sid = 13, DeviceSid = 2, FilePath = "path/folder2/hoge3.txt", UpdateDatetime = new DateTime(2020, 2,  1, 0, 0, 0 ), Container = "Container1"},
                        new DtDeviceFile(){Sid = 14, DeviceSid = 2, FilePath = "path/folder2/hoge4.txt", UpdateDatetime = new DateTime(2020, 2, 29, 23, 59, 59 ), Container = "Container1"},
                        new DtDeviceFile(){Sid = 15, DeviceSid = 2, FilePath = "path/folder2/hoge5.txt", UpdateDatetime = new DateTime(2020, 3,  1, 0, 0, 0 ), Container = "Container1"},
                        new DtDeviceFile(){Sid = 16, DeviceSid = 2, FilePath = "path/folder2/hoge6.txt", UpdateDatetime = new DateTime(2020, 3, 31, 23, 59, 59 ), Container = "Container1"},
                        },
                    new List<ArchiveFile>()
                        {
                        //new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder1/hoge1.txt" },
                        //new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder1/hoge2.txt" },
                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder1/hoge3.txt" },
                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder1/hoge4.txt" },
                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder1/hoge5.txt" },
                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder1/hoge6.txt" },

                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder2/hoge1.txt" },
                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder2/hoge2.txt" },
                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder2/hoge3.txt" },
                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder2/hoge4.txt" },
                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder2/hoge5.txt" },
                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder2/hoge6.txt" },
                        });
            }
            public Tuple<List<DtDeviceFile>, List<ArchiveFile>> フォルダ1_2の1月分のデータが削除済み()
            {
                return
                    new Tuple<List<DtDeviceFile>, List<ArchiveFile>>(
                    new List<DtDeviceFile>()
                        {
                        //new DtDeviceFile(){Sid = 1, DeviceSid = 1, FilePath = "path/folder1/hoge1.txt", UpdateDatetime = new DateTime(2020, 1,  1, 0, 0, 0 ), Container = "Container1"},
                        //new DtDeviceFile(){Sid = 2, DeviceSid = 1, FilePath = "path/folder1/hoge2.txt", UpdateDatetime = new DateTime(2020, 1, 31, 23, 59, 59 ), Container = "Container1"},
                        new DtDeviceFile(){Sid = 3, DeviceSid = 1, FilePath = "path/folder1/hoge3.txt", UpdateDatetime = new DateTime(2020, 2,  1, 0, 0, 0 ), Container = "Container1"},
                        new DtDeviceFile(){Sid = 4, DeviceSid = 1, FilePath = "path/folder1/hoge4.txt", UpdateDatetime = new DateTime(2020, 2, 29, 23, 59, 59 ), Container = "Container1"},
                        new DtDeviceFile(){Sid = 5, DeviceSid = 1, FilePath = "path/folder1/hoge5.txt", UpdateDatetime = new DateTime(2020, 3,  1, 0, 0, 0 ), Container = "Container1"},
                        new DtDeviceFile(){Sid = 6, DeviceSid = 1, FilePath = "path/folder1/hoge6.txt", UpdateDatetime = new DateTime(2020, 3, 31, 23, 59, 59 ), Container = "Container1"},

                        //new DtDeviceFile(){Sid = 11, DeviceSid = 2, FilePath = "path/folder2/hoge1.txt", UpdateDatetime = new DateTime(2020, 1,  1, 0, 0, 0 ), Container = "Container1"},
                        //new DtDeviceFile(){Sid = 12, DeviceSid = 2, FilePath = "path/folder2/hoge2.txt", UpdateDatetime = new DateTime(2020, 1, 31, 23, 59, 59 ), Container = "Container1"},
                        new DtDeviceFile(){Sid = 13, DeviceSid = 2, FilePath = "path/folder2/hoge3.txt", UpdateDatetime = new DateTime(2020, 2,  1, 0, 0, 0 ), Container = "Container1"},
                        new DtDeviceFile(){Sid = 14, DeviceSid = 2, FilePath = "path/folder2/hoge4.txt", UpdateDatetime = new DateTime(2020, 2, 29, 23, 59, 59 ), Container = "Container1"},
                        new DtDeviceFile(){Sid = 15, DeviceSid = 2, FilePath = "path/folder2/hoge5.txt", UpdateDatetime = new DateTime(2020, 3,  1, 0, 0, 0 ), Container = "Container1"},
                        new DtDeviceFile(){Sid = 16, DeviceSid = 2, FilePath = "path/folder2/hoge6.txt", UpdateDatetime = new DateTime(2020, 3, 31, 23, 59, 59 ), Container = "Container1"},
                        },
                    new List<ArchiveFile>()
                        {
                        //new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder1/hoge1.txt" },
                        //new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder1/hoge2.txt" },
                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder1/hoge3.txt" },
                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder1/hoge4.txt" },
                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder1/hoge5.txt" },
                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder1/hoge6.txt" },

                        //new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder2/hoge1.txt" },
                        //new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder2/hoge2.txt" },
                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder2/hoge3.txt" },
                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder2/hoge4.txt" },
                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder2/hoge5.txt" },
                        new ArchiveFile(){ContainerName = "Container1", FilePath = "path/folder2/hoge6.txt" },
                        });
            }
            public Tuple<List<DtDeviceFile>, List<ArchiveFile>> フォルダ1_2の1_2月分のデータが削除済み()
            {
                return null;
            }
        }

        #endregion
    }
}
