using System;
using System.Collections.Generic;

namespace Rms.Server.Core.DBAccessor.Models
{
    public partial class DtPlusServiceBillLog
    {
        public long Sid { get; set; }
        public long DeviceSid { get; set; }
        public string SourceEquipmentUid { get; set; }
        public string TypeName { get; set; }
        public bool? BillFlg { get; set; }
        public string PatientId { get; set; }
        public byte? Sex { get; set; }
        public byte? Age { get; set; }
        public string StudyInstanceUid { get; set; }
        public string SopInstanceUid { get; set; }
        public DateTime? StudyDatetime { get; set; }
        public double? MeasureValue { get; set; }
        public DateTime? MeasureDatetime { get; set; }
        public DateTime? CollectDatetime { get; set; }
        public DateTime CreateDatetime { get; set; }
        public DateTime UpdateDatetime { get; set; }

        public DtDevice DeviceS { get; set; }
    }
}
