using Polly;
using Polly.Retry;
using System;
using Rms.Server.Core.Utility;
using Microsoft.Azure.Devices.Common.Exceptions;
using Rms.Server.Core.Utility.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Rms.Server.Core.Abstraction.Pollies
{
    /// <summary>
    /// DB接続用のPollyクラス
    /// </summary>
    public class DBPolly
    {
        /// <summary>アプリケーション設定</summary>
        private readonly AppSettings settings;

        /// <summary>リトライポリシー</summary>
        private RetryPolicy retryPolicy;

        /// <summary>
        /// リトライしないエラー
        /// </summary>
        /// <remarks>
        /// 基本的なAzureのリトライポリシーに従い、確実にリトライ不要というものを対象としている。
        /// </remarks>
        private readonly List<Type> noRetryTypes = new List<Type>()
        {
            typeof(ArgumentException),
            typeof(UnauthorizedException),
            typeof(OperationCanceledException),
            typeof(RmsAlreadyExistException),
            typeof(ValidationException)
        };

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">アプリケーション設定</param>
        public DBPolly(AppSettings settings)
        {
            this.settings = settings;
            this.retryPolicy =
                Policy.Handle<Exception>(ex => !noRetryTypes.Contains(ex.GetType()))
                .OrInner<Exception>(ex => !noRetryTypes.Contains(ex.GetType()))
                .WaitAndRetry(settings.DbAccessMaxAttempts, retryCount => TimeSpan.FromSeconds(retryCount * settings.DbAccessDelayDeltaSeconds));
        }

        /// <summary>
        /// 引数に指定したActionを実行する
        /// </summary>
        /// <param name="action">実行するAction</param>
        public void Execute(Action action)
        {
            retryPolicy.Execute(action);
        }
    }
}
