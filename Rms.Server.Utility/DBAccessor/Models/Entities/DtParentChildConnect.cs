using System;
using System.Collections.Generic;

namespace Rms.Server.Utility.DBAccessor.Models
{
    public partial class DtParentChildConnect
    {
        public long Sid { get; set; }
        public long ParentDeviceSid { get; set; }
        public long ChildDeviceSid { get; set; }
        public bool? ParentResult { get; set; }
        public DateTime? ParentConfirmDatetime { get; set; }
        public DateTime? ParentLastConnectDatetime { get; set; }
        public bool? ChildResult { get; set; }
        public DateTime? ChildConfirmDatetime { get; set; }
        public DateTime? ChildLastConnectDatetime { get; set; }
        public DateTime? CollectDatetime { get; set; }
        public DateTime CreateDatetime { get; set; }
        public DateTime UpdateDatetime { get; set; }
    }
}
