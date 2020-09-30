using System;
using System.Collections.Generic;

namespace Rms.Server.Utility.DBAccessor.Models
{
    public partial class DtBloclogAnalysisResult
    {
        public long Sid { get; set; }
        public string EquipmentUid { get; set; }
        public string BloclogMonth { get; set; }
        public string DetectorName { get; set; }
        public string DetectorId { get; set; }
        public int? GpValue { get; set; }
        public string ImageFileName { get; set; }
        public short? FileNameNo { get; set; }
        public string ShadingResult { get; set; }
        public double? ShadingResultMcv { get; set; }
        public double? ShadingResultScv { get; set; }
        public double? ShadingResultMcvSv { get; set; }
        public double? ShadingResultScvSv1 { get; set; }
        public double? ShadingResultScvSv2 { get; set; }
        public bool? ImageType { get; set; }
        public int? ImageSize { get; set; }
        public bool? IsBillTarget { get; set; }
        public string LogFileName { get; set; }
        public DateTime CreateDatetime { get; set; }
    }
}
