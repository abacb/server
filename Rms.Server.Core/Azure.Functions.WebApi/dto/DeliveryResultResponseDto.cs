using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Core.Azure.Functions.WebApi.Dto
{
    /// <summary>
    /// 配信結果レスポンスDTO
    /// </summary>
    public class DeliveryResultResponseDto
    {
        /// <summary>
        /// SID
        /// </summary>
        [Required]
        [JsonProperty("sid")]
        public long Sid { get; set; }

        /// <summary>
        /// 端末SID
        /// </summary>
        [Required]
        [JsonProperty("deviceSid")]
        public long DeviceSid { get; set; }

        /// <summary>
        /// 最上位端末SID
        /// </summary>
        [Required]
        [JsonProperty("gatewayDeviceSid")]
        public long GatewayDeviceSid { get; set; }

        /// <summary>
        /// 作成日時(UTC)
        /// </summary>
        [Required]
        [JsonProperty("createDatetime")]
        public DateTime? CreateDatetime { get; set; }

        /// <summary>
        /// 適用結果履歴一覧
        /// </summary>
        [Required]
        [JsonProperty("installResultHistories")]
        public InstallResultHistoryResponseDto[] InstallResultHistories { get; set; }
    }
}
