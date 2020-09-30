using Newtonsoft.Json;
using System.Linq;
using System.Reflection;

namespace Rms.Server.Core.Utility
{
    ////[【C\#】リフレクションを使用してToString関数を手軽に実装する \- コガネブログ](http://baba-s.hatenablog.com/entry/2014/02/27/000000)

    /// <summary>
    /// object型の拡張メソッドを管理するクラス
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// 区切り記号として使用する文字列
        /// </summary>
        private const string SEPARATOR = ",";

        /// <summary>
        /// 複合書式指定文字列
        /// </summary>
        private const string FORMAT = "{0}:{1}";

#pragma warning disable SA1310 // FieldNamesMustNotContainUnderscore
        #region Generated Code
        /// <summary>
        /// 複合書式指定文字列(パラメタ名つき)
        /// </summary>
        private const string FORMAT_WITH_PARAMNAME = "<{0}[{1}]>";
        #endregion

        /// <summary>
        /// オブジェクトをシリアライズして返します
        /// </summary>
        /// <param name="obj">オブジェクト</param>
        /// <returns>nullの場合:NULL　それ以外の場合:オブジェクトをシリアライズ</returns>
        public static string ToStringJson(this object obj)
        {
            return
                obj == null ?
                "Null" :
                JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// オブジェクトをシリアライズして返します(インデント付き)
        /// </summary>
        /// <param name="obj">オブジェクト</param>
        /// <returns>nullの場合:NULL　それ以外の場合:オブジェクトをシリアライズ</returns>
        public static string ToStringJsonIndented(this object obj)
        {
            return
                obj == null ?
                "Null" :
                JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        /// <summary>
        /// すべての公開フィールドの情報を文字列にして返します
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="obj">オブジェクト</param>
        /// <returns>すべての公開フィールドの情報</returns>
        public static string ToStringFields<T>(this T obj)
        {
            return obj == null ?
                "Null" :
                string.Join(
                    SEPARATOR, 
                    obj.GetType()
                    .GetFields(BindingFlags.Instance | BindingFlags.Public)
                    .Select(c => string.Format(FORMAT, c.Name, c.GetValue(obj))));
        }

        /// <summary>
        /// すべての公開プロパティの情報を文字列にして返します
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="obj">オブジェクト</param>
        /// <returns>すべての公開プロパティの情報</returns>
        public static string ToStringProperties<T>(this T obj)
        {
            return obj == null ?
                "Null" :
                string.Join(
                    SEPARATOR,
                    obj.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(c => c.CanRead)
                    .Select(c => string.Format(FORMAT, c.Name, c.GetValue(obj, null))));
        }

        /// <summary>
        /// すべての公開フィールドと公開プロパティの情報を文字列にして返します
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="obj">オブジェクト</param>
        /// <returns> すべての公開フィールドと公開プロパティの情報</returns>
        public static string ToStringReflection<T>(this T obj)
        {
            return obj == null ?
                "Null" :
                string.Join(
                    SEPARATOR,
                    obj.ToStringFields(),
                    obj.ToStringProperties());
        }
    }
}
