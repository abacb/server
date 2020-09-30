using System;
using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Utility.Models.Entites;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// IDtSoftVersionRepository
    /// </summary>
    public interface IDtSoftVersionRepository : IRepository
    {
        /// <summary>
        /// 引数に指定したDtSoftVersionをDT_SOFT_VERSIONテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <param name="equipmentModelCode">機器型式コード</param>
        /// <returns>追加したデータ。エラーが発生した場合には例外を投げるためnullを返さない</returns>
        DtSoftVersion CreateDtSoftVersionIfAlreadyMessageThrowEx(DtSoftVersion inData, string equipmentModelCode);
    }
}
