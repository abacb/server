using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Utility.Utility.Models
{
    /// <summary>
    /// IoT Hubデバイステレメトリイベント
    /// </summary>
    public class IotHubDeviceTelemetry
    {
        /// <summary>
        /// アプリケーションプロパティ(メッセージに追加できるユーザー定義の文字列)
        /// </summary>
        [JsonProperty("properties")]
        public object Properties { get; set; }

        /// <summary>
        /// システムプロパティ
        /// </summary>
        [JsonProperty("systemProperties")]
        public D2CIotHubSystemProperties SystemProperties { get; set; }

        /// <summary>
        /// デバイスからのメッセージの内容
        /// </summary>
        [Required]
        [JsonProperty("body")]
        public object Body { get; set; }

        /// <summary>
        /// D2C IoT Hubメッセージシステムプロパティ(使用するプロパティのみ定義)
        /// </summary>
        public class D2CIotHubSystemProperties
        {
            /// <summary>
            /// メッセージID
            /// </summary>
            [JsonProperty("message-id")]
            public string MessageId { get; set; }
        }
    }
}
