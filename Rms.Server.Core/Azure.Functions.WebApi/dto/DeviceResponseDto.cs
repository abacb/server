using Newtonsoft.Json;
using Rms.Server.Core.Azure.Utility.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Core.Azure.Functions.WebApi.Dto
{
    /// <summary>
    /// デバイスレスポンスDTO
    /// </summary>
    public class DeviceResponseDto
    {
        /// <summary>
        /// SID
        /// </summary>
        [Required]
        [JsonProperty("sid")]
        public long Sid { get; set; }

        /// <summary>
        /// エッジID
        /// </summary>
        [Required]
        [JsonProperty("edgeId")]
        public Guid EdgeId { get; set; }

        /// <summary>
        /// インストールタイプ
        /// </summary>
        [Required]
        [JsonProperty("installType")]
        public InstallTypeMasterDto InstallType { get; set; }

        /// <summary>
        /// デバイスUID（機器管理番号）
        /// </summary>
        [Required]
        [JsonProperty("equipmentUid")]
        public string EquipmentUid { get; set; }

        /// <summary>
        /// モデル（型式）
        /// </summary>
        [Required]
        [NestedValidate]
        [JsonProperty("models")]
        public ModelMasterDto Models { get; set; }

        /// <summary>
        /// 作成日時(UTC)
        /// </summary>
        [Required]
        [JsonProperty("createDatetime")]
        public DateTime? CreateDatetime { get; set; }

        /// <summary>
        /// 更新日時(UTC)
        /// </summary>
        [Required]
        [JsonProperty("updateDatetime")]
        public DateTime? UpdateDatetime { get; set; }

        /// <summary>
        /// リモート接続UID
        /// </summary>
        [JsonProperty("remoteConnectUid")]
        public string RemoteConnectUid { get; set; }
    }
}
