using System;

namespace Rms.Server.Core.Utility
{
    /// <summary>
    /// アサート
    /// </summary>
    public static class Assert
    {
        /// <summary>
        /// Assertを行う。
        /// </summary>
        /// <param name="actual">対象</param>
        /// <param name="targetName">対象名</param>
        /// <param name="memberName">呼び出し元メンバ名</param>
        /// <param name="sourceFilePath">呼び出し元ファイルパス</param>
        /// <param name="sourceLineNumber">呼び出し元ファイル行数</param>
        public static void IfNullOrEmpty( 
            string actual,
            string targetName,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            if (string.IsNullOrWhiteSpace(actual)) 
            {
                throw new ArgumentNullException(CreateMessage($"{targetName} is null or empty or white space.", memberName, sourceFilePath, sourceLineNumber));
            }
        }

        /// <summary>
        /// Assertを行う。Nullの場合ArgumentNullExceptionを発行する。
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="actual">対象</param>
        /// <param name="memberName">呼び出し元メンバ名</param>
        /// <param name="sourceFilePath">呼び出し元ファイルパス</param>
        /// <param name="sourceLineNumber">呼び出し元ファイル行数</param>
        public static void IfNull<T>(
            T actual,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            if (actual == null)
            {
                throw new ArgumentNullException(CreateMessage($"{typeof(T)} is null.", memberName, sourceFilePath, sourceLineNumber));
            }
        }

        /// <summary>
        /// メッセージを作成する。
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="memberName">呼び出し元メンバ名</param>
        /// <param name="sourceFilePath">呼び出し元ファイルパス</param>
        /// <param name="sourceLineNumber">呼び出し元ファイル行数</param>
        /// <returns>作成したメッセージを返す</returns>
        private static string CreateMessage(
            string message,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            return string.Format("[{1}][{2}][{3}] {0}", message, memberName, sourceFilePath, sourceLineNumber);
        }
    }
}
