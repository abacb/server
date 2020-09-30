using System;
using System.Collections.Generic;

namespace Rms.Server.Utility.DBAccessor.Models
{
    public partial class DtAlarmDefConnectionMonitor
    {
        public long Sid { get; set; }
        public string TypeCode { get; set; }
        public string ErrorCode { get; set; }
        public string AnalysisResultErrorCode { get; set; }
        public string Target { get; set; }
        public int ValueFrom { get; set; }
        public bool ValueEqualFrom { get; set; }
        public int? ValueTo { get; set; }
        public bool? ValueEqualTo { get; set; }
        public byte AlarmLevel { get; set; }
        public string AlarmTitle { get; set; }
        public string AlarmDescription { get; set; }
        public DateTime CreateDatetime { get; set; }
        public DateTime UpdateDatetime { get; set; }
    }
}
