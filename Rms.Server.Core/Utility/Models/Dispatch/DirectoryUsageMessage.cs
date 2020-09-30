using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rms.Server.Core.Utility.Models.Entites;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Core.Utility.Models.Dispatch
{
    /// <summary>
    /// ディレクトリ使用量
    /// </summary>
    public class DirectoryUsageMessage : IConvertibleModel<DtDirectoryUsage>
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
        /// 機種コード
        /// </summary>
        [Required]
        [JsonProperty(nameof(TypeCode))]
        public string TypeCode { get; set; }

        /// <summary>
        /// 詳細情報
        /// </summary>
        [JsonRequired]
        [JsonProperty(nameof(DetailInfo))]
        public JArray DetailInfo { get; set; }

        /// <summary>
        /// 変換
        /// </summary>
        /// <param name="deviceId">デバイスSID</param>
        /// <param name="eventData">イベント情報</param>
        /// <returns>DtDirectoryUsage</returns>
        public DtDirectoryUsage Convert(long deviceId, RmsEvent eventData)
        {
            return new DtDirectoryUsage
            {
                //// Sid
                //// TypeCodeはDBに入れない
                DeviceSid = deviceId,
                SourceEquipmentUid = SourceEquipmentUID,
                DetailInfo = DetailInfo != null ? JsonConvert.SerializeObject(DetailInfo, Formatting.Indented) : null,
                CollectDatetime = CollectDT,
                MessageId = eventData?.MessageId
                //// CreateDatetime
                //// DtDevice
            };
        }
    }
}
