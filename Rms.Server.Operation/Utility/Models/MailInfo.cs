using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Operation.Utility.Models
{
    /// <summary>
    /// メール情報定義
    /// </summary>
    public class MailInfo
    {
        /// <summary>
        /// 宛先アドレス
        /// </summary>
        [Required]
        [JsonProperty("mail_address_to")]
        public string MailAddressTo { get; set; }

        /// <summary>
        /// 送信元アドレス
        /// </summary>
        [Required]
        [JsonProperty("mail_address_from")]
        public string MailAddressFrom { get; set; }

        /// <summary>
        /// 件名
        /// </summary>
        [Required]
        [JsonProperty("mail_subject")]
        public string MailSubject { get; set; }

        /// <summary>
        /// お客様番号
        /// </summary>
        [JsonProperty("customer_number")]
        public int? CustomerNumber { get; set; }

        /// <summary>
        /// 顧客名
        /// </summary>
        [JsonProperty("customer_name")]
        public string CustomerName { get; set; }

        /// <summary>
        /// 機器シリアル番号
        /// </summary>
        [JsonProperty("equipment_serial_number")]
        public string EquipmentSerialNumber { get; set; }

        /// <summary>
        /// 機器管理番号
        /// </summary>
        [JsonProperty("equipment_number")]
        public string EquipmentNumber { get; set; }

        /// <summary>
        /// 機器名
        /// </summary>
        [JsonProperty("equipment_name")]
        public string EquipmentName { get; set; }

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
        [JsonProperty("alarm_level")]
        public byte? AlarmLevel { get; set; }

        /// <summary>
        /// イベント日時
        /// </summary>
        [JsonProperty("event_date")]
        public string EventDate { get; set; }

        /// <summary>
        /// 説明
        /// </summary>
        [JsonProperty("alarm_description")]
        public string AlarmDescription { get; set; }
    }
}
