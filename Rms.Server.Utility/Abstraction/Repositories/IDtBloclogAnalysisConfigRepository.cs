using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Utility.Utility.Models;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// DT_BLOCLOG_ANALYSIS_CONFIGテーブルリポジトリのインターフェース
    /// </summary>
    public interface IDtBloclogAnalysisConfigRepository : IRepository
    {
        /// <summary>
        /// DT_BLOCLOG_ANALYSIS_CONFIGテーブルからDtBloclogAnalysisConfigを取得する
        /// </summary>
        /// <param name="isNormalized">規格化フラグ</param>
        /// <param name="allowNotExist">取得件数が0件である場合を正常系とする場合はtrueを、異常系とする場合はfalseを指定する</param>
        /// <returns>取得したデータ</returns>
        DtBloclogAnalysisConfig ReadDtBloclogAnalysisConfig(bool isNormalized, bool allowNotExist = true);
    }
}
