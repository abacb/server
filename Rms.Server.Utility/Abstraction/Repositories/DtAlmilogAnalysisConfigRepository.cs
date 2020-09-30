using Microsoft.Extensions.Logging;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Utility.Utility.Extensions;
using Rms.Server.Utility.Utility.Models;
using System;
using System.Linq;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// DT_ALMILOG_ANALYSIS_CONFIGテーブルのリポジトリ
    /// </summary>
    public partial class DtAlmilogAnalysisConfigRepository : IDtAlmilogAnalysisConfigRepository
    {
        /// <summary>ロガー</summary>
        private readonly ILogger<DtAlmilogAnalysisConfigRepository> _logger;

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
        public DtAlmilogAnalysisConfigRepository(ILogger<DtAlmilogAnalysisConfigRepository> logger, ITimeProvider timePrivder, DBPolly dbPolly, AppSettings appSettings)
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
        /// DT_ALMILOG_ANALYSIS_CONFIGテーブルからDtAlmilogAnalysisConfigを取得する
        /// </summary>
        /// <param name="detectorName">Detector名称</param>
        /// <param name="isNormalized">規格化フラグ</param>
        /// <param name="allowNotExist">取得件数が0件である場合を正常系とする場合はtrueを、異常系とする場合はfalseを指定する</param>
        /// <returns>取得したデータ</returns>
        public DtAlmilogAnalysisConfig ReadDtAlmilogAnalysisConfig(string detectorName, bool isNormalized, bool allowNotExist = true)
        {
            DtAlmilogAnalysisConfig model = null;
            try
            {
                _logger.EnterJson("{0}", new { detectorName, isNormalized });

                DBAccessor.Models.DtAlmilogAnalysisConfig entity = null;
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        entity = db.DtAlmilogAnalysisConfig.FirstOrDefault(x => x.DetectorName == detectorName && x.IsNormalized == isNormalized);
                    }
                });

                if (entity != null)
                {
                    model = entity.ToModel();
                }
                else
                {
                    if (!allowNotExist)
                    {
                        throw new RmsException("DT_ALMILOG_ANALYSIS_CONFIGテーブルに該当レコードが存在しません。");
                    }
                }

                return model;
            }
            catch (RmsException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_ALMILOG_ANALYSIS_CONFIGテーブルのSelectに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }
    }
}
