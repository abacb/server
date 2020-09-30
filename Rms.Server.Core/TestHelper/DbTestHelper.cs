using CsvHelper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rms.Server.Core.Abstraction.Repositories;
// using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Utility.Models.Entites;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace TestHelper
{
    /// <summary>
    /// DBを使用したテストのhelperクラス
    /// </summary>
    public static class DbTestHelper
    {
        public static InstancesOnDb CreateMasterTables(InstancesOnDb list = null)
        {
            if (list == null)
            {
                list = new InstancesOnDb();
            }

            var builder = new TestDiProviderBuilder();
            builder.ServiceCollection.AddTransient<IMtDeliveryFileTypeRepository, MtDeliveryFileTypeRepository>();
            var provider = builder.Build();

            // 配信ファイル種別マスタテーブル
            {
                var repository = provider.GetRequiredService<IMtDeliveryFileTypeRepository>();

                // A / Lソフト
                var newdata = new MtDeliveryFileType()
                {
                    Code = Rms.Server.Core.Utility.Const.DeliveryFileType.AlSoft
                };
                var result = repository.CreateMtDeliveryFileType(newdata);
                Assert.IsNotNull(result);
                list.Add(result);

                // コンソールのHotfix
                newdata = new MtDeliveryFileType()
                {
                    Code = Rms.Server.Core.Utility.Const.DeliveryFileType.HotFixConsole
                };
                result = repository.CreateMtDeliveryFileType(newdata);
                Assert.IsNotNull(result);
                list.Add(result);

                // HobbitのHotfix
                newdata = new MtDeliveryFileType()
                {
                    Code = Rms.Server.Core.Utility.Const.DeliveryFileType.HotFixHobbit
                };
                result = repository.CreateMtDeliveryFileType(newdata);
                Assert.IsNotNull(result);
                list.Add(result);

                // パッケージ
                newdata = new MtDeliveryFileType()
                {
                    Code = Rms.Server.Core.Utility.Const.DeliveryFileType.Package
                };
                result = repository.CreateMtDeliveryFileType(newdata);
                Assert.IsNotNull(result);
                list.Add(result);
            }

            // 接続ステータスマスタテーブル
            {
                var repository = provider.GetRequiredService<IMtConnectStatusRepository>();

                // 未接続（接続歴なし）
                var connectStatusNewData = new MtConnectStatus()
                {
                    Code = Rms.Server.Core.Utility.Const.ConnectStatus.Unconnected
                };
                var result = repository.CreateMtConnectStatus(connectStatusNewData);
                Assert.IsNotNull(result);
                list.Add(result);

                // 接続中
                connectStatusNewData = new MtConnectStatus()
                {
                    Code = Rms.Server.Core.Utility.Const.ConnectStatus.Connected
                };
                result = repository.CreateMtConnectStatus(connectStatusNewData);
                Assert.IsNotNull(result);
                list.Add(result);

                // 切断
                connectStatusNewData = new MtConnectStatus()
                {
                    Code = Rms.Server.Core.Utility.Const.ConnectStatus.Disconnected
                };
                result = repository.CreateMtConnectStatus(connectStatusNewData);
                Assert.IsNotNull(result);
                list.Add(result);
            }

            // 配信グループステータスマスタテーブル
            {
                var repository = provider.GetRequiredService<IMtDeliveryGroupStatusRepository>();

                // 未開始（開始前）
                var newdata = new MtDeliveryGroupStatus()
                {
                    Code = Rms.Server.Core.Utility.Const.DeliveryGroupStatus.NotStarted
                };
                var result = repository.CreateMtDeliveryGroupStatus(newdata);
                Assert.IsNotNull(result);
                list.Add(result);

                // 開始済み
                newdata = new MtDeliveryGroupStatus()
                {
                    Code = Rms.Server.Core.Utility.Const.DeliveryGroupStatus.Started
                };
                result = repository.CreateMtDeliveryGroupStatus(newdata);
                Assert.IsNotNull(result);
                list.Add(result);

                // 完了
                newdata = new MtDeliveryGroupStatus()
                {
                    Code = Rms.Server.Core.Utility.Const.DeliveryGroupStatus.Completed
                };
                result = repository.CreateMtDeliveryGroupStatus(newdata);
                Assert.IsNotNull(result);
                list.Add(result);
            }

            // 適用結果ステータスマスタテーブル
            {
                var repository = provider.GetRequiredService<IMtInstallResultStatusRepository>();

                // 未開始
                var newdata = new MtInstallResultStatus()
                {
                    Code = Rms.Server.Core.Utility.Const.InstallResultStatus.NotStarted
                };
                var result = repository.CreateMtInstallResultStatus(newdata);
                Assert.IsNotNull(result);
                list.Add(result);

                // メッセージ送信済み
                newdata = new MtInstallResultStatus()
                {
                    Code = Rms.Server.Core.Utility.Const.InstallResultStatus.MessageSent
                };
                result = repository.CreateMtInstallResultStatus(newdata);
                Assert.IsNotNull(result);
                list.Add(result);

                // 対象外
                newdata = new MtInstallResultStatus()
                {
                    Code = Rms.Server.Core.Utility.Const.InstallResultStatus.Nottarget
                };
                result = repository.CreateMtInstallResultStatus(newdata);
                Assert.IsNotNull(result);
                list.Add(result);

                // ダウンロード済み
                newdata = new MtInstallResultStatus()
                {
                    Code = Rms.Server.Core.Utility.Const.InstallResultStatus.Downloaded
                };
                result = repository.CreateMtInstallResultStatus(newdata);
                Assert.IsNotNull(result);
                list.Add(result);

                // 配布中
                newdata = new MtInstallResultStatus()
                {
                    Code = Rms.Server.Core.Utility.Const.InstallResultStatus.Dispatching
                };
                result = repository.CreateMtInstallResultStatus(newdata);
                Assert.IsNotNull(result);
                list.Add(result);

                // 配布済み
                newdata = new MtInstallResultStatus()
                {
                    Code = Rms.Server.Core.Utility.Const.InstallResultStatus.Dispatched
                };
                result = repository.CreateMtInstallResultStatus(newdata);
                Assert.IsNotNull(result);
                list.Add(result);

                // ユーザによる適用キャンセル
                newdata = new MtInstallResultStatus()
                {
                    Code = Rms.Server.Core.Utility.Const.InstallResultStatus.Usercanceled
                };
                result = repository.CreateMtInstallResultStatus(newdata);
                Assert.IsNotNull(result);
                list.Add(result);

                // 適用済み
                newdata = new MtInstallResultStatus()
                {
                    Code = Rms.Server.Core.Utility.Const.InstallResultStatus.Installed
                };
                result = repository.CreateMtInstallResultStatus(newdata);
                Assert.IsNotNull(result);
                list.Add(result);
            }

            // 機器分類マスタテーブル
            {
                var repository = provider.GetRequiredService<IMtEquipmentTypeRepository>();

                // ゲートウェイ
                var newdata = new MtEquipmentType()
                {
                    Code = Rms.Server.Core.Utility.Const.EquipmentType.Gateway
                };
                var result = repository.CreateMtEquipmentType(newdata);
                Assert.IsNotNull(result);
                list.Add(result);

                // デバイス
                newdata = new MtEquipmentType()
                {
                    Code = Rms.Server.Core.Utility.Const.EquipmentType.Device
                };
                result = repository.CreateMtEquipmentType(newdata);
                Assert.IsNotNull(result);
                list.Add(result);

                // ユニット
                newdata = new MtEquipmentType()
                {
                    Code = Rms.Server.Core.Utility.Const.EquipmentType.Unit
                };
                result = repository.CreateMtEquipmentType(newdata);
                Assert.IsNotNull(result);
                list.Add(result);
            }

            // 機器型式マスタテーブル
            {
                var repository = provider.GetRequiredService<IMtEquipmentModelRepository>();
                var newdata = new MtEquipmentModel()
                {
                    EquipmentTypeSid = list.GetMtEquipmentlTypeSid(Rms.Server.Core.Utility.Const.EquipmentType.Gateway),
                    Code = "model1"
                };
                var result = repository.CreateMtEquipmentModel(newdata);
                Assert.IsNotNull(result);
                list.Add(result);

                newdata = new MtEquipmentModel()
                {
                    EquipmentTypeSid = list.GetMtEquipmentlTypeSid(Rms.Server.Core.Utility.Const.EquipmentType.Gateway),
                    Code = "model2"
                };
                result = repository.CreateMtEquipmentModel(newdata);
                Assert.IsNotNull(result);
                list.Add(result);

                newdata = new MtEquipmentModel()
                {
                    EquipmentTypeSid = list.GetMtEquipmentlTypeSid(Rms.Server.Core.Utility.Const.EquipmentType.Gateway),
                    Code = "model3"
                };
                result = repository.CreateMtEquipmentModel(newdata);
                Assert.IsNotNull(result);
                list.Add(result);
            }

            // インストールタイプマスタテーブル
            {
                var repository = provider.GetRequiredService<IMtInstallTypeRepository>();

                // RSPC
                var newdata = new MtInstallType()
                {
                    EquipmentTypeSid = list.GetMtEquipmentlTypeSid(Rms.Server.Core.Utility.Const.EquipmentType.Gateway),
                    Code = Rms.Server.Core.Utility.Const.InstallType.Rspc
                };
                var result = repository.CreateMtInstallType(newdata);
                Assert.IsNotNull(result);
                list.Add(result);

                // Console
                newdata = new MtInstallType()
                {
                    EquipmentTypeSid = list.GetMtEquipmentlTypeSid(Rms.Server.Core.Utility.Const.EquipmentType.Gateway),
                    Code = Rms.Server.Core.Utility.Const.InstallType.ConsoleRspc
                };
                result = repository.CreateMtInstallType(newdata);
                Assert.IsNotNull(result);
                list.Add(result);

                // Console（RSPCレス）
                newdata = new MtInstallType()
                {
                    EquipmentTypeSid = list.GetMtEquipmentlTypeSid(Rms.Server.Core.Utility.Const.EquipmentType.Gateway),
                    Code = Rms.Server.Core.Utility.Const.InstallType.ConsoleRspcless
                };
                result = repository.CreateMtInstallType(newdata);
                Assert.IsNotNull(result);
                list.Add(result);

                // Console MINI（単独）
                newdata = new MtInstallType()
                {
                    EquipmentTypeSid = list.GetMtEquipmentlTypeSid(Rms.Server.Core.Utility.Const.EquipmentType.Gateway),
                    Code = Rms.Server.Core.Utility.Const.InstallType.ConsoleMini
                };
                result = repository.CreateMtInstallType(newdata);
                Assert.IsNotNull(result);
                list.Add(result);

                // Console MINI（Hobbit連携）
                newdata = new MtInstallType()
                {
                    EquipmentTypeSid = list.GetMtEquipmentlTypeSid(Rms.Server.Core.Utility.Const.EquipmentType.Gateway),
                    Code = Rms.Server.Core.Utility.Const.InstallType.ConsoleMiniHobbit
                };
                result = repository.CreateMtInstallType(newdata);
                Assert.IsNotNull(result);
                list.Add(result);

                // Hobbitサーバ
                newdata = new MtInstallType()
                {
                    EquipmentTypeSid = list.GetMtEquipmentlTypeSid(Rms.Server.Core.Utility.Const.EquipmentType.Gateway),
                    Code = Rms.Server.Core.Utility.Const.InstallType.HobbitServer
                };
                result = repository.CreateMtInstallType(newdata);
                Assert.IsNotNull(result);
                list.Add(result);

                // Hobbitクライアント
                newdata = new MtInstallType()
                {
                    EquipmentTypeSid = list.GetMtEquipmentlTypeSid(Rms.Server.Core.Utility.Const.EquipmentType.Gateway),
                    Code = Rms.Server.Core.Utility.Const.InstallType.HobbitClient
                };
                result = repository.CreateMtInstallType(newdata);
                Assert.IsNotNull(result);
                list.Add(result);

                // リモート端末
                newdata = new MtInstallType()
                {
                    EquipmentTypeSid = list.GetMtEquipmentlTypeSid(Rms.Server.Core.Utility.Const.EquipmentType.Gateway),
                    Code = Rms.Server.Core.Utility.Const.InstallType.Remote
                };
                result = repository.CreateMtInstallType(newdata);
                Assert.IsNotNull(result);
                list.Add(result);
            }

            return list;
        }

        public static InstancesOnDb CreateDevices(InstancesOnDb list = null)
        {
            if (list == null)
            {
                list = new InstancesOnDb();
            }

            var builder = new TestDiProviderBuilder();
            builder.ServiceCollection.AddTransient<IMtDeliveryFileTypeRepository, MtDeliveryFileTypeRepository>();
            using (var provider = builder.Build())
            {
                // 端末データテーブル
                {
                    var deviceNewData = new DtDevice()
                    {
                        EquipmentModelSid = list.GetMtEquipmentModel().Sid,
                        InstallTypeSid = list.GetMtInstallType().Sid,
                        ConnectStatusSid = list.GetMtConnectStatusSid(),
                        EdgeId = Guid.NewGuid(),
                        EquipmentUid = Guid.NewGuid().ToString().Substring(0, 29),
                        RemoteConnectUid = Guid.NewGuid().ToString().Substring(0, 29)
                    };
                    var deviceRepository = provider.GetRequiredService<IDtDeviceRepository>();
                    var deviceCreateResult = deviceRepository.CreateDtDevice(deviceNewData);
                    Assert.IsNotNull(deviceCreateResult);
                    // deviceSid = deviceCreateResult.Entity.Sid;
                    list.Add(deviceCreateResult);
                }
            }

            return list;
        }

        public static InstancesOnDb CreateDeliveries(InstancesOnDb list = null)
        {
            if (list == null)
            {
                list = new InstancesOnDb();
            }

            var builder = new TestDiProviderBuilder();
            builder.ServiceCollection.AddTransient<IMtDeliveryFileTypeRepository, MtDeliveryFileTypeRepository>();
            var provider = builder.Build();

            {
                var deliveryFileNewData = new DtDeliveryFile()
                {
                    DeliveryFileTypeSid = list.Get<MtDeliveryFileType>()[0].Sid,
                    InstallTypeSid = list.GetMtInstallType().Sid,
                    FilePath = "folderName/file",
                    //ContainerName = "Container",
                };
                var repository = provider.GetRequiredService<IDtDeliveryFileRepository>();
                var result = repository.CreateDtDeliveryFile(deliveryFileNewData);
                Assert.IsNotNull(result);
                list.Add(result);
            }

            return list;
        }

        /// <summary>
        /// 対象DBの全データを削除する
        /// </summary>
        public static void DeleteAll()
        {
            // 接続文字列取得
            string connectionString = new Rms.Server.Core.Utility.AppSettings().PrimaryDbConnectionString;

            var sql = File.ReadAllText("resources/DeleteDeliveries.sql");
            ExecuteSql(sql, connectionString);

            sql = File.ReadAllText("resources/DeleteDevices.sql");
            ExecuteSql(sql, connectionString);

            sql = File.ReadAllText("resources/DeleteMasters.sql");
            ExecuteSql(sql, connectionString);
        }

        /// <summary>
        /// 対象DBの全データを削除して、SIDを1から再スタートさせるようにする
        /// </summary>
        public static void DeleteAllReseed()
        {
            // 接続文字列取得
            string connectionString = new Rms.Server.Core.Utility.AppSettings().PrimaryDbConnectionString;

            var sql = File.ReadAllText("resources/DeleteDeliveriesReseed.sql");
            ExecuteSql(sql, connectionString);

            sql = File.ReadAllText("resources/DeleteDevicesReseed.sql");
            ExecuteSql(sql, connectionString);

            sql = File.ReadAllText("resources/DeleteMastersReseed.sql");
            ExecuteSql(sql, connectionString);
        }

        /// <summary>
        /// テーブルへのデータ登録を行うSQL文を実行する
        /// </summary>
        /// <param name="tblName">テーブル名(キャメルケースでも可)</param>
        /// <param name="columnValueDic">カラム名(キャメルケースでも可)と値の組み合わせDictionary</param>
        public static void ExecuteInsertSqlCommand(string tblName, Dictionary<string, object> columnValueDic)
        {
            if(tblName == null || columnValueDic == null || columnValueDic.Count == 0)
            {
                return;
            }

            string sqlCommand = "insert into core.";

            // 引数からsqlcommandを作成する
            sqlCommand += ToUpperSnakeCase(tblName) + "(";
            foreach(var key in columnValueDic.Keys)
            {
                // 引数名(カラム名)はアッパースネークケースにして羅列する
                sqlCommand += ToUpperSnakeCase(key) + ",";
            }
            sqlCommand = sqlCommand.Substring(0, sqlCommand.Length-1) + ") values (";
            foreach (var value in columnValueDic.Values)
            {
                if(value.GetType() == typeof(string) || value.GetType() == typeof(DateTime) || value.GetType() == typeof(bool))
                {
                    // 文字列扱いにして投入する
                    string strValue = "'" + value.ToString() + "'";
                    sqlCommand += strValue + ",";
                }
                else
                {
                    sqlCommand += value + ",";
                }
            }
            sqlCommand = sqlCommand.Substring(0, sqlCommand.Length - 1) + ")";

            // 接続文字列取得
            string connectionString = new Rms.Server.Core.Utility.AppSettings().PrimaryDbConnectionString;
            ExecuteSql(sqlCommand, connectionString);
        }

        /// <summary>
        /// SQLファイルの内容を実行する
        /// </summary>
        /// <param name="sqlFilePath">SQLファイルパス</param>
        /// <returns>実行した/しなかった</returns>
        public static bool ExecSqlFromFilePath(string sqlFilePath)
        {
            if (!File.Exists(sqlFilePath))
            {
                return false;
            }

            // 接続文字列取得
            string connectionString = new Rms.Server.Core.Utility.AppSettings().PrimaryDbConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string cmdText = File.ReadAllText(sqlFilePath);
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            return true;
        }

        /// <summary>
        /// 指定したSelect文でデータベースからTableDataを取得する
        /// </summary>
        /// <param name="commandText">Select文</param>
        /// <returns>データベースから取得したTableData</returns>
        public static DataTable SelectTable(string commandText)
        {
            // 接続文字列取得
            string connectionString = new Rms.Server.Core.Utility.AppSettings().PrimaryDbConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(commandText, connection))
            {
                try
                {
                    connection.Open();
                    var adapter = new SqlDataAdapter(command);

                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    return table;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// CSVファイルからTableDataを取得する
        /// </summary>
        /// <param name="filepath">CSVファイルのパス</param>
        /// <param name="tableName">テーブル名</param>
        /// <returns>CSVファイルから取得したTableData</returns>
        public static  DataTable SelectCsv(string filepath, string commandText)
        {
            DataTable table = new DataTable();
            if (!File.Exists(filepath))
            {
                return table;
            }

            // 接続文字列取得
            string connectionString = new Rms.Server.Core.Utility.AppSettings().PrimaryDbConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(commandText, connection))
            {
                try
                {
                    connection.Open();
                    var adapter = new SqlDataAdapter(command);

                    adapter.FillSchema(table, SchemaType.Source);

                    using (var streamReader = new StreamReader(filepath, Encoding.UTF8))
                    using (var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                    {
                        csv.Read();
                        csv.ReadHeader();
                        while (csv.Read())
                        {
                            var row = table.NewRow();
                            foreach (DataColumn column in table.Columns)
                            {
                                if (csv.GetField(column.ColumnName) == "null")
                                {
                                    row[column.ColumnName] = DBNull.Value;
                                }
                                else
                                {
                                    row[column.ColumnName] = csv.GetField(column.DataType, column.ColumnName);
                                }
                            }
                            table.Rows.Add(row);
                        }
                    }
                }
                finally
                {
                    connection.Close();
                }
            }

            return table;
        }

        /// <summary>
        /// 文字列をアッパースネークケースに変換する
        /// </summary>
        /// <param name="str">変換前文字列</param>
        /// <returns>変換後文字列</returns>
        private static string ToUpperSnakeCase(string str)
        {
            return Regex.Replace(str, "([a-z])([A-Z])", "$1_$2").ToUpper();
        }
        
        private static void ExecuteSql(string sql, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
