using Newtonsoft.Json;
using Rms.Server.Core.Azure.Utility.Validations;
using Rms.Server.Core.Utility.Models.Entites;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Core.Utility.Models.Dispatch
{
    /// <summary>
    /// ソフトバージョン
    /// </summary>
    public class SoftVersionMessage : IConvertibleModel<DtSoftVersion>
    {
        /// <summary>
        /// 発生元機器UID
        /// </summary>
        [Required]
        [JsonProperty(nameof(SourceEquipmentUID))]
        public string SourceEquipmentUID { get; set; }

        /// <summary>
        /// 型式コード
        /// </summary>
        [Required]
        [JsonProperty(nameof(ModelCode))]
        public string ModelCode { get; set; }

        /// <summary>
        /// バージョン
        /// </summary>
        [Required]
        [JsonProperty(nameof(Version))]
        public string Version { get; set; }

        /// <summary>
        /// 収集日時
        /// </summary>
        [Required]
        [JsonProperty(nameof(CollectDT))]
        public DateTime? CollectDT { get; set; }

        /// <summary>
        /// 変換
        /// </summary>
        /// <param name="deviceId">デバイスSID</param>
        /// <param name="eventData">イベント情報</param>
        /// <returns>DtSoftVersion</returns>
        public DtSoftVersion Convert(long deviceId, RmsEvent eventData)
        {
            return new DtSoftVersion
            {
                //// Sid
                DeviceSid = deviceId,
                //// EquipmentModelSid, この段階ではなにも入らない。DBに設定するときに、ModelCodeを使ってマスタテーブルのSIDを取得する
                SourceEquipmentUid = SourceEquipmentUID,
                Version = Version,
                CollectDatetime = CollectDT,
                MessageId = eventData?.MessageId,
                //// CreateDatetime,
            };
        }
    }
}