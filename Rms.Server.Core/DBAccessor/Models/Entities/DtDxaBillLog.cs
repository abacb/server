using System;
using System.Collections.Generic;

namespace Rms.Server.Core.DBAccessor.Models
{
    public partial class DtDxaBillLog
    {
        public long Sid { get; set; }
        public long DeviceSid { get; set; }
        public string SoueceEquipmentUid { get; set; }
        public string StudyInstanceUid { get; set; }
        public string PatientId { get; set; }
        public byte? TypeName { get; set; }
        public DateTime? StudyDatetime { get; set; }
        public DateTime? MeasureDatetime { get; set; }
        public bool? OptionDxa { get; set; }
        public bool? ServiceMode { get; set; }
        public DateTime? CollectDatetime { get; set; }
        public DateTime CreateDatetime { get; set; }
        public DateTime UpdateDatetime { get; set; }

        public DtDevice DeviceS { get; set; }
    }
}
