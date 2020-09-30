using System;
using System.Collections.Generic;

namespace Rms.Server.Core.DBAccessor.Models
{
    public partial class MtEquipmentModel
    {
        public MtEquipmentModel()
        {
            DtDeliveryModel = new HashSet<DtDeliveryModel>();
            DtDevice = new HashSet<DtDevice>();
            DtSoftVersion = new HashSet<DtSoftVersion>();
            MtSoftVersionConvert = new HashSet<MtSoftVersionConvert>();
        }

        public long Sid { get; set; }
        public long EquipmentTypeSid { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime CreateDatetime { get; set; }

        public MtEquipmentType EquipmentTypeS { get; set; }
        public ICollection<DtDeliveryModel> DtDeliveryModel { get; set; }
        public ICollection<DtDevice> DtDevice { get; set; }
        public ICollection<DtSoftVersion> DtSoftVersion { get; set; }
        public ICollection<MtSoftVersionConvert> MtSoftVersionConvert { get; set; }
    }
}
