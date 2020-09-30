using Microsoft.Extensions.Logging;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Utility;

namespace Rms.Server.Core.Abstraction.Repositories.Blobs
{
    /// <summary>
    /// CollectingBlob
    /// </summary>
    public class FailureBlob : Blob
    {
        /// <summary>
        /// FailureBlob
        /// </summary>
        /// <param name="settings">設定</param>
        /// <param name="polly">BlobPolly</param>
        /// <param name="logger">logger</param>
        public FailureBlob(
            AppSettings settings,
            BlobPolly polly,
            ILogger<FailureBlob> logger) : base(
            settings,
            settings.FailureBlobConnectionString,
            polly,
            logger)
        {
            // HACK: 本来ならこのように呼び出し元を出力したいが、コンストラクタによる呼び出しはリフレクションを使用しているようで、
            // Azure Functions上で起動すると以下のようなエラーが出るため、行わない。
            // > Anonymously Hosted DynamicMethods Assembly: Object reference not set to an instance of an object.
            // [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            // [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            // [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0
        }
    }
}
