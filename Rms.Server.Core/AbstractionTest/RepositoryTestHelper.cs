using System.Data.SqlClient;
using System.IO;

namespace Rms.Server.Core.AbstractionTest
{
    public class RepositoryTestHelper
    {
        /// <summary>
        /// DB接続文字列
        /// </summary>
        private static readonly string _dbConnectionString = new Utility.AppSettings().PrimaryDbConnectionString;

        /// <summary>
        /// 指定文字数の文字列を作成する
        /// </summary>
        /// <param name="length">文字数</param>
        /// <returns>文字数分の"a"の文字列</returns>
        public static string CreateSpecifiedNumberString(ushort length)
        {
            string ret = string.Empty;
            for (int i = 0; i < length; i++)
            {
                ret += "a";
            }

            return ret;
        }

        /// <summary>
        /// 挿入SQLを実行する
        /// </summary>
        /// <param name="sqlFilePath">SQLファイルパス</param>
        /// <returns>実行した/しなかった</returns>
        public static bool ExecInsertSql(string sqlFilePath)
        {
            bool doesExec = false;
            if (File.Exists(sqlFilePath))
            {
                using (SqlConnection connection = new SqlConnection(_dbConnectionString))
                {
                    connection.Open();

                    string cmdText = File.ReadAllText(sqlFilePath);
                    using (SqlCommand command = new SqlCommand(cmdText, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                doesExec = true;
            }

            return doesExec;
        }

        /// <summary>
        /// 削除SQLを実行する
        /// </summary>
        /// <param name="sqlFilePath">SQLファイルパス</param>
        /// <returns>実行した/しなかった</returns>
        public static bool ExecDeleteSql(string sqlFilePath)
        {
            bool doesExec = false;
            if (File.Exists(sqlFilePath))
            {
                using (SqlConnection connection = new SqlConnection(_dbConnectionString))
                {
                    connection.Open();

                    string cmdText = File.ReadAllText(sqlFilePath);
                    using (SqlCommand command = new SqlCommand(cmdText, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                doesExec = true;
            }

            return doesExec;
        }
    }
}
