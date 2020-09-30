using System;
using System.Collections.Generic;

namespace Rms.Server.Operation.DBAccessor.Models
{
    public partial class DtAlarmConfig
    {
        public long Sid { get; set; }
        public byte AlarmLevelFrom { get; set; }
        public byte AlarmLevelTo { get; set; }
        public string MailAddress { get; set; }
        public byte? MailSendingInterval { get; set; }
        public string Description { get; set; }
        public DateTime CreateDatetime { get; set; }
        public DateTime UpdateDatetime { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
