using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Extensions;
using Rms.Server.Utility.Service.Services;
using Rms.Server.Utility.Utility;
using System;

namespace Rms.Sever.Utility.Azure.Functions.ConnectionMonitorTest
{
    /// <summary>
    /// 通信監視アプリテスト
    /// </summary>
    public class ConnectionMonitorControllerTest
    {
        /// <summary>
        /// 設定
        /// </summary>
        private readonly UtilityAppSettings _settings;

        /// <summary>
        /// 親子間通信データテーブル監視サービス
        /// </summary>
        private readonly IParentChildrenConnectionMonitorService _parentChildrenConnectionService;

        /// <summary>
        /// 端末データテーブル監視サービス
        /// </summary>
        private readonly IDeviceConnectionMonitorService _deviceConnectionService;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">設定</param>
        /// <param name="parentChildrenConnectionService">親子間通信データテーブル監視サービスクラス</param>
        /// <param name="deviceConnectionService">端末データテーブル監視サービスクラス</param>
        public ConnectionMonitorControllerTest(UtilityAppSettings settings, IParentChildrenConnectionMonitorService parentChildrenConnectionService, IDeviceConnectionMonitorService deviceConnectionService)
        {
            Assert.IfNull(settings);
            Assert.IfNull(parentChildrenConnectionService);
            Assert.IfNull(deviceConnectionService);

            _settings = settings;
            _parentChildrenConnectionService = parentChildrenConnectionService;
            _deviceConnectionService = deviceConnectionService;
        }

        /// <summary>
        /// 通信監視テスト
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="log">ロガー</param>
        [FunctionName("ConnectionMonitorTest")]
        public void ConnectionMonitorTest([EventHubTrigger("ms-010", Connection = "ConnectionString")] string message, ILogger log)
        {
            log.EnterJson("{0}", message);
            try
            {
                var result = _parentChildrenConnectionService.ReadAlarmDefinition();
            }
            catch (Exception ex)
            {
                // [TODO]:Blobへクライアントメッセージを保存する処理を追加する(ControllerではなくServiceで実行する?)
                log.Error(ex, "想定外のエラー");
            }
            finally
            {
                log.LeaveJson("{0}", message);
            }
        }
    }
}
