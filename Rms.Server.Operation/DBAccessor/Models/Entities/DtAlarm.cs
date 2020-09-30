using System;
using System.Collections.Generic;

namespace Rms.Server.Operation.DBAccessor.Models
{
    public partial class DtAlarm
    {
        public long Sid { get; set; }
        public long EquipmentSid { get; set; }
        public string TypeCode { get; set; }
        public string ErrorCode { get; set; }
        public byte? AlarmLevel { get; set; }
        public string AlarmTitle { get; set; }
        public string AlarmDescription { get; set; }
        public DateTime? AlarmDatetime { get; set; }
        public string AlarmDefId { get; set; }
        public DateTime? EventDatetime { get; set; }
        public bool? HasMail { get; set; }
        public string MessageId { get; set; }
        public DateTime CreateDatetime { get; set; }

        public DtEquipment EquipmentS { get; set; }
    }
}
