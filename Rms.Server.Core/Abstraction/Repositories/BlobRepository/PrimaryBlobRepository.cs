using Microsoft.Extensions.Logging;
using Rms.Server.Core.Abstraction.Models;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Abstraction.Repositories.Blobs;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Core.Utility.Extensions;
using System;
using Blob = Rms.Server.Core.Abstraction.Repositories.Blobs.Blob;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// PrimaryBlobRepository
    /// </summary>
    public class PrimaryBlobRepository : IPrimaryRepository
    {
        /// <summary>
        /// PrimaryBlobクライアント
        /// </summary>
        private readonly Blob _primaryBlob;

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
        /// <param name="primaryBlob">PrimaryBlob</param>
        /// <param name="polly">BlobPolly</param>
        /// <param name="logger">Logger</param>
        public PrimaryBlobRepository(
            AppSettings settings,
            PrimaryBlob primaryBlob,
            BlobPolly polly,
            ILogger<PrimaryBlobRepository> logger)
        {
            Assert.IfNull(settings);
            Assert.IfNull(primaryBlob);
            Assert.IfNull(logger);
            Assert.IfNull(polly);

            _settings = settings;
            _primaryBlob = primaryBlob;
            _polly = polly;
            _log = logger;
        }

        /// <summary>
        /// 対象のファイルを削除する。
        /// </summary>
        /// <param name="file">ファイル</param>
        public void Delete(ArchiveFile file)
        {
            _log.EnterJson($"{nameof(file)}:{0}", file);
            try
            {
                _primaryBlob.Delete(file);
            }
            catch (Exception ex)
            {
                string path = string.Format("{0}/{1}", file.ContainerName, file.FilePath);
                throw new RmsException(string.Format("Blobの削除に失敗しました（delete {0}）", path), ex);
            }
            finally
            {
                _log.Leave();
            }
        }
    }
}
