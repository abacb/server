using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Utility.Utility.Models
{
    /// <summary>
    /// エラーログ
    /// </summary>
    public class ErrorLog
    {
        /// <summary>
        /// 発生元機器UID
        /// </summary>
        [Required]
        [MaxLength(30)]
        [JsonProperty("SourceEquipmentUID")]
        public string SourceEquipmentUid { get; set; }

        /// <summary>
        /// 収集日時
        /// </summary>
        [Required]
        [MaxLength(20)]
        [JsonProperty("CollectDT")]
        public string CollectDt { get; set; }

        /// <summary>
        /// 機種コード
        /// </summary>
        [Required]
        [MaxLength(30)]
        [JsonProperty("TypeCode")]
        public string TypeCode { get; set; }

        /// <summary>
        /// エラーコード
        /// </summary>
        [Required]
        [MaxLength(20)]
        [JsonProperty("ErrorCode")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// エラー内容
        /// </summary>
        [Required]
        [MaxLength(1000)]
        [JsonProperty("ErrorContents")]
        public string ErrorContents { get; set; }

        /// <summary>
        /// イベント発生日時
        /// </summary>
        [Required]
        [MaxLength(30)]
        [JsonProperty("EventDT")]
        public string EventDt { get; set; }
    }
}
