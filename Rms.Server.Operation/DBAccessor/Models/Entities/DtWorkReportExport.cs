using System;
using System.Collections.Generic;

namespace Rms.Server.Operation.DBAccessor.Models
{
    public partial class DtWorkReportExport
    {
        public long Sid { get; set; }
        public long StartWorkId { get; set; }
        public DateTime? StartWorkDatetime { get; set; }
        public long EndWorkId { get; set; }
        public DateTime? EndWorkDatetime { get; set; }
        public string FileName { get; set; }
        public DateTime CreateDatetime { get; set; }
    }
}
