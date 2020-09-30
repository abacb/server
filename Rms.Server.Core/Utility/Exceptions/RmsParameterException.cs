using System;

namespace Rms.Server.Core.Utility.Exceptions
{
    /// <summary>
    /// パラメータ不正が発生した場合の例外クラス
    /// </summary>
    public class RmsParameterException : RmsException
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RmsParameterException() : base()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">例外の原因を説明するエラーメッセージ</param>
        public RmsParameterException(string message) : base(message)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">例外の原因を説明するエラーメッセージ</param>
        /// <param name="innerException">現在の例外の原因となった例外。</param>
        public RmsParameterException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
