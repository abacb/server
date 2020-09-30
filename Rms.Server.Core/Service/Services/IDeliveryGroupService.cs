using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Utility.Models.Entites;

namespace Rms.Server.Core.Service.Services
{
    /// <summary>
    /// 配信グループ用サービスのインターフェース
    /// </summary>
    public interface IDeliveryGroupService
    {
        /// <summary>
        /// 配信グループを追加する
        /// </summary>
        /// <param name="utilParam">配信グループパラメータ</param>
        /// <returns>DBに追加したパラメータ(Result付き)</returns>
        Result<DtDeliveryGroup> Create(DtDeliveryGroup utilParam);

        /// <summary>
        /// 配信グループを更新する
        /// </summary>
        /// <param name="utilParam">配信グループパラメータ</param>
        /// <returns>DB更新したパラメータ(Result付き)</returns>
        Result<DtDeliveryGroup> Update(DtDeliveryGroup utilParam);

        /// <summary>
        /// 配信グループを削除する
        /// </summary>
        /// <param name="sid">削除する配信グループのSID</param>
        /// <param name="rowVersion">削除する配信グループのRowVersion</param>
        /// <returns>DB削除したパラメータ(Result付き)</returns>
        Result<DtDeliveryGroup> Delete(long sid, byte[] rowVersion);
    }
}
