using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Rms.Server.Core.Azure.Functions.WebApi.Dto
{
#pragma warning disable SA1402 // FileMayOnlyContainASingleType
    /// <summary>
    /// 配信ファイルレスポンスDTO
    /// </summary>
    public class DeliveryFileResponseDto
    {
        /// <summary>
        /// SID
        /// </summary>
        [Required]
        [JsonProperty("sid")]
        public long Sid { get; set; }

        /// <summary>
        /// 配信ファイル種別SID
        /// </summary>
        /// <remarks>配信ファイル種別マスタで該当するID</remarks>
        [Required]
        [JsonProperty("deliveryFileTypeSid")]
        public long DeliveryFileTypeSid { get; set; }

        /// <summary>
        /// 配信対象ファイルのBlob上のパス
        /// </summary>
        [Required]
        [MaxLength(300)]
        [JsonProperty("filePath")]
        public string FilePath { get; set; }

        /// <summary>
        /// 型式
        /// </summary>
        [JsonProperty("equipmentModels")]
        public IEnumerable<DeliveryModelDto> EquipmentModels { get; set; }

        /// <summary>
        /// インストールタイプSID
        /// </summary>
        [JsonProperty("installTypeSid")]
        public long? InstallTypeSid { get; set; }

        /// <summary>
        /// ファイルのバージョン情報
        /// </summary>
        [MaxLength(30)]
        [JsonProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// 適用対象バージョン。カンマ区切りの文字列。
        /// </summary>
        [MaxLength(300)]
        [JsonProperty("installableVersion")]
        public string InstallableVersion { get; set; }

        /// <summary>
        /// 説明
        /// </summary>
        [MaxLength(200)]
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// 情報ID
        /// </summary>
        [MaxLength(45)]
        [JsonProperty("informationId")]
        public string InformationId { get; set; }

        /// <summary>
        /// 中止フラグ
        /// </summary>
        [JsonProperty("isCanceled")]
        public bool? IsCanceled { get; set; }

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
        /// 楽観的同時制御用のバージョン番号
        /// </summary>
        [Required]
        [JsonProperty("rowVersion")]
        public long? RowVersion { get; set; }
    }

    /// <summary>
    /// 配信ファイル型式DTO
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "関連クラスのためとりまとめる")]
    public class DeliveryModelDto
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [JsonProperty("sid")]
        public long Sid { get; set; }

        /// <summary>
        /// EquipmentModelSid
        /// </summary>
        [Required]
        [JsonProperty("equipmentModelSid")]
        public long EquipmentModelSid { get; set; }

        /// <summary>
        /// 作成日時(UTC)
        /// </summary>
        [Required]
        [JsonProperty("createDatetime")]
        public DateTime? CreateDatetime { get; set; }
    }
}