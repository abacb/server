using System;
using System.Collections.Generic;

namespace Rms.Server.Core.DBAccessor.Models
{
    public partial class DtDeliveryModel
    {
        public long Sid { get; set; }
        public long DeliveryFileSid { get; set; }
        public long EquipmentModelSid { get; set; }
        public DateTime CreateDatetime { get; set; }

        public DtDeliveryFile DeliveryFileS { get; set; }
        public MtEquipmentModel EquipmentModelS { get; set; }
    }
}
