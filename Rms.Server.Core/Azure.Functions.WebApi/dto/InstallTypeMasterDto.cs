using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Core.Azure.Functions.WebApi.Dto
{
    /// <summary>
    /// インストールタイプマスタDTO
    /// </summary>
    public class InstallTypeMasterDto
    {
        /// <summary>
        /// SID
        /// </summary>
        [Required]
        [JsonProperty("installTypeSid")]
        public long? InstallTypeSid { get; set; }

        /// <summary>
        /// コード
        /// </summary>
        [Required]
        [MaxLength(30)]
        [JsonProperty("installTypeCode")]
        public string InstallTypeCode { get; set; }
    }
}
