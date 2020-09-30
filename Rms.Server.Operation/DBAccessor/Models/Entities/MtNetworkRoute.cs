using System;
using System.Collections.Generic;

namespace Rms.Server.Operation.DBAccessor.Models
{
    public partial class MtNetworkRoute
    {
        public MtNetworkRoute()
        {
            DtEquipment = new HashSet<DtEquipment>();
        }

        public long Sid { get; set; }
        public string Code { get; set; }
        public string NetworkName { get; set; }
        public int? NetworkBandwidth { get; set; }
        public DateTime CreateTimedate { get; set; }
        public DateTime UpdateDatetime { get; set; }

        public ICollection<DtEquipment> DtEquipment { get; set; }
    }
}
