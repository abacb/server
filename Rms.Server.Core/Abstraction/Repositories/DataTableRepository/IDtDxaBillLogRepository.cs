using System;
using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Utility.Models.Entites;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// IDtDxaBillLogRepository
    /// </summary>
    public interface IDtDxaBillLogRepository : IRepository
    {
        /// <summary>
        /// 引数に指定したDtDxaBillLogをDT_DXA_BILL_LOGテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <returns>追加したデータ。エラーが発生した場合には例外を投げるためnullを返さない</returns>
        DtDxaBillLog Upsert(DtDxaBillLog inData); 
    }
}
