using System;
using System.Collections.Generic;

namespace Rms.Server.Operation.DBAccessor.Models
{
    public partial class DtEquipment
    {
        public DtEquipment()
        {
            DtAlarm = new HashSet<DtAlarm>();
            InverseParentEquipmentS = new HashSet<DtEquipment>();
            InverseTopEquipmentS = new HashSet<DtEquipment>();
        }

        public long Sid { get; set; }
        public long InstallBaseSid { get; set; }
        public long NetworkRouteSid { get; set; }
        public long? TopEquipmentSid { get; set; }
        public long? ParentEquipmentSid { get; set; }
        public byte Hierarchy { get; set; }
        public string HierarchyPath { get; set; }
        public string HierarchyOrder { get; set; }
        public string EquipmentNumber { get; set; }
        public string HostName { get; set; }
        public string IpAddress { get; set; }
        public string Description { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime CreateDatetime { get; set; }
        public DateTime UpdateDatetime { get; set; }

        public DtInstallBase InstallBaseS { get; set; }
        public MtNetworkRoute NetworkRouteS { get; set; }
        public DtEquipment ParentEquipmentS { get; set; }
        public DtEquipment TopEquipmentS { get; set; }
        public ICollection<DtAlarm> DtAlarm { get; set; }
        public ICollection<DtEquipment> InverseParentEquipmentS { get; set; }
        public ICollection<DtEquipment> InverseTopEquipmentS { get; set; }
    }
}
