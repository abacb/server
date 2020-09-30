using System;
using System.Collections.Generic;

namespace Rms.Server.Core.DBAccessor.Models
{
    public partial class MtSoftVersionConvert
    {
        public long Sid { get; set; }
        public long EquipmentModelSid { get; set; }
        public string DisplayVersion { get; set; }
        public string InternalVersion { get; set; }
        public string Description { get; set; }
        public DateTime CreateDatetime { get; set; }

        public MtEquipmentModel EquipmentModelS { get; set; }
    }
}
