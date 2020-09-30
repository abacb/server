using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Rms.Server.Core.Abstraction.Models;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Core.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

namespace Rms.Server.Core.Abstraction.Repositories.Blobs
{
    /// <summary>
    /// Blob
    /// </summary>
    public abstract class Blob : IBlob
    {
        /// <summary>
        /// コンテナ
        /// </summary>
        /// <remarks>
        /// BlobContainer自体の更新を使用としなければ、CloudBlobContainerの共有は問題なさそう。
        /// [Azure Storage Blob types \(CloudBlobContainer, CloudBlobClient, etc\.\) and thread safety \- Stack Overflow](https://stackoverflow.com/questions/6915424/azure-storage-blob-types-cloudblobcontainer-cloudblobclient-etc-and-thread)
        /// </remarks>
        private readonly Dictionary<string, CloudBlobContainer> containers
            = new Dictionary<string, CloudBlobContainer>();

        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// アプリケーション設定
        /// </summary>
        private readonly AppSettings _settings;

        /// <summary>
        /// BlobPolly
        /// </summary>
        private readonly BlobPolly _polly;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">アプリケーション設定</param>
        /// <param name="connectionString">接続文字列</param>
        /// <param name="polly">BlobPolly</param>
        /// <param name="logger">logger</param>
        /// <param name="memberName">呼び出し元メンバ名</param>
        /// <param name="sourceFilePath">呼び出し元ファイルパス</param>
        /// <param name="sourceLineNumber">呼び出し元ファイル行数</param>
        public Blob(
            AppSettings settings,
            string connectionString,
            BlobPolly polly,
            ILogger<Blob> logger,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            // 本クラスはabstractであるため、派生クラスコンストラクタの呼び出し元を出力するためにmemberName類を渡す。
            Assert.IfNull(settings, memberName, sourceFilePath, sourceLineNumber);
            Assert.IfNullOrEmpty(connectionString, "connectionString", memberName, sourceFilePath, sourceLineNumber);
            Assert.IfNull(polly, memberName, sourceFilePath, sourceLineNumber);
            Assert.IfNull(logger, memberName, sourceFilePath, sourceLineNumber);

            _settings = settings;
            var _account = CloudStorageAccount.Parse(connectionString);
            Client = _account.CreateCloudBlobClient();

            Client.DefaultRequestOptions.RetryPolicy =
                 new ExponentialRetry(TimeSpan.FromSeconds(_settings.BlobAccessDelayDeltaSeconds), _settings.BlobAccessMaxAttempts);

            _polly = polly;
            _logger = logger;
        }

        /// <summary>
        /// CloudBlobClient
        /// </summary>
        public CloudBlobClient Client { get; }

        /// <summary>
        /// CloudBlockBlobをArchiveFileに変換する
        /// </summary>
        /// <param name="blob">CloudBlockBlob</param>
        /// <returns>ArchiveFile</returns>
        public static ArchiveFile BlobToArchiveFile(CloudBlockBlob blob)
        {
            var archiveFile = new ArchiveFile
            {
                ContainerName = blob.Container.Name,
                FilePath = blob.Name,
                CreatedAt = blob.Properties?.Created?.UtcDateTime ?? default(DateTime)
            };
            archiveFile.SetMetaData(blob.Metadata);
            return archiveFile;
        }

        /// <summary>
        /// 読み込み用のBlobコンテナクラスを取得する
        /// </summary>
        /// <param name="containerName">コンテナ名</param>
        /// <returns>CloudBlobContainer</returns>
        public CloudBlobContainer GetContainerReadOnly(string containerName)
        {
            // 同一コンテナ名でインスタンスを共有している。
            // 以下の理由によるが、問題が起きそうなら特にやらなくてもよい。
            // ・MSのコードを見ると、Getで毎回Newしている
            // ・同一コンテナのインスタンスは更新しなければ共有可能らしい
            if (containers.ContainsKey(containerName))
            {
                return containers[containerName];
            }
            else
            {
                var container = Client.GetContainerReference(containerName);
                containers.Add(containerName, container);
                return container;
            }
        }

        /// <summary>
        /// ストレージアカウント間でファイルをコピーする
        /// </summary>
        /// <param name="sourceFile">コピー元</param>
        /// <param name="destinationBlob">コピー先Blob</param>
        /// <param name="destinationFile">コピー先ファイル</param>
        /// <exception cref="AggregateException">Result/Waitを使用しているため。詳細はinnerExceptionを参照</exception>
        public void Copy(ArchiveFile sourceFile, Blob destinationBlob, ArchiveFile destinationFile)
        {
            CloudBlockBlob destinationBlockBlob = destinationBlob.ArchiveFileToBlob(destinationFile);
            CloudBlockBlob sourceBlockBlob = ArchiveFileToBlob(sourceFile);

            // 本来ならStartCopyAsync等の、ローカルへのDownloadを介さない処理を使いたいが,
            // StartCopyAsyncはストレージアカウントを跨ぐ場合、URIを参照するため、
            // コピー元がpublic Accessでないと失敗する。
            // ・・と考えていたが、接続文字列に認証文字まで含めている場合問題ないようなのでこちらは使用しないようにする。
            _polly.Execute(
                () =>
                {
                    var destContainer = destinationBlob.GetContainerReadOnly(destinationFile.ContainerName);
                    destContainer.CreateIfNotExistsAsync().Wait();

                    using (var targetBlobStream = destinationBlockBlob.OpenWriteAsync().Result)
                    using (var sourceBlobStream = sourceBlockBlob.OpenReadAsync().Result)
                    {
                        sourceBlobStream.CopyToAsync(targetBlobStream).Wait();
                    }
                });
        }

        /// <summary>
        /// ストレージにファイルをアップロードする
        /// </summary>
        /// <param name="file">アップロードするファイルの情報</param>
        /// <param name="stream">アップロードするファイルの内容</param>
        /// <param name="overwrite">ファイルを上書きする場合はtrueを、上書きしない場合はfalseを指定する</param>
        public void Upload(ArchiveFile file, Stream stream, bool overwrite = true)
        {
            CloudBlockBlob blockBlob = ArchiveFileToBlob(file);
            _polly.Execute(() =>
            {
                if (!overwrite && blockBlob.ExistsAsync().Result)
                {
                    throw new RmsAlreadyExistException(string.Format("file already exists. (upload {0})", file.FilePath));
                }

                blockBlob.UploadFromStreamAsync(stream).Wait();
            });
        }

        /// <summary>
        /// クラウド側でコピーを行う。
        /// </summary>
        /// <param name="sourceFile">コピー元ファイル</param>
        /// <param name="destinationBlob">コピー先Blob</param>
        /// <param name="destinationFile">コピー先ファイル</param>
        /// <returns>成功:true  失敗:false</returns>
        public bool CopyOnServer(ArchiveFile sourceFile, Blob destinationBlob, ArchiveFile destinationFile)
        {
            CloudBlockBlob destinationBlockBlob = destinationBlob.ArchiveFileToBlob(destinationFile);
            CloudBlockBlob sourceBlockBlob = ArchiveFileToBlob(sourceFile);

            bool result = false;

            // 本来ならStartCopyAsync等の、ローカルへのDownloadを介さない処理を使いたいが,
            // StartCopyAsyncはストレージアカウントを跨ぐ場合、URIを参照するため、
            // コピー元がpublic Accessでないと失敗する。
            _polly.Execute(
                () =>
                {
                    var destContainer = destinationBlob.GetContainerReadOnly(destinationFile.ContainerName);
                    destContainer.CreateIfNotExistsAsync().Wait();

                    // MSサポートに問い合わせたところ、
                    // StartCopyAsyncをWaitすることで完了まで待機してもらえるということなので、その前提で実装する。
                    destinationBlockBlob.StartCopyAsync(sourceBlockBlob).Wait();
                    _logger.Debug($"Copy result [{destinationBlockBlob.CopyState}] from [{sourceFile.ContainerName}/{sourceFile.FilePath}] to [{destinationFile.ContainerName}/{destinationFile.FilePath}] ");

                    if (destinationBlockBlob.CopyState != null &&
                    destinationBlockBlob.CopyState.Status == CopyStatus.Success)
                    {
                        result = true;
                    }
                });
            return result;
        }

        /// <summary>
        /// 対象のファイルを削除する。
        /// </summary>
        /// <param name="file">ファイル</param>
        public void Delete(ArchiveFile file)
        {
            CloudBlockBlob blockBlob = ArchiveFileToBlob(file);
            _polly.Execute(
                () =>
                {
                    blockBlob.DeleteAsync().Wait();
                });
        }

        /// <summary>
        /// ArchiveFileをCloudBlockBlobに変換する
        /// </summary>
        /// <param name="file">ArchiveFile</param>
        /// <returns>CloudBlockBlob</returns>
        private CloudBlockBlob ArchiveFileToBlob(ArchiveFile file)
        {
            CloudBlobContainer container = GetContainerReadOnly(file.ContainerName);
            CloudBlockBlob blob = container.GetBlockBlobReference(file.FilePath);

            foreach (var metadata in file.MetaData)
            {
                blob.Metadata[metadata.Key] = metadata.Value;
            }

            return blob;
        }
    }
}