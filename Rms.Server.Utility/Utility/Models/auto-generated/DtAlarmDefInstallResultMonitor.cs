//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはテンプレートから生成されました。
//
//     このファイルを手動で変更すると、アプリケーションで予期しない動作が発生する可能性があります。
//     このファイルに対する手動の変更は、コードが再生成されると上書きされます。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Rms.Server.Utility.Utility.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DtAlarmDefInstallResultMonitor
    {
        public long Sid { get; set; }
    
        public string TypeCode { get; set; }
    
        public string ErrorCode { get; set; }
    
        public bool IsAuto { get; set; }
    
        public string Process { get; set; }
    
        public Nullable<bool> IsSuccess { get; set; }
    
        public byte AlarmLevel { get; set; }
    
        public string AlarmTitle { get; set; }
    
        public string AlarmDescription { get; set; }
    
        public System.DateTime CreateDatetime { get; set; }
    
        public System.DateTime UpdateDatetime { get; set; }
    
    }
}
