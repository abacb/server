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
    /// DtDeviceクラス
    /// </summary>
    [ModelMetadataType(typeof(DtDeviceModelMetaData))]
    public partial class DtDevice
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="model">Utility.Models.Entites.DtDeviceのインスタンス</param>
        public DtDevice(Utility.Models.Entites.DtDevice model)
        {
            this.Sid = model.Sid;
            this.EquipmentModelSid = model.EquipmentModelSid;
            this.InstallTypeSid = model.InstallTypeSid;
            this.ConnectStatusSid = model.ConnectStatusSid;
            this.EdgeId = model.EdgeId;
            this.EquipmentUid = model.EquipmentUid;
            this.RemoteConnectUid = model.RemoteConnectUid;
            this.RmsSoftVersion = model.RmsSoftVersion;
            this.ConnectStartDatetime = model.ConnectStartDatetime;
            this.ConnectUpdateDatetime = model.ConnectUpdateDatetime;
            this.CreateDatetime = model.CreateDatetime;
            this.UpdateDatetime = model.UpdateDatetime;
            this.DtDeliveryResultDeviceS = model.DtDeliveryResult.Select(y => new DtDeliveryResult(y)).ToHashSet();
            this.DtDeliveryResultGwDeviceS = model.DtDeliveryResult1.Select(y => new DtDeliveryResult(y)).ToHashSet();
            this.DtDeviceFile = model.DtDeviceFile.Select(y => new DtDeviceFile(y)).ToHashSet();
            this.DtDirectoryUsage = model.DtDirectoryUsage.Select(y => new DtDirectoryUsage(y)).ToHashSet();
            this.DtDiskDrive = model.DtDiskDrive.Select(y => new DtDiskDrive(y)).ToHashSet();
            this.DtDrive = model.DtDrive.Select(y => new DtDrive(y)).ToHashSet();
            this.DtDxaBillLog = model.DtDxaBillLog.Select(y => new DtDxaBillLog(y)).ToHashSet();
            this.DtDxaQcLog = model.DtDxaQcLog.Select(y => new DtDxaQcLog(y)).ToHashSet();
            this.DtEquipmentUsage = model.DtEquipmentUsage.Select(y => new DtEquipmentUsage(y)).ToHashSet();
            this.DtInstallResult = model.DtInstallResult.Select(y => new DtInstallResult(y)).ToHashSet();
            this.DtInventory = model.DtInventory.Select(y => new DtInventory(y)).ToHashSet();
            this.DtParentChildConnectChildDeviceS = model.DtParentChildConnect.Select(y => new DtParentChildConnect(y)).ToHashSet();
            this.DtParentChildConnectParentDeviceS = model.DtParentChildConnect1.Select(y => new DtParentChildConnect(y)).ToHashSet();
            this.DtPlusServiceBillLog = model.DtPlusServiceBillLog.Select(y => new DtPlusServiceBillLog(y)).ToHashSet();
            this.DtSoftVersion = model.DtSoftVersion.Select(y => new DtSoftVersion(y)).ToHashSet();
            this.ConnectStatusS = model.MtConnectStatus == null ?
                null :
                new MtConnectStatus(model.MtConnectStatus);
            this.InstallTypeS = model.MtInstallType == null ?
                null :
                new MtInstallType(model.MtInstallType);
            this.EquipmentModelS = model.MtEquipmentModel == null ?
                null :
                new MtEquipmentModel(model.MtEquipmentModel);
        }
    
        /// <summary>
        /// このインスタンスの各プロパティに、引数に指定されたDtDeviceのプロパティの値をコピーする
        /// </summary>
        /// <param name="entity">コピー元のDtDevice</param>
        public void CopyFrom(DtDevice entity)
        {
            this.Sid = entity.Sid;
            this.EquipmentModelSid = entity.EquipmentModelSid;
            this.InstallTypeSid = entity.InstallTypeSid;
            this.ConnectStatusSid = entity.ConnectStatusSid;
            this.EdgeId = entity.EdgeId;
            this.EquipmentUid = entity.EquipmentUid;
            this.RemoteConnectUid = entity.RemoteConnectUid;
            this.RmsSoftVersion = entity.RmsSoftVersion;
            this.ConnectStartDatetime = entity.ConnectStartDatetime;
            this.ConnectUpdateDatetime = entity.ConnectUpdateDatetime;
            this.CreateDatetime = entity.CreateDatetime;
            this.UpdateDatetime = entity.UpdateDatetime;
            this.DtDeliveryResultDeviceS = entity.DtDeliveryResultDeviceS;
            this.DtDeliveryResultGwDeviceS = entity.DtDeliveryResultGwDeviceS;
            this.DtDeviceFile = entity.DtDeviceFile;
            this.DtDirectoryUsage = entity.DtDirectoryUsage;
            this.DtDiskDrive = entity.DtDiskDrive;
            this.DtDrive = entity.DtDrive;
            this.DtDxaBillLog = entity.DtDxaBillLog;
            this.DtDxaQcLog = entity.DtDxaQcLog;
            this.DtEquipmentUsage = entity.DtEquipmentUsage;
            this.DtInstallResult = entity.DtInstallResult;
            this.DtInventory = entity.DtInventory;
            this.DtParentChildConnectChildDeviceS = entity.DtParentChildConnectChildDeviceS;
            this.DtParentChildConnectParentDeviceS = entity.DtParentChildConnectParentDeviceS;
            this.DtPlusServiceBillLog = entity.DtPlusServiceBillLog;
            this.DtSoftVersion = entity.DtSoftVersion;
            this.ConnectStatusS = entity.ConnectStatusS;
            this.InstallTypeS = entity.InstallTypeS;
            this.EquipmentModelS = entity.EquipmentModelS;
        }
    
        /// <summary>
        /// このインスタンスを、それと同等なUtility.Models.Entites.DtDevice型に変換する。
        /// 各変換メソッド共通部分
        /// </summary>
        /// <returns></returns>
        private Utility.Models.Entites.DtDevice ToModelCommonPart()
        {
            Utility.Models.Entites.DtDevice model = new Utility.Models.Entites.DtDevice();
            model.Sid = this.Sid;
            model.EquipmentModelSid = this.EquipmentModelSid;
            model.InstallTypeSid = this.InstallTypeSid;
            model.ConnectStatusSid = this.ConnectStatusSid;
            model.EdgeId = this.EdgeId;
            model.EquipmentUid = this.EquipmentUid;
            model.RemoteConnectUid = this.RemoteConnectUid;
            model.RmsSoftVersion = this.RmsSoftVersion;
            model.ConnectStartDatetime = this.ConnectStartDatetime;
            model.ConnectUpdateDatetime = this.ConnectUpdateDatetime;
            model.CreateDatetime = this.CreateDatetime;
            model.UpdateDatetime = this.UpdateDatetime;
            return model;
        }
    
        /// <summary>
        /// このインスタンスを、それと同等なUtility.Models.Entites.DtDevice型に変換する。
        /// </summary>
        /// <returns></returns>
        public Utility.Models.Entites.DtDevice ToModel()
        {
            Utility.Models.Entites.DtDevice model = ToModelCommonPart();
            model.DtDeliveryResult = this.DtDeliveryResultDeviceS.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtDeliveryResult1 = this.DtDeliveryResultGwDeviceS.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtDeviceFile = this.DtDeviceFile.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtDirectoryUsage = this.DtDirectoryUsage.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtDiskDrive = this.DtDiskDrive.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtDrive = this.DtDrive.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtDxaBillLog = this.DtDxaBillLog.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtDxaQcLog = this.DtDxaQcLog.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtEquipmentUsage = this.DtEquipmentUsage.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtInstallResult = this.DtInstallResult.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtInventory = this.DtInventory.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtParentChildConnect = this.DtParentChildConnectChildDeviceS.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtParentChildConnect1 = this.DtParentChildConnectParentDeviceS.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtPlusServiceBillLog = this.DtPlusServiceBillLog.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtSoftVersion = this.DtSoftVersion.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.MtConnectStatus = this.ConnectStatusS?.ToParentModel(this.GetType());
            model.MtInstallType = this.InstallTypeS?.ToParentModel(this.GetType());
            model.MtEquipmentModel = this.EquipmentModelS?.ToParentModel(this.GetType());
    
            return model;
        }
    
        /// <summary>
        /// このインスタンスを、それと同等なUtility.Models.Entites.DtDevice型に変換する。
        /// 親エンティティとして生成するため、子エンティティの情報はもたない
        /// </summary>
        /// <returns></returns>
        public Utility.Models.Entites.DtDevice ToParentModel(Type childType)
        {
            Utility.Models.Entites.DtDevice model = ToModelCommonPart();
            // 親子間の参照無限ループを避けるためにタイプチェック
            if (this.DtDeliveryResultDeviceS.LastOrDefault()?.GetType() != childType)
            {
                model.DtDeliveryResult = this.DtDeliveryResultDeviceS.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            }
            if (this.DtDeliveryResultGwDeviceS.LastOrDefault()?.GetType() != childType)
            {
                model.DtDeliveryResult1 = this.DtDeliveryResultGwDeviceS.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            }
            if (this.DtDeviceFile.LastOrDefault()?.GetType() != childType)
            {
                model.DtDeviceFile = this.DtDeviceFile.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            }
            if (this.DtDirectoryUsage.LastOrDefault()?.GetType() != childType)
            {
                model.DtDirectoryUsage = this.DtDirectoryUsage.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            }
            if (this.DtDiskDrive.LastOrDefault()?.GetType() != childType)
            {
                model.DtDiskDrive = this.DtDiskDrive.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            }
            if (this.DtDrive.LastOrDefault()?.GetType() != childType)
            {
                model.DtDrive = this.DtDrive.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            }
            if (this.DtDxaBillLog.LastOrDefault()?.GetType() != childType)
            {
                model.DtDxaBillLog = this.DtDxaBillLog.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            }
            if (this.DtDxaQcLog.LastOrDefault()?.GetType() != childType)
            {
                model.DtDxaQcLog = this.DtDxaQcLog.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            }
            if (this.DtEquipmentUsage.LastOrDefault()?.GetType() != childType)
            {
                model.DtEquipmentUsage = this.DtEquipmentUsage.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            }
            if (this.DtInstallResult.LastOrDefault()?.GetType() != childType)
            {
                model.DtInstallResult = this.DtInstallResult.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            }
            if (this.DtInventory.LastOrDefault()?.GetType() != childType)
            {
                model.DtInventory = this.DtInventory.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            }
            if (this.DtParentChildConnectChildDeviceS.LastOrDefault()?.GetType() != childType)
            {
                model.DtParentChildConnect = this.DtParentChildConnectChildDeviceS.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            }
            if (this.DtParentChildConnectParentDeviceS.LastOrDefault()?.GetType() != childType)
            {
                model.DtParentChildConnect1 = this.DtParentChildConnectParentDeviceS.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            }
            if (this.DtPlusServiceBillLog.LastOrDefault()?.GetType() != childType)
            {
                model.DtPlusServiceBillLog = this.DtPlusServiceBillLog.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            }
            if (this.DtSoftVersion.LastOrDefault()?.GetType() != childType)
            {
                model.DtSoftVersion = this.DtSoftVersion.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            }
            model.MtConnectStatus = this.ConnectStatusS?.ToParentModel(this.GetType());
            model.MtInstallType = this.InstallTypeS?.ToParentModel(this.GetType());
            model.MtEquipmentModel = this.EquipmentModelS?.ToParentModel(this.GetType());
    
            return model;
        }
    
        /// <summary>
        /// このインスタンスを、それと同等なUtility.Models.Entites.DtDevice型に変換する。
        /// 子エンティティとして生成するため、親エンティティの情報をもたない
        /// </summary>
        /// <returns></returns>
        public Utility.Models.Entites.DtDevice ToChildModel(Type parentType)
        {
            Utility.Models.Entites.DtDevice model = ToModelCommonPart();
            model.DtDeliveryResult = this.DtDeliveryResultDeviceS.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtDeliveryResult1 = this.DtDeliveryResultGwDeviceS.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtDeviceFile = this.DtDeviceFile.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtDirectoryUsage = this.DtDirectoryUsage.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtDiskDrive = this.DtDiskDrive.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtDrive = this.DtDrive.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtDxaBillLog = this.DtDxaBillLog.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtDxaQcLog = this.DtDxaQcLog.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtEquipmentUsage = this.DtEquipmentUsage.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtInstallResult = this.DtInstallResult.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtInventory = this.DtInventory.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtParentChildConnect = this.DtParentChildConnectChildDeviceS.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtParentChildConnect1 = this.DtParentChildConnectParentDeviceS.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtPlusServiceBillLog = this.DtPlusServiceBillLog.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            model.DtSoftVersion = this.DtSoftVersion.Select(y => y.ToChildModel(this.GetType())).ToHashSet();
            // 親子間の参照無限ループを避けるためにタイプチェック
            if (this.ConnectStatusS?.GetType() != parentType)
            {
                model.MtConnectStatus = this.ConnectStatusS?.ToParentModel(this.GetType());
            }
            if (this.InstallTypeS?.GetType() != parentType)
            {
                model.MtInstallType = this.InstallTypeS?.ToParentModel(this.GetType());
            }
            if (this.EquipmentModelS?.GetType() != parentType)
            {
                model.MtEquipmentModel = this.EquipmentModelS?.ToParentModel(this.GetType());
            }
    
            return model;
        }
    }
    
    /// <summary>
    /// DtDeviceのメタデータクラス
    /// </summary>
    public  class DtDeviceModelMetaData
    {
        [Key]
        [Required(ErrorMessage = "Sid is required.")]
        public long Sid { get; set; }
    
        [Required(ErrorMessage = "InstallTypeSid is required.")]
        public long InstallTypeSid { get; set; }
    
        [Required(ErrorMessage = "ConnectStatusSid is required.")]
        public long ConnectStatusSid { get; set; }
    
        [Required(ErrorMessage = "EdgeId is required.")]
        public System.Guid EdgeId { get; set; }
    
        [StringLength(30, ErrorMessage = "EquipmentUid length should be less than 30 symbols.")]
        [RegularExpression(Utility.Const.AsciiCodeCharactersReg, ErrorMessage = "EquipmentUid is only allowed for ASCII code characters.")]
        public string EquipmentUid { get; set; }
    
        [StringLength(64, ErrorMessage = "RemoteConnectUid length should be less than 64 symbols.")]
        [RegularExpression(Utility.Const.AsciiCodeCharactersReg, ErrorMessage = "RemoteConnectUid is only allowed for ASCII code characters.")]
        public string RemoteConnectUid { get; set; }
    
        [StringLength(30, ErrorMessage = "RmsSoftVersion length should be less than 30 symbols.")]
        [RegularExpression(Utility.Const.AsciiCodeCharactersReg, ErrorMessage = "RmsSoftVersion is only allowed for ASCII code characters.")]
        public string RmsSoftVersion { get; set; }
    
        [Required(ErrorMessage = "CreateDatetime is required.")]
        public System.DateTime CreateDatetime { get; set; }
    
        [Required(ErrorMessage = "UpdateDatetime is required.")]
        public System.DateTime UpdateDatetime { get; set; }
    
    }
}
