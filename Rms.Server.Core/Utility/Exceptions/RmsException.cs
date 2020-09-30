using System;

namespace Rms.Server.Core.Utility.Exceptions
{
    /// <summary>
    /// RMSで発生する例外の基本クラス
    /// </summary>
    public class RmsException : ApplicationException
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RmsException() : base()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">例外の原因を説明するエラーメッセージ</param>
        public RmsException(string message) : base(message)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">例外の原因を説明するエラーメッセージ</param>
        /// <param name="innerException">現在の例外の原因となった例外。</param>
        public RmsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
