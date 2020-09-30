using Microsoft.Extensions.Logging;
using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Core.Service.Models;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Rms.Server.Core.Service.Services
{
    /// <summary>
    /// CleanDbService
    /// </summary>
    public class CleanDbService : ICleanDbService
    {
        /// <summary>
        /// AppSettings
        /// </summary>
        private readonly AppSettings _settings;

        /// <summary>
        /// ロガー
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// ServiceProvider
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// TimeProvider
        /// </summary>
        private readonly ITimeProvider _timeProvider;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">AppSettings</param>
        /// <param name="logger">ロガー</param>
        /// <param name="serviceProvider">サービスプロバイダー</param>
        /// <param name="timeProvider">タイムプロバイダー</param>
        public CleanDbService(
            AppSettings settings,
            ILogger<CleanDbService> logger,
            IServiceProvider serviceProvider,
            ITimeProvider timeProvider)
        {
            Assert.IfNull(settings);
            Assert.IfNull(logger);
            Assert.IfNull(serviceProvider);
            Assert.IfNull(timeProvider);

            _settings = settings;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _timeProvider = timeProvider;
        }

        /// <summary>
        /// Cleanメソッド内の処理ステータス
        /// </summary>
        protected enum CleanStatus
        {
            /// <summary>
            /// GetType呼び出し中
            /// </summary>
            GetType = 0,

            /// <summary>
            /// DeleteExceedsMonthsAllData呼び出し中
            /// </summary>
            DeleteExceedsMonthsAllData,
        }

        /// <summary>
        /// エンティティモデル名からリポジトリのフル名称を生成する際のフォーマットを取得する
        /// </summary>
        protected virtual string RepositoryNameFormat
        {
            get
            {
                return "Rms.Server.Core.Abstraction.Repositories.{0}Repository, Rms.Server.Core.Abstraction";
            }
        }

        /// <summary>
        /// 不要データの削除
        /// </summary>
        public void Clean()
        {
            try
            {
                OutputLogEnter(_logger);

                // 削除設定を取得する
                var targetSettings = _settings.GetConfigs(CleanDbSetting.KeyPrefix);

                // 正常設定されているデータ群の取得
                IEnumerable<CleanDbSetting> settings =
                    targetSettings.Select(x =>
                    {
                        try
                        {
                            return CleanDbSetting.Create(x.Key, x.Value, RepositoryNameFormat);
                        }
                        catch (RmsInvalidAppSettingException e)
                        {
                            OutputLog002(_logger, e, new object[] { e.Message });
                            return null;
                        }
                    })
                    .Where(x => x != null);

                foreach (var setting in settings)
                {
                    CleanStatus status = CleanStatus.GetType;

                    try
                    {
                        // 処理リポジトリがあるかチェック
                        Type repoType = Type.GetType(setting.RepositoryFullName);
                        if (repoType == null)
                        {
                            // 存在しないクラス指定なので処理しない
                            throw new RmsInvalidAppSettingException(string.Format("{0} is not supported.", CleanDbSetting.KeyPrefix + setting.ModelName));
                        }

                        ICleanRepository repository = _serviceProvider.GetService(repoType) as ICleanRepository;
                        if (repository == null)
                        {
                            // 削除対象のクラスでないので処理しない
                            throw new RmsInvalidAppSettingException(string.Format("{0} is not supported.", CleanDbSetting.KeyPrefix + setting.ModelName));
                        }

                        // DBから指定月数を超過しているデータの削除を依頼する
                        status = CleanStatus.DeleteExceedsMonthsAllData;
                        if (repository.DeleteExceedsMonthsAllData(CreateThreshold(setting)) > 0)
                        {
                            OutputLog004(_logger, new object[] { setting.RepositoryFullName });
                        }
                    }
                    catch (RmsInvalidAppSettingException e)
                    {
                        OutputLog002(_logger, e, new object[] { e.Message });
                    }
                    catch (Exception e)
                    {
                        switch (status)
                        {
                            case CleanStatus.DeleteExceedsMonthsAllData:
                                OutputLog003(_logger, e, new object[] { setting.RepositoryFullName });
                                break;
                            case CleanStatus.GetType:
                            default:
                                // GetTypeがnullを返さずにExceptionを投げた場合は、
                                // RmsInvalidAppSettingException発生時と同じ処理とする
                                OutputLog002(_logger, e, new object[] { e.Message });
                                break;
                        }
                    }
                }

                return;
            }
            catch (Exception ex)
            {
                OutputLog001(_logger, ex);
            }
            finally
            {
                OutputLogLeave(_logger);
            }
        }

        /// <summary>
        /// 閾値を作成する
        /// </summary>
        /// <param name="setting">設定</param>
        /// <returns>閾値</returns>
        protected DateTime CreateThreshold(CleanDbSetting setting)
        {
            // 年月まででそれ以下は考慮外という仕様
            DateTime thresholdMonth = _timeProvider.UtcNow.AddMonths(-setting.RetentionPeriodMonth);
            return new DateTime(thresholdMonth.Year, thresholdMonth.Month, 1);
        }

        /// <summary>
        /// Enterログを出力する
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="callerMemberName">呼び出し元メンバ名。指定不要。</param>
        /// <param name="callerFilePath">呼び出し元ファイル名。指定不要。</param>
        /// <param name="callerLineNumber">呼び出し元行番号。指定不要。</param>
        protected virtual void OutputLogEnter(ILogger logger, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
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
        protected virtual void OutputLogLeave(ILogger logger, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
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
        protected virtual void OutputLog001(ILogger logger, Exception ex, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Utility.Extensions.LoggerExtensions.Error(logger, ex, nameof(Utility.Properties.Resources.CO_DBC_DBC_001), callerMemberName, callerFilePath, callerLineNumber);
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
        protected virtual void OutputLog002(ILogger logger, Exception ex, object[] args, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Utility.Extensions.LoggerExtensions.Error(logger, ex, nameof(Utility.Properties.Resources.CO_DBC_DBC_002), args, callerMemberName, callerFilePath, callerLineNumber);
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
        protected virtual void OutputLog003(ILogger logger, Exception ex, object[] args, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Utility.Extensions.LoggerExtensions.Error(logger, ex, nameof(Utility.Properties.Resources.CO_DBC_DBC_003), args, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// メッセージコード004のログを出力する
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="args">メッセージ引数</param>
        /// <param name="callerMemberName">呼び出し元メンバ名。指定不要。</param>
        /// <param name="callerFilePath">呼び出し元ファイル名。指定不要。</param>
        /// <param name="callerLineNumber">呼び出し元行番号。指定不要。</param>
        protected virtual void OutputLog004(ILogger logger, object[] args, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Utility.Extensions.LoggerExtensions.Info(logger, nameof(Utility.Properties.Resources.CO_DBC_DBC_004), args, callerMemberName, callerFilePath, callerLineNumber);
        }
    }
}
