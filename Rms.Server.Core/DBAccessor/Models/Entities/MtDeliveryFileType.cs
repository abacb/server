using System;
using System.Collections.Generic;

namespace Rms.Server.Core.DBAccessor.Models
{
    public partial class MtDeliveryFileType
    {
        public MtDeliveryFileType()
        {
            DtDeliveryFile = new HashSet<DtDeliveryFile>();
        }

        public long Sid { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime CreateDatetime { get; set; }

        public ICollection<DtDeliveryFile> DtDeliveryFile { get; set; }
    }
}
