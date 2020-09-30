using System;

namespace Rms.Server.Core.Service.Models
{
    /// <summary>
    /// リモート接続要求
    /// </summary>
    public class RequestRemote
    {
        /// <summary>
        /// デバイスID
        /// </summary>
        public long DeviceId { get; set; }

        /// <summary>
        /// セッションコード
        /// </summary>
        public string SessionCode { get; set; }
    }
}
