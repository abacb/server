using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Utility.Utility.Models;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// DT_ALMILOG_ANALYSIS_CONFIGテーブルリポジトリのインターフェース
    /// </summary>
    public interface IDtAlmilogAnalysisConfigRepository : IRepository
    {
        /// <summary>
        /// DT_ALMILOG_ANALYSIS_CONFIGテーブルからDtAlmilogAnalysisConfigを取得する
        /// </summary>
        /// <param name="detectorName">Detector名称</param>
        /// <param name="isNormalized">規格化フラグ</param>
        /// <param name="allowNotExist">取得件数が0件である場合を正常系とする場合はtrueを、異常系とする場合はfalseを指定する</param>
        /// <returns>取得したデータ</returns>
        DtAlmilogAnalysisConfig ReadDtAlmilogAnalysisConfig(string detectorName, bool isNormalized, bool allowNotExist = true);
    }
}
