﻿using System;
using System.Collections.Generic;

namespace Rms.Server.Utility.DBAccessor.Models
{
    public partial class MtInstallType
    {
        public long Sid { get; set; }
        public long EquipmentTypeSid { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime CreateDatetime { get; set; }
    }
}
