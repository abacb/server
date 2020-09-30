using Rms.Server.Core.Utility.Models.Entites;
using System.Collections.Generic;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// 配信グループリポジトリの追加インターフェース
    /// </summary>
    public interface IDtDeliveryGroupRepository
    {
        /// <summary>
        /// 引数に指定したDtDeliveryGroupをDT_DELIVERY_GROUPテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <returns>処理結果</returns>
        DtDeliveryGroup CreateDtDeliveryGroup(DtDeliveryGroup inData);

        /// <summary>
        /// DT_DELIVERY_GROUPテーブルからDtDeliveryGroupを取得する
        /// </summary>
        /// <param name="sid">取得するデータのSID</param>
        /// <returns>取得したデータ</returns>
        DtDeliveryGroup ReadDtDeliveryGroup(long sid);

        /// <summary>
        /// データの削除
        /// </summary>
        /// <param name="sid">削除するデータのSID</param>
        /// <param name="rowVersion">rowversion</param>
        /// <returns>削除したデータ</returns>
        DtDeliveryGroup DeleteDtDeliveryGroup(long sid, byte[] rowVersion);

        /// <summary>
        /// データの更新(配信ステータスが「配信前」の時のみ)
        /// </summary>
        /// <param name="inData">更新データ</param>
        /// <returns>更新されたDB内容</returns>
        DtDeliveryGroup UpdateDtDeliveryGroupIfDeliveryNotStart(DtDeliveryGroup inData);

        /// <summary>
        /// 配信開始可能なデータを取得する
        /// </summary>
        /// <returns>配信開始可能なデータ群</returns>
        IEnumerable<DtDeliveryGroup> ReadStartableDtDeliveryGroup();

        /// <summary>
        /// データの更新(配信ステータスを指定SIDのものにする)
        /// </summary>
        /// <param name="sid">更新する配信グループデータのSID</param>
        /// <returns>更新されたDB内容</returns>
        DtDeliveryGroup UpdateDtDeliveryGroupStatusStarted(long sid);

        /// <summary>
        /// 配信用に親・子エンティティデータをIncludeしたデータを取得する
        /// </summary>
        /// <param name="sid">配信グループSID</param>
        /// <returns>配信用に親・子エンティティデータをIncludeしたデータ</returns>
        DtDeliveryGroup ReadDeliveryIncludedDtDeliveryGroup(long sid);

        /// <summary>
        /// 指定した最上位（ゲートウェイ）機器IDを持ち、
        /// 配信グループステータスが開始済みかつ
        /// ダウンロードが完了していない（適用結果ステータスが"notstarted"または"messagesent"）
        /// 配信グループのリストを取得する
        /// </summary>
        /// <param name="gatewaySid">最上位（ゲートウェイ）機器SID</param>
        /// <returns>結果</returns>
        List<DtDeliveryGroup> GetDevicesByGatewaySidNotCompletedDownload(long gatewaySid);
    }
}
