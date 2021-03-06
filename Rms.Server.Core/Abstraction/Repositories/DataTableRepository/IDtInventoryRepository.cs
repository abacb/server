﻿using System;
using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Utility.Models.Entites;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// IDtInventoryRepository
    /// </summary>
    public interface IDtInventoryRepository : IRepository
    {
        /// <summary>
        /// 引数に指定したDtInventoryをDT_INVENTORYテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <returns>追加したデータ。エラーが発生した場合には例外を投げるためnullを返さない</returns>
        DtInventory CreateDtInventoryIfAlreadyMessageThrowEx(DtInventory inData);
    }
}
