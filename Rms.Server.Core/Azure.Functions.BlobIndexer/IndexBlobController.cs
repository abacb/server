using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Rms.Server.Core.Service.Services;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Extensions;
using Rms.Server.Core.Utility.Properties;
using System;

namespace Rms.Server.Core.Azure.Functions.BlobIndexer
{
    /// <summary>
    /// BlobIndexerのエントリポイント
    /// </summary>
    public class IndexBlobController
    {
        /// <summary>
        /// AppSettings
        /// </summary>
        private readonly AppSettings _settings;

        /// <summary>
        /// Service
        /// </summary>
        private readonly IIndexBlobService _service;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">設定</param>
        /// <param name="service">Service</param>
        public IndexBlobController(
            AppSettings settings,
            IIndexBlobService service)
        {
            Assert.IfNull(settings);
            Assert.IfNull(service);

            _settings = settings;
            _service = service;
        }

        /// <summary>
        /// IndexBlobを実行する
        /// </summary>
        /// <param name="myTimer">タイマー</param>
        /// <param name="log">ロガー</param>
        [FunctionName("BlobIndexer")]
        public void IndexBlob(
            [TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, 
            ILogger log)
        {
            log.EnterJson("{0}", myTimer);

            try
            {
                _service.Index();
            }
            catch (Exception ex)
            {
                log.Error(ex, nameof(Resources.CO_BLI_BLI_001));
            }
            finally
            {
                log.LeaveJson("{0}", myTimer);
            }
        }
    }
}
