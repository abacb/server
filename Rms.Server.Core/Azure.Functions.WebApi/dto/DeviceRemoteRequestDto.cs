using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace RmsRms.Server.Core.Azure.Functions.WebApi.Dto
{
    /// <summary>
    /// リモート接続開始要求DTO
    /// </summary>
    /// <remarks>
    /// nullableを使用するのは、リクエストの欠落と初期値の区別をつけるため。
    /// </remarks>
    public class DeviceRemoteRequestDto
    {
        /// <summary>
        /// セッションコード
        /// </summary>
        [Required]
        [JsonProperty("sessionCode")]
        public string SessionCode { get; set; }
    }
}
