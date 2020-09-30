using CsvHelper;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;

namespace Rms.Server.Test
{
    /// <summary>
    /// DataTableHelper
    /// </summary>
    public static class DataTableHelper
    {
        /// <summary>
        /// 指定したSelect文でデータベースからTableDataを取得する
        /// </summary>
        /// <param name="connectionString">DBの接続文字列</param>
        /// <param name="commandText">Select文</param>
        /// <returns>データベースから取得したTableData</returns>
        public static DataTable SelectTable(string connectionString, string commandText)
        {
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
        /// <param name="connectionString">DBの接続文字列</param>
        /// <param name="filepath">CSVファイルのパス</param>
        /// <param name="tableName">テーブル名</param>
        /// <param name="columnNames">カラム名リスト</param>
        /// <returns>CSVファイルから取得したTableData</returns>
        /// <returns></returns>
        public static DataTable SelectCsv(string connectionString, string filepath, string tableName, string columnNames = "*")
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(string.Format("SELECT {0} FROM {1}", columnNames, tableName), connection))
            {
                try
                {
                    connection.Open();
                    var adapter = new SqlDataAdapter(command);

                    DataTable table = new DataTable();
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

                    return table;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
