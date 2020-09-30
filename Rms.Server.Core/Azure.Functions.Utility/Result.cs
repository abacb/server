using System;

namespace Rms.Server.Core.Azure.Functions.Utility
{
    /// <summary>
    /// 結果返却用クラス
    /// </summary>
    /// <typeparam name="T">Enumのクラス</typeparam>
    public class Result<T> where T : Enum
    {
        /// <summary>
        /// 結果の初期化コンストラクタ
        /// </summary>
        /// <param name="resultCode">結果コード(型指定)</param>
        /// <param name="message">結果メッセージ</param>
        public Result(T resultCode, string message)
        {
            ResultCode = resultCode;
            Message = message;
        }

        /// <summary>
        /// 結果コード(型指定)
        /// </summary>
        public T ResultCode { get; }

        /// <summary>
        /// 結果メッセージ
        /// </summary>
        public string Message { get; }
    }
}
