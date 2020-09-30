using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Utility.Utility.Models;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// DT_SMART_ANALYSIS_RESULTテーブルリポジトリのインターフェース
    /// </summary>
    public interface IDtSmartAnalysisResultRepository : IRepository
    {
        /// <summary>
        /// 引数に指定したDtSmartAnalysisResultをDT_SMART_ANALYSIS_RESULTテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <returns>処理結果</returns>
        DtSmartAnalysisResult CreateDtSmartAnalysisResult(DtSmartAnalysisResult inData);

        /// <summary>
        /// DT_SMART_ANALYSIS_RESULTテーブルからDtSmartAnalysisResultを取得する
        /// </summary>
        /// <param name="diskDrive">ディスクドライブ</param>
        /// <returns>取得したデータ</returns>
        DtSmartAnalysisResult ReadDtSmartAnalysisResult(DiskDrive diskDrive);

        /// <summary>
        /// 引数に指定したDtSmartAnalysisResultでDT_SMART_ANALYSIS_RESULTテーブルを更新する
        /// </summary>
        /// <param name="inData">更新するデータ</param>
        /// <returns>更新したデータ</returns>
        DtSmartAnalysisResult UpdateDtSmartAnalysisResult(DtSmartAnalysisResult inData);
    }
}
