using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Utility.Utility.Models
{
    /// <summary>
    /// 適用結果
    /// </summary>
    public class InstallResult
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
        /// 配信結果SID
        /// </summary>
        [MaxLength(36)]
        [JsonProperty("DeliveryResultSID")]
        public long? DeliveryResultSid { get; set; }

        /// <summary>
        /// 自動適用
        /// </summary>
        [Required]
        [JsonProperty("Auto")]
        public bool Auto { get; set; }

        /// <summary>
        /// インストール方法
        /// </summary>
        [Required]
        [MaxLength(20)]
        [JsonProperty("Method")]
        public string Method { get; set; }

        /// <summary>
        /// アップデート処理プロセス
        /// </summary>
        [MaxLength(3)]
        [JsonProperty("Process")]
        public string Process { get; set; }

        /// <summary>
        /// アップデート処理開始日時
        /// </summary>
        [Required]
        [MaxLength(30)]
        [JsonProperty("UpdateStart")]
        public string UpdateStart { get; set; }

        /// <summary>
        /// アップデート処理終了日時
        /// </summary>
        [Required]
        [MaxLength(30)]
        [JsonProperty("UpdateEnd")]
        public string UpdateEnd { get; set; }

        /// <summary>
        /// コンピュータ名
        /// </summary>
        [MaxLength(20)]
        [JsonProperty("ComputerName")]
        public string ComputerName { get; set; }

        /// <summary>
        /// IPアドレス
        /// </summary>
        [MaxLength(120)]
        [JsonProperty("IpAddress")]
        public string IpAddress { get; set; }

        /// <summary>
        /// サーバ・クライアント種別
        /// </summary>
        [MaxLength(20)]
        [JsonProperty("ServerClientKind")]
        public string ServerClientKind { get; set; }

        /// <summary>
        /// アップデート処理前の内部バージョン
        /// </summary>
        [MaxLength(30)]
        [JsonProperty("BeforeVersion")]
        public string BeforeVersion { get; set; }

        /// <summary>
        /// アップデート処理後の内部バージョン
        /// </summary>
        [MaxLength(30)]
        [JsonProperty("AfterVersion")]
        public string AfterVersion { get; set; }

        /// <summary>
        /// 成功（成否）
        /// </summary>
        [Required]
        [JsonProperty("Success")]
        public bool Success { get; set; }

        /// <summary>
        /// 状態
        /// </summary>
        [JsonProperty("State")]
        public ushort? State { get; set; }

        /// <summary>
        /// エラーコード
        /// </summary>
        [MaxLength(20)]
        [JsonProperty("ErrorCode")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// エラー内容
        /// </summary>
        [MaxLength(1000)]
        [JsonProperty("ErrorDescription")]
        public string ErrorDescription { get; set; }

        /// <summary>
        /// リリースバージョン
        /// </summary>
        [MaxLength(30)]
        [JsonProperty("ReleaseVersion")]
        public string ReleaseVersion { get; set; }

        /// <summary>
        /// 機種コード
        /// </summary>
        [MaxLength(30)]
        [JsonProperty("TypeCode")]
        public string TypeCode { get; set; }

        /// <summary>
        /// イベント発生日時
        /// </summary>
        [Required]
        [MaxLength(30)]
        [JsonProperty("EventDT")]
        public string EventDt { get; set; }
    }
}
