using Microsoft.Extensions.Logging;
using Rms.Server.Core.Abstraction.Models;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Abstraction.Repositories.Blobs;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Extensions;
using System.IO;
using System.Text;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// DeliveringBlobRepository
    /// </summary>
    public class DeliveringBlobRepository : IDeliveringRepository
    {
        /// <summary>
        /// DeliveringBlobクライアント
        /// </summary>
        private readonly Blob _deliveringBlob;

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger _log;

        /// <summary>
        /// AppSettings
        /// </summary>
        private readonly AppSettings _settings;

        /// <summary>
        /// BlobPolly
        /// </summary>
        private readonly BlobPolly _polly;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">AppSettings</param>
        /// <param name="deliveringBlob">DeliveringBlob</param>
        /// <param name="polly">BlobPolly</param>
        /// <param name="logger">Logger</param>
        public DeliveringBlobRepository(
            AppSettings settings,
            DeliveringBlob deliveringBlob,
            BlobPolly polly,
            ILogger<DeliveringBlobRepository> logger)
        {
            Assert.IfNull(settings);
            Assert.IfNull(deliveringBlob);
            Assert.IfNull(logger);
            Assert.IfNull(polly);

            _settings = settings;
            _deliveringBlob = deliveringBlob;
            _log = logger;
            _polly = polly;
        }

        /// <summary>
        /// DeliveringBlobにファイルをアップロードする
        /// </summary>
        /// <param name="file">アップロードするファイルの情報</param>
        /// <param name="contents">アップロードするファイルの内容</param>
        public void Upload(ArchiveFile file, string contents)
        {
            _log.EnterJson("{0}", new { file, contents });

            try
            {
                using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(contents)))
                {
                    _deliveringBlob.Upload(file, stream);
                }
            }
            finally
            {
                _log.Leave();
            }
        }
    }
}
