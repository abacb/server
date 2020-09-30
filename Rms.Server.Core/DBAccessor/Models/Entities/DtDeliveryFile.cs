using System;
using System.Collections.Generic;

namespace Rms.Server.Core.DBAccessor.Models
{
    public partial class DtDeliveryFile
    {
        public DtDeliveryFile()
        {
            DtDeliveryGroup = new HashSet<DtDeliveryGroup>();
            DtDeliveryModel = new HashSet<DtDeliveryModel>();
        }

        public long Sid { get; set; }
        public long DeliveryFileTypeSid { get; set; }
        public long? InstallTypeSid { get; set; }
        public string FilePath { get; set; }
        public string Version { get; set; }
        public string InstallableVersion { get; set; }
        public string Description { get; set; }
        public string InformationId { get; set; }
        public bool? IsCanceled { get; set; }
        public DateTime CreateDatetime { get; set; }
        public DateTime UpdateDatetime { get; set; }
        public byte[] RowVersion { get; set; }

        public MtDeliveryFileType DeliveryFileTypeS { get; set; }
        public MtInstallType InstallTypeS { get; set; }
        public ICollection<DtDeliveryGroup> DtDeliveryGroup { get; set; }
        public ICollection<DtDeliveryModel> DtDeliveryModel { get; set; }
    }
}
