using Rms.Server.Utility.Utility.Models;
using static Rms.Server.Utility.Utility.LogAnalysisDllWrapper;

namespace Rms.Server.Utility.Service.Services
{
    /// <summary>
    /// 骨塩アルミスロープログ解析サービスのインターフェース
    /// </summary>
    public interface IDipAlmiLogAnalyzerService
    {
        /// <summary>
        /// 同一ファイル名の解析結果が登録されているか確認する
        /// </summary>
        /// <param name="logFileName">ログファイル名</param>
        /// <param name="messageId">メッセージID</param>
        /// <returns>解析結果が未登録の場合trueを返す。解析結果が登録済み、あるいは、処理に失敗した場合falseを返す。</returns>
        bool CheckDuplicateAnalysisReuslt(string logFileName, string messageId);

        /// <summary>
        /// アルミロープログ解析を行う
        /// </summary>
        /// <param name="dipAlmiSlopeLog">骨塩アルミスロープログ</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="_analysisData">アルミスロープログ解析対象データ</param>
        /// <param name="_analysisResult">アルミスロープログ解析結果</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool AnalyzeDipAlmiLog(DipAlmiSlopeLog dipAlmiSlopeLog, string messageId, out AlmiLogAnalysisData _analysisData, out AlmiLogAnalysisResult _analysisResult);

        /// <summary>
        /// アルミロープログ解析結果をDBに登録する
        /// </summary>
        /// <param name="dipAlmiSlopeLog">骨塩アルミスロープログ</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="_analysisData">アルミスロープログ解析対象データ</param>
        /// <param name="_analysisResult">アルミスロープログ解析結果</param>
        /// <param name="model">DBへの登録結果</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool RegistAlmiLogAnalysisResultToDb(DipAlmiSlopeLog dipAlmiSlopeLog, string messageId, AlmiLogAnalysisData _analysisData, AlmiLogAnalysisResult _analysisResult, out DtAlmilogAnalysisResult model);

        /// <summary>
        /// Failureストレージに再送用メッセージをアップロードする
        /// </summary>
        /// <param name="messageSchemaId">メッセージスキーマID</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="message">メッセージ</param>
        void UpdateToFailureStorage(string messageSchemaId, string messageId, string message);
    }
}
