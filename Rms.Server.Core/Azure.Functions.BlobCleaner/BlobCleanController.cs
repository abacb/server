using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Rms.Server.Core.Service.Services;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Extensions;
using Rms.Server.Core.Utility.Properties;
using System;

namespace Rms.Server.Core.Azure.Functions.BlobCleaner
{
    /// <summary>
    /// BlobCleanerのエントリポイント
    /// </summary>
    public class BlobCleanController
    {
        /// <summary>
        /// AppSettings
        /// </summary>
        private readonly AppSettings _settings;

        /// <summary>
        /// Service
        /// </summary>
        private readonly ICleanBlobService _service;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">設定</param>
        /// <param name="service">ICleanBlobServiceのインスタンス</param>
        public BlobCleanController(AppSettings settings, ICleanBlobService service)
        {
            Assert.IfNull(settings);
            Assert.IfNull(service);

            _settings = settings;
            _service = service;
        }

        /// <summary>
        /// CleanBlob処理を実行する
        /// </summary>
        /// <param name="myTimer">タイマー</param>
        /// <param name="log">ロガー</param>
        [FunctionName("BlobCleaner")]
        public void CleanBlob(
            // UTCで毎月7日12:00に実行(ローカルタイム計算で日付がずれないようにする)
            [TimerTrigger("0 0 12 7 * *")]TimerInfo myTimer,
            // [TimerTrigger("0 */1 * * * *")]TimerInfo myTimer,
            ILogger log)
        {
            log.EnterJson("{0}", myTimer);
            try
            {
                _service.Clean();
            }
            catch (Exception ex)
            {
                log.Error(ex, nameof(Resources.CO_BLC_BLC_001));
            }
            finally
            {
                log.LeaveJson("{0}", myTimer);
            }
        }
    }
}
