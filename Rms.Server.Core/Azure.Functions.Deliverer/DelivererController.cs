using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Rms.Server.Core.Service.Services;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Extensions;
using Rms.Server.Core.Utility.Properties;
using System;

namespace Rms.Server.Core.Azure.Functions.Deliverer
{
    /// <summary>
    /// Delivererの定期実行
    /// </summary>
    public class DelivererController
    {
        /// <summary>
        /// Service
        /// </summary>
        private readonly IDelivererService _service;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="service">ICleanDbServiceのインスタンス</param>
        public DelivererController(IDelivererService service)
        {
            Assert.IfNull(service);

            _service = service;
        }

        /// <summary>
        /// Deliverer処理を実行する
        /// </summary>
        /// <param name="myTimer">タイマー</param>
        /// <param name="log">ロガー</param>
        [FunctionName("Deliverer")]
        public void Delivery(
            //// 毎時1分に定期実行
            [TimerTrigger("1 */1 * * *")]TimerInfo myTimer,
            ILogger log)
        {
            log.EnterJson("{0}", myTimer);
            try
            {
                // Sq1: 定期実行
                _service.StartDelivery();
            }
            catch (Exception ex)
            {
                log.Error(ex, nameof(Resources.CO_DLV_DLV_001));
            }
            finally
            {
                log.LeaveJson("{0}", myTimer);
            }
        }
    }
}
