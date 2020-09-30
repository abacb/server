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
    /// DtPlusServiceBillLogクラス
    /// </summary>
    [ModelMetadataType(typeof(DtPlusServiceBillLogModelMetaData))]
    public partial class DtPlusServiceBillLog
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DtPlusServiceBillLog()
        {
        }
    
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="model">Utility.Models.Entites.DtPlusServiceBillLogのインスタンス</param>
        public DtPlusServiceBillLog(Utility.Models.Entites.DtPlusServiceBillLog model)
        {
            this.Sid = model.Sid;
            this.DeviceSid = model.DeviceSid;
            this.SourceEquipmentUid = model.SourceEquipmentUid;
            this.TypeName = model.TypeName;
            this.BillFlg = model.BillFlg;
            this.PatientId = model.PatientId;
            this.Sex = model.Sex;
            this.Age = model.Age;
            this.StudyInstanceUid = model.StudyInstanceUid;
            this.SopInstanceUid = model.SopInstanceUid;
            this.StudyDatetime = model.StudyDatetime;
            this.MeasureValue = model.MeasureValue;
            this.MeasureDatetime = model.MeasureDatetime;
            this.CollectDatetime = model.CollectDatetime;
            this.CreateDatetime = model.CreateDatetime;
            this.UpdateDatetime = model.UpdateDatetime;
            this.DeviceS = model.DtDevice == null ?
                null :
                new DtDevice(model.DtDevice);
        }
    
        /// <summary>
        /// このインスタンスの各プロパティに、引数に指定されたDtPlusServiceBillLogのプロパティの値をコピーする
        /// </summary>
        /// <param name="entity">コピー元のDtPlusServiceBillLog</param>
        public void CopyFrom(DtPlusServiceBillLog entity)
        {
            this.Sid = entity.Sid;
            this.DeviceSid = entity.DeviceSid;
            this.SourceEquipmentUid = entity.SourceEquipmentUid;
            this.TypeName = entity.TypeName;
            this.BillFlg = entity.BillFlg;
            this.PatientId = entity.PatientId;
            this.Sex = entity.Sex;
            this.Age = entity.Age;
            this.StudyInstanceUid = entity.StudyInstanceUid;
            this.SopInstanceUid = entity.SopInstanceUid;
            this.StudyDatetime = entity.StudyDatetime;
            this.MeasureValue = entity.MeasureValue;
            this.MeasureDatetime = entity.MeasureDatetime;
            this.CollectDatetime = entity.CollectDatetime;
            this.CreateDatetime = entity.CreateDatetime;
            this.UpdateDatetime = entity.UpdateDatetime;
            this.DeviceS = entity.DeviceS;
        }
    
        /// <summary>
        /// このインスタンスを、それと同等なUtility.Models.Entites.DtPlusServiceBillLog型に変換する。
        /// 各変換メソッド共通部分
        /// </summary>
        /// <returns></returns>
        private Utility.Models.Entites.DtPlusServiceBillLog ToModelCommonPart()
        {
            Utility.Models.Entites.DtPlusServiceBillLog model = new Utility.Models.Entites.DtPlusServiceBillLog();
            model.Sid = this.Sid;
            model.DeviceSid = this.DeviceSid;
            model.SourceEquipmentUid = this.SourceEquipmentUid;
            model.TypeName = this.TypeName;
            model.BillFlg = this.BillFlg;
            model.PatientId = this.PatientId;
            model.Sex = this.Sex;
            model.Age = this.Age;
            model.StudyInstanceUid = this.StudyInstanceUid;
            model.SopInstanceUid = this.SopInstanceUid;
            model.StudyDatetime = this.StudyDatetime;
            model.MeasureValue = this.MeasureValue;
            model.MeasureDatetime = this.MeasureDatetime;
            model.CollectDatetime = this.CollectDatetime;
            model.CreateDatetime = this.CreateDatetime;
            model.UpdateDatetime = this.UpdateDatetime;
            return model;
        }
    
        /// <summary>
        /// このインスタンスを、それと同等なUtility.Models.Entites.DtPlusServiceBillLog型に変換する。
        /// </summary>
        /// <returns></returns>
        public Utility.Models.Entites.DtPlusServiceBillLog ToModel()
        {
            Utility.Models.Entites.DtPlusServiceBillLog model = ToModelCommonPart();
            model.DtDevice = this.DeviceS?.ToParentModel(this.GetType());
    
            return model;
        }
    
        /// <summary>
        /// このインスタンスを、それと同等なUtility.Models.Entites.DtPlusServiceBillLog型に変換する。
        /// 親エンティティとして生成するため、子エンティティの情報はもたない
        /// </summary>
        /// <returns></returns>
        public Utility.Models.Entites.DtPlusServiceBillLog ToParentModel(Type childType)
        {
            Utility.Models.Entites.DtPlusServiceBillLog model = ToModelCommonPart();
            model.DtDevice = this.DeviceS?.ToParentModel(this.GetType());
    
            return model;
        }
    
        /// <summary>
        /// このインスタンスを、それと同等なUtility.Models.Entites.DtPlusServiceBillLog型に変換する。
        /// 子エンティティとして生成するため、親エンティティの情報をもたない
        /// </summary>
        /// <returns></returns>
        public Utility.Models.Entites.DtPlusServiceBillLog ToChildModel(Type parentType)
        {
            Utility.Models.Entites.DtPlusServiceBillLog model = ToModelCommonPart();
            // 親子間の参照無限ループを避けるためにタイプチェック
            if (this.DeviceS?.GetType() != parentType)
            {
                model.DtDevice = this.DeviceS?.ToParentModel(this.GetType());
            }
    
            return model;
        }
    }
    
    /// <summary>
    /// DtPlusServiceBillLogのメタデータクラス
    /// </summary>
    public  class DtPlusServiceBillLogModelMetaData
    {
        [Key]
        [Required(ErrorMessage = "Sid is required.")]
        public long Sid { get; set; }
    
        [Required(ErrorMessage = "DeviceSid is required.")]
        public long DeviceSid { get; set; }
    
        [StringLength(30, ErrorMessage = "SourceEquipmentUid length should be less than 30 symbols.")]
        [RegularExpression(Utility.Const.AsciiCodeCharactersReg, ErrorMessage = "SourceEquipmentUid is only allowed for ASCII code characters.")]
        public string SourceEquipmentUid { get; set; }
    
        [StringLength(128, ErrorMessage = "TypeName length should be less than 128 symbols.")]
        [RegularExpression(Utility.Const.AsciiCodeCharactersReg, ErrorMessage = "TypeName is only allowed for ASCII code characters.")]
        public string TypeName { get; set; }
    
        [StringLength(128, ErrorMessage = "PatientId length should be less than 128 symbols.")]
        [RegularExpression(Utility.Const.AsciiCodeCharactersReg, ErrorMessage = "PatientId is only allowed for ASCII code characters.")]
        public string PatientId { get; set; }
    
        [StringLength(128, ErrorMessage = "StudyInstanceUid length should be less than 128 symbols.")]
        [RegularExpression(Utility.Const.AsciiCodeCharactersReg, ErrorMessage = "StudyInstanceUid is only allowed for ASCII code characters.")]
        public string StudyInstanceUid { get; set; }
    
        [StringLength(128, ErrorMessage = "SopInstanceUid length should be less than 128 symbols.")]
        [RegularExpression(Utility.Const.AsciiCodeCharactersReg, ErrorMessage = "SopInstanceUid is only allowed for ASCII code characters.")]
        public string SopInstanceUid { get; set; }
    
        [Required(ErrorMessage = "CreateDatetime is required.")]
        public System.DateTime CreateDatetime { get; set; }
    
        [Required(ErrorMessage = "UpdateDatetime is required.")]
        public System.DateTime UpdateDatetime { get; set; }
    
    }
}
