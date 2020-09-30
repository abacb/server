namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// Queueリポジトリインターフェース
    /// </summary>
    public interface IQueueRepository
    {
        /// <summary>
        /// Queue Storageへメッセージを送信する
        /// </summary>
        /// <param name="message">送信するメッセージ</param>
        void SendMessageToAlarmQueue(string message);
    }
}
