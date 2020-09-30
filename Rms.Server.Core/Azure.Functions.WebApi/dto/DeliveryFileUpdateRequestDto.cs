using Newtonsoft.Json;
using Rms.Server.Core.Azure.Utility.Validations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Rms.Server.Core.Azure.Functions.WebApi.Dto
{
#pragma warning disable SA1402 // FileMayOnlyContainASingleType
    // HACK: Summry内に表記している各制約は当初各プロパティのSummryに含んでいたが、
    // NestされているクラスについてはSwagger側の制約によりDescriptionが表示されないため、
    // Descriptionと、それを使用している型に併記する。
    // https://github.com/swagger-api/swagger-ui/issues/5139

    /// <summary>
    /// 配信ファイル更新リクエストDTO<br/>
    /// ・型式について、配信ファイル種別が"hotfix_console","hotfix_hobbit"の場合必須(要素数1以上)。それ以外の場合無視する。<br/>
    /// ・インストールタイプについて、配信ファイル種別が"rms"の場合必須。それ以外の場合無視する。<br/>
    /// ・ファイルのバージョン情報について、配信ファイル種別が"rms","hotfix_console","hotfix_hobbit"の場合必須。それ以外の場合無視する。ASCII文字のみ許可する。<br/>
    /// ・適用対象バージョンについて、カンマ区切りの文字列。配信ファイル種別が"hotfix_console","hotfix_hobbit"の場合必須。それ以外の場合無視する。ASCII文字のみ許可する。<br/>
    /// ・情報IDについて、配信ファイル種別が"hotfix_console","hotfix_hobbit"の場合必須。<br/>
    /// </summary>
    /// <remarks>
    /// nullableを使用するのは、リクエストの欠落と初期値の区別をつけるため。
    /// ASCII文字のみ許可の判定は、DbAccessorで実施する。
    /// </remarks>
    public class DeliveryFileUpdateRequestDto
    {
        // 配信ファイルの場合、配信ファイル種別に必須項目、入力任意、入力無視項目が存在する。
        // コードとしてはこの区別を、配信ファイル種別ごとに本クラスの派生クラスにし、
        // パラメタチェックでその派生クラスを使用することで実装する。

        // ただしそれらをSwaggerに表示するために、本クラスの但し書きとして場合分けコメントを記載する必要がある。

        /// <summary>
        /// 配信ファイル種別
        /// </summary>
        /// <remarks>配信ファイル種別マスタ。更新としては必要ではないが、バリデーションチェックのために必要</remarks>
        [Required]
        [NestedValidate]
        [JsonProperty("deliveryFileType")]
        public DeliveryFileTypeMasterDto DeliveryFileType { get; set; }

        /// <summary>
        /// 型式。配信ファイル種別が"hotfix_console","hotfix_hobbit"の場合必須(要素数1以上)。それ以外の場合無視する。
        /// </summary>
        [NestedValidate]
        [JsonProperty("equipmentModels")]
        public IEnumerable<ModelMasterDto> EquipmentModels { get; set; }

        /// <summary>
        /// インストールタイプ。配信ファイル種別が"rms"の場合必須。それ以外の場合無視する。
        /// </summary>
        [NestedValidate]
        [JsonProperty("installType")]
        public InstallTypeMasterDto InstallType { get; set; }

        /// <summary>
        /// ファイルのバージョン情報。配信ファイル種別が"rms","hotfix_console","hotfix_hobbit"の場合必須。それ以外の場合無視する。
        /// ASCII文字のみ許可する。
        /// </summary>
        [MaxLength(30)]
        [JsonProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// 適用対象バージョン。カンマ区切りの文字列。配信ファイル種別が"hotfix_console","hotfix_hobbit"の場合必須。それ以外の場合無視する。
        /// ASCII文字のみ許可する。
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
        /// 情報ID。配信ファイル種別が"hotfix_console","hotfix_hobbit"の場合必須。
        /// </summary>
        [MaxLength(45)]
        [JsonProperty("informationId")]
        public string InformationId { get; set; }

        /// <summary>
        /// 楽観的同時制御用のバージョン番号
        /// </summary>
        [Required]
        [JsonProperty("rowVersion")]
        public long? RowVersion { get; set; }
    }

    /// <summary>
    /// A/Lソフト配信ファイル更新リクエストDTO
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "関連クラスのためとりまとめる")]
    public class DeliveryFileUpdateRequestTypeAlSoft : DeliveryFileUpdateRequestDto
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="source">DeliveryFileUpdateRequestDto</param>
        public DeliveryFileUpdateRequestTypeAlSoft(DeliveryFileUpdateRequestDto source)
        {
            //// そのクラスで「非表示」がないプロパティだけコピーして本オブジェクトを作成する

            this.DeliveryFileType = source.DeliveryFileType;
            //// this.EquipmentModels = source.EquipmentModels;
            this.InstallType = source.InstallType;
            this.Version = source.Version;
            //// this.InstallableVersion = source.InstallableVersion;
            this.Description = source.Description;
            this.InformationId = source.InformationId;
            this.RowVersion = source.RowVersion;
        }

        // 以下、入力必須項目のみ作成する。

        /// <summary>
        /// インストールタイプ
        /// </summary>
        [Required]
        [NestedValidate]
        [JsonProperty("installType")]
        public InstallTypeMasterDto InstallTypeEx => this.InstallType;

        /// <summary>
        /// ファイルのバージョン情報
        /// </summary>
        [Required]
        [JsonProperty("version")]
        public string VersionEx => this.Version;
    }

    /// <summary>
    /// HotFix配信ファイル更新リクエストDTO
    /// </summary>
    public class DeliveryFileUpdateRequestTypeHotFix : DeliveryFileUpdateRequestDto
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="source">DeliveryFileUpdateRequestDto</param>
        public DeliveryFileUpdateRequestTypeHotFix(DeliveryFileUpdateRequestDto source)
        {
            //// そのクラスで「非表示」がないプロパティだけコピーして本オブジェクトを作成する

            this.DeliveryFileType = source.DeliveryFileType;
            this.EquipmentModels = source.EquipmentModels;
            //// this.InstallType = source.InstallType;
            this.Version = source.Version;
            this.InstallableVersion = source.InstallableVersion;
            this.Description = source.Description;
            this.InformationId = source.InformationId;
            this.RowVersion = source.RowVersion;
        }

        // 以下、入力必須項目のみ作成する。

        /// <summary>
        /// 型式
        /// </summary>
        [Required]
        [RequiredAtLeastOneElement]
        [NestedValidate]
        [JsonProperty("equipmentModels")]
        public IEnumerable<ModelMasterDto> EquipmentModelsEx => this.EquipmentModels;

        /// <summary>
        /// ファイルのバージョン情報
        /// </summary>
        [Required]
        [JsonProperty("version")]
        public string VersionEx => this.Version;

        /// <summary>
        /// 適用対象バージョン。カンマ区切りの文字列。
        /// </summary>
        [Required]
        [JsonProperty("installableVersion")]
        public string InstallableVersionEx => this.InstallableVersion;

        /// <summary>
        /// 情報ID
        /// </summary>
        [Required]
        [JsonProperty("informationId")]
        public string InformationIdEx => this.InformationId;
    }

    /// <summary>
    /// パッケージ配信ファイル更新リクエストDTO
    /// </summary>
    public class DeliveryFileUpdateRequestTypePackage : DeliveryFileUpdateRequestDto
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="source">DeliveryFileUpdateRequestDto</param>
        public DeliveryFileUpdateRequestTypePackage(DeliveryFileUpdateRequestDto source)
        {
            //// そのクラスで「非表示」がないプロパティだけコピーして本オブジェクトを作成する

            this.DeliveryFileType = source.DeliveryFileType;
            //// this.EquipmentModels = source.EquipmentModels;
            //// this.InstallType = source.InstallType;
            //// this.Version = source.Version;
            //// this.InstallableVersion = source.InstallableVersion;
            this.Description = source.Description;
            this.InformationId = source.InformationId;
            this.RowVersion = source.RowVersion;
        }

        // パッケージの場合独自の必須入力項目はない。
    }
}