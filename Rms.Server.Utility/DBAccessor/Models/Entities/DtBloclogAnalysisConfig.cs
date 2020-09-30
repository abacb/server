using System;
using System.Collections.Generic;

namespace Rms.Server.Utility.DBAccessor.Models
{
    public partial class DtBloclogAnalysisConfig
    {
        public long Sid { get; set; }
        public bool IsNormalized { get; set; }
        public int TopUnevennessSkipValue { get; set; }
        public int BottomUnevennessSkipValue { get; set; }
        public double AlsStandardValue { get; set; }
        public double McvStandardValue { get; set; }
        public double ScvStandardValue1 { get; set; }
        public double ScvStandardValue2 { get; set; }
        public DateTime CreateDatetime { get; set; }
        public DateTime UpdateDatetime { get; set; }
    }
}
