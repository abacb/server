using Microsoft.Extensions.Logging;
using Rms.Server.Core.Utility;
using System;
using System.Runtime.CompilerServices;

namespace Rms.Server.Operation.Service.Services
{
    /// <summary>
    /// CleanDbService
    /// </summary>
    public class CleanDbService : Core.Service.Services.CleanDbService
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">AppSettings</param>
        /// <param name="logger">ロガー</param>
        /// <param name="serviceProvider">サービスプロバイダー</param>
        /// <param name="timeProvider">タイムプロバイダー</param>
        public CleanDbService(AppSettings settings, ILogger<CleanDbService> logger, IServiceProvider serviceProvider, ITimeProvider timeProvider)
            : base(settings, logger, serviceProvider, timeProvider)
        {
        }

        /// <summary>
        /// エンティティモデル名からリポジトリのフル名称を生成する際のフォーマットを取得する
        /// </summary>
        protected override string RepositoryNameFormat
        {
            get
            {
                return "Rms.Server.Operation.Abstraction.Repositories.{0}Repository, Rms.Server.Operation.Abstraction";
            }
        }

        /// <summary>
        /// Enterログを出力する
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="callerMemberName">呼び出し元メンバ名。指定不要。</param>
        /// <param name="callerFilePath">呼び出し元ファイル名。指定不要。</param>
        /// <param name="callerLineNumber">呼び出し元行番号。指定不要。</param>
        protected override void OutputLogEnter(ILogger logger, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Utility.Extensions.LoggerExtensions.Enter(logger, string.Empty, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Leaveログを出力する
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="callerMemberName">呼び出し元メンバ名。指定不要。</param>
        /// <param name="callerFilePath">呼び出し元ファイル名。指定不要。</param>
        /// <param name="callerLineNumber">呼び出し元行番号。指定不要。</param>
        protected override void OutputLogLeave(ILogger logger, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Utility.Extensions.LoggerExtensions.Leave(logger, string.Empty, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// メッセージコード001のログを出力する
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="ex">例外</param>
        /// <param name="callerMemberName">呼び出し元メンバ名。指定不要。</param>
        /// <param name="callerFilePath">呼び出し元ファイル名。指定不要。</param>
        /// <param name="callerLineNumber">呼び出し元行番号。指定不要。</param>
        protected override void OutputLog001(ILogger logger, Exception ex, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Utility.Extensions.LoggerExtensions.Error(logger, ex, nameof(Utility.Properties.Resources.OP_DBC_DBC_001), callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// メッセージコード002のログを出力する
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="ex">例外</param>
        /// <param name="args">メッセージ引数</param>
        /// <param name="callerMemberName">呼び出し元メンバ名。指定不要。</param>
        /// <param name="callerFilePath">呼び出し元ファイル名。指定不要。</param>
        /// <param name="callerLineNumber">呼び出し元行番号。指定不要。</param>
        protected override void OutputLog002(ILogger logger, Exception ex, object[] args, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Utility.Extensions.LoggerExtensions.Error(logger, ex, nameof(Utility.Properties.Resources.OP_DBC_DBC_002), args, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// メッセージコード003のログを出力する
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="ex">例外</param>
        /// <param name="args">メッセージ引数</param>
        /// <param name="callerMemberName">呼び出し元メンバ名。指定不要。</param>
        /// <param name="callerFilePath">呼び出し元ファイル名。指定不要。</param>
        /// <param name="callerLineNumber">呼び出し元行番号。指定不要。</param>
        protected override void OutputLog003(ILogger logger, Exception ex, object[] args, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Utility.Extensions.LoggerExtensions.Error(logger, ex, nameof(Utility.Properties.Resources.OP_DBC_DBC_003), args, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// メッセージコード004のログを出力する
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="args">メッセージ引数</param>
        /// <param name="callerMemberName">呼び出し元メンバ名。指定不要。</param>
        /// <param name="callerFilePath">呼び出し元ファイル名。指定不要。</param>
        /// <param name="callerLineNumber">呼び出し元行番号。指定不要。</param>
        protected override void OutputLog004(ILogger logger, object[] args, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Utility.Extensions.LoggerExtensions.Info(logger, nameof(Utility.Properties.Resources.OP_DBC_DBC_004), args, callerMemberName, callerFilePath, callerLineNumber);
        }
    }
}
