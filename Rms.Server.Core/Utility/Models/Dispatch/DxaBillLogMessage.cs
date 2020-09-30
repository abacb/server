using Newtonsoft.Json;
using Rms.Server.Core.Utility.Models.Entites;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Core.Utility.Models.Dispatch
{
    /// <summary>
    /// DXA課金ログ
    /// </summary>
    public class DxaBillLogMessage : IConvertibleModel<DtDxaBillLog>
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
        /// 患者ID
        /// </summary>
        [Required]
        [JsonProperty(nameof(PatientID))]
        public string PatientID { get; set; }

        /// <summary>
        /// 計測名称
        /// </summary>
        [Required]
        [JsonProperty(nameof(TypeName))]
        public byte? TypeName { get; set; }

        /// <summary>
        /// 検査日時
        /// </summary>
        [Required]
        [JsonProperty(nameof(StudyDT))]
        public DateTime? StudyDT { get; set; }

        /// <summary>
        /// 計測日時
        /// </summary>
        [Required]
        [JsonProperty(nameof(MeasureDT))]
        public DateTime? MeasureDT { get; set; }

        /// <summary>
        /// DXAライセンスキー
        /// </summary>
        [Required]
        [JsonProperty(nameof(OptionDXA))]
        public bool? OptionDXA { get; set; }

        /// <summary>
        /// サービスモード
        /// </summary>
        [Required]
        [JsonProperty(nameof(ServiceMode))]
        public bool? ServiceMode { get; set; }

        /// <summary>
        /// 変換
        /// </summary>
        /// <param name="deviceId">デバイスSID</param>
        /// <param name="eventData">イベント情報</param>
        /// <returns>DtDxaBillLog</returns>
        public DtDxaBillLog Convert(long deviceId, RmsEvent eventData)
        {
            return new DtDxaBillLog()
            {
                //// Sid
                DeviceSid = deviceId,
                SoueceEquipmentUid = SourceEquipmentUID,
                StudyInstanceUid = StudyInstanceUID,
                PatientId = PatientID,
                TypeName = TypeName,
                StudyDatetime = StudyDT,
                MeasureDatetime = MeasureDT,
                OptionDxa = OptionDXA,
                ServiceMode = ServiceMode,
                CollectDatetime = CollectDT,
                //// MessageId = eventData?.MessageId // 更新データのためMessageIdは持たない。
                //// CreateDatetime
                //// DtDevice
            };
        }
    }
}
