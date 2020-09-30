using Rms.Server.Core.Abstraction.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// ICollectingRepository
    /// </summary>
    public interface ICollectingRepository
    {
        /// <summary>
        /// 対象のファイルを削除する。
        /// </summary>
        /// <param name="file">ファイル</param>
        void Delete(ArchiveFile file);

        /// <summary>
        /// 指定directory下のファイルを取得する
        /// </summary>
        /// <param name="directory">ディレクトリ</param>
        /// <returns>ファイル</returns>
        IEnumerable<ArchiveFile> GetArchiveFiles(ArchiveDirectory directory);

        /// <summary>
        /// コレクティングBlobからプライマリBlobにコピーする
        /// </summary>
        /// <param name="source">コピー元</param>
        /// <param name="destination">コピー先</param>
        void CopyToPrimary(ArchiveFile source, ArchiveFile destination);

        /// <summary>
        /// 同一Blob上のコンテナにファイルをコピーする。
        /// </summary>
        /// <param name="source">CollectingBlobのコピー元</param>
        /// <param name="destination">CollectingBlobのコピー先</param>
        void Copy(ArchiveFile source, ArchiveFile destination);
    }
}
