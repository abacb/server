using System;
using System.Collections.Generic;

namespace Rms.Server.Core.DBAccessor.Models
{
    public partial class DtDeviceFileAttribute
    {
        public long Sid { get; set; }
        public long DeviceFileSid { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public DateTime CreateDatetime { get; set; }
        public DateTime UpdateDatetime { get; set; }

        public DtDeviceFile DeviceFileS { get; set; }
    }
}
