using Rms.Server.Core.Abstraction.Models;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// IDeliveringRepository
    /// </summary>
    public interface IDeliveringRepository
    {
        /// <summary>
        /// DeliveringBlobにファイルをアップロードする
        /// </summary>
        /// <param name="file">アップロードするファイルの情報</param>
        /// <param name="contents">アップロードするファイルの内容</param>
        void Upload(ArchiveFile file, string contents);
    }
}
