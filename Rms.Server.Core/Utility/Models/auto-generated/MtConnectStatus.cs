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
    
    public partial class MtConnectStatus
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MtConnectStatus()
        {
            this.DtDevice = new HashSet<DtDevice>();
        }
        public long Sid { get; set; }
    
        public string Code { get; set; }
    
        public string Description { get; set; }
    
        public System.DateTime CreateDatetime { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DtDevice> DtDevice { get; set; }
    
    }
}
