using System;
using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Utility.Models.Entites;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// IDtEquipmentUsageRepository
    /// </summary>
    public interface IDtEquipmentUsageRepository : IRepository
    {
        /// <summary>
        /// 引数に指定したDtEquipmentUsageをDT_EQUIPMENT_USAGEテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <returns>追加したデータ。エラーが発生した場合には例外を投げるためnullを返さない</returns>
        DtEquipmentUsage CreateDtEquipmentUsageIfAlreadyMessageThrowEx(DtEquipmentUsage inData);
    }
}
