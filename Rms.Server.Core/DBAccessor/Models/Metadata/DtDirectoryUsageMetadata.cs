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
    /// DtDirectoryUsageクラス
    /// </summary>
    [ModelMetadataType(typeof(DtDirectoryUsageModelMetaData))]
    public partial class DtDirectoryUsage
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DtDirectoryUsage()
        {
        }
    
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="model">Utility.Models.Entites.DtDirectoryUsageのインスタンス</param>
        public DtDirectoryUsage(Utility.Models.Entites.DtDirectoryUsage model)
        {
            this.Sid = model.Sid;
            this.DeviceSid = model.DeviceSid;
            this.SourceEquipmentUid = model.SourceEquipmentUid;
            this.DetailInfo = model.DetailInfo;
            this.CollectDatetime = model.CollectDatetime;
            this.MessageId = model.MessageId;
            this.CreateDatetime = model.CreateDatetime;
            this.DeviceS = model.DtDevice == null ?
                null :
                new DtDevice(model.DtDevice);
        }
    
        /// <summary>
        /// このインスタンスの各プロパティに、引数に指定されたDtDirectoryUsageのプロパティの値をコピーする
        /// </summary>
        /// <param name="entity">コピー元のDtDirectoryUsage</param>
        public void CopyFrom(DtDirectoryUsage entity)
        {
            this.Sid = entity.Sid;
            this.DeviceSid = entity.DeviceSid;
            this.SourceEquipmentUid = entity.SourceEquipmentUid;
            this.DetailInfo = entity.DetailInfo;
            this.CollectDatetime = entity.CollectDatetime;
            this.MessageId = entity.MessageId;
            this.CreateDatetime = entity.CreateDatetime;
            this.DeviceS = entity.DeviceS;
        }
    
        /// <summary>
        /// このインスタンスを、それと同等なUtility.Models.Entites.DtDirectoryUsage型に変換する。
        /// 各変換メソッド共通部分
        /// </summary>
        /// <returns></returns>
        private Utility.Models.Entites.DtDirectoryUsage ToModelCommonPart()
        {
            Utility.Models.Entites.DtDirectoryUsage model = new Utility.Models.Entites.DtDirectoryUsage();
            model.Sid = this.Sid;
            model.DeviceSid = this.DeviceSid;
            model.SourceEquipmentUid = this.SourceEquipmentUid;
            model.DetailInfo = this.DetailInfo;
            model.CollectDatetime = this.CollectDatetime;
            model.MessageId = this.MessageId;
            model.CreateDatetime = this.CreateDatetime;
            return model;
        }
    
        /// <summary>
        /// このインスタンスを、それと同等なUtility.Models.Entites.DtDirectoryUsage型に変換する。
        /// </summary>
        /// <returns></returns>
        public Utility.Models.Entites.DtDirectoryUsage ToModel()
        {
            Utility.Models.Entites.DtDirectoryUsage model = ToModelCommonPart();
            model.DtDevice = this.DeviceS?.ToParentModel(this.GetType());
    
            return model;
        }
    
        /// <summary>
        /// このインスタンスを、それと同等なUtility.Models.Entites.DtDirectoryUsage型に変換する。
        /// 親エンティティとして生成するため、子エンティティの情報はもたない
        /// </summary>
        /// <returns></returns>
        public Utility.Models.Entites.DtDirectoryUsage ToParentModel(Type childType)
        {
            Utility.Models.Entites.DtDirectoryUsage model = ToModelCommonPart();
            model.DtDevice = this.DeviceS?.ToParentModel(this.GetType());
    
            return model;
        }
    
        /// <summary>
        /// このインスタンスを、それと同等なUtility.Models.Entites.DtDirectoryUsage型に変換する。
        /// 子エンティティとして生成するため、親エンティティの情報をもたない
        /// </summary>
        /// <returns></returns>
        public Utility.Models.Entites.DtDirectoryUsage ToChildModel(Type parentType)
        {
            Utility.Models.Entites.DtDirectoryUsage model = ToModelCommonPart();
            // 親子間の参照無限ループを避けるためにタイプチェック
            if (this.DeviceS?.GetType() != parentType)
            {
                model.DtDevice = this.DeviceS?.ToParentModel(this.GetType());
            }
    
            return model;
        }
    }
    
    /// <summary>
    /// DtDirectoryUsageのメタデータクラス
    /// </summary>
    public  class DtDirectoryUsageModelMetaData
    {
        [Key]
        [Required(ErrorMessage = "Sid is required.")]
        public long Sid { get; set; }
    
        [Required(ErrorMessage = "DeviceSid is required.")]
        public long DeviceSid { get; set; }
    
        [StringLength(30, ErrorMessage = "SourceEquipmentUid length should be less than 30 symbols.")]
        [RegularExpression(Utility.Const.AsciiCodeCharactersReg, ErrorMessage = "SourceEquipmentUid is only allowed for ASCII code characters.")]
        public string SourceEquipmentUid { get; set; }
    
        [StringLength(64, ErrorMessage = "MessageId length should be less than 64 symbols.")]
        [RegularExpression(Utility.Const.AsciiCodeCharactersReg, ErrorMessage = "MessageId is only allowed for ASCII code characters.")]
        public string MessageId { get; set; }
    
        [Required(ErrorMessage = "CreateDatetime is required.")]
        public System.DateTime CreateDatetime { get; set; }
    
    }
}
