//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはテンプレートから生成されました。
//
//     このファイルを手動で変更すると、アプリケーションで予期しない動作が発生する可能性があります。
//     このファイルに対する手動の変更は、コードが再生成されると上書きされます。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Rms.Server.Utility.DBAccessor.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    
    /// <summary>
    /// DtInventoryクラス
    /// </summary>
    [ModelMetadataType(typeof(DtInventoryModelMetaData))]
    public partial class DtInventory
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DtInventory()
        {
        }
    
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="model">Rms.Server.Utility.Utility.Models.DtInventoryのインスタンス</param>
        public DtInventory(Rms.Server.Utility.Utility.Models.DtInventory model)
        {
            this.Sid = model.Sid;
            this.DeviceSid = model.DeviceSid;
            this.SourceEquipmentUid = model.SourceEquipmentUid;
            this.DetailInfo = model.DetailInfo;
            this.CollectDatetime = model.CollectDatetime;
            this.MessageId = model.MessageId;
            this.IsLatest = model.IsLatest;
            this.CreateDatetime = model.CreateDatetime;
        }
    
        /// <summary>
        /// このインスタンスの各プロパティに、引数に指定されたDtInventoryのプロパティの値をコピーする
        /// </summary>
        /// <param name="entity">コピー元のDtInventory</param>
        public void CopyFrom(DtInventory entity)
        {
            this.Sid = entity.Sid;
            this.DeviceSid = entity.DeviceSid;
            this.SourceEquipmentUid = entity.SourceEquipmentUid;
            this.DetailInfo = entity.DetailInfo;
            this.CollectDatetime = entity.CollectDatetime;
            this.MessageId = entity.MessageId;
            this.IsLatest = entity.IsLatest;
            this.CreateDatetime = entity.CreateDatetime;
        }
    
        /// <summary>
        /// このインスタンスを、それと同等なRms.Server.Utility.Utility.Models.DtInventory型に変換する。
        /// </summary>
        /// <returns></returns>
        public Rms.Server.Utility.Utility.Models.DtInventory ToModel()
        {
            Rms.Server.Utility.Utility.Models.DtInventory model = new Rms.Server.Utility.Utility.Models.DtInventory();
            model.Sid = this.Sid;
            model.DeviceSid = this.DeviceSid;
            model.SourceEquipmentUid = this.SourceEquipmentUid;
            model.DetailInfo = this.DetailInfo;
            model.CollectDatetime = this.CollectDatetime;
            model.MessageId = this.MessageId;
            model.IsLatest = this.IsLatest;
            model.CreateDatetime = this.CreateDatetime;
    
            return model;
        }
    }
    
    /// <summary>
    /// DtInventoryのメタデータクラス
    /// </summary>
    public  class DtInventoryModelMetaData
    {
        [Key]
        [Required(ErrorMessage = "Sid is required.")]
        public long Sid { get; set; }
    
        [Required(ErrorMessage = "DeviceSid is required.")]
        public long DeviceSid { get; set; }
    
        [StringLength(30, ErrorMessage = "SourceEquipmentUid length should be less than 30 symbols.")]
        public string SourceEquipmentUid { get; set; }
    
        [StringLength(64, ErrorMessage = "MessageId length should be less than 64 symbols.")]
        public string MessageId { get; set; }
    
        [Required(ErrorMessage = "IsLatest is required.")]
        public bool IsLatest { get; set; }
    
        [Required(ErrorMessage = "CreateDatetime is required.")]
        public System.DateTime CreateDatetime { get; set; }
    
    }
}