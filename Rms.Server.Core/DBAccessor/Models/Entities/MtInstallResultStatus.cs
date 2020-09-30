using System;
using System.Collections.Generic;

namespace Rms.Server.Core.DBAccessor.Models
{
    public partial class MtInstallResultStatus
    {
        public MtInstallResultStatus()
        {
            DtInstallResult = new HashSet<DtInstallResult>();
        }

        public long Sid { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime CreateDatetime { get; set; }

        public ICollection<DtInstallResult> DtInstallResult { get; set; }
    }
}
