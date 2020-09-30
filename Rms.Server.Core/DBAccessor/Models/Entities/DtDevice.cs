using System;
using System.Collections.Generic;

namespace Rms.Server.Core.DBAccessor.Models
{
    public partial class DtDevice
    {
        public DtDevice()
        {
            DtDeliveryResultDeviceS = new HashSet<DtDeliveryResult>();
            DtDeliveryResultGwDeviceS = new HashSet<DtDeliveryResult>();
            DtDeviceFile = new HashSet<DtDeviceFile>();
            DtDirectoryUsage = new HashSet<DtDirectoryUsage>();
            DtDiskDrive = new HashSet<DtDiskDrive>();
            DtDrive = new HashSet<DtDrive>();
            DtDxaBillLog = new HashSet<DtDxaBillLog>();
            DtDxaQcLog = new HashSet<DtDxaQcLog>();
            DtEquipmentUsage = new HashSet<DtEquipmentUsage>();
            DtInstallResult = new HashSet<DtInstallResult>();
            DtInventory = new HashSet<DtInventory>();
            DtParentChildConnectChildDeviceS = new HashSet<DtParentChildConnect>();
            DtParentChildConnectParentDeviceS = new HashSet<DtParentChildConnect>();
            DtPlusServiceBillLog = new HashSet<DtPlusServiceBillLog>();
            DtSoftVersion = new HashSet<DtSoftVersion>();
        }

        public long Sid { get; set; }
        public long? EquipmentModelSid { get; set; }
        public long InstallTypeSid { get; set; }
        public long ConnectStatusSid { get; set; }
        public Guid EdgeId { get; set; }
        public string EquipmentUid { get; set; }
        public string RemoteConnectUid { get; set; }
        public string RmsSoftVersion { get; set; }
        public DateTime? ConnectStartDatetime { get; set; }
        public DateTime? ConnectUpdateDatetime { get; set; }
        public DateTime CreateDatetime { get; set; }
        public DateTime UpdateDatetime { get; set; }

        public MtConnectStatus ConnectStatusS { get; set; }
        public MtEquipmentModel EquipmentModelS { get; set; }
        public MtInstallType InstallTypeS { get; set; }
        public ICollection<DtDeliveryResult> DtDeliveryResultDeviceS { get; set; }
        public ICollection<DtDeliveryResult> DtDeliveryResultGwDeviceS { get; set; }
        public ICollection<DtDeviceFile> DtDeviceFile { get; set; }
        public ICollection<DtDirectoryUsage> DtDirectoryUsage { get; set; }
        public ICollection<DtDiskDrive> DtDiskDrive { get; set; }
        public ICollection<DtDrive> DtDrive { get; set; }
        public ICollection<DtDxaBillLog> DtDxaBillLog { get; set; }
        public ICollection<DtDxaQcLog> DtDxaQcLog { get; set; }
        public ICollection<DtEquipmentUsage> DtEquipmentUsage { get; set; }
        public ICollection<DtInstallResult> DtInstallResult { get; set; }
        public ICollection<DtInventory> DtInventory { get; set; }
        public ICollection<DtParentChildConnect> DtParentChildConnectChildDeviceS { get; set; }
        public ICollection<DtParentChildConnect> DtParentChildConnectParentDeviceS { get; set; }
        public ICollection<DtPlusServiceBillLog> DtPlusServiceBillLog { get; set; }
        public ICollection<DtSoftVersion> DtSoftVersion { get; set; }
    }
}
