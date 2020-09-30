using Rms.Server.Utility.Utility.Models;
using static Rms.Server.Utility.Utility.LogAnalysisDllWrapper;

namespace Rms.Server.Utility.Service.Services
{
    /// <summary>
    /// 骨塩ムラログ解析サービスのインターフェース
    /// </summary>
    public interface IDipBlocLogAnalyzerService
    {
        /// <summary>
        /// 同一ファイル名の解析結果が登録されているか確認する
        /// </summary>
        /// <param name="logFileName">ログファイル名</param>
        /// <param name="messageId">メッセージID</param>
        /// <returns>解析結果が未登録の場合trueを返す。解析結果が登録済み、あるいは、処理に失敗した場合falseを返す。</returns>
        bool CheckDuplicateAnalysisReuslt(string logFileName, string messageId);

        /// <summary>
        /// ムラログ解析を行う
        /// </summary>
        /// <param name="dipBlocLog">骨塩ムラログ</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="_analysisData">ムラログ解析対象データ</param>
        /// <param name="_analysisResult">ムラログ解析結果</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool AnalyzeDipBlocLog(DipBlocLog dipBlocLog, string messageId, out BlocLogAnalysisData _analysisData, out BlocLogAnalysisResult _analysisResult);

        /// <summary>
        /// ムラログ解析結果をDBに登録する
        /// </summary>
        /// <param name="dipBlocLog">骨塩ムラログ</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="_analysisData">ムラログ解析対象データ</param>
        /// <param name="_analysisResult">ムラログ解析結果</param>
        /// <param name="model">DBへの登録結果</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool RegistBlocLogAnalysisResultToDb(DipBlocLog dipBlocLog, string messageId, BlocLogAnalysisData _analysisData, BlocLogAnalysisResult _analysisResult, out DtBloclogAnalysisResult model);

        /// <summary>
        /// Failureストレージに再送用メッセージをアップロードする
        /// </summary>
        /// <param name="messageSchemaId">メッセージスキーマID</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="message">メッセージ</param>
        void UpdateToFailureStorage(string messageSchemaId, string messageId, string message);
    }
}
