using System;
using System.Collections.Generic;

namespace Rms.Server.Utility.DBAccessor.Models
{
    public partial class DtInventory
    {
        public long Sid { get; set; }
        public long DeviceSid { get; set; }
        public string SourceEquipmentUid { get; set; }
        public string DetailInfo { get; set; }
        public DateTime? CollectDatetime { get; set; }
        public string MessageId { get; set; }
        public bool IsLatest { get; set; }
        public DateTime CreateDatetime { get; set; }
    }
}
