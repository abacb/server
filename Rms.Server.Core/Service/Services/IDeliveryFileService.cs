using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Utility.Models.Entites;

namespace Rms.Server.Core.Service.Services
{
    /// <summary>
    /// 配信ファイル用サービスのインターフェース
    /// </summary>
    public interface IDeliveryFileService
    {
        /// <summary>
        /// 配信ファイルを追加する
        /// </summary>
        /// <param name="utilParam">配信ファイルパラメータ</param>
        /// <returns>DBに追加したパラメータ(Result付き)</returns>
        Result<DtDeliveryFile> Create(DtDeliveryFile utilParam);

        /// <summary>
        /// 配信ファイルを更新する
        /// </summary>
        /// <param name="utilParam">配信ファイルパラメータ</param>
        /// <returns>DB更新したパラメータ(Result付き)</returns>
        Result<DtDeliveryFile> Update(DtDeliveryFile utilParam);

        /// <summary>
        /// 配信ファイルを削除する
        /// </summary>
        /// <param name="sid">削除する配信ファイルのSID</param>
        /// <param name="rowVersion">削除する配信ファイルのRowVersion</param>
        /// <returns>DB削除したパラメータ(Result付き)</returns>
        Result<DtDeliveryFile> Delete(long sid, byte[] rowVersion);

        /// <summary>
        /// 中止フラグを更新する
        /// </summary>
        /// <param name="utilParam">更新する配信ファイルデータ</param>
        /// <returns>DB更新したデータ(Result付き)</returns>
        Result<DtDeliveryFile> PutCancelFlag(DtDeliveryFile utilParam);
    }
}
