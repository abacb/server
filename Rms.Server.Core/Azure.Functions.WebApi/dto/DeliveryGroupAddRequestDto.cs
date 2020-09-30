using Newtonsoft.Json;
using Rms.Server.Core.Azure.Utility.Validations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Core.Azure.Functions.WebApi.Dto
{
    /// <summary>
    /// 配信グループ追加リクエストDTO
    /// </summary>
    /// <remarks>
    /// nullableを使用するのは、リクエストの欠落と0の区別をつけるため。
    /// </remarks>
    public class DeliveryGroupAddRequestDto
    {
        /// <summary>
        /// 配信ファイルSID
        /// </summary>
        [Required]
        [JsonProperty("deliveryFileSid")]
        public long? DeliveryFileSid { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [MaxLength(100)]
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 配信開始日時(UTC)
        /// </summary>
        [Required]
        [JsonProperty("startDatetime")]
        public DateTime? StartDatetime { get; set; }

        /// <summary>
        /// ダウンロード遅延時間
        /// </summary>
        [Required]
        [Range(0, short.MaxValue)]
        [JsonProperty("downloadDelayTime")]
        public short? DownloadDelayTime { get; set; }

        /// <summary>
        /// 配信先一覧。要素数1以上を必要とする。
        /// </summary>
        [Required]
        [RequiredAtLeastOneElement]
        [NestedValidate]
        [JsonProperty("deliveryDestinations")]
        public DeliveryResultAddRequestDto[] DeliveryDestinations { get; set; }
    }
}
