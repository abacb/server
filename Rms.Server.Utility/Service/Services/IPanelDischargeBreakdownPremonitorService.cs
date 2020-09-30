using Rms.Server.Utility.Utility.Models;
using System.Collections.Generic;

namespace Rms.Server.Utility.Service.Services
{
    /// <summary>
    /// パネル放電破壊予兆監視サービスのインターフェース
    /// </summary>
    public interface IPanelDischargeBreakdownPremonitorService
    {
        /// <summary>
        /// アラーム定義を取得する
        /// </summary>
        /// <param name="panelDischargeBreakdownPredictiveResutLog">パネル放電破壊予兆結果ログ</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="models">DBから取得したパラメータ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool ReadAlarmDefinition(PanelDischargeBreakdownPredictiveResutLog panelDischargeBreakdownPredictiveResutLog, string messageId, out IEnumerable<DtAlarmDefPanelDischargeBreakdownPremonitor> models);

        /// <summary>
        /// アラーム情報を作成しQueueStorageへ登録する
        /// </summary>
        /// <param name="panelDischargeBreakdownPredictiveResutLog">パネル放電破壊予兆結果ログ</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="alarmDef">アラーム定義</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool CreateAndEnqueueAlarmInfo(PanelDischargeBreakdownPredictiveResutLog panelDischargeBreakdownPredictiveResutLog, string messageId, IEnumerable<DtAlarmDefPanelDischargeBreakdownPremonitor> alarmDef);

        /// <summary>
        /// Failureストレージに再送用メッセージをアップロードする
        /// </summary>
        /// <param name="messageSchemaId">メッセージスキーマID</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="message">メッセージ</param>
        void UpdateToFailureStorage(string messageSchemaId, string messageId, string message);
    }
}
