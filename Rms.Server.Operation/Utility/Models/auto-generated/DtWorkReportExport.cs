//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはテンプレートから生成されました。
//
//     このファイルを手動で変更すると、アプリケーションで予期しない動作が発生する可能性があります。
//     このファイルに対する手動の変更は、コードが再生成されると上書きされます。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Rms.Server.Operation.Utility.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DtWorkReportExport
    {
        public long Sid { get; set; }
    
        public long StartWorkId { get; set; }
    
        public Nullable<System.DateTime> StartWorkDatetime { get; set; }
    
        public long EndWorkId { get; set; }
    
        public Nullable<System.DateTime> EndWorkDatetime { get; set; }
    
        public string FileName { get; set; }
    
        public System.DateTime CreateDatetime { get; set; }
    
    }
}
