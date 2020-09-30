using AzureFunctions.Extensions.Swashbuckle;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Rms.Server.Core.Azure.Functions.WebApi.StartUp;
using System.Reflection;

[assembly: WebJobsStartup(typeof(SwashBuckleStartup))]

namespace Rms.Server.Core.Azure.Functions.WebApi.StartUp
{
    // 初期状態だとここでビルドエラーになる。
    // 関連issue:
    // https://github.com/yuka1984/azure-functions-extensions-swashbuckle/issues/2
    // internal class swashbucklestartup : functionsstartup
    // {
    //    public override void configure(ifunctionshostbuilder builder)
    //    {
    //        //register the extension
    //        builder.addswashbuckle(assembly.getexecutingassembly());
    //    }
    // }

    /// <summary>
    /// SwashBuckleStartup
    /// </summary>
    internal class SwashBuckleStartup : IWebJobsStartup
    {
        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="builder">builder</param>
        public void Configure(IWebJobsBuilder builder)
        {
            // Register the extension
            builder.AddSwashBuckle(Assembly.GetExecutingAssembly());
        }
    }
}
