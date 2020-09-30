using Microsoft.Extensions.Logging;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Core.Utility.Extensions;
using Rms.Server.Core.Utility.Models.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// DT_STORAGE_CONFIGテーブルのリポジトリ
    /// </summary>
    public partial class DtStorageConfigRepository : IDtStorageConfigRepository
    {
        /// <summary>ロガー</summary>
        private readonly ILogger<DtStorageConfigRepository> _logger;
    
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
        public DtStorageConfigRepository(ILogger<DtStorageConfigRepository> logger, ITimeProvider timePrivder, DBPolly dbPolly, AppSettings appSettings)
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
        /// DT_STORAGE_CONFIGテーブルから全レコードを取得する
        /// </summary>
        /// <returns>ストレージ設定データリスト</returns>
        /// <remarks>
        /// テーブルからデータを取得できなかった場合にはnuilを返す
        /// </remarks>
        public List<DtStorageConfig> ReadDtStorageConfigs()
        {
            List<DtStorageConfig> models = null;
            try
            {
                _logger.Enter("ReadDtStorageConfigs");

                IQueryable<DBAccessor.Models.DtStorageConfig> entities = null;
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        // 全件取得
                        entities = db.DtStorageConfig.Select(x => x);
                    }
                });

                if (entities != null && entities.Count() > 0)
                {
                    models = new List<DtStorageConfig>();
                    foreach (DBAccessor.Models.DtStorageConfig config in entities)
                    {
                        DtStorageConfig model = config.ToModel();
                        models.Add(model);
                    }
                }

                return models;
            }
            catch (ValidationException e)
            {
                throw new RmsParameterException(e.ValidationResult.ErrorMessage, e);
            }
            catch (Exception e)
            {
                throw new RmsException("DT_STORAGE_CONFIGテーブルのSelectに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", models);
            }
        }
    }
}
