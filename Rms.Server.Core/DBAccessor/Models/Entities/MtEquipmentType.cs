using System;
using System.Collections.Generic;

namespace Rms.Server.Core.DBAccessor.Models
{
    public partial class MtEquipmentType
    {
        public MtEquipmentType()
        {
            MtEquipmentModel = new HashSet<MtEquipmentModel>();
            MtInstallType = new HashSet<MtInstallType>();
        }

        public long Sid { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime CreateDatetime { get; set; }

        public ICollection<MtEquipmentModel> MtEquipmentModel { get; set; }
        public ICollection<MtInstallType> MtInstallType { get; set; }
    }
}
