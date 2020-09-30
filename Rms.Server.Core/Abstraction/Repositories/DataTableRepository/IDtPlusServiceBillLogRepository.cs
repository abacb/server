using System;
using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Utility.Models.Entites;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// IDtPlusServiceBillLogRepository
    /// </summary>
    public interface IDtPlusServiceBillLogRepository : IRepository
    {
        /// <summary>
        /// 引数に指定したDtPlusServiceBillLogをDT_PLUS_SERVICE_BILL_LOGテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <returns>追加したデータ。エラーが発生した場合には例外を投げるためnullを返さない</returns>
        DtPlusServiceBillLog Upsert(DtPlusServiceBillLog inData);
    }
}
