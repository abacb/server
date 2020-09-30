namespace Rms.Server.Operation.DBAccessor.Models
{
    /// <summary>
    /// DtInstallBaseクラス
    /// </summary>
    public partial class DtInstallBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <remarks>デフォルトコンストラクタが無いとExceptionが発生するので自作</remarks>
        public DtInstallBase()
        {
        }

        /// <summary>
        /// このインスタンスを、それと同等なRms.Server.Operation.Utility.Models.DtInstallBase型に変換する。
        /// DtEquipmentは設定しない。
        /// </summary>
        /// <returns></returns>
        public Rms.Server.Operation.Utility.Models.DtInstallBase ToModelExcludedEquipment()
        {
            Rms.Server.Operation.Utility.Models.DtInstallBase model = new Rms.Server.Operation.Utility.Models.DtInstallBase();
            model.Sid = this.Sid;
            model.EquipmentNumber = this.EquipmentNumber;
            model.EquipmentName = this.EquipmentName;
            model.EquipmentSerialNumber = this.EquipmentSerialNumber;
            model.InstallCompleteDate = this.InstallCompleteDate;
            model.InstallFeatures = this.InstallFeatures;
            model.CustomerNumber = this.CustomerNumber;
            model.CustomerName = this.CustomerName;
            model.ZipCode = this.ZipCode;
            model.Address = this.Address;
            model.Telephone = this.Telephone;
            model.ScssName = this.ScssName;
            model.Outsourcer = this.Outsourcer;
            model.RemoveDate = this.RemoveDate;
            model.ImportCreateDatetime = this.ImportCreateDatetime;
            model.ImportUpdateDatetime = this.ImportUpdateDatetime;
            model.CreateDatetime = this.CreateDatetime;
            model.UpdateDatetime = this.UpdateDatetime;

            return model;
        }
    }
}
