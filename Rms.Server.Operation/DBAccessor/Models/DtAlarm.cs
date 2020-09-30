namespace Rms.Server.Operation.DBAccessor.Models
{
    /// <summary>
    /// DtAlarmクラス
    /// </summary>
    public partial class DtAlarm
    {
        /// <summary>
        /// このインスタンスの各プロパティに、引数に指定されたDtAlarmのプロパティの値をコピーする
        /// DtEquipmentは除く
        /// </summary>
        /// <param name="entity">コピー元のDtAlarm</param>
        public void CopyExcludingEquipmentFrom(Rms.Server.Operation.Utility.Models.DtAlarm entity)
        {
            this.Sid = entity.Sid;
            this.EquipmentSid = entity.EquipmentSid;
            this.TypeCode = entity.TypeCode;
            this.ErrorCode = entity.ErrorCode;
            this.AlarmLevel = entity.AlarmLevel;
            this.AlarmTitle = entity.AlarmTitle;
            this.AlarmDescription = entity.AlarmDescription;
            this.AlarmDatetime = entity.AlarmDatetime;
            this.AlarmDefId = entity.AlarmDefId;
            this.EventDatetime = entity.EventDatetime;
            this.HasMail = entity.HasMail;
            this.MessageId = entity.MessageId;
            this.CreateDatetime = entity.CreateDatetime;
        }

        /// <summary>
        /// このインスタンスを、それと同等なRms.Server.Operation.Utility.Models.DtAlarm型に変換する。
        /// DtEquipmentは設定しない。
        /// </summary>
        /// <returns></returns>
        public Rms.Server.Operation.Utility.Models.DtAlarm ToModelExcludedDtEquipment()
        {
            Rms.Server.Operation.Utility.Models.DtAlarm model = new Rms.Server.Operation.Utility.Models.DtAlarm();
            model.Sid = this.Sid;
            model.EquipmentSid = this.EquipmentSid;
            model.TypeCode = this.TypeCode;
            model.ErrorCode = this.ErrorCode;
            model.AlarmLevel = this.AlarmLevel;
            model.AlarmTitle = this.AlarmTitle;
            model.AlarmDescription = this.AlarmDescription;
            model.AlarmDatetime = this.AlarmDatetime;
            model.AlarmDefId = this.AlarmDefId;
            model.EventDatetime = this.EventDatetime;
            model.HasMail = this.HasMail;
            model.MessageId = this.MessageId;
            model.CreateDatetime = this.CreateDatetime;

            return model;
        }
    }
}
