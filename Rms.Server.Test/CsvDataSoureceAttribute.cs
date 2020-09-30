using CsvHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Rms.Server.Test
{
    /// <summary>
    /// CsvDataSource
    /// </summary>
    /// <remarks>
    /// <para>CsvDataSourceアトリビュートは、Microsoft.VisualStudio.TestTools.UnitTesting.DataSourceAttribute と同等の機能を提供するアトリビュートです。</para>
    /// <para>.net coreでは、Microsoft.VisualStudio.TestTools.UnitTesting.DataSourceAttributeは非サポートのため、代替としてCsvDataSourceアトリビュートを使用してください。</para>
    /// <para>DataSource非サポートに関する情報：</para>
    /// <para>    https://docs.microsoft.com/ja-jp/visualstudio/test/how-to-create-a-data-driven-unit-test?view=vs-2019#add-a-testcontext-to-the-test-class </para>
    /// <para>DataSource代替手段に関する情報：</para>
    /// <para>    https://www.meziantou.net/mstest-v2-data-tests.htm </para>
    /// </remarks>
    public class CsvDataSoureceAttribute : Attribute, ITestDataSource
    {
        /// <summary>CSVファイルパス</summary>
        private string filepath;

        /// <summary>CSVファイルのエンコード</summary>
        private Encoding encoding = Encoding.UTF8;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="filepath">CSVファイルパス</param>
        public CsvDataSoureceAttribute(string filepath)
        {
            this.filepath = filepath;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="filepath">CSVファイルパス</param>
        /// <param name="encoding">CSVファイルのエンコード</param>
        public CsvDataSoureceAttribute(string filepath, Encoding encoding) : this(filepath)
        {
            this.encoding = encoding;
        }

        /// <summary>
        /// CSVファイルからテストデータを取得する
        /// </summary>
        /// <param name="methodInfo">テストメソッド</param>
        /// <returns>テストデータのリスト</returns>
        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            // テストメソッドの引数名一覧
            IEnumerable<string> parameterNames = methodInfo.GetParameters().Select(x => x.Name);

            // Csvファイル読み込み処理
            using (var reader = new CsvReader(new StreamReader(this.filepath, this.encoding), CultureInfo.InvariantCulture))
            {
                // next record
                while (reader.Read())
                {
                    // １レコードを、「項目名と値のDictionary」にマップ
                    IDictionary<string, object> dictionary = reader.GetRecord<dynamic>() as IDictionary<string, object>;

                    // "null"と記載されたデータはnullにする
                    var nullDataKeys = dictionary.Where(pair => pair.Value.Equals("null"))
                            .Select(pair => pair.Key)
                            .ToList();
                    foreach (var key in nullDataKeys)
                    {
                        dictionary[key] = null;
                    }

                    // 「項目名と値のDictionary」をobject配列に変換（テストメソッドの引数順）
                    object[] data = parameterNames.Select(x => dictionary.FirstOrDefault(y => string.Compare(x, y.Key, true) == 0).Value).ToArray();

                    yield return data;
                }
            }
        }

        /// <summary>
        /// テストの表示名
        /// </summary>
        /// <param name="methodInfo">テストメソッド</param>
        /// <param name="data">テストデータ</param>
        /// <returns>テストの表示名</returns>
        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            // テストメソッドの備考
            string remarks = string.Empty;

            if (data != null)
            {
                // テストメソッドの引数名一覧
                List<string> parameterNames = methodInfo.GetParameters().Select(x => x.Name.ToLower()).ToList();

                foreach (string searchParamName in new string[] { "no", "remarks" })
                {
                    int paramIndex = parameterNames.IndexOf(searchParamName);
                    if (paramIndex >= 0)
                    {
                        remarks += string.Format("[{0}]", data[paramIndex]);
                    }
                }

                if (string.IsNullOrEmpty(remarks))
                {
                    remarks = string.Format("({0})", string.Join(", ", data));
                }
            }

            return string.Format(CultureInfo.InvariantCulture, "{0}{1}", methodInfo.Name, remarks);
        }
    }
}
