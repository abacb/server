using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Operation.Utility.Models
{
    /// <summary>
    /// アラーム情報定義
    /// </summary>
    public class AlarmInfo
    {
        /// <summary>
        /// 機器UID
        /// </summary>
        [Required]
        [JsonProperty("source_equipment_uid")]
        public string SourceEquipmentUid { get; set; }

        /// <summary>
        /// 機種コード
        /// </summary>
        [JsonProperty("type_code")]
        public string TypeCode { get; set; }

        /// <summary>
        /// エラーコード
        /// </summary>
        [JsonProperty("error_code")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// アラームレベル
        /// </summary>
        [JsonProperty("alarm_level", Required = Required.Always)]
        public byte AlarmLevel { get; set; }

        /// <summary>
        /// アラームタイトル
        /// </summary>
        [JsonProperty("alarm_title")]
        public string AlarmTitle { get; set; }

        /// <summary>
        /// アラーム内容
        /// </summary>
        [JsonProperty("alarm_description")]
        public string AlarmDescription { get; set; }

        /// <summary>
        /// アラーム日時
        /// </summary>
        [Required]
        [JsonProperty("alarm_datetime")]
        public string AlarmDatetime { get; set; }

        /// <summary>
        /// イベント日時
        /// </summary>
        [Required]
        [JsonProperty("event_datetime")]
        public string EventDatetime { get; set; }

        /// <summary>
        /// アラーム定義ID
        /// </summary>
        [Required]
        [JsonProperty("alarm_def_id")]
        public string AlarmDefId { get; set; }

        /// <summary>
        /// メッセージID
        /// </summary>
        [JsonProperty("message_id")]
        public string MessageId { get; set; }
    }
}
