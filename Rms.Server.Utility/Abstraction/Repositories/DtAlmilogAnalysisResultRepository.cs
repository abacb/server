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
    /// DT_ALMILOG_ANALYSIS_RESULTテーブルのリポジトリ
    /// </summary>
    public partial class DtAlmilogAnalysisResultRepository : IDtAlmilogAnalysisResultRepository, Core.Abstraction.Repositories.ICleanRepository
    {
        /// <summary>ロガー</summary>
        private readonly ILogger<DtAlmilogAnalysisResultRepository> _logger;

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
        public DtAlmilogAnalysisResultRepository(ILogger<DtAlmilogAnalysisResultRepository> logger, ITimeProvider timePrivder, DBPolly dbPolly, AppSettings appSettings)
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
        /// 引数に指定したDtAlmilogAnalysisResultをDT_ALMILOG_ANALYSIS_RESULTテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <returns>処理結果</returns>
        public DtAlmilogAnalysisResult CreateDtAlmilogAnalysisResult(DtAlmilogAnalysisResult inData)
        {
            DtAlmilogAnalysisResult model = null;
            try
            {
                _logger.EnterJson("{0}", inData);

                DBAccessor.Models.DtAlmilogAnalysisResult entity = new DBAccessor.Models.DtAlmilogAnalysisResult(inData);

                // バリデーション
                Validator.ValidateObject(entity, new ValidationContext(entity, null, null));

                _dbPolly.Execute(() =>
                {
                    entity.CreateDatetime = _timePrivder.UtcNow;

                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        var dbdata = db.DtAlmilogAnalysisResult.Add(entity).Entity;
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
                throw new RmsException("DT_ALMILOG_ANALYSIS_RESULTテーブルへのInsertに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// DT_ALMILOG_ANALYSIS_RESULTテーブルに条件に一致するDtAlmilogAnalysisResultが存在するか確認する
        /// </summary>
        /// <param name="logFileName">ログファイル名</param>
        /// <returns>存在する場合trueを、存在しない場合falseを返す</returns>
        public bool ExistDtAlmilogAnalysisResult(string logFileName)
        {
            bool result = false;
            try
            {
                _logger.EnterJson("{0}", logFileName);

                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        result = db.DtAlmilogAnalysisResult.Any(x => x.LogFileName == logFileName);
                    }
                });

                return result;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_ALMILOG_ANALYSIS_RESULTテーブルのSelectに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", result);
            }
        }

        /// <summary>
        /// DT_ALMILOG_ANALYSIS_RESULTテーブルからアラーム判定の対象データを取得する
        /// 機器UIDとDetector名称単位で判定済みデータを連続NG数-1件、未判定データを全件取得する
        /// 取得したアラーム判定対象データは次の順番でListに格納する(連続した1次元のデータとして格納)
        ///   機器UID1、Detector名称1、判定済みデータ1、判定済みデータ2...、未判定データ1、未判定データ2...
        ///   機器UID1、Detector名称2、判定済みデータ1、...
        ///   機器UID2、Detector名称1、...
        /// </summary>
        /// <param name="alarmCountThreshold">アラーム通知閾値</param>
        /// <returns>取得したデータ</returns>
        public IEnumerable<DtAlmilogAnalysisResult> ReadAlarmJudgementTarget(int alarmCountThreshold)
        {
            IEnumerable<DtAlmilogAnalysisResult> models = null;
            try
            {
                _logger.Enter();

                List<DBAccessor.Models.DtAlmilogAnalysisResult> entities = null;
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        // アラーム判定フラグが未判定の情報を取得する
                        var alarmJudgementTarget = db.DtAlmilogAnalysisResult.Where(x => x.IsAlarmJudged == false);

                        if (alarmCountThreshold >= 2)
                        {
                            // アラーム判定フラグが判定済みの最新情報を機器UID、Detector名称単位で連続NG数-1件取得する
                            var confirmedAnalysisResult =
                                db.DtAlmilogAnalysisResult
                                    .GroupBy(x => new { x.EquipmentUid, x.DetectorName })
                                    .SelectMany(x => x
                                        .Where(y => y.IsAlarmJudged == true)
                                        .OrderByDescending(y => y.AlmilogMonth)
                                        .ThenByDescending(y => y.FileNameNo)
                                        .Take(alarmCountThreshold - 1));

                            alarmJudgementTarget = alarmJudgementTarget.Concat(confirmedAnalysisResult);
                        }

                        alarmJudgementTarget =
                            alarmJudgementTarget
                                .OrderBy(x => x.EquipmentUid)
                                .ThenBy(x => x.DetectorName)
                                .ThenByDescending(x => x.IsAlarmJudged)
                                .ThenBy(x => x.AlmilogMonth)
                                .ThenBy(x => x.FileNameNo);

                        entities = alarmJudgementTarget.ToList();
                    }
                });

                if (entities.Any())
                {
                    models = entities.Select(x => x.ToModel());
                }

                return models;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_ALMILOG_ANALYSIS_RESULTテーブルのSelectに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", models);
            }
        }

        /// <summary>
        /// 引数に指定したSIDのレコードを対象にDT_ALMILOG_ANALYSIS_RESULTテーブルのIS_ALARM_JUDGEDをTRUEに更新する
        /// </summary>
        /// <param name="sidList">更新対象レコードのSID</param>
        /// <returns>更新レコード数</returns>
        public int UpdateIsAlarmJudgedTrue(IEnumerable<long> sidList)
        {
            try
            {
                _logger.EnterJson("{0}", new { sidList });

                int result = 0;

                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        var targets = (from r in db.DtAlmilogAnalysisResult where sidList.Contains(r.Sid) select r).ToList();

                        foreach (var target in targets)
                        {
                            target.IsAlarmJudged = true;
                        }

                        result = db.SaveChanges();
                    }
                });

                return result;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_ALMILOG_ANALYSIS_RESULTテーブルのUpdateに失敗しました。", e);
            }
            finally
            {
                _logger.Leave();
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
                        var targets = db.DtAlmilogAnalysisResult.Where(x => x.CreateDatetime < comparisonSourceDatetime);
                        db.DtAlmilogAnalysisResult.RemoveRange(targets);
                        result = db.SaveChanges();
                    }
                });

                return result;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_ALMILOG_ANALYSIS_RESULTテーブルのDeleteに失敗しました。", e);
            }
            finally
            {
                _logger.Leave();
            }
        }
    }
}
