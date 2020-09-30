using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Utility.Utility.Models
{
    /// <summary>
    /// Azure Event Grid イベントスキーマ
    /// </summary>
    public class AzureEventGridEventSchema
    {
        /// <summary>
        /// イベントの一意識別子
        /// </summary>
        [Required]
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// イベントソースの完全なリソースパス
        /// </summary>
        [Required]
        [JsonProperty("topic")]
        public string Topic { get; set; }

        /// <summary>
        /// 発行元が定義したイベントの対象のパス
        /// </summary>
        [Required]
        [JsonProperty("subject")]
        public string Subject { get; set; }

        /// <summary>
        /// このイベントソース用に登録されたイベントの種類のいずれか
        /// </summary>
        [Required]
        [JsonProperty("eventType")]
        public string EventType { get; set; }

        /// <summary>
        /// プロバイダーのUTC時刻に基づくイベントの生成時刻
        /// </summary>
        [Required]
        [JsonProperty("eventTime")]
        public string EventTime { get; set; }

        /// <summary>
        /// リソースプロバイダーに固有のイベントデータ
        /// </summary>
        [Required]
        [JsonProperty("data")]
        public object Data { get; set; }

        /// <summary>
        /// データオブジェクトのスキーマバージョン
        /// </summary>
        [Required]
        [JsonProperty("dataVersion")]
        public string DataVersion { get; set; }

        /// <summary>
        /// イベントメタデータのスキーマバージョン
        /// </summary>
        [Required]
        [JsonProperty("metadataVersion")]
        public string MetadataVersion { get; set; }
    }
}
