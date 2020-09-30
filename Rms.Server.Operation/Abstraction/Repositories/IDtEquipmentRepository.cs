using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Operation.Utility.Models;

namespace Rms.Server.Operation.Abstraction.Repositories
{
    /// <summary>
    /// DT_EQUIPMENTテーブルリポジトリのインターフェース
    /// </summary>
    public interface IDtEquipmentRepository : IRepository
    {
        /// <summary>
        /// DT_EQUIPMENTテーブルからDtEquipmentを取得する
        /// </summary>
        /// <param name="equipmentNumber">機器管理番号</param>
        /// <param name="allowNotExist">取得件数が0件である場合を正常系とする場合はtrueを、異常系とする場合はfalseを指定する</param>
        /// <returns>取得したデータ</returns>
        DtEquipment ReadDtEquipment(string equipmentNumber, bool allowNotExist = true);
    }
}
