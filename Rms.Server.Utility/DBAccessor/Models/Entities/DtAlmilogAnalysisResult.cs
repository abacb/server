using System;
using System.Collections.Generic;

namespace Rms.Server.Utility.DBAccessor.Models
{
    public partial class DtAlmilogAnalysisResult
    {
        public long Sid { get; set; }
        public string EquipmentUid { get; set; }
        public string AnalysisResult { get; set; }
        public double? CalculateInclinationValue { get; set; }
        public double? CalculateAreaValue { get; set; }
        public double? MaxInclinationValue { get; set; }
        public double? MinInclinationValue { get; set; }
        public int? StandardAreaValue { get; set; }
        public string AlmilogMonth { get; set; }
        public string DetectorName { get; set; }
        public string DetectorId { get; set; }
        public int? GpValue { get; set; }
        public string ImageFileName { get; set; }
        public short? FileNameNo { get; set; }
        public string ReverseResult { get; set; }
        public double? ReverseResultInclination { get; set; }
        public bool? IsAlarmJudged { get; set; }
        public bool? IsBillTarget { get; set; }
        public string LogFileName { get; set; }
        public DateTime CreateDatetime { get; set; }
    }
}
