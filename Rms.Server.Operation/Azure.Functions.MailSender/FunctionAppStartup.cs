﻿using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Rms.Server.Operation.Azure.Functions.MailSender;
using Rms.Server.Operation.Azure.Functions.StartUp;

[assembly: FunctionsStartup(typeof(FunctionAppStartup))]

namespace Rms.Server.Operation.Azure.Functions.MailSender
{
    /// <summary>
    /// FunctionsApp 共通のスタートアップ処理
    /// </summary>
    public class FunctionAppStartup : FunctionsStartup
    {
        /// <summary>
        /// 設定する
        /// </summary>
        /// <param name="builder">ビルダー</param>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder = FunctionsHostBuilderExtend.AddUtility(builder);
        }
    }
}
