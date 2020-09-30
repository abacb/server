using System;
using System.Collections.Generic;

namespace Rms.Server.Operation.DBAccessor.Models
{
    public partial class DtInstallBase
    {
        public long Sid { get; set; }
        public string EquipmentNumber { get; set; }
        public string EquipmentName { get; set; }
        public string EquipmentSerialNumber { get; set; }
        public DateTime? InstallCompleteDate { get; set; }
        public string InstallFeatures { get; set; }
        public int? CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public string ZipCode { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string ScssName { get; set; }
        public string Outsourcer { get; set; }
        public DateTime? RemoveDate { get; set; }
        public DateTime? ImportCreateDatetime { get; set; }
        public DateTime? ImportUpdateDatetime { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public DateTime? UpdateDatetime { get; set; }

        public DtEquipment DtEquipment { get; set; }
    }
}
