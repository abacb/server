using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Core.Azure.Functions.WebApi.Dto
{
    /// <summary>
    /// 配信ファイルステータス更新リクエストDTO
    /// </summary>
    public class DeliveryFileStatusUpdateRequestDto
    {
        /// <summary>
        /// 配信リクエスト
        /// </summary>
        public enum RequestDeliveryStatus
        {
            /// <summary>
            /// 配信中止
            /// </summary>
            Stop = 0,

            /// <summary>
            /// 配信開始
            /// </summary>
            Start
        }

        /// <summary>
        /// 配信ステータス
        /// </summary>
        [Required]
        [JsonProperty("deliveryStatus")]
        [JsonConverter(typeof(StringEnumConverter))]
        public RequestDeliveryStatus? DeliveryStatus { get; set; }

        /// <summary>
        /// 楽観的同時制御用のバージョン番号
        /// </summary>
        [Required]
        [JsonProperty("rowVersion")]
        public long? RowVersion { get; set; }
    }
}
