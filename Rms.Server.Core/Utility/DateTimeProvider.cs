using System;

namespace Rms.Server.Core.Utility
{
    // [c\# \- DateTime provider to using and mocking \- Stack Overflow](https://stackoverflow.com/questions/31716771/datetime-provider-to-using-and-mocking)

    /// <summary>
    /// DateTimeの提供元
    /// </summary>
    public class DateTimeProvider : ITimeProvider
    {
        /// <summary>
        /// 現在時刻(localtime)を取得する
        /// </summary>
        public virtual DateTime Now => DateTime.Now;

        /// <summary>
        /// 現在時刻(utc)を取得する
        /// </summary>
        public virtual DateTime UtcNow => DateTime.UtcNow;

        /// <summary>
        /// タイムゾーンを取得する
        /// </summary>
        public virtual TimeZoneInfo TimeZone => TimeZoneInfo.Local;
    }
}
