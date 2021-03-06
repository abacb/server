//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはテンプレートから生成されました。
//
//     このファイルを手動で変更すると、アプリケーションで予期しない動作が発生する可能性があります。
//     このファイルに対する手動の変更は、コードが再生成されると上書きされます。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Rms.Server.Core.DBAccessor.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    
    /// <summary>
    /// DtScriptConfigクラス
    /// </summary>
    [ModelMetadataType(typeof(DtScriptConfigModelMetaData))]
    public partial class DtScriptConfig
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DtScriptConfig()
        {
        }
    
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="model">Utility.Models.Entites.DtScriptConfigのインスタンス</param>
        public DtScriptConfig(Utility.Models.Entites.DtScriptConfig model)
        {
            this.Sid = model.Sid;
            this.InstallTypeSid = model.InstallTypeSid;
            this.Name = model.Name;
            this.Version = model.Version;
            this.FileName = model.FileName;
            this.Location = model.Location;
            this.CreateDatetime = model.CreateDatetime;
            this.InstallTypeS = model.MtInstallType == null ?
                null :
                new MtInstallType(model.MtInstallType);
        }
    
        /// <summary>
        /// このインスタンスの各プロパティに、引数に指定されたDtScriptConfigのプロパティの値をコピーする
        /// </summary>
        /// <param name="entity">コピー元のDtScriptConfig</param>
        public void CopyFrom(DtScriptConfig entity)
        {
            this.Sid = entity.Sid;
            this.InstallTypeSid = entity.InstallTypeSid;
            this.Name = entity.Name;
            this.Version = entity.Version;
            this.FileName = entity.FileName;
            this.Location = entity.Location;
            this.CreateDatetime = entity.CreateDatetime;
            this.InstallTypeS = entity.InstallTypeS;
        }
    
        /// <summary>
        /// このインスタンスを、それと同等なUtility.Models.Entites.DtScriptConfig型に変換する。
        /// 各変換メソッド共通部分
        /// </summary>
        /// <returns></returns>
        private Utility.Models.Entites.DtScriptConfig ToModelCommonPart()
        {
            Utility.Models.Entites.DtScriptConfig model = new Utility.Models.Entites.DtScriptConfig();
            model.Sid = this.Sid;
            model.InstallTypeSid = this.InstallTypeSid;
            model.Name = this.Name;
            model.Version = this.Version;
            model.FileName = this.FileName;
            model.Location = this.Location;
            model.CreateDatetime = this.CreateDatetime;
            return model;
        }
    
        /// <summary>
        /// このインスタンスを、それと同等なUtility.Models.Entites.DtScriptConfig型に変換する。
        /// </summary>
        /// <returns></returns>
        public Utility.Models.Entites.DtScriptConfig ToModel()
        {
            Utility.Models.Entites.DtScriptConfig model = ToModelCommonPart();
            model.MtInstallType = this.InstallTypeS?.ToParentModel(this.GetType());
    
            return model;
        }
    
        /// <summary>
        /// このインスタンスを、それと同等なUtility.Models.Entites.DtScriptConfig型に変換する。
        /// 親エンティティとして生成するため、子エンティティの情報はもたない
        /// </summary>
        /// <returns></returns>
        public Utility.Models.Entites.DtScriptConfig ToParentModel(Type childType)
        {
            Utility.Models.Entites.DtScriptConfig model = ToModelCommonPart();
            model.MtInstallType = this.InstallTypeS?.ToParentModel(this.GetType());
    
            return model;
        }
    
        /// <summary>
        /// このインスタンスを、それと同等なUtility.Models.Entites.DtScriptConfig型に変換する。
        /// 子エンティティとして生成するため、親エンティティの情報をもたない
        /// </summary>
        /// <returns></returns>
        public Utility.Models.Entites.DtScriptConfig ToChildModel(Type parentType)
        {
            Utility.Models.Entites.DtScriptConfig model = ToModelCommonPart();
            // 親子間の参照無限ループを避けるためにタイプチェック
            if (this.InstallTypeS?.GetType() != parentType)
            {
                model.MtInstallType = this.InstallTypeS?.ToParentModel(this.GetType());
            }
    
            return model;
        }
    }
    
    /// <summary>
    /// DtScriptConfigのメタデータクラス
    /// </summary>
    public  class DtScriptConfigModelMetaData
    {
        [Key]
        [Required(ErrorMessage = "Sid is required.")]
        public long Sid { get; set; }
    
        [Required(ErrorMessage = "InstallTypeSid is required.")]
        public long InstallTypeSid { get; set; }
    
        [StringLength(30, ErrorMessage = "Name length should be less than 30 symbols.")]
        [RegularExpression(Utility.Const.AsciiCodeCharactersReg, ErrorMessage = "Name is only allowed for ASCII code characters.")]
        public string Name { get; set; }
    
        [StringLength(64, ErrorMessage = "FileName length should be less than 64 symbols.")]
        public string FileName { get; set; }
    
        [StringLength(300, ErrorMessage = "Location length should be less than 300 symbols.")]
        public string Location { get; set; }
    
        [Required(ErrorMessage = "CreateDatetime is required.")]
        public System.DateTime CreateDatetime { get; set; }
    
    }
}
