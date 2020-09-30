namespace Rms.Server.Operation.Abstraction.Repositories
{
    /// <summary>
    /// Queueリポジトリインターフェース
    /// </summary>
    public interface IQueueRepository
    {
        /// <summary>
        /// メールキューへメッセージを送信する
        /// </summary>
        /// <param name="message">送信するメッセージ</param>
        void SendMessageToMailQueue(string message);
    }
}
