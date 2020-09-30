using System;
using System.Collections.Generic;

namespace Rms.Server.Utility.DBAccessor.Models
{
    public partial class DtAlarmSmartPremonitor
    {
        public long Sid { get; set; }
        public string SmartId { get; set; }
        public string ChangeCountThreshold { get; set; }
        public string AnalysisResultErrorCode { get; set; }
        public byte AlarmLevel { get; set; }
        public string AlarmTitle { get; set; }
        public string AlarmDescription { get; set; }
        public DateTime CreateDatetime { get; set; }
        public DateTime UpdateDatetime { get; set; }
    }
}
