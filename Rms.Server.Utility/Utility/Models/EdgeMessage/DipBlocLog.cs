using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Utility.Utility.Models
{
    /// <summary>
    /// 骨塩ムラログ
    /// </summary>
    public class DipBlocLog
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
        /// 発生年月日
        /// </summary>
        [Required]
        [MaxLength(30)]
        [JsonProperty("OccurrenceYM")]
        public string OccurrenceYm { get; set; }

        /// <summary>
        /// 通番
        /// </summary>
        [Required]
        [MaxLength(10)]
        [JsonProperty("SNumber")]
        public string SNumber { get; set; }

        /// <summary>
        /// Detector名称
        /// </summary>
        [Required]
        [MaxLength(30)]
        [JsonProperty("DetectorName")]
        public string DetectorName { get; set; }

        /// <summary>
        /// DetectorID
        /// </summary>
        [Required]
        [MaxLength(30)]
        [JsonProperty("DetectorID")]
        public string DetectorId { get; set; }

        /// <summary>
        /// GP値
        /// </summary>
        [Required]
        [MaxLength(30)]
        [JsonProperty("GPValue")]
        public string GpValue { get; set; }

        /// <summary>
        /// ファイル名称
        /// </summary>
        [Required]
        [MaxLength(30)]
        [JsonProperty("FileName")]
        public string FileName { get; set; }

        /// <summary>
        /// サービスマンフラグ
        /// </summary>
        [JsonProperty("ServiceFLG")]
        public bool? ServiceFlg { get; set; }

        /// <summary>
        /// ファイル作成日時
        /// </summary>
        [Required]
        [MaxLength(30)]
        [JsonProperty("FileCreateDT")]
        public string FileCreateDt { get; set; }

        /// <summary>
        /// プロファイル値
        /// </summary>
        [Required]
        [MaxLength(3000)]
        [JsonProperty("ProfileValue")]
        public string[] ProfileValue { get; set; }

        /// <summary>
        /// ログファイル名
        /// </summary>
        [Required]
        [MaxLength(64)]
        [JsonProperty("LogFileName")]
        public string LogFileName { get; set; }
    }
}
