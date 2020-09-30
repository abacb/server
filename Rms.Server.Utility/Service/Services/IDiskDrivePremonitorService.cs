using Rms.Server.Utility.Utility.Models;
using System.Collections.Generic;

namespace Rms.Server.Utility.Service.Services
{
    /// <summary>
    /// ディスクドライブ予兆監視サービスのインターフェース
    /// </summary>
    public interface IDiskDrivePremonitorService
    {
        /// <summary>
        /// 登録済みのSMART解析結果判定テーブルを取得する
        /// </summary>
        /// <param name="diskDrive">ディスクドライブ</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="model">DBから取得したパラメータ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool ReadSmartAnalysisResult(DiskDrive diskDrive, string messageId, out DtSmartAnalysisResult model);

        /// <summary>
        /// アラーム定義を取得する
        /// </summary>
        /// <param name="messageId">メッセージID</param>
        /// <param name="model">DBから取得したパラメータ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool ReadAlarmDefinition(string messageId, out DtAlarmSmartPremonitor model);

        /// <summary>
        /// 受信メッセージからID=C5のSMART属性情報を取得する
        /// </summary>
        /// <param name="diskDrive">ディスクドライブ</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="smartAttirubteInfoC5">SMART属性情報C5</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool ReadSmartAttirubteInfoC5(DiskDrive diskDrive, string messageId, out DiskDrive.SmartAttributeInfoSchema smartAttirubteInfoC5);

        /// <summary>
        /// アラーム判定を実行しSMART解析判定結果を更新する
        /// </summary>
        /// <param name="diskDrive">ディスクドライブ</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="analysisResult">解析判定結果</param>
        /// <param name="alarmDef">アラーム定義</param>
        /// <param name="smartAttirubteInfoC5">SMART属性情報C5</param>
        /// <param name="needsAlarmCreation">アラーム生成要・不要フラグ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool JudgeAlarmCreationAndUpdateSmartAnalysisResult(DiskDrive diskDrive, string messageId, DtSmartAnalysisResult analysisResult, DtAlarmSmartPremonitor alarmDef, DiskDrive.SmartAttributeInfoSchema smartAttirubteInfoC5, out bool needsAlarmCreation);

        /// <summary>
        /// アラーム情報を作成しQueueStorageへ登録する
        /// </summary>
        /// <param name="diskDrive">ディスクドライブ</param>
        /// <param name="smartAttirubteInfoC5">SMART属性情報(C5)</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="analysisResult">解析判定結果</param>
        /// <param name="alarmDef">アラーム定義</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool CreateAndEnqueueAlarmInfo(DiskDrive diskDrive, DiskDrive.SmartAttributeInfoSchema smartAttirubteInfoC5, string messageId, DtSmartAnalysisResult analysisResult, DtAlarmSmartPremonitor alarmDef);

        /// <summary>
        /// Failureストレージに再送用メッセージをアップロードする
        /// </summary>
        /// <param name="messageSchemaId">メッセージスキーマID</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="message">メッセージ</param>
        void UpdateToFailureStorage(string messageSchemaId, string messageId, string message);
    }
}
