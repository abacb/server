using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Utility.Models.Entites;
using System;
using System.Collections.Generic;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// IDtInstallResultRepository
    /// </summary>
    public interface IDtInstallResultRepository : IRepository
    {
        /// <summary>
        /// 引数に指定したDtInstallResultをDT_INSTALL_RESULTテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <param name="state">状態。適用結果ステータスマスタテーブルのコード値が入る。</param>
        /// <returns>追加したデータ。エラーが発生した場合には例外を投げるためnullを返さない</returns>
        DtInstallResult CreateDtInstallResultIfAlreadyMessageThrowEx(DtInstallResult inData, string state);

        /// <summary>
        /// 引数指定グループ内の配信結果内で、ゲートウェイ機器SIDをもつデータの適用結果ステータスをmessagesentにして作成する
        /// </summary>
        /// <remarks>配信グループデータは参照のみ行い、メンバを変更しない</remarks>
        /// <param name="inData">配信グループデータ</param>
        /// <param name="gatewaySid">ゲートウェイ機器SID</param>
        /// <returns>追加した適用結果データ群</returns>
        IEnumerable<DtInstallResult> CreateDtInstallResultStatusSent(DtDeliveryGroup inData, long gatewaySid);

        /// <summary>
        /// 配信結果ステータスを"messagesent"に更新する
        /// </summary>
        /// <param name="gatewaySid">最上位（ゲートウェイ）機器SID</param>
        /// <returns>結果</returns>
        DtInstallResult UpdateInstallResultStatusToMessageSent(long gatewaySid);
    }
}
