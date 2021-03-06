//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはテンプレートから生成されました。
//
//     このファイルを手動で変更すると、アプリケーションで予期しない動作が発生する可能性があります。
//     このファイルに対する手動の変更は、コードが再生成されると上書きされます。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Rms.Server.Utility.Utility.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DtAlmilogAnalysisResult
    {
        public long Sid { get; set; }
    
        public string EquipmentUid { get; set; }
    
        public string AnalysisResult { get; set; }
    
        public Nullable<double> CalculateInclinationValue { get; set; }
    
        public Nullable<double> CalculateAreaValue { get; set; }
    
        public Nullable<double> MaxInclinationValue { get; set; }
    
        public Nullable<double> MinInclinationValue { get; set; }
    
        public Nullable<int> StandardAreaValue { get; set; }
    
        public string AlmilogMonth { get; set; }
    
        public string DetectorName { get; set; }
    
        public string DetectorId { get; set; }
    
        public Nullable<int> GpValue { get; set; }
    
        public string ImageFileName { get; set; }
    
        public Nullable<short> FileNameNo { get; set; }
    
        public string ReverseResult { get; set; }
    
        public Nullable<double> ReverseResultInclination { get; set; }
    
        public Nullable<bool> IsAlarmJudged { get; set; }
    
        public Nullable<bool> IsBillTarget { get; set; }
    
        public string LogFileName { get; set; }
    
        public System.DateTime CreateDatetime { get; set; }
    
    }
}
