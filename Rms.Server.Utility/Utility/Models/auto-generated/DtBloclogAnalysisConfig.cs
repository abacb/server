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
    
        public System.DateTime CreateDatetime { get; set; }
    
        public System.DateTime UpdateDatetime { get; set; }
    
    }
}
