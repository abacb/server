using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Core.Azure.Functions.WebApi.Dto
{
    /// <summary>
    /// 型式マスタ
    /// </summary>
    public class ModelMasterDto
    {
        /// <summary>
        /// SID
        /// </summary>
        [Required]
        [JsonProperty("modelSid")]
        public long? ModelSid { get; set; }

        /// <summary>
        /// コード
        /// </summary>
        [Required]
        [MaxLength(30)]
        [JsonProperty("modelCode")]
        public string ModelCode { get; set; }
    }
}
