using System;
using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Utility.Models.Entites;
using System.Collections.Generic;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// IDtStorageConfigRepository
    /// </summary>
    public interface IDtStorageConfigRepository : IRepository
    {
        /// <summary>
        /// DT_STORAGE_CONFIGテーブルから全レコードを取得する
        /// </summary>
        /// <returns>ストレージ設定データリスト</returns>
        List<DtStorageConfig> ReadDtStorageConfigs();
    }
}
