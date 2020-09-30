using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Utility.Utility.Models
{
    /// <summary>
    /// ディレクトリ使用量
    /// </summary>
    public class DirectoryUsage
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
        /// 詳細情報
        /// </summary>
        [Required]
        [JsonProperty("DetailInfo")]
        public IEnumerable<DetailInfoSchema> DetailInfo { get; set; }

        /// <summary>
        /// 詳細情報スキーマ
        /// </summary>
        public class DetailInfoSchema
        {
            /// <summary>
            /// フルパス情報
            /// </summary>
            [Required]
            [MaxLength(300)]
            [JsonProperty("FullPath")]
            public string FullPath { get; set; }

            /// <summary>
            /// 容量サイズ(Byte)
            /// </summary>
            [Required]
            [JsonProperty("Size")]
            public int Size { get; set; }
        }
    }
}
