using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Utility.Utility.Models;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// DT_BLOCLOG_ANALYSIS_RESULTテーブルリポジトリのインターフェース
    /// </summary>
    public interface IDtBloclogAnalysisResultRepository : IRepository
    {
        /// <summary>
        /// 引数に指定したDtBloclogAnalysisResultをDT_BLOCLOG_ANALYSIS_RESULTテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <returns>処理結果</returns>
        DtBloclogAnalysisResult CreateDtBloclogAnalysisResult(DtBloclogAnalysisResult inData);

        /// <summary>
        /// DT_BLOCLOG_ANALYSIS_RESULTテーブルに条件に一致するDtBloclogAnalysisResultが存在するか確認する
        /// </summary>
        /// <param name="logFileName">ログファイル名</param>
        /// <returns>存在する場合trueを、存在しない場合falseを返す</returns>
        bool ExistDtBloclogAnalysisResult(string logFileName);
    }
}
