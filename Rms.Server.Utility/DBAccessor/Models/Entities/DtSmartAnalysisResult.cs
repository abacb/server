using System;
using System.Collections.Generic;

namespace Rms.Server.Utility.DBAccessor.Models
{
    public partial class DtSmartAnalysisResult
    {
        public long Sid { get; set; }
        public string EquipmentUid { get; set; }
        public string DiskSerialNumber { get; set; }
        public long C5RawData { get; set; }
        public short? C5RawDataChanges { get; set; }
        public short? C5ChangesThreshhold { get; set; }
        public short? C5ChangesThreshholdOverCount { get; set; }
        public DateTime? C5ChangesThreshholdLastDatetime { get; set; }
        public DateTime CreateDatetime { get; set; }
        public DateTime UpdateDatetime { get; set; }
    }
}
