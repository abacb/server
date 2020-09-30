using Microsoft.Extensions.Logging;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Utility;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// DT_INVENTORYテーブルのリポジトリ
    /// </summary>
    public partial class DtInventoryRepository : IDtInventoryRepository
    {
        /// <summary>ロガー</summary>
        private readonly ILogger<DtInventoryRepository> _logger;
    
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
        public DtInventoryRepository(ILogger<DtInventoryRepository> logger, ITimeProvider timePrivder, DBPolly dbPolly, AppSettings appSettings)
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
    }
}
