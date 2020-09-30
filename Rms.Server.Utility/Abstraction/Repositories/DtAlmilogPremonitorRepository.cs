using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Utility.Utility.Extensions;
using Rms.Server.Utility.Utility.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// DT_ALMILOG_PREMONITORテーブルのリポジトリ
    /// </summary>
    public partial class DtAlmilogPremonitorRepository : IDtAlmilogPremonitorRepository
    {
        /// <summary>ロガー</summary>
        private readonly ILogger<DtAlmilogPremonitorRepository> _logger;

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
        public DtAlmilogPremonitorRepository(ILogger<DtAlmilogPremonitorRepository> logger, ITimeProvider timePrivder, DBPolly dbPolly, AppSettings appSettings)
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
        /// DT_ALMILOG_PREMONITORテーブルからDtAlmilogPremonitorを取得する
        /// </summary>
        /// <param name="almilogAnalysisResult">アルミスロープログ解析結果</param>
        /// <param name="allowNotExist">取得件数が0件である場合を正常系とする場合はtrueを、異常系とする場合はfalseを指定する</param>
        /// <param name="allowMultiple">取得件数が2件以上である場合を正常系とする場合はtrueを、異常系とする場合はfalseを指定する</param>
        /// <returns>取得したデータ</returns>
        public IEnumerable<DtAlmilogPremonitor> ReadDtAlmilogPremonitor(DtAlmilogAnalysisResult almilogAnalysisResult, bool allowNotExist = true, bool allowMultiple = true)
        {
            IEnumerable<DtAlmilogPremonitor> models = null;
            try
            {
                _logger.EnterJson("{0}", almilogAnalysisResult);

                List<DBAccessor.Models.DtAlmilogPremonitor> entities = null;
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        entities = db.DtAlmilogPremonitor
                                     .Where(x => x.DetectorName == almilogAnalysisResult.DetectorName)
                                     .Where(x => x.JudgeResult == almilogAnalysisResult.AnalysisResult)
                                     .ToList();
                    }
                });

                if (!allowNotExist && (entities == null || entities.Count <= 0))
                {
                    var info = new { Sid = almilogAnalysisResult?.Sid, AnalysisResult = almilogAnalysisResult?.AnalysisResult, DetectorName = almilogAnalysisResult?.DetectorName };
                    throw new RmsException(string.Format("DT_ALMILOG_PREMONITORテーブルに該当レコードが存在しません。(アルミスロープログ解析結果: {0})", JsonConvert.SerializeObject(info)));
                }

                if (!allowMultiple && entities.Count > 1)
                {
                    var info = new { Sid = almilogAnalysisResult?.Sid, AnalysisResult = almilogAnalysisResult?.AnalysisResult, DetectorName = almilogAnalysisResult?.DetectorName };
                    throw new RmsException(string.Format("DT_ALMILOG_PREMONITORテーブルに該当レコードが複数あります。(アルミスロープログ解析結果: {0})", JsonConvert.SerializeObject(info)));
                }

                if (entities != null)
                {
                    models = entities.Select(x => x.ToModel());
                }

                return models;
            }
            catch (RmsException)
            {
                throw;
            }
            catch (Exception e)
            {
                var info = new { Sid = almilogAnalysisResult?.Sid, AnalysisResult = almilogAnalysisResult?.AnalysisResult, DetectorName = almilogAnalysisResult?.DetectorName };
                throw new RmsException(string.Format("DT_ALMILOG_PREMONITORテーブルのSelectに失敗しました。(アルミスロープログ解析結果: {0})", JsonConvert.SerializeObject(info)), e);
            }
            finally
            {
                _logger.LeaveJson("{0}", models);
            }
        }
    }
}
