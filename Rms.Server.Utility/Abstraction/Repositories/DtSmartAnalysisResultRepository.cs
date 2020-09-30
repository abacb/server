using Microsoft.Extensions.Logging;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Utility.Utility.Extensions;
using Rms.Server.Utility.Utility.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// DT_SMART_ANALYSIS_RESULTテーブルのリポジトリ
    /// </summary>
    public partial class DtSmartAnalysisResultRepository : IDtSmartAnalysisResultRepository
    {
        /// <summary>ロガー</summary>
        private readonly ILogger<DtSmartAnalysisResultRepository> _logger;
    
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
        public DtSmartAnalysisResultRepository(ILogger<DtSmartAnalysisResultRepository> logger, ITimeProvider timePrivder, DBPolly dbPolly, AppSettings appSettings)
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
        /// 引数に指定したDtSmartAnalysisResultをDT_SMART_ANALYSIS_RESULTテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <returns>処理結果</returns>
        public DtSmartAnalysisResult CreateDtSmartAnalysisResult(DtSmartAnalysisResult inData)
        {
            DtSmartAnalysisResult model = null;
            try
            {
                _logger.EnterJson("{0}", inData);
    
                DBAccessor.Models.DtSmartAnalysisResult entity = new DBAccessor.Models.DtSmartAnalysisResult(inData);
    
                // バリデーション
                Validator.ValidateObject(entity, new ValidationContext(entity, null, null));
    
                _dbPolly.Execute(() =>
                {
                    entity.CreateDatetime = _timePrivder.UtcNow;
                    entity.UpdateDatetime = _timePrivder.UtcNow;

                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        var dbdata = db.DtSmartAnalysisResult.Add(entity).Entity;
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
                throw new RmsException("DT_SMART_ANALYSIS_RESULTテーブルへのInsertに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// DT_SMART_ANALYSIS_RESULTテーブルからDtSmartAnalysisResultを取得する
        /// </summary>
        /// <param name="diskDrive">ディスクドライブ</param>
        /// <returns>取得したデータ</returns>
        public DtSmartAnalysisResult ReadDtSmartAnalysisResult(DiskDrive diskDrive)
        {
            DtSmartAnalysisResult model = null;
            try
            {
                _logger.EnterJson("{0}", diskDrive);

                DBAccessor.Models.DtSmartAnalysisResult entity = null;
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        entity = db.DtSmartAnalysisResult.FirstOrDefault(x => x.EquipmentUid == diskDrive.SourceEquipmentUid && x.DiskSerialNumber == diskDrive.SerialNo);
                    }
                });

                if (entity != null)
                {
                    model = entity.ToModel();
                }

                return model;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_SMART_ANALYSIS_RESULTテーブルのSelectに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// 引数に指定したDtSmartAnalysisResultでDT_SMART_ANALYSIS_RESULTテーブルを更新する
        /// </summary>
        /// <param name="inData">更新するデータ</param>
        /// <returns>更新したデータ</returns>
        public DtSmartAnalysisResult UpdateDtSmartAnalysisResult(DtSmartAnalysisResult inData)
        {
            DtSmartAnalysisResult model = null;
            try
            {
                _logger.EnterJson("{0}", inData);
    
                DBAccessor.Models.DtSmartAnalysisResult entity = new DBAccessor.Models.DtSmartAnalysisResult(inData);
    
                // バリデーション
                Validator.ValidateObject(entity, new ValidationContext(entity, null, null));
    
                _dbPolly.Execute(() =>
                {
                    entity.UpdateDatetime = _timePrivder.UtcNow;
    
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        db.DtSmartAnalysisResult.Attach(entity);
    
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
                throw new RmsException("DT_SMART_ANALYSIS_RESULTテーブルのUpdateに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }
    }
}
