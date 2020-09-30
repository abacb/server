using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Utility.Utility.Models
{
    /// <summary>
    /// ディスクドライブ
    /// </summary>
    public class DiskDrive
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
        /// モデル
        /// </summary>
        [MaxLength(100)]
        [JsonProperty("Model")]
        public string Model { get; set; }

        /// <summary>
        /// メディアタイプ
        /// </summary>
        [MaxLength(20)]
        [JsonProperty("MediaType")]
        public string MediaType { get; set; }

        /// <summary>
        /// インターフェースタイプ
        /// </summary>
        [MaxLength(30)]
        [JsonProperty("InterfaceType")]
        public string InterfaceType { get; set; }

        /// <summary>
        /// シリアルナンバー
        /// </summary>
        [Required]
        [MaxLength(30)]
        [JsonProperty("SerialNo")]
        public string SerialNo { get; set; }

        /// <summary>
        /// SMART属性情報
        /// </summary>
        [JsonProperty("SmartAttributeInfo")]
        public IEnumerable<SmartAttributeInfoSchema> SmartAttributeInfo { get; set; }

        /// <summary>
        ///  SMART属性情報スキーマ
        /// </summary>
        public class SmartAttributeInfoSchema
        {
            /// <summary>
            /// ID値
            /// </summary>
            [MaxLength(2)]
            [JsonProperty("ID")]
            public string Id { get; set; }

            /// <summary>
            /// 現在値
            /// </summary>
            [JsonProperty("Value")]
            public short? Value { get; set; }

            /// <summary>
            /// 最悪値
            /// </summary>
            [JsonProperty("Worst")]
            public short? Worst { get; set; }

            /// <summary>
            /// 閾値
            /// </summary>
            [JsonProperty("Threshold")]
            public short? Threshold { get; set; }

            /// <summary>
            /// 生の値
            /// </summary>
            [MaxLength(17)]
            [JsonProperty("RawData")]
            public string RawData { get; set; }
        }
    }
}
