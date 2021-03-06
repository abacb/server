//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはテンプレートから生成されました。
//
//     このファイルを手動で変更すると、アプリケーションで予期しない動作が発生する可能性があります。
//     このファイルに対する手動の変更は、コードが再生成されると上書きされます。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using Microsoft.Data.SqlClient;
using Rms.Server.Core.Utility.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Rms.Server.Core.Utility.Models.Entites;
using Microsoft.Extensions.Logging;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Extensions;
using Rms.Server.Core.Abstraction.Pollies;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// DT_DELIVERY_RESULTテーブルのリポジトリ
    /// </summary>
    public partial class DtDeliveryResultRepository : IDtDeliveryResultRepository
    {
        /// <summary>ロガー</summary>
        private readonly ILogger<DtDeliveryResultRepository> _logger;
    
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
        public DtDeliveryResultRepository(ILogger<DtDeliveryResultRepository> logger, ITimeProvider timePrivder, DBPolly dbPolly, AppSettings appSettings)
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
        /// 引数に指定したDtDeliveryResultをDT_DELIVERY_RESULTテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <returns>処理結果</returns>
        public DtDeliveryResult CreateDtDeliveryResult(DtDeliveryResult inData)
        {
            DtDeliveryResult model = null;
            try
            {
                _logger.EnterJson("{0}", inData);
    
                Rms.Server.Core.DBAccessor.Models.DtDeliveryResult entity = new Rms.Server.Core.DBAccessor.Models.DtDeliveryResult(inData);
    
                _dbPolly.Execute(() =>
                {
                    entity.CreateDatetime = _timePrivder.UtcNow;
    
                    using (Rms.Server.Core.DBAccessor.Models.RmsDbContext db = new Rms.Server.Core.DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        var dbdata = db.DtDeliveryResult.Add(entity).Entity;
                        db.SaveChanges();
                        model = dbdata.ToModel();
                    }
                });
    
                return model;
            }
            catch (ValidationException e)
            {
                throw new RmsParameterException(e.ValidationResult.ErrorMessage, e);
            }
            catch (Exception e)
            {
                throw new RmsException("DT_DELIVERY_RESULTテーブルへのInsertに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }
    
        /// <summary>
        /// DT_DELIVERY_RESULTテーブルからDtDeliveryResultを取得する
        /// </summary>
        /// <param name="sid">取得するデータのSID</param>
        /// <returns>取得したデータ</returns>
        public DtDeliveryResult ReadDtDeliveryResult(long sid)
        {
            DtDeliveryResult model = null;
            try
            {
                _logger.Enter($"{nameof(sid)}={sid}");
    
                Rms.Server.Core.DBAccessor.Models.DtDeliveryResult entity = null;
                _dbPolly.Execute(() =>
                {
                    using (Rms.Server.Core.DBAccessor.Models.RmsDbContext db = new Rms.Server.Core.DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        entity = db.DtDeliveryResult.FirstOrDefault(x => x.Sid == sid);
                    }
                });
    
                if (entity != null)
                {
                    model = entity.ToModel();
                }
    
                return model;
            }
            catch (ValidationException e)
            {
                throw new RmsParameterException(e.ValidationResult.ErrorMessage, e);
            }
            catch (Exception e)
            {
                throw new RmsException("DT_DELIVERY_RESULTテーブルのSelectに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }
    
        /// <summary>
        /// 引数に指定したDtDeliveryResultでDT_DELIVERY_RESULTテーブルを更新する
        /// </summary>
        /// <param name="inData">更新するデータ</param>
        /// <returns>更新したデータ</returns>
        public DtDeliveryResult UpdateDtDeliveryResult(DtDeliveryResult inData)
        {
            DtDeliveryResult model = null;
            try
            {
                _logger.EnterJson("{0}", inData);
    
                Rms.Server.Core.DBAccessor.Models.DtDeliveryResult entity = new Rms.Server.Core.DBAccessor.Models.DtDeliveryResult(inData);
    
                _dbPolly.Execute(() =>
                {
    
                    using (Rms.Server.Core.DBAccessor.Models.RmsDbContext db = new Rms.Server.Core.DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        db.DtDeliveryResult.Attach(entity);
    
                        // 全フィールドを更新する
                        //     特定フィールドだけUpdateする場合は下記のように記述してください
                        //     db.Entry(entity).Property(x => x.UpdateDatetime).IsModified = true;
                        db.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
    
                        if (db.SaveChanges() > 0)
                        {
                            model = entity.ToModel();
                        }
                    }
                });
    
                return model;
            }
            catch (ValidationException e)
            {
                throw new RmsParameterException(e.ValidationResult.ErrorMessage, e);
            }
            catch (Exception e)
            {
                throw new RmsException("DT_DELIVERY_RESULTテーブルのUpdateに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }
    
        /// <summary>
        /// DT_DELIVERY_RESULTテーブルからDtDeliveryResultを削除する
        /// </summary>
        /// <param name="sid">削除するデータのSID</param>
        /// <returns>削除したデータ</returns>
        public DtDeliveryResult DeleteDtDeliveryResult(long sid)
        {
            DtDeliveryResult model = null;
            try
            {
                _logger.Enter($"{nameof(sid)}={sid}");
    
                Rms.Server.Core.DBAccessor.Models.DtDeliveryResult entity = new Rms.Server.Core.DBAccessor.Models.DtDeliveryResult() { Sid = sid };
                _dbPolly.Execute(() =>
                {
                    using (Rms.Server.Core.DBAccessor.Models.RmsDbContext db = new Rms.Server.Core.DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        db.DtDeliveryResult.Attach(entity);
                        db.DtDeliveryResult.Remove(entity);
    
                        if (db.SaveChanges() > 0)
                        {
                            model = entity.ToModel();
                        }
                    }
                });
    
                return model;
            }
            catch (ValidationException e)
            {
                throw new RmsParameterException(e.ValidationResult.ErrorMessage, e);
            }
            catch (Exception e)
            {
                throw new RmsException("DT_DELIVERY_RESULTテーブルのDeleteに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }
    }
}
