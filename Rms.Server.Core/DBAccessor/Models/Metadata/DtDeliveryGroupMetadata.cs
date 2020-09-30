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
    /// DtDeliveryGroupクラス
    /// </summary>
    [ModelMetadataType(typeof(DtDeliveryGroupModelMetaData))]
    public partial class DtDeliveryGroup
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="model">Utility.Models.Entites.DtDeliveryGroupのインスタンス</param>
        public DtDeliveryGroup(Utility.Models.Entites.DtDeliveryGroup model)
        {
            this.Sid = model.Sid;
            this.DeliveryFileSid = model.DeliveryFileSid;
            this.DeliveryGroupStatusSid = model.DeliveryGroupStatusSid;
            this.Name = model.Name;
            this.StartDatetime = model.StartDatetime;
            this.DownloadDelayTime = model.DownloadDelayTime;
            this.CreateDatetime = model.CreateDatetime;
            this.UpdateDatetime = model.UpdateDatetime;
            this.RowVersion = model.RowVersion;
            this.DtDeliveryResult = model.DtDeliveryResult.Select(y => new DtDeliveryResult(y)).ToHashSet();
            this.DeliveryFileS = model.DtDeliveryFile == null ?
                null :
                new DtDeliveryFile(model.DtDeliveryFile);
            this.DeliveryGroupStatusS = model.MtDeliveryGroupStatus == null ?
                null :
                new MtDeliveryGroupStatus(model.MtDeliveryGroupStatus);
        }
    
        /// <summary>
        /// このインスタンスの各プロパティに、引数に指定されたDtDeliveryGroupのプロパティの値をコピーする
        /// </summary>
        /// <param name="entity">コピー元のDtDeliveryGroup</param>
        public void CopyFrom(DtDeliveryGroup entity)
        {
            this.Sid = entity.Sid;
            this.DeliveryFileSid = entity.DeliveryFileSid;
            this.DeliveryGroupStatusSid = entity.DeliveryGroupStatusSid;
            this.Name = entity.Name;
            this.StartDatetime = entity.StartDatetime;
            this.DownloadDelayTime = entity.DownloadDelayTime;
            this.CreateDatetime = entity.CreateDatetime;
            this.UpdateDatetime = entity.UpdateDatetime;
            this.RowVersion = entity.RowVersion;
            this.DtDeliveryResult = entity.DtDeliveryResult;
            this.DeliveryFileS = entity.DeliveryFileS;
            this.DeliveryGroupStatusS = entity.DeliveryGroupStatusS;
        }
    
        /// <summary>
        /// このインスタンスを、それと同等なUtility.Models.Entites.DtDeliveryGroup型に変換する。
        /// 各変換メソッド共通部分
        /// </summary>
        /// <returns></returns>
        private Utility.Models.Entites.DtDeliveryGroup ToModelCommonPart()
        {
            Utility.Models.Entites.DtDeliveryGroup model = new Utility.Models.Entites.DtDeliveryGroup();
            model.Sid = this.Sid;
            model.DeliveryFileSid = this.DeliveryFileSid;
            model.DeliveryGroupStatusSid = this.DeliveryGroupStatusSid;
            model.Name = this.Name;
            model.StartDatetime = this.StartDatetime;
            model.DownloadDelayTime = this.DownloadDelayTime;
            model.CreateDatetime = this.CreateDatetime;
            model.UpdateDatetime = this.UpdateDatetime;
            model.RowVersion = this.RowVersion;
            return model;
        }
    
        /// <summary>
        /// このインスタンスを、それと同等なUtility.Models.Entites.DtDeliveryGroup型に変換する。
        /// </summary>
        /// <returns></returns>
        public Utility.Models.Entites.DtDeliveryGroup ToModel()
        {
            Utility.Models.Entites.DtDeliveryGroup model = ToModelCommonPart();
            model.DtDeliveryResult = this.DtDeliveryResult.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtDeliveryFile = this.DeliveryFileS?.ToParentModel(this.GetType());
            model.MtDeliveryGroupStatus = this.DeliveryGroupStatusS?.ToParentModel(this.GetType());
    
            return model;
        }
    
        /// <summary>
        /// このインスタンスを、それと同等なUtility.Models.Entites.DtDeliveryGroup型に変換する。
        /// 親エンティティとして生成するため、子エンティティの情報はもたない
        /// </summary>
        /// <returns></returns>
        public Utility.Models.Entites.DtDeliveryGroup ToParentModel(Type childType)
        {
            Utility.Models.Entites.DtDeliveryGroup model = ToModelCommonPart();
            // 親子間の参照無限ループを避けるためにタイプチェック
            if (this.DtDeliveryResult.LastOrDefault()?.GetType() != childType)
            {
                model.DtDeliveryResult = this.DtDeliveryResult.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            }
            model.DtDeliveryFile = this.DeliveryFileS?.ToParentModel(this.GetType());
            model.MtDeliveryGroupStatus = this.DeliveryGroupStatusS?.ToParentModel(this.GetType());
    
            return model;
        }
    
        /// <summary>
        /// このインスタンスを、それと同等なUtility.Models.Entites.DtDeliveryGroup型に変換する。
        /// 子エンティティとして生成するため、親エンティティの情報をもたない
        /// </summary>
        /// <returns></returns>
        public Utility.Models.Entites.DtDeliveryGroup ToChildModel(Type parentType)
        {
            Utility.Models.Entites.DtDeliveryGroup model = ToModelCommonPart();
            model.DtDeliveryResult = this.DtDeliveryResult.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            // 親子間の参照無限ループを避けるためにタイプチェック
            if (this.DeliveryFileS?.GetType() != parentType)
            {
                model.DtDeliveryFile = this.DeliveryFileS?.ToParentModel(this.GetType());
            }
            if (this.DeliveryGroupStatusS?.GetType() != parentType)
            {
                model.MtDeliveryGroupStatus = this.DeliveryGroupStatusS?.ToParentModel(this.GetType());
            }
    
            return model;
        }
    }
    
    /// <summary>
    /// DtDeliveryGroupのメタデータクラス
    /// </summary>
    public  class DtDeliveryGroupModelMetaData
    {
        [Key]
        [Required(ErrorMessage = "Sid is required.")]
        public long Sid { get; set; }
    
        [Required(ErrorMessage = "DeliveryFileSid is required.")]
        public long DeliveryFileSid { get; set; }
    
        [Required(ErrorMessage = "DeliveryGroupStatusSid is required.")]
        public long DeliveryGroupStatusSid { get; set; }
    
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name length should be less than 100 symbols.")]
        public string Name { get; set; }
    
        [Required(ErrorMessage = "CreateDatetime is required.")]
        public System.DateTime CreateDatetime { get; set; }
    
        [Required(ErrorMessage = "UpdateDatetime is required.")]
        public System.DateTime UpdateDatetime { get; set; }
    
        [Required(ErrorMessage = "RowVersion is required.")]
        [MaxLength(8, ErrorMessage = "RowVersion length should be less than 8 elements.")]
        public byte[] RowVersion { get; set; }
    
    }
}