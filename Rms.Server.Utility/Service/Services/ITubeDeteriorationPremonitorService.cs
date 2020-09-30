using Rms.Server.Utility.Utility.Models;
using System.Collections.Generic;

namespace Rms.Server.Utility.Service.Services
{
    /// <summary>
    /// 管球劣化予兆監視サービスのインターフェース
    /// </summary>
    public interface ITubeDeteriorationPremonitorService
    {
        /// <summary>
        /// アラーム定義を取得する
        /// </summary>
        /// <param name="tubeDeteriorationPredictiveResutLog">管球劣化予兆結果ログ</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="models">DBから取得したパラメータ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool ReadAlarmDefinition(TubeDeteriorationPredictiveResutLog tubeDeteriorationPredictiveResutLog, string messageId, out IEnumerable<DtAlarmDefTubeDeteriorationPremonitor> models);

        /// <summary>
        /// アラーム情報を作成しQueueStorageへ登録する
        /// </summary>
        /// <param name="tubeDeteriorationPredictiveResutLog">管球劣化予兆結果ログ</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="alarmDef">アラーム定義</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool CreateAndEnqueueAlarmInfo(TubeDeteriorationPredictiveResutLog tubeDeteriorationPredictiveResutLog, string messageId, IEnumerable<DtAlarmDefTubeDeteriorationPremonitor> alarmDef);

        /// <summary>
        /// Failureストレージに再送用メッセージをアップロードする
        /// </summary>
        /// <param name="messageSchemaId">メッセージスキーマID</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="message">メッセージ</param>
        void UpdateToFailureStorage(string messageSchemaId, string messageId, string message);
    }
}
