using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rms.Server.Core.Utility.Models.Entites;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Core.Utility.Models.Dispatch
{
    /// <summary>
    /// 機器利用実績
    /// </summary>
    public class UsageMessage : IConvertibleModel<DtEquipmentUsage>
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
        /// 詳細情報
        /// </summary>
        [JsonProperty(nameof(DetailInfo))]
        public JToken DetailInfo { get; set; }

        /// <summary>
        /// 変換
        /// </summary>
        /// <param name="deviceId">デバイスSID</param>
        /// <param name="eventData">イベント情報</param>
        /// <returns>DtEquipmentUsage</returns>
        public DtEquipmentUsage Convert(long deviceId, RmsEvent eventData)
        {
            return new DtEquipmentUsage
            {
                //// Sid
                DeviceSid = deviceId,
                SourceEquipmentUid = SourceEquipmentUID,
                DetailInfo = DetailInfo.HasValues ? JsonConvert.SerializeObject(DetailInfo, Formatting.Indented) : null,
                CollectDatetime = CollectDT,
                MessageId = eventData?.MessageId
                //// CreateDatetime
                //// DtDevice
            };
        }
    }
}
