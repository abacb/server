using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Core.Azure.Functions.WebApi.Dto
{
    /// <summary>
    /// 配信ファイル種別マスタDTO
    /// </summary>
    public class DeliveryFileTypeMasterDto
    {
        /// <summary>
        /// SID
        /// </summary>
        [Required]
        [JsonProperty("deliveryFileTypeSid")]
        public long? DeliveryFileTypeSid { get; set; }

        /// <summary>
        /// コード
        /// </summary>
        [Required]
        [MaxLength(20)]
        [JsonProperty("deliveryFileTypeCode")]
        public string DeliveryFileTypeCode { get; set; }
    }
}
