using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Rms.Server.Core.Utility;

namespace Rms.Server.Utility.DBAccessor.Models
{
    /// <summary>
    /// DBコンテキスト
    /// </summary>
    public partial class RmsDbContext : DbContext
    {
        /// <summary>
        /// ログファクトリ
        /// </summary>
        public static readonly LoggerFactory LoggerFactory = new LoggerFactory(new[] { new DebugLoggerProvider((category, level) => category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information) });

        /// <summary>アプリケーション設定</summary>
        private readonly AppSettings _appSettings;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="appSettings">アプリケーション設定</param>
        public RmsDbContext(AppSettings appSettings)
        {
            this._appSettings = appSettings;
        }
    }
}
