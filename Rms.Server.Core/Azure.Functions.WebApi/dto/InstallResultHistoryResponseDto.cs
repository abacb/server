using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Core.Azure.Functions.WebApi.Dto
{
    /// <summary>
    /// 適用結果履歴レスポンスDTO
    /// </summary>
    public class InstallResultHistoryResponseDto
    {
        /// <summary>
        /// SID
        /// </summary>
        [Required]
        [JsonProperty("sid")]
        public long Sid { get; set; }

        /// <summary>
        /// 端末SID
        /// </summary>
        [Required]
        [JsonProperty("deviceSid")]
        public long? DeviceSid { get; set; }

        /// <summary>
        /// 配信結果SID
        /// </summary>
        [JsonProperty("deliveryResultSid")]
        public long? DeliveryResultSid { get; set; }

        /// <summary>
        /// 適用結果ステータスsid
        /// </summary>
        [Required]
        [JsonProperty("installResultStatusSid")]
        public long InstallResultStatusSid { get; set; }

        /// <summary>
        /// 発生元機器UID
        /// </summary>
        [JsonProperty("sourceEquipmentUid")]
        public string SourceEquipmentUid { get; set; }

        /// <summary>
        /// リリースバージョン
        /// </summary>
        [JsonProperty("releaseVersion")]
        public string ReleaseVersion { get; set; }

        /// <summary>
        /// 前バージョン
        /// </summary>
        [JsonProperty("beforeVersion")]
        public string BeforeVersion { get; set; }

        /// <summary>
        /// 後バージョン
        /// </summary>
        [JsonProperty("afterVervion")]
        public string AfterVervion { get; set; }

        /// <summary>
        /// 成功（成否）
        /// </summary>
        [JsonProperty("isSuccess")]
        public bool? IsSuccess { get; set; }

        /// <summary>
        /// エラーコード
        /// </summary>
        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// エラー内容(説明)
        /// </summary>
        [JsonProperty("errorDescription")]
        public string ErrorDescription { get; set; }

        /// <summary>
        /// 自動適用フラグ
        /// </summary>
        [JsonProperty("isAuto")]
        public bool? IsAuto { get; set; }

        /// <summary>
        /// インストール方法
        /// </summary>
        [JsonProperty("method")]
        public string Method { get; set; }

        /// <summary>
        /// アップデート処理プロセス
        /// </summary>
        [JsonProperty("process")]
        public string Process { get; set; }

        /// <summary>
        /// アップデート処理開始日時
        /// </summary>
        [JsonProperty("updateStratDatetime")]
        public DateTime? UpdateStratDatetime { get; set; }

        /// <summary>
        /// アップデート処理終了日時
        /// </summary>
        [JsonProperty("updateEndDatetime")]
        public DateTime? UpdateEndDatetime { get; set; }

        /// <summary>
        /// コンピュータ名
        /// </summary>
        [JsonProperty("computerName")]
        public string ComputerName { get; set; }

        /// <summary>
        /// IPアドレス
        /// </summary>
        [JsonProperty("ipAddress")]
        public string IpAddress { get; set; }

        /// <summary>
        /// サーバ・クライアント種別
        /// </summary>
        [JsonProperty("serverClientKind")]
        public string ServerClientKind { get; set; }

        /// <summary>
        /// 修理報告出力フラグ
        /// </summary>
        [JsonProperty("hasRepairReport")]
        public bool? HasRepairReport { get; set; }

        /// <summary>
        /// イベント日時
        /// </summary>
        [JsonProperty("eventDatetime")]
        public DateTime? EventDatetime { get; set; }

        /// <summary>
        /// 収集日時
        /// </summary>
        [JsonProperty("collectDatetime")]
        public DateTime? CollectDatetime { get; set; }

        /// <summary>
        /// メッセージID
        /// </summary>
        [JsonProperty("messageId")]
        public string MessageId { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        [JsonProperty("createDatetime")]
        public DateTime? CreateDatetime { get; set; }
    }
}
