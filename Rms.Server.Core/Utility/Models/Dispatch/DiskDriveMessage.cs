using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rms.Server.Core.Utility.Models.Entites;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Core.Utility.Models.Dispatch
{
    /// <summary>
    /// ディスクドライブ
    /// </summary>
    public class DiskDriveMessage : IConvertibleModel<DtDiskDrive>
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
        /// モデル
        /// </summary>
        [JsonProperty(nameof(Model))]
        public string Model { get; set; }

        /// <summary>
        /// メディアタイプ
        /// </summary>
        [JsonProperty(nameof(MediaType))]
        public string MediaType { get; set; }

        /// <summary>
        /// インターフェースタイプ
        /// </summary>
        [JsonProperty(nameof(InterfaceType))]
        public string InterfaceType { get; set; }

        /// <summary>
        /// シリアルナンバー
        /// </summary>
        [Required]
        [JsonProperty(nameof(SerialNo))]
        public string SerialNo { get; set; }

        /// <summary>
        /// SMART属性情報
        /// </summary>
        [JsonProperty(nameof(SmartAttributeInfo))]
        public JArray SmartAttributeInfo { get; set; }

        /// <summary>
        /// 変換
        /// </summary>
        /// <param name="deviceId">デバイスSID</param>
        /// <param name="eventData">イベント情報</param>
        /// <returns>DtDiskDrive</returns>
        public DtDiskDrive Convert(long deviceId, RmsEvent eventData)
        {
            return new DtDiskDrive
            {
                //// Sid
                DeviceSid = deviceId,
                SourceEquipmentUid = SourceEquipmentUID,
                Model = Model,
                MediaType = MediaType,
                InterfaceType = InterfaceType,
                SerialNumber = SerialNo,
                SmartAttributeInfo = SmartAttributeInfo != null ? JsonConvert.SerializeObject(SmartAttributeInfo, Formatting.Indented) : null,
                CollectDatetime = CollectDT,
                MessageId = eventData?.MessageId
                //// CreateDatetime
                //// DtDevice
            };
        }
    }
}
