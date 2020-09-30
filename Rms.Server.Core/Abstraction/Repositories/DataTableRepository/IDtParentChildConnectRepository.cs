using System;
using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Utility.Models.Dispatch;
using Rms.Server.Core.Utility.Models.Entites;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// IDtParentChildConnectRepository
    /// </summary>
    public interface IDtParentChildConnectRepository : IRepository
    {
        /// <summary>
        /// DT_PARENT_CHILD_CONNECTテーブルにメッセージを追加またはメッセージの内容で更新処理を行う（親フラグ=trueの場合）
        /// </summary>
        /// <param name="inData">更新データ</param>
        /// <returns>追加または更新したデータ。「確認日時」が既存レコードより古く更新しなかった場合には、nullを返す</returns>
        DtParentChildConnect Save(DtParentChildConnectFromParent inData);

        /// <summary>
        /// DT_PARENT_CHILD_CONNECTテーブルにメッセージを追加またはメッセージの内容で更新処理を行う（親フラグ=falseの場合）
        /// </summary>
        /// <param name="inData">更新データ</param>
        /// <returns>追加または更新したデータ。「確認日時」が既存レコードより古く更新しなかった場合には、nullを返す</returns>
        DtParentChildConnect Save(DtParentChildConnectFromChild inData);
    }
}
