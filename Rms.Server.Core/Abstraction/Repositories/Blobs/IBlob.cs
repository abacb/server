using Microsoft.WindowsAzure.Storage.Blob;

namespace Rms.Server.Core.Abstraction.Repositories.Blobs
{
    /// <summary>
    /// IBlob
    /// </summary>
    public interface IBlob
    {
        /// <summary>
        /// 読み取り専用のコンテナを取得する
        /// </summary>
        /// <param name="containerName">コンテナ名</param>
        /// <returns>CloudBlobContainer</returns>
        CloudBlobContainer GetContainerReadOnly(string containerName);
    }
}
