using Rms.Server.Operation.Utility.Models;

namespace Rms.Server.Operation.Service.Services
{
    /// <summary>
    /// AlarmRegisterサービスのインターフェース
    /// </summary>
    public interface IAlarmRegisterService
    {
        /// <summary>
        /// 同一メッセージIDのアラームが登録されているか確認する
        /// </summary>
        /// <param name="messageId">メッセージID</param>
        /// <returns>アラームが未登録の場合falseを、アラームが登録済みの場合trueを、処理に失敗した場合nullを返す。</returns>
        bool? ExistsSameMessageIdAlarm(string messageId);

        /// <summary>
        /// アラーム設定を取得する
        /// </summary>
        /// <param name="alarmInfo">アラーム情報</param>
        /// <param name="equipment">機器データ</param>
        /// <param name="alarmConfig">アラーム設定</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool ReadDtAlarmConfig(AlarmInfo alarmInfo, out DtEquipment equipment, out DtAlarmConfig alarmConfig);

        /// <summary>
        /// メール送信が必要か判定する
        /// </summary>
        /// <param name="messageId">メッセージID</param>
        /// <param name="alarmConfig">アラーム設定</param>
        /// <param name="alarmDefId">アラーム定義ID</param>
        /// <param name="needMail">メール送信が必要な場合true、不要な場合false</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool NeedsMailSending(string messageId, DtAlarmConfig alarmConfig, string alarmDefId, out bool needMail);

        /// <summary>
        /// アラームをDBに登録する
        /// </summary>
        /// <param name="messageId">メッセージID</param>
        /// <param name="hasMail">メールありフラグ</param>
        /// <param name="alarmInfo">アラーム情報</param>
        /// <param name="equipment">機器データ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool RegistAlarmInfo(string messageId, bool hasMail, AlarmInfo alarmInfo, DtEquipment equipment);

        /// <summary>
        /// メール送信情報を作成しQueueStorageへ登録する
        /// </summary>
        /// <param name="messageId">メッセージID</param>
        /// <param name="alarmInfo">アラーム情報</param>
        /// <param name="alarmConfig">アラーム設定</param>
        /// <param name="equipment">機器データ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool CreateAndEnqueueAlarmInfo(string messageId, AlarmInfo alarmInfo, DtAlarmConfig alarmConfig, DtEquipment equipment);

        /// <summary>
        /// Failureストレージに再送用メッセージをアップロードする
        /// </summary>
        /// <param name="messageId">メッセージID</param>
        /// <param name="message">メッセージ</param>
        void UpdateToFailureStorage(string messageId, string message);
    }
}
