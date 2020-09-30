using System;

namespace Rms.Server.Core.Utility.Exceptions
{
    /// <summary>
    /// 未配信ではない配信ファイル情報を変更しようとした場合に発生する例外クラス
    /// </summary>
    public class RmsCannotChangeDeliveredFileException : RmsException
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RmsCannotChangeDeliveredFileException() : base()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">例外の原因を説明するエラーメッセージ</param>
        public RmsCannotChangeDeliveredFileException(string message) : base(message)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">例外の原因を説明するエラーメッセージ</param>
        /// <param name="innerException">現在の例外の原因となった例外。</param>
        public RmsCannotChangeDeliveredFileException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
