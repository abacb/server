using System;
using System.Collections.Generic;

namespace Rms.Server.Utility.DBAccessor.Models
{
    public partial class DtAlmilogAnalysisConfig
    {
        public long Sid { get; set; }
        public string DetectorName { get; set; }
        public bool IsNormalized { get; set; }
        public double MinSlopeValue { get; set; }
        public double MiddleSlopeValue { get; set; }
        public double MaxSlopeValue { get; set; }
        public int LowVoltageAreaValue { get; set; }
        public int HighVoltageAreaValue { get; set; }
        public string AreaStandardData { get; set; }
        public DateTime CreateDatetime { get; set; }
        public DateTime UpdateDatetime { get; set; }
    }
}
