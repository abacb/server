using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Core.Azure.Functions.WebApi.Dto
{
    /// <summary>
    /// 配信グループレスポンスDTO
    /// </summary>
    public class DeliveryGroupResponseDto
    {
        /// <summary>
        /// SID
        /// </summary>
        [Required]
        [JsonProperty("sid")]
        public long Sid { get; set; }

        /// <summary>
        /// 配信ファイルSID
        /// </summary>
        [Required]
        [JsonProperty("deliveryFileSid")]
        public long DeliveryFileSid { get; set; }

        /// <summary>
        /// 配信グループステータスSID
        /// </summary>
        [Required]
        [JsonProperty("deliveryGroupStatusSid")]
        public long DeliveryGroupStatusSid { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 配信開始日時(UTC)
        /// </summary>
        [Required]
        [JsonProperty("startDatetime")]
        public DateTime StartDatetime { get; set; }

        /// <summary>
        /// ダウンロード遅延時間
        /// </summary>
        [JsonProperty("downloadDelayTime")]
        public short? DownloadDelayTime { get; set; }

        /// <summary>
        /// 作成日時(UTC)
        /// </summary>
        [Required]
        [JsonProperty("createDatetime")]
        public DateTime? CreateDatetime { get; set; }

        /// <summary>
        /// 更新日時(UTC)
        /// </summary>
        [Required]
        [JsonProperty("updateDatetime")]
        public DateTime? UpdateDatetime { get; set; }

        /// <summary>
        /// 楽観的同時制御用のバージョン番号
        /// </summary>
        [Required]
        [JsonProperty("rowVersion")]
        public long RowVersion { get; set; }

        /// <summary>
        /// 配信先一覧
        /// </summary>
        [Required]
        [JsonProperty("deliveryDestinations")]
        public DeliveryResultResponseDto[] DeliveryDestinations { get; set; }
    }
}
