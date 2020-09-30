using Rms.Server.Utility.Utility.Models;
using System.Collections.Generic;

namespace Rms.Server.Utility.Service.Services
{
    /// <summary>
    /// パネル欠陥予兆監視サービスのインターフェース
    /// </summary>
    public interface IPanelDefectPremonitorService
    {
        /// <summary>
        /// アラーム定義を取得する
        /// </summary>
        /// <param name="panelDefectPredictiveResutLog">パネル欠陥予兆結果ログ</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="models">DBから取得したパラメータ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool ReadAlarmDefinition(PanelDefectPredictiveResutLog panelDefectPredictiveResutLog, string messageId, out IEnumerable<DtAlarmDefPanelDefectPremonitor> models);

        /// <summary>
        /// アラーム情報を作成しQueueStorageへ登録する
        /// </summary>
        /// <param name="panelDefectPredictiveResutLog">パネル欠陥予兆結果ログ</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="alarmDef">アラーム定義</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool CreateAndEnqueueAlarmInfo(PanelDefectPredictiveResutLog panelDefectPredictiveResutLog, string messageId, IEnumerable<DtAlarmDefPanelDefectPremonitor> alarmDef);

        /// <summary>
        /// Failureストレージに再送用メッセージをアップロードする
        /// </summary>
        /// <param name="messageSchemaId">メッセージスキーマID</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="message">メッセージ</param>
        void UpdateToFailureStorage(string messageSchemaId, string messageId, string message);
    }
}
