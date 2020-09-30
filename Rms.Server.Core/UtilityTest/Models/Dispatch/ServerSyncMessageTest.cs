using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rms.Server.Core.Azure.Utility.Validations;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Models.Dispatch;
using Rms.Server.Core.Utility.Models.Entites;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TestHelper;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace UtilityTest.Validations
{
    /// <summary>
    /// ServerSyncMessageTest
    /// </summary>
    [TestClass()]
    public class ServerSyncMessageTest
    {
        #region Equals

        /// <summary>
        /// メッセージクラスインスタンス比較メソッドの挙動確認
        /// </summary>
        [TestMethod]
        public void EqualsTest()
        {
            const int DataSize = 3;

            const string StorageName = "StorageName{0}";
            const string Url = "Url{0}";
            const string Sas = "Sas{0}";
            const string ScriptName = "ScriptName{0}";
            const string FileName = "FileName{0}";
            const int Version = 1;
            const string Location = "Location{0}";

            ServerSyncMessage m1 = new ServerSyncMessage();
            ServerSyncMessage m2 = new ServerSyncMessage();
            List<DtStorageConfig> storageConfigs;
            List<DtScriptConfig> scriptConfigs;

            // 2つのメッセージの内容が一致するケース
            {
                storageConfigs = new List<DtStorageConfig>();
                scriptConfigs = new List<DtScriptConfig>();

                for (int i = 1; i <= DataSize; i++)
                {
                    DtStorageConfig config = CreateStorageConfig(
                        string.Format(StorageName, i), string.Format(Url, i), string.Format(Sas, i));
                    storageConfigs.Add(config);
                }

                for (int i = 1; i <= DataSize; i++)
                {
                    DtScriptConfig config = CreateScriptConfig(
                        string.Format(ScriptName, i), string.Format(FileName, i),
                        Version, string.Format(Location, i));
                    scriptConfigs.Add(config);
                }

                m1.SetScriptConfigs(scriptConfigs);
                m1.SetStorageConfigs(storageConfigs);
                m2.SetScriptConfigs(scriptConfigs);
                m2.SetStorageConfigs(storageConfigs);

                bool result = ServerSyncMessage.Equals(m1, m2);
                string message = ServerSyncMessage.CreateJsonString(m1);

                Assert.AreEqual(true, result);
                Assert.IsNotNull(message);
                Assert.AreNotEqual(string.Empty, message);
            }

            // 2つのメッセージの内容が一致するケース
            // リストに格納された順序が異なっているケース
            {
                storageConfigs = new List<DtStorageConfig>();
                scriptConfigs = new List<DtScriptConfig>();

                for (int i = DataSize; i > 0; i--)
                {
                    DtStorageConfig config = CreateStorageConfig(
                        string.Format(StorageName, i), string.Format(Url, i), string.Format(Sas, i));
                    storageConfigs.Add(config);
                }

                for (int i = DataSize; i > 0; i--)
                {
                    DtScriptConfig config = CreateScriptConfig(
                        string.Format(ScriptName, i), string.Format(FileName, i),
                        Version, string.Format(Location, i));
                    scriptConfigs.Add(config);
                }


                m2.SetScriptConfigs(scriptConfigs);
                m2.SetStorageConfigs(storageConfigs);

                bool result = ServerSyncMessage.Equals(m1, m2);

                string message = ServerSyncMessage.CreateJsonString(m2);

                Assert.AreEqual(true, result);
                Assert.IsNotNull(message);
                Assert.AreNotEqual(string.Empty, message);
            }

            // 2つのメッセージの内容が一致しないケース
            // 2つのメッセージの要素数が一致しないケース
            {
                storageConfigs = new List<DtStorageConfig>();
                scriptConfigs = new List<DtScriptConfig>();

                for (int i = 1; i <= DataSize + 1; i++)
                {
                    DtStorageConfig config = CreateStorageConfig(
                        string.Format(StorageName, i), string.Format(Url, i), string.Format(Sas, i));
                    storageConfigs.Add(config);
                }

                for (int i = 1; i <= DataSize + 1; i++)
                {
                    DtScriptConfig config = CreateScriptConfig(
                        string.Format(ScriptName, i), string.Format(FileName, i),
                        Version, string.Format(Location, i));
                    scriptConfigs.Add(config);
                }

                m2.SetScriptConfigs(scriptConfigs);
                m2.SetStorageConfigs(storageConfigs);

                bool result = ServerSyncMessage.Equals(m1, m2);
                string message = ServerSyncMessage.CreateJsonString(m2);

                Assert.AreEqual(false, result);
                Assert.IsNotNull(message);
                Assert.AreNotEqual(string.Empty, message);
            }

            // 2つのメッセージの内容が一致しないケース
            // 2つのメッセージの要素数は一致するが、Nameが一部一致しないケース
            {
                storageConfigs = new List<DtStorageConfig>();
                scriptConfigs = new List<DtScriptConfig>();

                for (int i = 1; i <= DataSize; i++)
                {
                    DtStorageConfig config;

                    if (i == 1)
                    {
                        config = CreateStorageConfig(
                            string.Format("NAME{0}", i),
                            string.Format(Url, i),
                            string.Format(Sas, i));
                    }
                    else
                    {
                        config = CreateStorageConfig(
                            string.Format(StorageName, i),
                            string.Format(Url, i),
                            string.Format(Sas, i));
                    }

                    storageConfigs.Add(config);
                }

                for (int i = 1; i <= DataSize; i++)
                {
                    DtScriptConfig config;

                    if (i == 1)
                    {
                        config = CreateScriptConfig(
                            string.Format("NAME{0}", i),
                            string.Format(FileName, i),
                            Version, string.Format(Location, i));
                    }
                    else
                    {
                        config = CreateScriptConfig(
                            string.Format(ScriptName, i),
                            string.Format(FileName, i),
                            Version, string.Format(Location, i));
                    }

                    scriptConfigs.Add(config);
                }

                m2.SetScriptConfigs(scriptConfigs);
                m2.SetStorageConfigs(storageConfigs);

                bool result = ServerSyncMessage.Equals(m1, m2);
                string message = ServerSyncMessage.CreateJsonString(m2);

                Assert.AreEqual(false, result);
                Assert.IsNotNull(message);
                Assert.AreNotEqual(string.Empty, message);
            }

            // 2つのメッセージの内容が一致しないケース
            // 2つのメッセージの要素数とNameは一致するが、内容（文字列等）が一致しないケース
            {
                storageConfigs = new List<DtStorageConfig>();
                scriptConfigs = new List<DtScriptConfig>();

                for (int i = 1; i <= DataSize; i++)
                {
                    DtStorageConfig config = CreateStorageConfig(
                        string.Format(StorageName, i), 
                        string.Format("URL{0}", i), 
                        string.Format("SAS{0}", i));
                    storageConfigs.Add(config);
                }

                for (int i = 1; i <= DataSize; i++)
                {
                    DtScriptConfig config = CreateScriptConfig(
                        string.Format(ScriptName, i), 
                        string.Format("FILENAME{0}", i),
                        Version, 
                        string.Format("LOCATION{0}", i));
                    scriptConfigs.Add(config);
                }

                m2.SetScriptConfigs(scriptConfigs);
                m2.SetStorageConfigs(storageConfigs);

                bool result = ServerSyncMessage.Equals(m1, m2);
                string message = ServerSyncMessage.CreateJsonString(m2);

                Assert.AreEqual(false, result);
                Assert.IsNotNull(message);
                Assert.AreNotEqual(string.Empty, message);
            }

            // 2つのメッセージの内容が一致しないケース
            // スクリプト設定のバージョンのみが一致しないケース
            {
                storageConfigs = new List<DtStorageConfig>();
                scriptConfigs = new List<DtScriptConfig>();

                for (int i = 1; i <= DataSize; i++)
                {
                    DtStorageConfig config = CreateStorageConfig(
                        string.Format(StorageName, i), string.Format(Url, i), string.Format(Sas, i));
                    storageConfigs.Add(config);
                }

                for (int i = 1; i <= DataSize; i++)
                {
                    DtScriptConfig config;

                    if (i == 1)
                    {
                        config = CreateScriptConfig(
                            string.Format(ScriptName, i), string.Format(FileName, i),
                            Version + 1, string.Format(Location, i));
                    }
                    else
                    {
                        config = CreateScriptConfig(
                            string.Format(ScriptName, i), string.Format(FileName, i),
                            Version, string.Format(Location, i));
                    }

                    scriptConfigs.Add(config);
                }

                m2.SetScriptConfigs(scriptConfigs);
                m2.SetStorageConfigs(storageConfigs);

                bool result = ServerSyncMessage.Equals(m1, m2);
                string message = ServerSyncMessage.CreateJsonString(m2);

                Assert.AreEqual(false, result);
                Assert.IsNotNull(message);
                Assert.AreNotEqual(string.Empty, message);
            }
        }

        /// <summary>
        /// 設定値データリストが空の場合にメッセージがnullや空文字列にならないこと、
        /// メッセージクラスのスクリプトおよびストレージ辞書がnullでないことを確認するテスト
        /// </summary>
        [TestMethod]
        public void EqualsTestWithEmpty()
        {
            ServerSyncMessage m = new ServerSyncMessage();
            List<DtScriptConfig> scriptConfigs = new List<DtScriptConfig>();
            List<DtStorageConfig> storageConfigs = new List<DtStorageConfig>();
            m.SetScriptConfigs(scriptConfigs);
            m.SetStorageConfigs(storageConfigs);

            string message = ServerSyncMessage.CreateJsonString(m);

            Assert.IsNotNull(message);
            Assert.AreNotEqual(string.Empty, message);
            Assert.IsNotNull(m.Script);
            Assert.IsNotNull(m.Storage);
        }

        #endregion

        #region SetStorageConfig

        /// <summary>
        /// ストレージ設定
        /// </summary>
        [TestMethod]
        public void SetStorageConfigsTest()
        {
            ServerSyncMessage m;
            List<DtStorageConfig> storageConfigs;

            // 正常系
            {
                m = new ServerSyncMessage();
                storageConfigs = new List<DtStorageConfig>();

                DtStorageConfig config = CreateStorageConfig("TestA", "UrlA", "SasA");
                storageConfigs.Add(config);
                config = CreateStorageConfig("TestB", "UrlB", "SasB");
                storageConfigs.Add(config);

                bool result = m.SetStorageConfigs(storageConfigs);
                string message = ServerSyncMessage.CreateJsonString(m);

                Assert.AreEqual(2, m.Storage.Count);
                Assert.AreEqual(true, result);
                Assert.IsNotNull(m.Script);
                Assert.IsNotNull(m.Storage);
                Assert.IsNotNull(message);
                Assert.AreNotEqual(string.Empty, message);
            }

            // nullを設定したケース
            {
                m = new ServerSyncMessage();
                storageConfigs = new List<DtStorageConfig>();

                bool result = m.SetStorageConfigs(null);
                string message = ServerSyncMessage.CreateJsonString(m);

                Assert.AreEqual(false, result);
                Assert.IsNotNull(m.Script);
                Assert.IsNotNull(m.Storage);
                Assert.IsNotNull(message);
                Assert.AreNotEqual(string.Empty, message);
            }

            // 空のリストを設定したケース
            {
                m = new ServerSyncMessage();
                storageConfigs = new List<DtStorageConfig>();

                bool result = m.SetStorageConfigs(storageConfigs);
                string message = ServerSyncMessage.CreateJsonString(m);

                Assert.AreEqual(true, result);
                Assert.IsNotNull(m.Script);
                Assert.IsNotNull(m.Storage);
                Assert.IsNotNull(message);
                Assert.AreNotEqual(string.Empty, message);
            }
        }

        #endregion

        #region SetScriptConfig

        /// <summary>
        /// スクリプト設定
        /// </summary>
        [TestMethod]
        public void SetScriptConfigsTest()
        {
            ServerSyncMessage m;
            List<DtScriptConfig> scriptConfigs;

            // 同じ名称だがバージョンの異なるレコードがDBに存在するケース
            {
                m = new ServerSyncMessage();
                scriptConfigs = new List<DtScriptConfig>();

                DtScriptConfig config = CreateScriptConfig("TestA", "FileName", 1, "Location");
                scriptConfigs.Add(config);

                config = CreateScriptConfig("TestA", "FileName", 2, "Location");    // キー名が同じでバージョンが異なる要素を追加
                scriptConfigs.Add(config);

                bool result = m.SetScriptConfigs(scriptConfigs);
                string message = ServerSyncMessage.CreateJsonString(m);

                Assert.AreEqual(true, result);
                Assert.AreEqual(1, m.Script.Count);
                Assert.AreEqual(2, m.Script["TestA"].Version);
                Assert.IsNotNull(m.Script);
                Assert.IsNotNull(m.Storage);
                Assert.IsNotNull(message);
                Assert.AreNotEqual(string.Empty, message);
            }

            // 同じ名称だがバージョンの異なるレコードがDBに存在するケース
            // 最新のレコードがリストの先頭にある場合
            {
                m = new ServerSyncMessage();
                scriptConfigs = new List<DtScriptConfig>();

                DtScriptConfig config = CreateScriptConfig("TestA", "FileName", 2, "Location");
                scriptConfigs.Add(config);

                config = CreateScriptConfig("TestA", "FileName", 1, "Location");    // キー名が同じでバージョンが異なる要素を追加
                scriptConfigs.Add(config);

                bool result = m.SetScriptConfigs(scriptConfigs);
                string message = ServerSyncMessage.CreateJsonString(m);

                Assert.AreEqual(true, result);
                Assert.AreEqual(1, m.Script.Count);
                Assert.AreEqual(2, m.Script["TestA"].Version);
                Assert.IsNotNull(m.Script);
                Assert.IsNotNull(m.Storage);
                Assert.IsNotNull(message);
                Assert.AreNotEqual(string.Empty, message);
            }

            // nullを設定したケース
            {
                m = new ServerSyncMessage();
                scriptConfigs = new List<DtScriptConfig>();

                bool result = m.SetScriptConfigs(null);
                string message = ServerSyncMessage.CreateJsonString(m);

                Assert.AreEqual(false, result);
                Assert.IsNotNull(m.Script);
                Assert.IsNotNull(m.Storage);
                Assert.IsNotNull(message);
                Assert.AreNotEqual(string.Empty, message);
            }

            // 空のリストを設定したケース
            {
                m = new ServerSyncMessage();
                scriptConfigs = new List<DtScriptConfig>();

                bool result = m.SetScriptConfigs(scriptConfigs);
                string message = ServerSyncMessage.CreateJsonString(m);

                Assert.AreEqual(true, result);
                Assert.IsNotNull(m.Script);
                Assert.IsNotNull(m.Storage);
                Assert.IsNotNull(message);
                Assert.AreNotEqual(string.Empty, message);
            }
        }

        #endregion

        #region データ生成用メソッド

        /// <summary>
        /// ストレージ設定データを生成する
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="url">Url</param>
        /// <param name="sas">Sas</param>
        /// <returns>ストレージ設定</returns>
        private DtStorageConfig CreateStorageConfig(string name, string url, string sas)
        {
            return new DtStorageConfig
            {
                Name = name,
                Url = url,
                Sas = sas
            };
        }

        /// <summary>
        /// スクリプト設定データを生成する
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="fileName">FileName</param>
        /// <param name="version">Version</param>
        /// <param name="location">Location</param>
        /// <returns>スクリプト設定</returns>
        private DtScriptConfig CreateScriptConfig(string name, string fileName, int version, string location)
        {
            return new DtScriptConfig
            {
                Name = name,
                FileName = fileName,
                Version = (short)version,
                Location = location
            };
        }

        #endregion
    }
}