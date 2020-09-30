using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Core.Azure.Functions.WebApi.Dto
{
    /// <summary>
    /// 配信結果追加リクエストDTO
    /// </summary>
    /// <remarks>
    /// nullableを使用するのは、リクエストの欠落と初期値の区別をつけるため。
    /// </remarks>
    public class DeliveryResultAddRequestDto
    {
        /// <summary>
        /// 端末SID
        /// </summary>
        [Required]
        [JsonProperty("deviceSid")]
        public long? DeviceSid { get; set; }

        /// <summary>
        /// 最上位端末SID
        /// </summary>
        [Required]
        [JsonProperty("gatewayDeviceSid")]
        public long? GatewayDeviceSid { get; set; }
    }
}
