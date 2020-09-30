using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Json.Internal;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Rms.Server.Core.Azure.Functions.Startup;
using Rms.Server.Core.Azure.Functions.WebApi.StartUp;

[assembly: FunctionsStartup(typeof(FunctionAppStartup))]

namespace Rms.Server.Core.Azure.Functions.WebApi.StartUp
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
            // FunctionsAppのWebApiが、enumを正しく解釈せず、エラーを出力したままレスポンスが返らない現象の対策。
            // 以下のIssueの対策をそのまま使用している。
            // [HttpTrigger binding to POCO fails on Enums · Issue \#486 · Azure/azure\-webjobs\-sdk\-extensions · GitHub](https://github.com/Azure/azure-webjobs-sdk-extensions/issues/486)
            builder.Services.AddTransient<IConfigureOptions<MvcOptions>, MvcJsonMvcOptionsSetup>();

            // #7205全リクエスト情報の表示に関連するコード
            ////20200416定例にて全リクエスト情報のAzureでの監視は、対応しないことになったためコメントアウト
            ////https://github.com/microsoft/ApplicationInsights-dotnet/issues/1152 を参考にコーディングしたが、効果を確認できなかった。
            ////builder.Services.AddSingleton<ITelemetryInitializer, HeaderTelemetryInitializer>();

            builder = FunctionsHostBuilderExtend.AddUtility(builder);
        }
    }
}
