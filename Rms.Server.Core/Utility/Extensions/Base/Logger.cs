using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Resources;

namespace Rms.Server.Core.Utility.ExtensionsBase
{
    /// <summary>
    /// ログ出力クラス
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// ログメッセージのフォーマット
        /// </summary>
        /// <remarks>{0}：message、{1}：callerMemberName、{2}：callerFilePath、{3}：callerLineNumber、{4}：メッセージコード</remarks>
        private static readonly string StandardMessageFormat = "[{2}({3})][{1}][{4}] {0}";

        /// <summary>
        /// ログメッセージのフォーマット
        /// </summary>
        /// <remarks>{0}：message、{1}：callerMemberName、{2}：callerFilePath、{3}：callerLineNumber</remarks>
        private static readonly string SimpleMessageFormat = "[{2}({3})][{1}] {0}";

        /// <summary>
        /// ログメッセージのフォーマット
        /// </summary>
        /// <remarks>{0}：message、{1}：callerMemberName、{2}：callerFilePath、{3}：callerLineNumber</remarks>
        private static readonly string EnterMessageFormat = "[{2}({3})][{1}](START) {0}";

        /// <summary>
        /// ログメッセージのフォーマット
        /// </summary>
        /// <remarks>{0}：message、{1}：callerMemberName、{2}：callerFilePath、{3}：callerLineNumber</remarks>
        private static readonly string LeaveMessageFormat = "[{2}({3})][{1}](END) {0}";

        /// <summary>
        /// DEBUGレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="resourceManager">Resources.ResourceManagerを指定する</param>
        /// <param name="cultureInfo">Resources.Cultureを指定する</param>
        /// <param name="message">ログメッセージのフォーマット</param>
        /// <param name="args">ログメッセージのフォーマットの引数</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void Debug(ILogger logger, ResourceManager resourceManager, CultureInfo cultureInfo, string message, object[] args, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            message = string.Format(SimpleMessageFormat, message, callerMemberName, Path.GetFileName(callerFilePath), callerLineNumber);
            logger.LogDebug(message, args);
        }

        /// <summary>
        /// DEBUGレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="resourceManager">Resources.ResourceManagerを指定する</param>
        /// <param name="cultureInfo">Resources.Cultureを指定する</param>
        /// <param name="message">ログメッセージのフォーマット</param>
        /// <param name="jsonObject">ログメッセージのフォーマットの引数（JSONに変換されます）</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void DebugJson(ILogger logger, ResourceManager resourceManager, CultureInfo cultureInfo, string message, object jsonObject, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            Debug(logger, resourceManager, cultureInfo, message, new object[] { ToJsonString(jsonObject) }, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// INFOレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="resourceManager">Resources.ResourceManagerを指定する</param>
        /// <param name="cultureInfo">Resources.Cultureを指定する</param>
        /// <param name="messageCode">メッセージコード</param>
        /// <param name="args">ログメッセージのフォーマットの引数</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void Info(ILogger logger, ResourceManager resourceManager, CultureInfo cultureInfo, string messageCode, object[] args, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            string message = resourceManager.GetString(messageCode, cultureInfo);
            message = string.Format(StandardMessageFormat, message, callerMemberName, Path.GetFileName(callerFilePath), callerLineNumber, messageCode);
            logger.LogInformation(message, args);
        }

        /// <summary>
        /// INFOレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="resourceManager">Resources.ResourceManagerを指定する</param>
        /// <param name="cultureInfo">Resources.Cultureを指定する</param>
        /// <param name="messageCode">メッセージコード</param>
        /// <param name="jsonObject">ログメッセージのフォーマットの引数（JSONに変換されます）</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void InfoJson(ILogger logger, ResourceManager resourceManager, CultureInfo cultureInfo, string messageCode, object jsonObject, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            Info(logger, resourceManager, cultureInfo, messageCode, new object[] { ToJsonString(jsonObject) }, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// WARNレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="resourceManager">Resources.ResourceManagerを指定する</param>
        /// <param name="cultureInfo">Resources.Cultureを指定する</param>
        /// <param name="messageCode">メッセージコード</param>
        /// <param name="args">ログメッセージのフォーマットの引数</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void Warn(ILogger logger, ResourceManager resourceManager, CultureInfo cultureInfo, string messageCode, object[] args, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            string message = resourceManager.GetString(messageCode, cultureInfo);
            message = string.Format(StandardMessageFormat, message, callerMemberName, Path.GetFileName(callerFilePath), callerLineNumber, messageCode);
            logger.LogWarning(message, args);
        }

        /// <summary>
        /// WARNレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="resourceManager">Resources.ResourceManagerを指定する</param>
        /// <param name="cultureInfo">Resources.Cultureを指定する</param>
        /// <param name="exception">例外</param>
        /// <param name="messageCode">メッセージコード</param>
        /// <param name="args">ログメッセージのフォーマットの引数</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void Warn(ILogger logger, ResourceManager resourceManager, CultureInfo cultureInfo, Exception exception, string messageCode, object[] args, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            string message = resourceManager.GetString(messageCode, cultureInfo);
            message = string.Format(StandardMessageFormat, message, callerMemberName, Path.GetFileName(callerFilePath), callerLineNumber, messageCode);
            logger.LogWarning(exception, message, args);
        }

        /// <summary>
        /// WARNレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="resourceManager">Resources.ResourceManagerを指定する</param>
        /// <param name="cultureInfo">Resources.Cultureを指定する</param>
        /// <param name="messageCode">メッセージコード</param>
        /// <param name="jsonObject">ログメッセージのフォーマットの引数（JSONに変換されます）</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void WarnJson(ILogger logger, ResourceManager resourceManager, CultureInfo cultureInfo, string messageCode, object jsonObject, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            Warn(logger, resourceManager, cultureInfo, messageCode, new object[] { ToJsonString(jsonObject) }, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// WARNレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="resourceManager">Resources.ResourceManagerを指定する</param>
        /// <param name="cultureInfo">Resources.Cultureを指定する</param>
        /// <param name="exception">例外</param>
        /// <param name="messageCode">ログメッセージのフォーマット</param>
        /// <param name="jsonObject">ログメッセージのフォーマットの引数（JSONに変換されます）</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void WarnJson(ILogger logger, ResourceManager resourceManager, CultureInfo cultureInfo, Exception exception, string messageCode, object jsonObject, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            Warn(logger, resourceManager, cultureInfo, exception, messageCode, new object[] { ToJsonString(jsonObject) }, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// ERRORレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="resourceManager">Resources.ResourceManagerを指定する</param>
        /// <param name="cultureInfo">Resources.Cultureを指定する</param>
        /// <param name="messageCode">メッセージコード</param>
        /// <param name="args">ログメッセージのフォーマットの引数</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void Error(ILogger logger, ResourceManager resourceManager, CultureInfo cultureInfo, string messageCode, object[] args, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            string message = resourceManager.GetString(messageCode, cultureInfo);
            message = string.Format(StandardMessageFormat, message, callerMemberName, Path.GetFileName(callerFilePath), callerLineNumber, messageCode);
            logger.LogError(message, args);
        }

        /// <summary>
        /// ERRORレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="resourceManager">Resources.ResourceManagerを指定する</param>
        /// <param name="cultureInfo">Resources.Cultureを指定する</param>
        /// <param name="exception">例外</param>
        /// <param name="messageCode">メッセージコード</param>
        /// <param name="args">ログメッセージのフォーマットの引数</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void Error(ILogger logger, ResourceManager resourceManager, CultureInfo cultureInfo, Exception exception, string messageCode, object[] args, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            string message = resourceManager.GetString(messageCode, cultureInfo);
            message = string.Format(StandardMessageFormat, message, callerMemberName, Path.GetFileName(callerFilePath), callerLineNumber, messageCode);
            logger.LogError(exception, message, args);
        }

        /// <summary>
        /// ERRORレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="resourceManager">Resources.ResourceManagerを指定する</param>
        /// <param name="cultureInfo">Resources.Cultureを指定する</param>
        /// <param name="messageCode">ログメッセージのフォーマット</param>
        /// <param name="jsonObject">ログメッセージのフォーマットの引数（JSONに変換されます）</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void ErrorJson(ILogger logger, ResourceManager resourceManager, CultureInfo cultureInfo, string messageCode, object jsonObject, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            Error(logger, resourceManager, cultureInfo, messageCode, new object[] { ToJsonString(jsonObject) }, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// ERRORレベルのログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="resourceManager">Resources.ResourceManagerを指定する</param>
        /// <param name="cultureInfo">Resources.Cultureを指定する</param>
        /// <param name="exception">例外</param>
        /// <param name="messageCode">ログメッセージのフォーマット</param>
        /// <param name="jsonObject">ログメッセージのフォーマットの引数（JSONに変換されます）</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void ErrorJson(ILogger logger, ResourceManager resourceManager, CultureInfo cultureInfo, Exception exception, string messageCode, object jsonObject, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            Error(logger, resourceManager, cultureInfo, exception, messageCode, new object[] { ToJsonString(jsonObject) }, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// 処理開始時のログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="resourceManager">Resources.ResourceManagerを指定する</param>
        /// <param name="cultureInfo">Resources.Cultureを指定する</param>
        /// <param name="message">ログメッセージのフォーマット</param>
        /// <param name="args">ログメッセージのフォーマットの引数</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void Enter(ILogger logger, ResourceManager resourceManager, CultureInfo cultureInfo, string message, object[] args, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            string formattedMessage = string.Format(EnterMessageFormat, message, callerMemberName, Path.GetFileName(callerFilePath), callerLineNumber);
            logger.LogDebug(formattedMessage, args);
        }

        /// <summary>
        /// 処理開始時のログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="resourceManager">Resources.ResourceManagerを指定する</param>
        /// <param name="cultureInfo">Resources.Cultureを指定する</param>
        /// <param name="message">ログメッセージのフォーマット</param>
        /// <param name="jsonObject">ログメッセージのフォーマットの引数（JSONに変換されます）</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void EnterJson(ILogger logger, ResourceManager resourceManager, CultureInfo cultureInfo, string message, object jsonObject, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            Enter(logger, resourceManager, cultureInfo, message, new object[] { ToJsonString(jsonObject) }, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// 処理終了時のログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="resourceManager">Resources.ResourceManagerを指定する</param>
        /// <param name="cultureInfo">Resources.Cultureを指定する</param>
        /// <param name="message">ログメッセージのフォーマット</param>
        /// <param name="args">ログメッセージのフォーマットの引数</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void Leave(ILogger logger, ResourceManager resourceManager, CultureInfo cultureInfo, string message, object[] args, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            string formattedMessage = string.Format(LeaveMessageFormat, message, callerMemberName, Path.GetFileName(callerFilePath), callerLineNumber);
            logger.LogDebug(formattedMessage, args);
        }

        /// <summary>
        /// 処理終了時のログを出力する
        /// </summary>
        /// <param name="logger">ILoggerインスタンス</param>
        /// <param name="resourceManager">Resources.ResourceManagerを指定する</param>
        /// <param name="cultureInfo">Resources.Cultureを指定する</param>
        /// <param name="message">ログメッセージのフォーマット</param>
        /// <param name="jsonObject">ログメッセージのフォーマットの引数（JSONに変換されます）</param>
        /// <param name="callerMemberName">メンバ名（指定不要）</param>
        /// <param name="callerFilePath">ファイルパス（指定不要）</param>
        /// <param name="callerLineNumber">行数（指定不要）</param>
        public static void LeaveJson(ILogger logger, ResourceManager resourceManager, CultureInfo cultureInfo, string message, object jsonObject, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            Leave(logger, resourceManager, cultureInfo, message, new object[] { ToJsonString(jsonObject) }, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// 引数に指定されたオブジェクトをJSON文字列に変換する
        /// </summary>
        /// <typeparam name="T">オブジェクトの型</typeparam>
        /// <param name="obj">オブジェクト</param>
        /// <returns>JSON文字列</returns>
        public static string ToJsonString<T>(T obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj);
            }
            catch
            {
#if DEBUG
                throw;
#else
                return "???";
#endif
            }
        }
    }
}
