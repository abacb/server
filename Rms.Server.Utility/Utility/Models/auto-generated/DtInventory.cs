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
    
    public partial class DtInventory
    {
        public long Sid { get; set; }
    
        public long DeviceSid { get; set; }
    
        public string SourceEquipmentUid { get; set; }
    
        public string DetailInfo { get; set; }
    
        public Nullable<System.DateTime> CollectDatetime { get; set; }
    
        public string MessageId { get; set; }
    
        public bool IsLatest { get; set; }
    
        public System.DateTime CreateDatetime { get; set; }
    
    }
}
