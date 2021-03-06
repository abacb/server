﻿using Microsoft.Extensions.Logging;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Core.Utility.Extensions;
using Rms.Server.Core.Utility.Models.Entites;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// DT_EQUIPMENT_USAGEテーブルのリポジトリ
    /// </summary>
    public partial class DtEquipmentUsageRepository : IDtEquipmentUsageRepository
    {
        /// <summary>ロガー</summary>
        private readonly ILogger<DtEquipmentUsageRepository> _logger;
    
        /// <summary>DateTimeの提供元</summary>
        private readonly ITimeProvider _timePrivder;
    
        /// <summary>DB接続用のPolly</summary>
        private readonly DBPolly _dbPolly;
    
        /// <summary>アプリケーション設定</summary>
        private readonly AppSettings _appSettings;
    
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="timePrivder">DateTimeの提供元</param>
        /// <param name="dbPolly">DB接続用のPolly</param>
        /// <param name="appSettings">アプリケーション設定</param>
        public DtEquipmentUsageRepository(ILogger<DtEquipmentUsageRepository> logger, ITimeProvider timePrivder, DBPolly dbPolly, AppSettings appSettings)
        {
            Assert.IfNull(logger);
            Assert.IfNull(timePrivder);
            Assert.IfNull(dbPolly);
            Assert.IfNull(appSettings);
            _logger = logger;
            _timePrivder = timePrivder;
            _dbPolly = dbPolly;
            _appSettings = appSettings;
        }

        /// <summary>
        /// 引数に指定したDtEquipmentUsageをDT_EQUIPMENT_USAGEテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <returns>追加したデータ。エラーが発生した場合には例外を投げるためnullを返さない</returns>
        public DtEquipmentUsage CreateDtEquipmentUsageIfAlreadyMessageThrowEx(DtEquipmentUsage inData)
        {
            DtEquipmentUsage model = null;
            try
            {
                _logger.EnterJson("{0}", inData);
    
                DBAccessor.Models.DtEquipmentUsage entity = new DBAccessor.Models.DtEquipmentUsage(inData);
    
                _dbPolly.Execute(() =>
                {
                    // メッセージIDがなければ追加
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        var addedAlready = db.DtEquipmentUsage.FirstOrDefault(x => x.MessageId == inData.MessageId);
                        if (addedAlready != null)
                        {
                            throw new RmsAlreadyExistException(string.Format("MessageId [{0}] はすでに追加済みです。", inData.MessageId));
                        }

                        var dbdata = db.DtEquipmentUsage.Add(entity).Entity;
                        db.SaveChanges(_timePrivder);
                        model = dbdata.ToModel();
                    }
                });
    
                return model;
            }
            catch (RmsAlreadyExistException)
            {
                throw;
            }
            catch (ValidationException e)
            {
                throw new RmsParameterException(e.ValidationResult.ErrorMessage, e);
            }
            catch (Exception e)
            {
                throw new RmsException("DT_EQUIPMENT_USAGEテーブルへのInsertに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }
    }
}
