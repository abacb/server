using System;
using System.Collections.Generic;

namespace Rms.Server.Core.DBAccessor.Models
{
    public partial class DtDeviceFile
    {
        public DtDeviceFile()
        {
            DtDeviceFileAttribute = new HashSet<DtDeviceFileAttribute>();
        }

        public long Sid { get; set; }
        public long DeviceSid { get; set; }
        public string SourceEquipmentUid { get; set; }
        public string Container { get; set; }
        public string FilePath { get; set; }
        public DateTime? CollectDatetime { get; set; }
        public DateTime CreateDatetime { get; set; }
        public DateTime UpdateDatetime { get; set; }

        public DtDevice DeviceS { get; set; }
        public ICollection<DtDeviceFileAttribute> DtDeviceFileAttribute { get; set; }
    }
}
