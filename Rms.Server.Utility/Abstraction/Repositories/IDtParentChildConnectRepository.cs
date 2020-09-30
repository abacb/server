using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Utility.Utility.Models;
using System.Collections.Generic;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// DT_PARENT_CHILD_CONNECTテーブルリポジトリのインターフェース
    /// </summary>
    public interface IDtParentChildConnectRepository : IRepository
    {
        /// <summary>
        /// DT_PARENT_CHILD_CONNECTテーブルからDtParentChildConnectを取得する
        /// </summary>
        /// <returns>取得したデータ</returns>
        IEnumerable<DtParentChildConnect> ReadDtParentChildConnect();
    }
}
