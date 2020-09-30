using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Utility.Utility.Models
{
    /// <summary>
    /// パネル欠陥予兆結果ログ
    /// </summary>
    public class PanelDefectPredictiveResutLog
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
        [MaxLength(5)]
        [JsonProperty("ErrorCode")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// イベント発生日時
        /// </summary>
        [Required]
        [MaxLength(30)]
        [JsonProperty("EventDT")]
        public string EventDt { get; set; }
    }
}
