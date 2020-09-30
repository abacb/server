using Newtonsoft.Json;

namespace Rms.Server.Core.Service.Models
{
    /// <summary>
    /// 機器の設置設定
    /// </summary>
    [JsonObject]
    public class InstallBaseDeviceConfig
    {
        /// <summary>
        /// エッジID
        /// </summary>
        [JsonProperty("edgeId")]
        public string EdgeId { get; set; }

        /// <summary>
        /// インストールタイプ
        /// </summary>
        [JsonProperty("installType")]
        public string InstallType { get; set; }

        /// <summary>
        /// デバイスUID（機器管理番号）
        /// </summary>
        [JsonProperty("equipmentUid")]
        public string EquipmentUid { get; set; }

        /// <summary>
        /// デバイス名（機器名）
        /// </summary>
        [JsonProperty("equipmentName")]
        public string EquipmentName { get; set; }

        /// <summary>
        /// シリアル番号
        /// </summary>
        [JsonProperty("equipmentSerialNumber")]
        public string EquipmentSerialNumber { get; set; }

        /// <summary>
        /// 設置場所
        /// </summary>
        [JsonProperty("installFeatures")]
        public string InstallFeatures { get; set; }

        /// <summary>
        /// IPアドレス
        /// </summary>
        [JsonProperty("ipAddress")]
        public string IpAddress { get; set; }

        /// <summary>
        /// ホスト名
        /// </summary>
        [JsonProperty("hostName")]
        public string HostName { get; set; }

        /// <summary>
        /// モデル（型式）
        /// </summary>
        [JsonProperty("model")]
        public string Model { get; set; }

        /// <summary>
        /// 回線種別
        /// </summary>
        [JsonProperty("networkRoute")]
        public string NetworkRoute { get; set; }
    }
}
