using Microsoft.Extensions.Logging;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Models;
using Rms.Server.Operation.Utility.Extensions;
using Rms.Server.Operation.Utility.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Rms.Server.Operation.Abstraction.Repositories
{
    /// <summary>
    /// DT_INSTALL_BASEテーブルのリポジトリ
    /// </summary>
    public partial class DtInstallBaseRepository : IDtInstallBaseRepository
    {
        /// <summary>ロガー</summary>
        private readonly ILogger<DtInstallBaseRepository> _logger;

        /// <summary>DateTimeの提供元</summary>
        private readonly ITimeProvider _timeProvider;

        /// <summary>DB接続用のPolly</summary>
        private readonly DBPolly _dbPolly;

        /// <summary>アプリケーション設定</summary>
        private readonly AppSettings _appSettings;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="timeProvider">DateTimeの提供元</param>
        /// <param name="dbPolly">DB接続用のPolly</param>
        /// <param name="appSettings">アプリケーション設定</param>
        public DtInstallBaseRepository(ILogger<DtInstallBaseRepository> logger, ITimeProvider timeProvider, DBPolly dbPolly, AppSettings appSettings)
        {
            Assert.IfNull(logger);
            Assert.IfNull(timeProvider);
            Assert.IfNull(dbPolly);
            Assert.IfNull(appSettings);

            _logger = logger;
            _timeProvider = timeProvider;
            _dbPolly = dbPolly;
            _appSettings = appSettings;
        }
    }
}
