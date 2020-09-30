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
    /// DtInstallResultクラス
    /// </summary>
    [ModelMetadataType(typeof(DtInstallResultModelMetaData))]
    public partial class DtInstallResult
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DtInstallResult()
        {
        }
    
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="model">Utility.Models.Entites.DtInstallResultのインスタンス</param>
        public DtInstallResult(Utility.Models.Entites.DtInstallResult model)
        {
            this.Sid = model.Sid;
            this.DeviceSid = model.DeviceSid;
            this.DeliveryResultSid = model.DeliveryResultSid;
            this.InstallResultStatusSid = model.InstallResultStatusSid;
            this.SourceEquipmentUid = model.SourceEquipmentUid;
            this.ReleaseVersion = model.ReleaseVersion;
            this.BeforeVersion = model.BeforeVersion;
            this.AfterVervion = model.AfterVervion;
            this.IsSuccess = model.IsSuccess;
            this.ErrorCode = model.ErrorCode;
            this.ErrorDescription = model.ErrorDescription;
            this.IsAuto = model.IsAuto;
            this.Method = model.Method;
            this.Process = model.Process;
            this.UpdateStratDatetime = model.UpdateStratDatetime;
            this.UpdateEndDatetime = model.UpdateEndDatetime;
            this.ComputerName = model.ComputerName;
            this.IpAddress = model.IpAddress;
            this.ServerClientKind = model.ServerClientKind;
            this.HasRepairReport = model.HasRepairReport;
            this.EventDatetime = model.EventDatetime;
            this.CollectDatetime = model.CollectDatetime;
            this.MessageId = model.MessageId;
            this.CreateDatetime = model.CreateDatetime;
            this.DeliveryResultS = model.DtDeliveryResult == null ?
                null :
                new DtDeliveryResult(model.DtDeliveryResult);
            this.DeviceS = model.DtDevice == null ?
                null :
                new DtDevice(model.DtDevice);
            this.InstallResultStatusS = model.MtInstallResultStatus == null ?
                null :
                new MtInstallResultStatus(model.MtInstallResultStatus);
        }
    
        /// <summary>
        /// このインスタンスの各プロパティに、引数に指定されたDtInstallResultのプロパティの値をコピーする
        /// </summary>
        /// <param name="entity">コピー元のDtInstallResult</param>
        public void CopyFrom(DtInstallResult entity)
        {
            this.Sid = entity.Sid;
            this.DeviceSid = entity.DeviceSid;
            this.DeliveryResultSid = entity.DeliveryResultSid;
            this.InstallResultStatusSid = entity.InstallResultStatusSid;
            this.SourceEquipmentUid = entity.SourceEquipmentUid;
            this.ReleaseVersion = entity.ReleaseVersion;
            this.BeforeVersion = entity.BeforeVersion;
            this.AfterVervion = entity.AfterVervion;
            this.IsSuccess = entity.IsSuccess;
            this.ErrorCode = entity.ErrorCode;
            this.ErrorDescription = entity.ErrorDescription;
            this.IsAuto = entity.IsAuto;
            this.Method = entity.Method;
            this.Process = entity.Process;
            this.UpdateStratDatetime = entity.UpdateStratDatetime;
            this.UpdateEndDatetime = entity.UpdateEndDatetime;
            this.ComputerName = entity.ComputerName;
            this.IpAddress = entity.IpAddress;
            this.ServerClientKind = entity.ServerClientKind;
            this.HasRepairReport = entity.HasRepairReport;
            this.EventDatetime = entity.EventDatetime;
            this.CollectDatetime = entity.CollectDatetime;
            this.MessageId = entity.MessageId;
            this.CreateDatetime = entity.CreateDatetime;
            this.DeliveryResultS = entity.DeliveryResultS;
            this.DeviceS = entity.DeviceS;
            this.InstallResultStatusS = entity.InstallResultStatusS;
        }
    
        /// <summary>
        /// このインスタンスを、それと同等なUtility.Models.Entites.DtInstallResult型に変換する。
        /// 各変換メソッド共通部分
        /// </summary>
        /// <returns></returns>
        private Utility.Models.Entites.DtInstallResult ToModelCommonPart()
        {
            Utility.Models.Entites.DtInstallResult model = new Utility.Models.Entites.DtInstallResult();
            model.Sid = this.Sid;
            model.DeviceSid = this.DeviceSid;
            model.DeliveryResultSid = this.DeliveryResultSid;
            model.InstallResultStatusSid = this.InstallResultStatusSid;
            model.SourceEquipmentUid = this.SourceEquipmentUid;
            model.ReleaseVersion = this.ReleaseVersion;
            model.BeforeVersion = this.BeforeVersion;
            model.AfterVervion = this.AfterVervion;
            model.IsSuccess = this.IsSuccess;
            model.ErrorCode = this.ErrorCode;
            model.ErrorDescription = this.ErrorDescription;
            model.IsAuto = this.IsAuto;
            model.Method = this.Method;
            model.Process = this.Process;
            model.UpdateStratDatetime = this.UpdateStratDatetime;
            model.UpdateEndDatetime = this.UpdateEndDatetime;
            model.ComputerName = this.ComputerName;
            model.IpAddress = this.IpAddress;
            model.ServerClientKind = this.ServerClientKind;
            model.HasRepairReport = this.HasRepairReport;
            model.EventDatetime = this.EventDatetime;
            model.CollectDatetime = this.CollectDatetime;
            model.MessageId = this.MessageId;
            model.CreateDatetime = this.CreateDatetime;
            return model;
        }
    
        /// <summary>
        /// このインスタンスを、それと同等なUtility.Models.Entites.DtInstallResult型に変換する。
        /// </summary>
        /// <returns></returns>
        public Utility.Models.Entites.DtInstallResult ToModel()
        {
            Utility.Models.Entites.DtInstallResult model = ToModelCommonPart();
            model.DtDeliveryResult = this.DeliveryResultS?.ToParentModel(this.GetType());
            model.DtDevice = this.DeviceS?.ToParentModel(this.GetType());
            model.MtInstallResultStatus = this.InstallResultStatusS?.ToParentModel(this.GetType());
    
            return model;
        }
    
        /// <summary>
        /// このインスタンスを、それと同等なUtility.Models.Entites.DtInstallResult型に変換する。
        /// 親エンティティとして生成するため、子エンティティの情報はもたない
        /// </summary>
        /// <returns></returns>
        public Utility.Models.Entites.DtInstallResult ToParentModel(Type childType)
        {
            Utility.Models.Entites.DtInstallResult model = ToModelCommonPart();
            model.DtDeliveryResult = this.DeliveryResultS?.ToParentModel(this.GetType());
            model.DtDevice = this.DeviceS?.ToParentModel(this.GetType());
            model.MtInstallResultStatus = this.InstallResultStatusS?.ToParentModel(this.GetType());
    
            return model;
        }
    
        /// <summary>
        /// このインスタンスを、それと同等なUtility.Models.Entites.DtInstallResult型に変換する。
        /// 子エンティティとして生成するため、親エンティティの情報をもたない
        /// </summary>
        /// <returns></returns>
        public Utility.Models.Entites.DtInstallResult ToChildModel(Type parentType)
        {
            Utility.Models.Entites.DtInstallResult model = ToModelCommonPart();
            // 親子間の参照無限ループを避けるためにタイプチェック
            if (this.DeliveryResultS?.GetType() != parentType)
            {
                model.DtDeliveryResult = this.DeliveryResultS?.ToParentModel(this.GetType());
            }
            if (this.DeviceS?.GetType() != parentType)
            {
                model.DtDevice = this.DeviceS?.ToParentModel(this.GetType());
            }
            if (this.InstallResultStatusS?.GetType() != parentType)
            {
                model.MtInstallResultStatus = this.InstallResultStatusS?.ToParentModel(this.GetType());
            }
    
            return model;
        }
    }
    
    /// <summary>
    /// DtInstallResultのメタデータクラス
    /// </summary>
    public  class DtInstallResultModelMetaData
    {
        [Key]
        [Required(ErrorMessage = "Sid is required.")]
        public long Sid { get; set; }
    
        [Required(ErrorMessage = "DeviceSid is required.")]
        public long DeviceSid { get; set; }
    
        [Required(ErrorMessage = "InstallResultStatusSid is required.")]
        public long InstallResultStatusSid { get; set; }
    
        [StringLength(30, ErrorMessage = "SourceEquipmentUid length should be less than 30 symbols.")]
        [RegularExpression(Utility.Const.AsciiCodeCharactersReg, ErrorMessage = "SourceEquipmentUid is only allowed for ASCII code characters.")]
        public string SourceEquipmentUid { get; set; }
    
        [StringLength(30, ErrorMessage = "ReleaseVersion length should be less than 30 symbols.")]
        [RegularExpression(Utility.Const.AsciiCodeCharactersReg, ErrorMessage = "ReleaseVersion is only allowed for ASCII code characters.")]
        public string ReleaseVersion { get; set; }
    
        [StringLength(30, ErrorMessage = "BeforeVersion length should be less than 30 symbols.")]
        [RegularExpression(Utility.Const.AsciiCodeCharactersReg, ErrorMessage = "BeforeVersion is only allowed for ASCII code characters.")]
        public string BeforeVersion { get; set; }
    
        [StringLength(30, ErrorMessage = "AfterVervion length should be less than 30 symbols.")]
        [RegularExpression(Utility.Const.AsciiCodeCharactersReg, ErrorMessage = "AfterVervion is only allowed for ASCII code characters.")]
        public string AfterVervion { get; set; }
    
        [StringLength(20, ErrorMessage = "ErrorCode length should be less than 20 symbols.")]
        [RegularExpression(Utility.Const.AsciiCodeCharactersReg, ErrorMessage = "ErrorCode is only allowed for ASCII code characters.")]
        public string ErrorCode { get; set; }
    
        [StringLength(20, ErrorMessage = "Method length should be less than 20 symbols.")]
        [RegularExpression(Utility.Const.AsciiCodeCharactersReg, ErrorMessage = "Method is only allowed for ASCII code characters.")]
        public string Method { get; set; }
    
        [StringLength(12, ErrorMessage = "Process length should be less than 12 symbols.")]
        [RegularExpression(Utility.Const.AsciiCodeCharactersReg, ErrorMessage = "Process is only allowed for ASCII code characters.")]
        public string Process { get; set; }
    
        [StringLength(30, ErrorMessage = "ComputerName length should be less than 30 symbols.")]
        [RegularExpression(Utility.Const.AsciiCodeCharactersReg, ErrorMessage = "ComputerName is only allowed for ASCII code characters.")]
        public string ComputerName { get; set; }
    
        [StringLength(200, ErrorMessage = "IpAddress length should be less than 200 symbols.")]
        [RegularExpression(Utility.Const.AsciiCodeCharactersReg, ErrorMessage = "IpAddress is only allowed for ASCII code characters.")]
        public string IpAddress { get; set; }
    
        [StringLength(20, ErrorMessage = "ServerClientKind length should be less than 20 symbols.")]
        [RegularExpression(Utility.Const.AsciiCodeCharactersReg, ErrorMessage = "ServerClientKind is only allowed for ASCII code characters.")]
        public string ServerClientKind { get; set; }
    
        [StringLength(64, ErrorMessage = "MessageId length should be less than 64 symbols.")]
        [RegularExpression(Utility.Const.AsciiCodeCharactersReg, ErrorMessage = "MessageId is only allowed for ASCII code characters.")]
        public string MessageId { get; set; }
    
        [Required(ErrorMessage = "CreateDatetime is required.")]
        public System.DateTime CreateDatetime { get; set; }
    
    }
}
