using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using System.Collections.Generic;
using System.Linq;

namespace Rms.Server.Operation.Utility
{
    /// <summary>
    /// QueueStorage操作
    /// </summary>
    public class QueueOperation
    {
        /// <summary>
        /// Queueへメッセージを送信する
        /// </summary>
        /// <param name="queueName">Queue名称</param>
        /// <param name="connectionString">Storageアカウント接続文字列</param>
        /// <param name="message">メッセージ</param>
        public static void SendMessage(string queueName, string connectionString, string message)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(queueName);

            var queueMessage = new CloudQueueMessage(message);
            queue.AddMessage(queueMessage);
        }

        /// <summary>
        /// Queueからメッセージを受信する
        /// </summary>
        /// <param name="queueName">Queue名称</param>
        /// <param name="connectionString">Storageアカウント接続文字列</param>
        /// <returns>メッセージ</returns>
        public static List<string> GetMessages(string queueName, string connectionString)
        {
            List<string> messages = new List<string>();

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(queueName);

            IEnumerable<CloudQueueMessage> queueMessages;
            while ((queueMessages = queue.GetMessages(32)) != null && queueMessages.Count() > 0)
            {
                foreach (CloudQueueMessage queueMessage in queueMessages)
                {
                    messages.Add(queueMessage.AsString);
                    queue.DeleteMessage(queueMessage);
                }
            }

            return messages;
        }
    }
}
