using System.Linq;

namespace Rms.Server.Operation.DBAccessor.Models
{
    /// <summary>
    /// DtEquipmentクラス
    /// </summary>
    public partial class DtEquipment
    {
        /// <summary>
        /// このインスタンスを、それと同等なRms.Server.Operation.Utility.Models.DtEquipment型に変換する。
        /// InstallBaseに値を設定する。
        /// </summary>
        /// <returns></returns>
        public Rms.Server.Operation.Utility.Models.DtEquipment ToModelIncludedInstallBase()
        {
            Rms.Server.Operation.Utility.Models.DtEquipment model = new Rms.Server.Operation.Utility.Models.DtEquipment();
            model.Sid = this.Sid;
            model.InstallBaseSid = this.InstallBaseSid;
            model.NetworkRouteSid = this.NetworkRouteSid;
            model.TopEquipmentSid = this.TopEquipmentSid;
            model.ParentEquipmentSid = this.ParentEquipmentSid;
            model.Hierarchy = this.Hierarchy;
            model.HierarchyPath = this.HierarchyPath;
            model.HierarchyOrder = this.HierarchyOrder;
            model.EquipmentNumber = this.EquipmentNumber;
            model.HostName = this.HostName;
            model.IpAddress = this.IpAddress;
            model.Description = this.Description;
            model.IsDeleted = this.IsDeleted;
            model.CreateDatetime = this.CreateDatetime;
            model.UpdateDatetime = this.UpdateDatetime;
            model.DtAlarm = this.DtAlarm.Select(y => y.ToModel()).ToHashSet();
            model.DtInstallBase = this.InstallBaseS.ToModelExcludedEquipment();

            return model;
        }
    }
}
