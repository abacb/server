using System;
using System.Collections.Generic;
using System.Text;

namespace Rms.Server.Core.Utility.Models.Dispatch
{
    /// <summary>
    /// 不正メッセージ
    /// </summary>
    public class UnexpectedMessage
    {
        /// <summary>
        /// 不正メッセージ
        /// </summary>
        /// <param name="messageSchemaId">メッセージスキーマID</param>
        /// <param name="body">Body</param>
        /// <param name="messageId">メッセージID</param>
        public UnexpectedMessage(string messageSchemaId, string body, string messageId)
        {
            MessageSchemaId = messageSchemaId;
            MessageId = messageId;
            Body = body;
        }

        /// <summary>
        /// メッセージスキーマID
        /// </summary>
        public string MessageSchemaId { get; }

        /// <summary>
        /// メッセージID。不明の場合はnull
        /// </summary>
        public string MessageId { get; }

        /// <summary>
        /// 保存するメッセージ全体
        /// </summary>
        public string Body { get; }
    }
}
