using System;
using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Utility.Models.Entites;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// IDtDriveRepository
    /// </summary>
    public interface IDtDriveRepository : IRepository
    {
        /// <summary>
        /// 引数に指定したDtDriveをDT_DRIVEテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <returns>追加したデータ。エラーが発生した場合には例外を投げるためnullを返さない</returns>
        DtDrive CreateDtDriveIfAlreadyMessageThrowEx(DtDrive inData); 
    }
}
