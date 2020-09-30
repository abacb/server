using System;
using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Utility.Models.Entites;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// IDtDxaQcLogRepository
    /// </summary>
    public interface IDtDxaQcLogRepository : IRepository
    {
        /// <summary>
        /// 引数に指定したDtDxaQcLogをDT_DXA_QC_LOGテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <returns>追加したデータ。エラーが発生した場合には例外を投げるためnullを返さない</returns>
        DtDxaQcLog CreateDtDxaQcLogIfAlreadyMessageThrowEx(DtDxaQcLog inData);
    }
}
