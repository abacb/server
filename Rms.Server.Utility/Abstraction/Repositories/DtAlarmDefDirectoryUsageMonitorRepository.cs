using Microsoft.Extensions.Logging;
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
    /// DT_ALARM_DEF_DIRECTORY_USAGE_MONITORテーブルのリポジトリ
    /// </summary>
    public partial class DtAlarmDefDirectoryUsageMonitorRepository : IDtAlarmDefDirectoryUsageMonitorRepository
    {
        /// <summary>ロガー</summary>
        private readonly ILogger<DtAlarmDefDirectoryUsageMonitorRepository> _logger;

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
        public DtAlarmDefDirectoryUsageMonitorRepository(ILogger<DtAlarmDefDirectoryUsageMonitorRepository> logger, ITimeProvider timePrivder, DBPolly dbPolly, AppSettings appSettings)
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
        /// DT_ALARM_DEF_DIRECTORY_USAGE_MONITORテーブルからDtAlarmDefDirectoryUsageMonitorを取得する
        /// </summary>
        /// <param name="directoryUsage">ディレクトリ使用量</param>
        /// <returns>取得したデータ</returns>
        public IEnumerable<DtAlarmDefDirectoryUsageMonitor> ReadDtAlarmDefDirectoryUsageMonitor(DirectoryUsage directoryUsage)
        {
            var models = new List<DtAlarmDefDirectoryUsageMonitor>();
            try
            {
                _logger.EnterJson("{0}", directoryUsage);

                List<DBAccessor.Models.DtAlarmDefDirectoryUsageMonitor> entities = null;
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        var query = db.DtAlarmDefDirectoryUsageMonitor
                        .Where(x => string.IsNullOrEmpty(x.TypeCode) || x.TypeCode == directoryUsage.TypeCode);

                        foreach (var detailInfo in directoryUsage.DetailInfo)
                        {
                            query = query.Where(x => string.IsNullOrEmpty(x.DirectoryPath) || x.DirectoryPath == detailInfo.FullPath)
                                         .Where(x => x.Size < detailInfo.Size);
                        }

                        entities = query.ToList();

                        // 検索されたレコードと検索条件の紐づけ(アラーム説明の文字列置換で必要となるため)
                        foreach (var detailInfo in directoryUsage.DetailInfo)
                        {
                            var filteredEntities = entities.Where(x => string.IsNullOrEmpty(x.DirectoryPath) || x.DirectoryPath == detailInfo.FullPath)
                                                           .Where(x => x.Size < detailInfo.Size);

                            // 同一条件で複数検索された場合を考慮(アラーム定義のミスだがそのまま処理してアラーム情報を複数作成する仕様)
                            foreach (var entity in filteredEntities)
                            {
                                DtAlarmDefDirectoryUsageMonitor model = entity.ToModel();
                                model.DirectoryUsage = detailInfo.Size;
                                models.Add(model);
                            }
                        }
                    }
                });

                return models;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_ALARM_DEF_CALIBRATION_PREMONITORテーブルのSelectに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", models);
            }
        }
    }
}
