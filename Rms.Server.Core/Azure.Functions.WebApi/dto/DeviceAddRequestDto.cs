using Newtonsoft.Json;
using Rms.Server.Core.Azure.Utility.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Rms.Server.Core.Azure.Functions.WebApi.Dto
{
#pragma warning disable SA1402 // FileMayOnlyContainASingleType

    /// <summary>
    /// デバイス追加リクエストDTO
    /// </summary>
    public class DeviceAddRequestDto
    {
        /// <summary>
        /// 自機のデバイス情報
        /// </summary>
        [Required]
        [NestedValidate]
        [JsonProperty("device")]
        public DeviceAddDto Device { get; set; }

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
    /// 関連デバイスDTO
    /// </summary>
    /// <remarks>
    /// Core側のDBに存在しないデータを含むため、リクエストで受け取る。そうでない値もあるが、二度手間になるためすべてリクエストで受ける。本項目の値チェックは行わない。
    /// </remarks>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "関連クラスのためとりまとめる")]
    public class RelatedDeviceDto
    {
        /// <summary>
        /// エッジID
        /// </summary>
        [JsonProperty("edgeId")]
        public Guid? EdgeId { get; set; }

        /// <summary>
        /// インストールタイプ
        /// </summary>
        [JsonProperty("installType")]
        public InstallTypeMasterDto InstallType { get; set; }

        /// <summary>
        /// デバイスUID（機器管理番号）
        /// </summary>
        [JsonProperty("equipmentUid")]
        public string EquipmentUid { get; set; }

        /// <summary>
        /// デバイス名（機器名）
        /// </summary>
        [JsonProperty("equipmentName")]
        public string EquipmentName { get; set; }

        /// <summary>
        /// シリアル番号
        /// </summary>
        [JsonProperty("equipmentSerialNumber")]
        public string EquipmentSerialNumber { get; set; }

        /// <summary>
        /// 設置場所
        /// </summary>
        [JsonProperty("installFeatures")]
        public string InstallFeatures { get; set; }

        /// <summary>
        /// IPアドレス
        /// </summary>
        [JsonProperty("ipAddress")]
        public string IpAddress { get; set; }

        /// <summary>
        /// ホスト名
        /// </summary>
        [JsonProperty("hostName")]
        public string HostName { get; set; }

        /// <summary>
        /// モデル（型式）
        /// </summary>
        [JsonProperty("model")]
        public ModelMasterDto Model { get; set; }

        /// <summary>
        /// 回線種別
        /// </summary>
        [JsonProperty("networkRoute")]
        public string NetworkRoute { get; set; }
    }

    /// <summary>
    /// デバイス登録DTO
    /// </summary>
    public class DeviceAddDto
    {
        /// <summary>
        /// インストールタイプ
        /// </summary>
        [Required]
        [NestedValidate]
        [JsonProperty("installType")]
        public InstallTypeMasterDto InstallType { get; set; }

        /// <summary>
        /// デバイスUID（機器管理番号）
        /// ASCII文字のみ許可する。
        /// </summary>
        /// <remarks>
        /// ASCII文字のみ許可の判定は、DbAccessorで実施する。
        /// </remarks>
        [Required]
        [MaxLength(30)]
        [JsonProperty("equipmentUid")]
        public string EquipmentUid { get; set; }

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
