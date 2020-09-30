using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Utility.Utility.Models;
using System.Collections.Generic;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// DT_ALMILOG_PREMONITORテーブルリポジトリのインターフェース
    /// </summary>
    public interface IDtAlmilogPremonitorRepository : IRepository
    {
        /// <summary>
        /// DT_ALMILOG_PREMONITORテーブルからDtAlmilogPremonitorを取得する
        /// </summary>
        /// <param name="almilogAnalysisResult">アルミスロープログ解析結果</param>
        /// <param name="allowNotExist">取得件数が0件である場合を正常系とする場合はtrueを、異常系とする場合はfalseを指定する</param>
        /// <param name="allowMultiple">取得件数が2件以上である場合を正常系とする場合はtrueを、異常系とする場合はfalseを指定する</param>
        /// <returns>取得したデータ</returns>
        IEnumerable<DtAlmilogPremonitor> ReadDtAlmilogPremonitor(DtAlmilogAnalysisResult almilogAnalysisResult, bool allowNotExist = true, bool allowMultiple = true);
    }
}
