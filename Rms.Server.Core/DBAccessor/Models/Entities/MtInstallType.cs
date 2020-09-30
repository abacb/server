using System;
using System.Collections.Generic;

namespace Rms.Server.Core.DBAccessor.Models
{
    public partial class MtInstallType
    {
        public MtInstallType()
        {
            DtDeliveryFile = new HashSet<DtDeliveryFile>();
            DtDevice = new HashSet<DtDevice>();
            DtScriptConfig = new HashSet<DtScriptConfig>();
        }

        public long Sid { get; set; }
        public long EquipmentTypeSid { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime CreateDatetime { get; set; }

        public MtEquipmentType EquipmentTypeS { get; set; }
        public ICollection<DtDeliveryFile> DtDeliveryFile { get; set; }
        public ICollection<DtDevice> DtDevice { get; set; }
        public ICollection<DtScriptConfig> DtScriptConfig { get; set; }
    }
}
