using Rms.Server.Core.Abstraction.Models;
using Rms.Server.Core.Utility.Models;
using System.Threading.Tasks;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// IPrimaryRepository
    /// </summary>
    public interface IPrimaryRepository : IRepository
    {
        /// <summary>
        /// 対象のファイルを削除する。
        /// </summary>
        /// <param name="file">ファイル</param>
        void Delete(ArchiveFile file);
    }
}
