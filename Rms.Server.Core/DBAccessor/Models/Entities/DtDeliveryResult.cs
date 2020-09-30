using System;
using System.Collections.Generic;

namespace Rms.Server.Core.DBAccessor.Models
{
    public partial class DtDeliveryResult
    {
        public DtDeliveryResult()
        {
            DtInstallResult = new HashSet<DtInstallResult>();
        }

        public long Sid { get; set; }
        public long DeviceSid { get; set; }
        public long GwDeviceSid { get; set; }
        public long DeliveryGroupSid { get; set; }
        public DateTime CreateDatetime { get; set; }

        public DtDeliveryGroup DeliveryGroupS { get; set; }
        public DtDevice DeviceS { get; set; }
        public DtDevice GwDeviceS { get; set; }
        public ICollection<DtInstallResult> DtInstallResult { get; set; }
    }
}
