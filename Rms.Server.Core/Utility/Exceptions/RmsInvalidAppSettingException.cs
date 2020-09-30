using System;
using System.Collections.Generic;
using System.Text;

namespace Rms.Server.Core.Utility.Exceptions
{
    /// <summary>
    /// RmsInvalidAppSettingException
    /// </summary>
    public class RmsInvalidAppSettingException : RmsException
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RmsInvalidAppSettingException() : base()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">例外の原因を説明するエラーメッセージ</param>
        public RmsInvalidAppSettingException(string message) : base(message)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">例外の原因を説明するエラーメッセージ</param>
        /// <param name="innerException">現在の例外の原因となった例外。</param>
        public RmsInvalidAppSettingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
