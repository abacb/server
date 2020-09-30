using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Rms.Server.Core.Service.Services;
using Rms.Server.Core.Utility;
using Rms.Server.Utility.Utility.Extensions;
using Rms.Server.Utility.Utility.Properties;
using System;

namespace Rms.Server.Utility.Azure.Functions.DbCleaner
{
    /// <summary>
    /// DbCleanerのWebAPI
    /// </summary>
    public class DbCleanController
    {
        /// <summary>
        /// Service
        /// </summary>
        private readonly ICleanDbService _service;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="service">ICleanDbServiceのインスタンス</param>
        public DbCleanController(ICleanDbService service)
        {
            Assert.IfNull(service);

            _service = service;
        }

        /// <summary>
        /// CleanDb処理を実行する
        /// </summary>
        /// <param name="myTimer">タイマー</param>
        /// <param name="log">ロガー</param>
        [FunctionName("DbCleaner")]
        public void CleanDb(
            [TimerTrigger("0 0 12 7 * *")]TimerInfo myTimer, // UTCで毎月7日12:00に実行(ローカルタイム計算で日付がずれないようにする)
            ILogger log)
        {
            log.EnterJson("{0}", myTimer);
            try
            {
                _service.Clean();
            }
            catch (Exception ex)
            {
                log.Error(ex, nameof(Resources.UT_DBC_DBC_001));
            }
            finally
            {
                log.LeaveJson("{0}", myTimer);
            }
        }
    }
}
