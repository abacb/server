using System;
using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Utility.Models.Entites;
using System.Collections.Generic;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// IDtScriptConfigRepository
    /// </summary>
    public interface IDtScriptConfigRepository : IRepository
    {
        /// <summary>
        /// DT_SCRIPT_CONFIGテーブルから指定したインストールタイプを持つレコードを取得する
        /// </summary>
        /// <param name="installTypeSid">インストールタイプSID</param>
        /// <returns>スクリプト設定データリスト</returns>
        /// <remarks>
        /// テーブルからデータを取得できなかった場合にはnuilを返す
        /// </remarks>
        List<DtScriptConfig> ReadDtScriptConfigs(long installTypeSid);
    }
}
