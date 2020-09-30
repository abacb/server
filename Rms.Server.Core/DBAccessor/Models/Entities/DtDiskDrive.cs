using System;
using System.Collections.Generic;

namespace Rms.Server.Core.DBAccessor.Models
{
    public partial class DtDiskDrive
    {
        public long Sid { get; set; }
        public long DeviceSid { get; set; }
        public string SourceEquipmentUid { get; set; }
        public string Model { get; set; }
        public string MediaType { get; set; }
        public string InterfaceType { get; set; }
        public string SerialNumber { get; set; }
        public string SmartAttributeInfo { get; set; }
        public DateTime? CollectDatetime { get; set; }
        public string MessageId { get; set; }
        public DateTime CreateDatetime { get; set; }

        public DtDevice DeviceS { get; set; }
    }
}
