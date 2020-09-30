using System;
using System.Collections.Generic;

namespace Rms.Server.Utility.DBAccessor.Models
{
    public partial class DtDevice
    {
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
    }
}
