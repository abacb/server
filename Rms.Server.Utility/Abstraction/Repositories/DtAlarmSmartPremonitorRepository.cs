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
    /// DT_ALARM_SMART_PREMONITORテーブルのリポジトリ
    /// </summary>
    public partial class DtAlarmSmartPremonitorRepository : IDtAlarmSmartPremonitorRepository
    {
        /// <summary>ロガー</summary>
        private readonly ILogger<DtAlarmSmartPremonitorRepository> _logger;
    
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
        public DtAlarmSmartPremonitorRepository(ILogger<DtAlarmSmartPremonitorRepository> logger, ITimeProvider timePrivder, DBPolly dbPolly, AppSettings appSettings)
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
        /// DT_ALARM_SMART_PREMONITORテーブルからDtAlarmSmartPremonitorを取得する
        /// </summary>
        /// <param name="smartAttributeInfoId">取得するデータのSMART項目ID</param>
        /// <returns>取得したデータ</returns>
        public DtAlarmSmartPremonitor ReadDtAlarmSmartPremonitor(string smartAttributeInfoId)
        {
            DtAlarmSmartPremonitor model = null;
            try
            {
                _logger.EnterJson("{0}", new { smartAttributeInfoId });

                DBAccessor.Models.DtAlarmSmartPremonitor entity = null;
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        // 取得できるアラーム定義は1つのみという前提
                        entity = db.DtAlarmSmartPremonitor.FirstOrDefault(x => x.SmartId == smartAttributeInfoId);
                    }
                });
    
                if (entity != null)
                {
                    model = entity.ToModel();
                }
    
                return model;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_ALARM_SMART_PREMONITORテーブルのSelectに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }
    }
}
