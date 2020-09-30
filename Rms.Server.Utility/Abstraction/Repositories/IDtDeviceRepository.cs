using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Utility.Utility.Models;
using System.Collections.Generic;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// DT_DEVICEテーブルリポジトリのインターフェース
    /// </summary>
    public interface IDtDeviceRepository : IRepository
    {
        /// <summary>
        /// DT_DEVICEテーブルからDtDeviceを取得する
        /// </summary>
        /// <returns>取得したデータ</returns>
        IEnumerable<DtDevice> ReadDtDevice();
    }
}
