using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Core.Azure.Functions.WebApi.Dto
{
    /// <summary>
    /// 配信グループ更新リクエストDTO
    /// </summary>
    /// <remarks>
    /// nullableを使用するのは、リクエストの欠落と初期値の区別をつけるため。
    /// </remarks>
    public class DeliveryGroupUpdateRequestDto
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [MaxLength(100)]
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 配信開始日時
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
        /// 楽観的同時制御用のバージョン番号
        /// </summary>
        [Required]
        [JsonProperty("rowVersion")]
        public long? RowVersion { get; set; }
    }
}
