using System;
using System.Collections.Generic;

namespace Rms.Server.Core.DBAccessor.Models
{
    public partial class DtSoftVersion
    {
        public long Sid { get; set; }
        public long DeviceSid { get; set; }
        public long EquipmentModelSid { get; set; }
        public string SourceEquipmentUid { get; set; }
        public string Version { get; set; }
        public string MessageId { get; set; }
        public DateTime? CollectDatetime { get; set; }
        public DateTime CreateDatetime { get; set; }

        public DtDevice DeviceS { get; set; }
        public MtEquipmentModel EquipmentModelS { get; set; }
    }
}
