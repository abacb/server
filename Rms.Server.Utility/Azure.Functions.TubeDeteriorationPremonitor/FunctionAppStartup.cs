using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Rms.Server.Utility.Azure.Functions.StartUp;
using Rms.Server.Utility.Azure.Functions.TubeDeteriorationPremonitor;

[assembly: FunctionsStartup(typeof(FunctionAppStartup))]

namespace Rms.Server.Utility.Azure.Functions.TubeDeteriorationPremonitor
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
