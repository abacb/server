//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはテンプレートから生成されました。
//
//     このファイルを手動で変更すると、アプリケーションで予期しない動作が発生する可能性があります。
//     このファイルに対する手動の変更は、コードが再生成されると上書きされます。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Rms.Server.Core.Utility.Models.Entites
{
    using System;
    using System.Collections.Generic;
    
    public partial class MtSoftVersionConvert
    {
        public long Sid { get; set; }
    
        public long EquipmentModelSid { get; set; }
    
        public string DisplayVersion { get; set; }
    
        public string InternalVersion { get; set; }
    
        public string Description { get; set; }
    
        public System.DateTime CreateDatetime { get; set; }
    
        public virtual MtEquipmentModel MtEquipmentModel { get; set; }
    
    }
}