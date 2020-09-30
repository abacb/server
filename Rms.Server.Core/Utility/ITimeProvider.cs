using System;

namespace Rms.Server.Core.Utility
{
    /// <summary>
    /// DateTimeの提供元インターフェース
    /// </summary>
    public interface ITimeProvider
    {
        /// <summary>
        /// 現在時刻(localtime)
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        /// 現在時刻(Utc)
        /// </summary>
        DateTime UtcNow { get; }

        /// <summary>
        /// タイムゾーン
        /// </summary>
        TimeZoneInfo TimeZone { get; }
    }
}
