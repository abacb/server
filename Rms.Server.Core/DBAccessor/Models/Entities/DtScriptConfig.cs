using System;
using System.Collections.Generic;

namespace Rms.Server.Core.DBAccessor.Models
{
    public partial class DtScriptConfig
    {
        public long Sid { get; set; }
        public long InstallTypeSid { get; set; }
        public string Name { get; set; }
        public short? Version { get; set; }
        public string FileName { get; set; }
        public string Location { get; set; }
        public DateTime CreateDatetime { get; set; }

        public MtInstallType InstallTypeS { get; set; }
    }
}
