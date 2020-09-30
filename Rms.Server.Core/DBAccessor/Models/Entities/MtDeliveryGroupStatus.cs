using System;
using System.Collections.Generic;

namespace Rms.Server.Core.DBAccessor.Models
{
    public partial class MtDeliveryGroupStatus
    {
        public MtDeliveryGroupStatus()
        {
            DtDeliveryGroup = new HashSet<DtDeliveryGroup>();
        }

        public long Sid { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime CreateDatetime { get; set; }

        public ICollection<DtDeliveryGroup> DtDeliveryGroup { get; set; }
    }
}
