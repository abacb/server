using System;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// ICleanRepository
    /// </summary>
    public interface ICleanRepository
    {
        /// <summary>
        /// 指定日時より作成日が古い非最新データを削除する
        /// </summary>
        /// <param name="comparisonSourceDatetime">比較対象日時</param>
        /// <returns>削除数</returns>
        int DeleteExceedsMonthsAllData(DateTime comparisonSourceDatetime);
    }
}
