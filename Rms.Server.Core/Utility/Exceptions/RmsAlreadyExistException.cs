using System;
using System.Collections.Generic;
using System.Text;

namespace Rms.Server.Core.Utility.Exceptions
{
    /// <summary>
    /// 既に保存済みのメッセージをDBに保存しようとした場合の例外クラス
    /// </summary>
    public class RmsAlreadyExistException : RmsException
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RmsAlreadyExistException() : base()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">例外の原因を説明するエラーメッセージ</param>
        public RmsAlreadyExistException(string message) : base(message)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">例外の原因を説明するエラーメッセージ</param>
        /// <param name="innerException">現在の例外の原因となった例外。</param>
        public RmsAlreadyExistException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
