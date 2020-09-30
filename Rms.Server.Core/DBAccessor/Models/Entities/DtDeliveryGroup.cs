using System;
using System.Collections.Generic;

namespace Rms.Server.Core.DBAccessor.Models
{
    public partial class DtDeliveryGroup
    {
        public DtDeliveryGroup()
        {
            DtDeliveryResult = new HashSet<DtDeliveryResult>();
        }

        public long Sid { get; set; }
        public long DeliveryFileSid { get; set; }
        public long DeliveryGroupStatusSid { get; set; }
        public string Name { get; set; }
        public DateTime? StartDatetime { get; set; }
        public short? DownloadDelayTime { get; set; }
        public DateTime CreateDatetime { get; set; }
        public DateTime UpdateDatetime { get; set; }
        public byte[] RowVersion { get; set; }

        public DtDeliveryFile DeliveryFileS { get; set; }
        public MtDeliveryGroupStatus DeliveryGroupStatusS { get; set; }
        public ICollection<DtDeliveryResult> DtDeliveryResult { get; set; }
    }
}
