using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rms.Server.Test
{
    public static class CloudBlobClientExtensions
    {
        /// <summary>
        /// Blob Storageからファイルを取得する
        /// </summary>
        /// <param name="blobClient">Blobクライアント</param>
        /// <param name="containerName">Blob Storageのコンテナ名</param>
        /// <param name="rootDir">取得先のディレクトリ</param>
        /// <returns>取得したファイルのローカルパス</returns>
        public static string[] GetFiles(this CloudBlobClient blobClient, string containerName, DirectoryInfo rootDir)
        {
            List<string> files = new List<string>();

            foreach (CloudBlockBlob blockBlob in blobClient.GetBlockBlobs(containerName))
            {
                string filePath = Path.Combine(rootDir.FullName, blockBlob.Name).Replace('/', '\\');
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                blockBlob.DownloadToFileAsync(filePath, FileMode.Create).Wait();

                files.Add(filePath);
            }

            return files.ToArray();
        }

        /// <summary>
        /// Blob StorageからCloudBlockBlobのリストを取得する
        /// </summary>
        /// <param name="blobClient">Blobクライアント</param>
        /// <param name="containerName">Blob Storageのコンテナ名</param>
        /// <returns>CloudBlockBlobのリスト</returns>
        public static IEnumerable<CloudBlockBlob> GetBlockBlobs(this CloudBlobClient blobClient, string containerName)
        {
            var container = blobClient.GetContainerReference(containerName);
            container.CreateIfNotExistsAsync().Wait();

            List<string> files = new List<string>();

            var token = default(BlobContinuationToken);
            do
            {
                var segment = container.ListBlobsSegmentedAsync(string.Empty, true, BlobListingDetails.Metadata, null, token, null, null).Result;

                token = segment.ContinuationToken;

                foreach (CloudBlockBlob blockBlob in segment.Results.OfType<CloudBlockBlob>())
                {
                    yield return blockBlob;
                }
            }
            while (token != null);
        }

        /// <summary>
        /// Blob Storageにファイルをアップロードする
        /// </summary>
        /// <param name="blobClient">アップロード先のBlobクライアント</param>
        /// <param name="containerName">Blob Storageのコンテナ名</param>
        /// <param name="rootDir">アップロードするファイルがあるディレクトリ</param>
        /// <param name="file">アップロードするファイル</param>
        public static void Upload(this CloudBlobClient blobClient, string containerName, DirectoryInfo rootDir, FileInfo file)
        {
            // Blob Storage用にパスを変換
            string blobPath = file.FullName.Replace(rootDir.FullName, string.Empty).Replace('\\', '/').TrimStart('/');

            var container = blobClient.GetContainerReference(containerName);
            container.CreateIfNotExistsAsync().Wait();

            var blockBlob = container.GetBlockBlobReference(blobPath);

            blockBlob.UploadFromFileAsync(file.FullName).Wait();
        }

        /// <summary>
        /// Blob Storageにファイルをアップロードする
        /// </summary>
        /// <param name="blobClient">アップロード先のBlobクライアント</param>
        /// <param name="containerName">Blob Storageのコンテナ名</param>
        /// <param name="rootDir">アップロードするファイルがあるディレクトリ</param>
        /// <param name="file">アップロードするファイル</param>
        public static void Upload(this CloudBlobClient blobClient, string containerName, string path, string contents)
        {
            var container = blobClient.GetContainerReference(containerName);
            container.CreateIfNotExistsAsync().Wait();

            var blockBlob = container.GetBlockBlobReference(path);

            blockBlob.UploadTextAsync(contents).Wait();
        }

        /// <summary>
        /// Blob Storageからファイルをダウンロードする
        /// </summary>
        /// <param name="blobClient">ダウンロード元のBlobクライアント</param>
        /// <param name="containerName">Blob Storageのコンテナ名</param>
        /// <param name="rootDir">ダウンロード先のディレクトリ</param>
        /// <param name="file">ダウンロード先のファイル</param>
        public static void Download(this CloudBlobClient blobClient, string containerName, DirectoryInfo rootDir, FileInfo file)
        {
            // Blob Storage用にパスを変換
            string blobPath = file.FullName.Replace(rootDir.FullName, string.Empty).Replace('\\', '/').TrimStart('/');

            var container = blobClient.GetContainerReference(containerName);
            container.CreateIfNotExistsAsync().Wait();

            var blockBlob = container.GetBlockBlobReference(blobPath);

            using (Stream stream = file.Create())
            {
                blockBlob.DownloadToFileAsync(file.FullName, FileMode.Create).Wait();
            }
        }

        /// <summary>
        /// Blob Storageからファイルをダウンロードする
        /// </summary>
        /// <param name="blobClient">ダウンロード元のBlobクライアント</param>
        /// <param name="containerName">Blob Storageのコンテナ名</param>
        /// <param name="rootDir">ダウンロード先のディレクトリ</param>
        /// <param name="file">ダウンロード先のファイル</param>
        public static void Download(this CloudBlobClient blobClient, string containerName, string path, out string contents)
        {
            var container = blobClient.GetContainerReference(containerName);
            container.CreateIfNotExistsAsync().Wait();

            var blockBlob = container.GetBlockBlobReference(path);

            contents = blockBlob.DownloadTextAsync().Result;
        }
    }
}
