using Rms.Server.Utility.Utility.Models;
using System;
using System.Collections.Generic;

namespace Rms.Server.Utility.Service.Services
{
    /// <summary>
    /// 骨塩アルミスロープログ予兆監視サービスのインターフェース
    /// </summary>
    public interface IDipAlmiLogPremonitorService
    {
        /// <summary>
        /// 解析結果データを取得する
        /// </summary>
        /// <param name="unjudgedItems">未判定解析結果データ</param>
        /// <param name="alarmItemsList">アラーム対象解析結果データ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool ReadAlarmJudgementTarget(out List<DtAlmilogAnalysisResult> unjudgedItems, out List<List<DtAlmilogAnalysisResult>> alarmItemsList);

        /// <summary>
        /// アラーム定義を取得する
        /// </summary>
        /// <param name="analysisResultsList">アラーム判定対象データ</param>
        /// <param name="models">DBから取得したパラメータ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool ReadAlarmDefinition(IEnumerable<IEnumerable<DtAlmilogAnalysisResult>> analysisResultsList, out List<List<Tuple<DtAlmilogAnalysisResult, DtAlmilogPremonitor>>> models);

        /// <summary>
        /// アラーム情報を作成しQueueStorageへ登録する
        /// </summary>
        /// <param name="alarmInfosList">アラーム対象解析結果とアラーム定義のリスト</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool CreateAndEnqueueAlarmInfo(IEnumerable<IEnumerable<Tuple<DtAlmilogAnalysisResult, DtAlmilogPremonitor>>> alarmInfosList);

        /// <summary>
        /// アラーム判定済みデータの更新を行う
        /// </summary>
        /// <param name="alarmJudgementTarget">アラーム判定対象データ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool UpdateAlarmJudgedAnalysisResult(IEnumerable<DtAlmilogAnalysisResult> alarmJudgementTarget);
    }
}
