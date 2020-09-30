using Microsoft.Extensions.Logging;
using Rms.Server.Core.Utility.ExtensionsBase;
using System;
using System.Runtime.CompilerServices;

namespace Rms.Server.Operation.Utility.Extensions
{
    using Rms.Server.Operation.Utility.Properties;

    /// <summary>
    /// ILoggerの拡張メソッドクラス
    /// </summary>
    /// <remarks>
    /// ログの中に複数のプレースホルダーを用意する場合には、***Jsonメソッドは使用しないこと。
    /// ***Jsonメソッドは、引数をシリアライズしてJson文字列にしてからログに埋め込むため、プレースホルダー数は1でなければならない。
    ///     例）
    ///     ○ _logger.Enter("{0}, {1}", new object[] { a, b });     // ***Jsonメソッドでない場合には複数のプレースホルダーを用意してもよい
    ///     ○ _logger.EnterJson("{0}", new object[] { a, b });      // ***Jsonメソッドの場合には、プレースホルダーは1つでよい
    ///     × _logger.EnterJson("{0}, {1}", new object[] { a, b }); // ***Jsonメソッドの場合に、複数のプレースホルダーを使用することはできない
    /// </remarks>
    public static class LoggerExtensions
    {
        /// <summary>
        /// DEBUGレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="message">ログメッセージのフォーマット</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void Debug(this ILogger logger, string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Logger.Debug(logger, Resources.ResourceManager, Resources.Culture, message, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// DEBUGレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="message">ログメッセージのフォーマット</param>
        /// <param name="args">ログメッセージのフォーマットの引数</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void Debug(this ILogger logger, string message, object[] args, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Logger.Debug(logger, Resources.ResourceManager, Resources.Culture, message, args, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// DEBUGレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="message">ログメッセージのフォーマット</param>
        /// <param name="jsonObject">ログメッセージのフォーマットの引数（JSONに変換されます）</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void DebugJson(this ILogger logger, string message, object jsonObject, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Logger.DebugJson(logger, Resources.ResourceManager, Resources.Culture, message, jsonObject, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// INFOレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="messageCode">メッセージコード</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void Info(this ILogger logger, string messageCode, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Logger.Info(logger, Resources.ResourceManager, Resources.Culture, messageCode, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// INFOレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="messageCode">メッセージコード</param>
        /// <param name="args">ログメッセージのフォーマットの引数</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void Info(this ILogger logger, string messageCode, object[] args, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Logger.Info(logger, Resources.ResourceManager, Resources.Culture, messageCode, args, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// INFOレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="messageCode">メッセージコード</param>
        /// <param name="jsonObject">ログメッセージのフォーマットの引数（JSONに変換されます）</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void InfoJson(this ILogger logger, string messageCode, object jsonObject, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Logger.InfoJson(logger, Resources.ResourceManager, Resources.Culture, messageCode, jsonObject, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// WARNレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="messageCode">メッセージコード</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void Warn(this ILogger logger, string messageCode, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Logger.Warn(logger, Resources.ResourceManager, Resources.Culture, messageCode, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// WARNレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="messageCode">メッセージコード</param>
        /// <param name="args">ログメッセージのフォーマットの引数</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void Warn(this ILogger logger, string messageCode, object[] args, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Logger.Warn(logger, Resources.ResourceManager, Resources.Culture, messageCode, args, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// WARNレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="exception">例外</param>
        /// <param name="messageCode">メッセージコード</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void Warn(this ILogger logger, Exception exception, string messageCode, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Logger.Warn(logger, Resources.ResourceManager, Resources.Culture, exception, messageCode, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// WARNレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="exception">例外</param>
        /// <param name="messageCode">メッセージコード</param>
        /// <param name="args">ログメッセージのフォーマットの引数</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void Warn(this ILogger logger, Exception exception, string messageCode, object[] args, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Logger.Warn(logger, Resources.ResourceManager, Resources.Culture, exception, messageCode, args, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// WARNレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="messageCode">メッセージコード</param>
        /// <param name="jsonObject">ログメッセージのフォーマットの引数（JSONに変換されます）</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void WarnJson(this ILogger logger, string messageCode, object jsonObject, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Logger.WarnJson(logger, Resources.ResourceManager, Resources.Culture, messageCode, jsonObject, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// WARNレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="exception">例外</param>
        /// <param name="messageCode">ログメッセージのフォーマット</param>
        /// <param name="jsonObject">ログメッセージのフォーマットの引数（JSONに変換されます）</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void WarnJson(this ILogger logger, Exception exception, string messageCode, object jsonObject, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Logger.WarnJson(logger, Resources.ResourceManager, Resources.Culture, exception, messageCode, jsonObject, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// ERRORレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="messageCode">メッセージコード</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void Error(this ILogger logger, string messageCode, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Logger.Error(logger, Resources.ResourceManager, Resources.Culture, messageCode, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// ERRORレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="messageCode">メッセージコード</param>
        /// <param name="args">ログメッセージのフォーマットの引数</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void Error(this ILogger logger, string messageCode, object[] args, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Logger.Error(logger, Resources.ResourceManager, Resources.Culture, messageCode, args, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// ERRORレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="exception">例外</param>
        /// <param name="messageCode">メッセージコード</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void Error(this ILogger logger, Exception exception, string messageCode, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Logger.Error(logger, Resources.ResourceManager, Resources.Culture, exception, messageCode, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// ERRORレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="exception">例外</param>
        /// <param name="messageCode">メッセージコード</param>
        /// <param name="args">ログメッセージのフォーマットの引数</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void Error(this ILogger logger, Exception exception, string messageCode, object[] args, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Logger.Error(logger, Resources.ResourceManager, Resources.Culture, exception, messageCode, args, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// ERRORレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="messageCode">ログメッセージのフォーマット</param>
        /// <param name="jsonObject">ログメッセージのフォーマットの引数（JSONに変換されます）</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void ErrorJson(this ILogger logger, string messageCode, object jsonObject, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Logger.ErrorJson(logger, Resources.ResourceManager, Resources.Culture, messageCode, jsonObject, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// ERRORレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="exception">例外</param>
        /// <param name="messageCode">ログメッセージのフォーマット</param>
        /// <param name="jsonObject">ログメッセージのフォーマットの引数（JSONに変換されます）</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void ErrorJson(this ILogger logger, Exception exception, string messageCode, object jsonObject, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Logger.ErrorJson(logger, Resources.ResourceManager, Resources.Culture, exception, messageCode, jsonObject, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// 処理開始時のログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="message">ログメッセージのフォーマット</param>
        /// <param name="args">ログメッセージのフォーマットの引数</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void Enter(this ILogger logger, string message, object[] args, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Logger.Enter(logger, Resources.ResourceManager, Resources.Culture, message, args, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// 処理開始時のログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="message">ログメッセージのフォーマット</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void Enter(this ILogger logger, string message = "", [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Logger.Enter(logger, Resources.ResourceManager, Resources.Culture, message, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// 処理開始時のログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="message">ログメッセージのフォーマット</param>
        /// <param name="jsonObject">ログメッセージのフォーマットの引数（JSONに変換されます）</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void EnterJson(this ILogger logger, string message, object jsonObject, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Logger.EnterJson(logger, Resources.ResourceManager, Resources.Culture, message, jsonObject, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// 処理終了時のログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="message">ログメッセージのフォーマット</param>
        /// <param name="args">ログメッセージのフォーマットの引数</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void Leave(this ILogger logger, string message, object[] args, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Logger.Leave(logger, Resources.ResourceManager, Resources.Culture, message, args, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// 処理終了時のログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="message">ログメッセージのフォーマット</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void Leave(this ILogger logger, string message = "", [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Logger.Leave(logger, Resources.ResourceManager, Resources.Culture, message, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// 処理終了時のログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="message">ログメッセージのフォーマット</param>
        /// <param name="jsonObject">ログメッセージのフォーマットの引数（JSONに変換されます）</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void LeaveJson(this ILogger logger, string message, object jsonObject, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Logger.LeaveJson(logger, Resources.ResourceManager, Resources.Culture, message, jsonObject, callerMemberName, callerFilePath, callerLineNumber);
        }
    }
}
