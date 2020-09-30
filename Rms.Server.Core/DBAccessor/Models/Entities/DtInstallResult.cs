using System;
using System.Collections.Generic;

namespace Rms.Server.Core.DBAccessor.Models
{
    public partial class DtInstallResult
    {
        public long Sid { get; set; }
        public long DeviceSid { get; set; }
        public long? DeliveryResultSid { get; set; }
        public long InstallResultStatusSid { get; set; }
        public string SourceEquipmentUid { get; set; }
        public string ReleaseVersion { get; set; }
        public string BeforeVersion { get; set; }
        public string AfterVervion { get; set; }
        public bool? IsSuccess { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public bool? IsAuto { get; set; }
        public string Method { get; set; }
        public string Process { get; set; }
        public DateTime? UpdateStratDatetime { get; set; }
        public DateTime? UpdateEndDatetime { get; set; }
        public string ComputerName { get; set; }
        public string IpAddress { get; set; }
        public string ServerClientKind { get; set; }
        public bool? HasRepairReport { get; set; }
        public DateTime? EventDatetime { get; set; }
        public DateTime? CollectDatetime { get; set; }
        public string MessageId { get; set; }
        public DateTime CreateDatetime { get; set; }

        public DtDeliveryResult DeliveryResultS { get; set; }
        public DtDevice DeviceS { get; set; }
        public MtInstallResultStatus InstallResultStatusS { get; set; }
    }
}
