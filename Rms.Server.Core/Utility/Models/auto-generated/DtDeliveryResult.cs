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
    
    public partial class DtDeliveryResult
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DtDeliveryResult()
        {
            this.DtInstallResult = new HashSet<DtInstallResult>();
        }
        public long Sid { get; set; }
    
        public long DeviceSid { get; set; }
    
        public long GwDeviceSid { get; set; }
    
        public long DeliveryGroupSid { get; set; }
    
        public System.DateTime CreateDatetime { get; set; }
    
        public virtual DtDeliveryGroup DtDeliveryGroup { get; set; }
    
        public virtual DtDevice DtDevice { get; set; }
    
        public virtual DtDevice DtDevice1 { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DtInstallResult> DtInstallResult { get; set; }
    
    }
}
