using System;
using System.Collections.Generic;
using System.Text;

namespace Rms.Server.Core.Utility.Models.Dispatch
{
    /// <summary>
    /// イベント情報
    /// </summary>
    public class RmsEvent
    {
        /// <summary>
        /// 日時
        /// </summary>
        /// <remarks>
        /// "iothub-enqueuedtime"
        /// </remarks>
        public DateTime MessageDateTime { get; set; }

        /// <summary>
        /// デバイスエッジID
        /// </summary>
        public Guid EdgeId { get; set; }

        /// <summary>
        /// イベントID
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// ボディ
        /// </summary>
        public string MessageBody { get; set; }
    }
}
