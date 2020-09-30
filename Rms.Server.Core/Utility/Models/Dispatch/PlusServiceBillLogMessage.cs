using Newtonsoft.Json;
using Rms.Server.Core.Utility.Models.Entites;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Core.Utility.Models.Dispatch
{
    /// <summary>
    /// PLUSサービス課金ログ				
    /// </summary>
    public class PlusServiceBillLogMessage : IConvertibleModel<DtPlusServiceBillLog>
    {
        /// <summary>
        /// 発生元機器UID
        /// </summary>
        [Required]
        [JsonProperty(nameof(SourceEquipmentUID))]
        public string SourceEquipmentUID { get; set; }

        /// <summary>
        /// 収集日時
        /// </summary>
        [Required]
        [JsonProperty(nameof(CollectDT))]
        public DateTime? CollectDT { get; set; }

        /// <summary>
        /// 検査インスタンスUID
        /// </summary>
        [Required]
        [JsonProperty(nameof(StudyInstanceUID))]
        public string StudyInstanceUID { get; set; }

        /// <summary>
        /// 画像インスタンスUID
        /// </summary>
        [Required]
        [JsonProperty(nameof(SOPInstanceUID))]
        public string SOPInstanceUID { get; set; }

        /// <summary>
        /// 計測名称
        /// </summary>
        [Required]
        [JsonProperty(nameof(TypeName))]
        public string TypeName { get; set; }

        /// <summary>
        /// 検査日時
        /// </summary>
        [Required]
        [JsonProperty(nameof(StudyDT))]
        public DateTime? StudyDT { get; set; }

        /// <summary>
        /// 患者ID
        /// </summary>
        [Required]
        [JsonProperty(nameof(PatientID))]
        public string PatientID { get; set; }

        /// <summary>
        /// 性別
        /// </summary>
        [Required]
        [JsonProperty(nameof(Sex))]
        public byte? Sex { get; set; }

        /// <summary>
        /// 検査時年齢
        /// </summary>
        [Required]
        [JsonProperty(nameof(Age))]
        public byte? Age { get; set; }

        /// <summary>
        /// 測定値
        /// </summary>
        [JsonProperty(nameof(MeasureValue))]
        public float? MeasureValue { get; set; }

        /// <summary>
        /// 測定日時
        /// </summary>
        [Required]
        [JsonProperty(nameof(MeasureDT))]
        public DateTime? MeasureDT { get; set; }

        /// <summary>
        /// 課金フラグ
        /// </summary>
        [Required]
        [JsonProperty(nameof(BillFLG))]
        public bool? BillFLG { get; set; }

        /// <summary>
        /// 変換
        /// </summary>
        /// <param name="deviceId">デバイスSID</param>
        /// <param name="eventData">イベント情報</param>
        /// <returns>DtPlusServiceBillLog</returns>
        public DtPlusServiceBillLog Convert(long deviceId, RmsEvent eventData)
        {
            return new DtPlusServiceBillLog()
            {
                //// Sid
                DeviceSid = deviceId,
                SourceEquipmentUid = SourceEquipmentUID,
                TypeName = TypeName,
                BillFlg = BillFLG,
                PatientId = PatientID,
                Sex = Sex,
                Age = Age,
                StudyInstanceUid = StudyInstanceUID,
                SopInstanceUid = SOPInstanceUID,
                StudyDatetime = StudyDT,
                MeasureValue = MeasureValue,
                MeasureDatetime = MeasureDT,
                CollectDatetime = CollectDT,
                ////MessageId = eventData?.MessageId // 更新データのためMessageIdは持たない。
                //// CreateDatetime
                //// DtDevice
            };
        }
    }
}
