using Newtonsoft.Json;
using Rms.Server.Core.Azure.Utility.Validations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Rms.Server.Core.Azure.Functions.WebApi.Dto
{
#pragma warning disable SA1402 // FileMayOnlyContainASingleType

    /// <summary>
    /// デバイス更新リクエストDTO
    /// </summary>
    public class DeviceUpdateRequestDto
    {
        /// <summary>
        /// 自機のデバイス情報
        /// </summary>
        [Required]
        [NestedValidate]
        [JsonProperty("device")]
        public DeviceUpdateDto Device { get; set; }

        /// <summary>
        /// 親機のデバイス情報
        /// </summary>
        [JsonProperty("parent")]
        public RelatedDeviceDto Parent { get; set; }

        /// <summary>
        /// 子機のデバイス情報
        /// </summary>
        [JsonProperty("children")]
        public IEnumerable<RelatedDeviceDto> Children { get; set; }
    }

    /// <summary>
    /// デバイス更新DTO
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "関連クラスのためとりまとめる")]
    public class DeviceUpdateDto
    {
        /// <summary>
        /// インストールタイプ
        /// </summary>
        [Required]
        [NestedValidate]
        [JsonProperty("installType")]
        public InstallTypeMasterDto InstallType { get; set; }

        /// <summary>
        /// デバイス名（機器名）
        /// </summary>
        [Required]
        [MaxLength(60)]
        [JsonProperty("equipmentName")]
        public string EquipmentName { get; set; }

        /// <summary>
        /// シリアル番号
        /// </summary>
        [Required]
        [MaxLength(30)]
        [JsonProperty("equipmentSerialNumber")]
        public string EquipmentSerialNumber { get; set; }

        /// <summary>
        /// 設置場所
        /// </summary>
        [MaxLength(64)]
        [JsonProperty("installFeatures")]
        public string InstallFeatures { get; set; }

        /// <summary>
        /// IPアドレス
        /// </summary>
        [MaxLength(200)]
        [JsonProperty("ipAddress")]
        public string IpAddress { get; set; }

        /// <summary>
        /// ホスト名
        /// </summary>
        [MaxLength(30)]
        [JsonProperty("hostname")]
        public string HostName { get; set; }

        /// <summary>
        /// モデル（型式）
        /// </summary>
        [Required]
        [NestedValidate]
        [JsonProperty("model")]
        public ModelMasterDto Model { get; set; }

        /// <summary>
        /// 回線種別
        /// </summary>
        [Required]
        [JsonProperty("networkRoute")]
        public string NetworkRoute { get; set; }
    }
}
