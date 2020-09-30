using Newtonsoft.Json;
using Rms.Server.Core.Utility.Models.Entites;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Core.Utility.Models.Dispatch
{
    /// <summary>
    /// 端末設定同期
    /// IoT Hubによるデバイスツイン更新イベントメッセージ
    /// </summary>
    public class TwinChangedMessage
    {
        /// <summary>
        /// 発生元機器UID
        /// </summary>
        [JsonProperty(nameof(RemoteConnectionUid))]
        public string RemoteConnectionUid { get; set; }

        /// <summary>
        /// 詳細情報
        /// </summary>
        [Required]
        [JsonProperty(nameof(SoftVersion))]
        public string SoftVersion { get; set; }

        /// <summary>
        /// 変換
        /// </summary>
        /// <param name="eventData">イベント情報</param>
        /// <returns>DtEquipmentUsage</returns>
        public DtTwinChanged Convert(RmsEvent eventData)
        {
            return new DtTwinChanged
            {
                RemoteConnectionUid = RemoteConnectionUid,
                SoftVersion = SoftVersion
            };
        }
    }
}
