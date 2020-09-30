using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Rms.Server.Core.Azure.Functions.Deliverer;
using Rms.Server.Core.Azure.Functions.Startup;

[assembly: FunctionsStartup(typeof(FunctionAppStartup))]

namespace Rms.Server.Core.Azure.Functions.Deliverer
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
