using Microsoft.Extensions.Logging;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Utility.Utility.Extensions;
using Rms.Server.Utility.Utility.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// DT_BLOCLOG_ANALYSIS_RESULTテーブルのリポジトリ
    /// </summary>
    public partial class DtBloclogAnalysisResultRepository : IDtBloclogAnalysisResultRepository, Core.Abstraction.Repositories.ICleanRepository
    {
        /// <summary>ロガー</summary>
        private readonly ILogger<DtBloclogAnalysisResultRepository> _logger;

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
        public DtBloclogAnalysisResultRepository(ILogger<DtBloclogAnalysisResultRepository> logger, ITimeProvider timePrivder, DBPolly dbPolly, AppSettings appSettings)
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
        /// 引数に指定したDtBloclogAnalysisResultをDT_BLOCLOG_ANALYSIS_RESULTテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <returns>処理結果</returns>
        public DtBloclogAnalysisResult CreateDtBloclogAnalysisResult(DtBloclogAnalysisResult inData)
        {
            DtBloclogAnalysisResult model = null;
            try
            {
                _logger.EnterJson("{0}", inData);

                DBAccessor.Models.DtBloclogAnalysisResult entity = new DBAccessor.Models.DtBloclogAnalysisResult(inData);

                // バリデーション
                Validator.ValidateObject(entity, new ValidationContext(entity, null, null));

                _dbPolly.Execute(() =>
                {
                    entity.CreateDatetime = _timePrivder.UtcNow;

                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        var dbdata = db.DtBloclogAnalysisResult.Add(entity).Entity;
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
                throw new RmsException("DT_BLOCLOG_ANALYSIS_RESULTテーブルへのInsertに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// DT_BLOCLOG_ANALYSIS_RESULTテーブルに条件に一致するDtBloclogAnalysisResultが存在するか確認する
        /// </summary>
        /// <param name="logFileName">ログファイル名</param>
        /// <returns>存在する場合trueを、存在しない場合falseを返す</returns>
        public bool ExistDtBloclogAnalysisResult(string logFileName)
        {
            bool result = false;
            try
            {
                _logger.EnterJson("{0}", logFileName);

                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        result = db.DtBloclogAnalysisResult.Any(x => x.LogFileName == logFileName);
                    }
                });

                return result;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_BLOCLOG_ANALYSIS_RESULTテーブルのSelectに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", result);
            }
        }

        /// <summary>
        /// 指定日時より作成日が古いデータを削除する
        /// </summary>
        /// <param name="comparisonSourceDatetime">比較対象日時</param>
        /// <returns>削除数</returns>
        public int DeleteExceedsMonthsAllData(DateTime comparisonSourceDatetime)
        {
            int result = 0;
            try
            {
                _logger.EnterJson("{0}", new { comparisonSourceDatetime });

                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        // 作成日時から指定月数超過しているデータを抽出し、削除する
                        var targets = db.DtBloclogAnalysisResult.Where(x => x.CreateDatetime < comparisonSourceDatetime);
                        db.DtBloclogAnalysisResult.RemoveRange(targets);
                        result = db.SaveChanges();
                    }
                });

                return result;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_BLOCLOG_ANALYSIS_RESULTテーブルのDeleteに失敗しました。", e);
            }
            finally
            {
                _logger.Leave();
            }
        }
    }
}
