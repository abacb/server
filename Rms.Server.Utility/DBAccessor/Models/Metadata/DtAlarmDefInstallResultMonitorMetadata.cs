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
    /// DtAlarmDefInstallResultMonitorクラス
    /// </summary>
    [ModelMetadataType(typeof(DtAlarmDefInstallResultMonitorModelMetaData))]
    public partial class DtAlarmDefInstallResultMonitor
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DtAlarmDefInstallResultMonitor()
        {
        }
    
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="model">Rms.Server.Utility.Utility.Models.DtAlarmDefInstallResultMonitorのインスタンス</param>
        public DtAlarmDefInstallResultMonitor(Rms.Server.Utility.Utility.Models.DtAlarmDefInstallResultMonitor model)
        {
            this.Sid = model.Sid;
            this.TypeCode = model.TypeCode;
            this.ErrorCode = model.ErrorCode;
            this.IsAuto = model.IsAuto;
            this.Process = model.Process;
            this.IsSuccess = model.IsSuccess;
            this.AlarmLevel = model.AlarmLevel;
            this.AlarmTitle = model.AlarmTitle;
            this.AlarmDescription = model.AlarmDescription;
            this.CreateDatetime = model.CreateDatetime;
            this.UpdateDatetime = model.UpdateDatetime;
        }
    
        /// <summary>
        /// このインスタンスの各プロパティに、引数に指定されたDtAlarmDefInstallResultMonitorのプロパティの値をコピーする
        /// </summary>
        /// <param name="entity">コピー元のDtAlarmDefInstallResultMonitor</param>
        public void CopyFrom(DtAlarmDefInstallResultMonitor entity)
        {
            this.Sid = entity.Sid;
            this.TypeCode = entity.TypeCode;
            this.ErrorCode = entity.ErrorCode;
            this.IsAuto = entity.IsAuto;
            this.Process = entity.Process;
            this.IsSuccess = entity.IsSuccess;
            this.AlarmLevel = entity.AlarmLevel;
            this.AlarmTitle = entity.AlarmTitle;
            this.AlarmDescription = entity.AlarmDescription;
            this.CreateDatetime = entity.CreateDatetime;
            this.UpdateDatetime = entity.UpdateDatetime;
        }
    
        /// <summary>
        /// このインスタンスを、それと同等なRms.Server.Utility.Utility.Models.DtAlarmDefInstallResultMonitor型に変換する。
        /// </summary>
        /// <returns></returns>
        public Rms.Server.Utility.Utility.Models.DtAlarmDefInstallResultMonitor ToModel()
        {
            Rms.Server.Utility.Utility.Models.DtAlarmDefInstallResultMonitor model = new Rms.Server.Utility.Utility.Models.DtAlarmDefInstallResultMonitor();
            model.Sid = this.Sid;
            model.TypeCode = this.TypeCode;
            model.ErrorCode = this.ErrorCode;
            model.IsAuto = this.IsAuto;
            model.Process = this.Process;
            model.IsSuccess = this.IsSuccess;
            model.AlarmLevel = this.AlarmLevel;
            model.AlarmTitle = this.AlarmTitle;
            model.AlarmDescription = this.AlarmDescription;
            model.CreateDatetime = this.CreateDatetime;
            model.UpdateDatetime = this.UpdateDatetime;
    
            return model;
        }
    }
    
    /// <summary>
    /// DtAlarmDefInstallResultMonitorのメタデータクラス
    /// </summary>
    public  class DtAlarmDefInstallResultMonitorModelMetaData
    {
        [Key]
        [Required(ErrorMessage = "Sid is required.")]
        public long Sid { get; set; }
    
        [Required(ErrorMessage = "TypeCode is required.")]
        [StringLength(30, ErrorMessage = "TypeCode length should be less than 30 symbols.")]
        public string TypeCode { get; set; }
    
        [StringLength(20, ErrorMessage = "ErrorCode length should be less than 20 symbols.")]
        public string ErrorCode { get; set; }
    
        [Required(ErrorMessage = "IsAuto is required.")]
        public bool IsAuto { get; set; }
    
        [StringLength(12, ErrorMessage = "Process length should be less than 12 symbols.")]
        public string Process { get; set; }
    
        [Required(ErrorMessage = "AlarmLevel is required.")]
        public byte AlarmLevel { get; set; }
    
        [StringLength(200, ErrorMessage = "AlarmTitle length should be less than 200 symbols.")]
        public string AlarmTitle { get; set; }
    
        [StringLength(1024, ErrorMessage = "AlarmDescription length should be less than 1024 symbols.")]
        public string AlarmDescription { get; set; }
    
        [Required(ErrorMessage = "CreateDatetime is required.")]
        public System.DateTime CreateDatetime { get; set; }
    
        [Required(ErrorMessage = "UpdateDatetime is required.")]
        public System.DateTime UpdateDatetime { get; set; }
    
    }
}