using Microsoft.EntityFrameworkCore;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Core.Utility.Extensions;
using System;
using System.Data.SqlClient;

namespace Rms.Server.Core.Abstraction.Repositories
{
    public partial class DtDiskDriveRepository : ICleanRepository
    {
        /// <summary>
        /// 指定日時より作成日が古い非最新データを削除する
        /// </summary>
        /// <param name="comparisonSourceDatetime">比較対象日時</param>
        /// <returns>削除数</returns>
        public int DeleteExceedsMonthsAllData(DateTime comparisonSourceDatetime)
        {
            int result = 0;
            try
            {
                _logger.Enter($"{nameof(comparisonSourceDatetime)}={comparisonSourceDatetime}");

                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        var CollectDatetime = new SqlParameter("CollectDatetime", comparisonSourceDatetime);

                        // 収集日時から指定月数超過しているデータを抽出し、削除する
                        var targets = db.DtDiskDrive
                        .FromSql(
                            @"
                            Select
                                *
                            From
                                core.DT_DISK_DRIVE tbl1
                            Where
                                tbl1.COLLECT_DATETIME < @CollectDatetime
                                and
                                tbl1.COLLECT_DATETIME <> (SELECT MAX(tbl2.COLLECT_DATETIME) FROM core.DT_DISK_DRIVE tbl2 Where tbl1.DEVICE_SID = tbl2.DEVICE_SID)",
                            CollectDatetime);

                        db.DtDiskDrive.RemoveRange(targets);
                        result = db.SaveChanges();
                    }
                });

                return result;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_DISK_DRIVEテーブルのDeleteに失敗しました。", e);
            }
            finally
            {
                _logger.Leave($"{nameof(result)}={result}");
            }
        }
    }
}
