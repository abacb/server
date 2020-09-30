using Rms.Server.Core.Utility.Models.Entites;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// 配信ファイルリポジトリの追加インターフェース
    /// </summary>
    public interface IDtDeliveryFileRepository : IRepository
    {
        /// <summary>
        /// 引数に指定したDtDeliveryFileをDT_DELIVERY_FILEテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <returns>処理結果</returns>
        DtDeliveryFile CreateDtDeliveryFile(DtDeliveryFile inData);

        /// <summary>
        /// DT_DELIVERY_FILEテーブルからDtDeliveryFileを取得する
        /// </summary>
        /// <param name="sid">取得するデータのSID</param>
        /// <returns>取得したデータ</returns>
        DtDeliveryFile ReadDtDeliveryFile(long sid);

        /// <summary>
        /// 配信ファイルに紐づいた配信グループが配信開始前であれば、
        /// 引数に指定したDtDeliveryFileでDT_DELIVERY_FILEテーブルを更新する
        /// </summary>
        /// <param name="inData">追加するデータ</param>
        /// <returns>ResultCode付きの追加したデータ</returns>
        DtDeliveryFile UpdateDtDeliveryFileIfNoGroupStarted(DtDeliveryFile inData);

        /// <summary>
        /// 中止フラグを更新する
        /// </summary>
        /// <param name="inData">配信ファイルデータ</param>
        /// <returns>ResultCode付きの更新後データ</returns>
        DtDeliveryFile UpdateCancelFlag(DtDeliveryFile inData);

        /// <summary>
        /// DT_DELIVERY_FILEテーブルからDtDeliveryFileを削除する
        /// </summary>
        /// <param name="sid">削除するデータのSID</param>
        /// <param name="rowVersion">rowversion</param>
        /// <returns>削除したデータ</returns>
        DtDeliveryFile DeleteDtDeliveryFile(long sid, byte[] rowVersion);
    }
}
