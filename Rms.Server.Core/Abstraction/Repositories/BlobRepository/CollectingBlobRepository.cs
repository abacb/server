using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Rms.Server.Core.Abstraction.Models;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Abstraction.Repositories.Blobs;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Core.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Blob = Rms.Server.Core.Abstraction.Repositories.Blobs.Blob;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// CollectingBlobRepository
    /// </summary>
    public class CollectingBlobRepository : ICollectingRepository
    {
        /// <summary>
        /// PrimaryBlobクライアント
        /// </summary>
        private readonly Blob _primaryBlob;

        /// <summary>
        /// CollectingBlobクライアント
        /// </summary>
        private readonly Blob _collectingBlob;

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
        /// <param name="collectingBlob">CollectingBlob</param>
        /// <param name="polly">BlobPolly</param>
        /// <param name="logger">Logger</param>
        public CollectingBlobRepository(
            AppSettings settings,
            PrimaryBlob primaryBlob,
            CollectingBlob collectingBlob,
            BlobPolly polly,
            ILogger<PrimaryBlobRepository> logger)
        {
            Assert.IfNull(settings);
            Assert.IfNull(primaryBlob);
            Assert.IfNull(collectingBlob);
            Assert.IfNull(logger);
            Assert.IfNull(polly);

            _settings = settings;
            _primaryBlob = primaryBlob;
            _collectingBlob = collectingBlob;
            _log = logger;
            _polly = polly;
        }

        /// <summary>
        /// 指定したBlobの削除処理を開始する。
        /// </summary>
        /// <param name="file">ファイル</param>
        public void Delete(ArchiveFile file)
        {
            _log.EnterJson($"{nameof(file)}:{0}", file);
            try
            {
                _collectingBlob.Delete(file);
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

        /// <summary>
        /// CollectingBlobからParimaryBlobにファイルをコピーする。
        /// </summary>
        /// <param name="source">CollectingBlobのコピー元</param>
        /// <param name="destination">ParimaryBlobのコピー先</param>
        public void CopyToPrimary(ArchiveFile source, ArchiveFile destination)
        {
            _log.EnterJson($"{0}", Tuple.Create(source, destination));
            try
            {
                _collectingBlob.CopyOnServer(source, _primaryBlob, destination);
            }
            catch (Exception ex)
            {
                string srcPath = string.Format("{0}/{1}", source.ContainerName, source.FilePath);
                string dstPath = string.Format("{0}/{1}", destination.ContainerName, destination.FilePath);
                throw new RmsException(string.Format("Blobのコピーに失敗しました（copy {0} to {1}）", srcPath, dstPath), ex);
            }
            finally
            {
                _log.Leave();
            }
        }

        /// <summary>
        /// 同一Blob上のコンテナにファイルをコピーする。
        /// </summary>
        /// <param name="source">CollectingBlobのコピー元</param>
        /// <param name="destination">CollectingBlobのコピー先</param>
        public void Copy(ArchiveFile source, ArchiveFile destination)
        {
            _log.EnterJson($"{0}", Tuple.Create(source, destination));

            try
            {
                if (!_collectingBlob.CopyOnServer(source, _collectingBlob, destination))
                {
                    throw new RmsException(string.Format("{0} return false.", nameof(_collectingBlob.CopyOnServer)));
                }
            }
            catch (Exception ex)
            {
                string srcPath = string.Format("{0}/{1}", source.ContainerName, source.FilePath);
                string dstPath = string.Format("{0}/{1}", destination.ContainerName, destination.FilePath);
                throw new RmsException(string.Format("Blobのコピーに失敗しました（copy {0} to {1}）", srcPath, dstPath), ex);
            }
            finally
            {
                _log.Leave();
            }
        }

        /// <summary>
        /// 指定したコンテナとdirectory以下のBlob一覧とメタデータを取得する。
        /// </summary>
        /// <param name="directory">ディレクトリ</param>
        /// <returns>ファイル一覧</returns>
        public IEnumerable<ArchiveFile> GetArchiveFiles(ArchiveDirectory directory)
        {
            _log.EnterJson("ArchiveDirectory: {0}", directory);

            Assert.IfNull(directory);
            CloudBlobContainer container = _collectingBlob.GetContainerReadOnly(directory.ContainerName ?? string.Empty);
            CloudBlobDirectory blobDirectory = container.GetDirectoryReference(directory.DirectoryPath);
            var blobs = new List<CloudBlockBlob>();

            _polly.Execute(
                () =>
                {
                    var token = default(BlobContinuationToken);
                    do
                    {
                        // Blob一覧を取得
                        BlobResultSegment segment = null;

                        // 指定しない場合上限の5000件ずつ取得される
                        segment = blobDirectory.ListBlobsSegmentedAsync(true, BlobListingDetails.Metadata, null, token, null, null).Result;

                        token = segment.ContinuationToken;
                        blobs.AddRange(segment.Results.OfType<CloudBlockBlob>());
                    }
                    while (token != null);
                });

            return blobs.Select(blob => Blob.BlobToArchiveFile(blob));
        }
    }
}
