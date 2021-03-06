﻿using System;
using System.Collections.Generic;

namespace Rms.Server.Core.DBAccessor.Models
{
    public partial class MtConnectStatus
    {
        public MtConnectStatus()
        {
            DtDevice = new HashSet<DtDevice>();
        }

        public long Sid { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime CreateDatetime { get; set; }

        public ICollection<DtDevice> DtDevice { get; set; }
    }
}
