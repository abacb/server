using System;
using System.Collections.Generic;

namespace Rms.Server.Core.Abstraction.Models
{
    /// <summary>
    /// デバイス接続情報クラス
    /// </summary>
    public class DeviceConnectionInfo
    {
        /// <summary>
        /// デバイスのエッジID
        /// </summary>
        public Guid EdgeId { get; set; }

        /// <summary>
        /// IoT Hubの接続文字列情報
        /// </summary>
        public KeyValuePair<string, string> IotHubConnectionString { get; set; }
    }
}