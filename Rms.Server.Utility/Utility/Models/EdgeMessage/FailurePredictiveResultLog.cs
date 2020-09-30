using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Utility.Utility.Models
{
    /// <summary>
    /// 故障予兆結果ログ
    /// </summary>
    public class FailurePredictiveResultLog
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
        /// 閾値
        /// </summary>
        [JsonProperty("Threshold")]
        public ushort? Threshold { get; set; }

        /// <summary>
        /// 発生回数
        /// </summary>
        [JsonProperty("NumOfTimes")]
        public ushort? NumOfTimes { get; set; }

        /// <summary>
        /// 最終イベント発生日時
        /// </summary>
        [MaxLength(30)]
        [JsonProperty("LastEventDT")]
        public string LastEventDt { get; set; }

        /// <summary>
        /// エラー内容
        /// </summary>
        [Required]
        [MaxLength(1000)]
        [JsonProperty("ErrorContents")]
        public string ErrorContents { get; set; }
    }
}
