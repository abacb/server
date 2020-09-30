using System;
using System.Collections.Generic;

namespace Rms.Server.Core.DBAccessor.Models
{
    public partial class DtStorageConfig
    {
        public long Sid { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Sas { get; set; }
        public DateTime CreateDatetime { get; set; }
        public DateTime UpdateDatetime { get; set; }
    }
}
