using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rms.Server.Core.Azure.Functions.Dispatcher.Models;
using Rms.Server.Core.Service.Services;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Extensions;
using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Utility.Models.Dispatch;
using Rms.Server.Core.Utility.Properties;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rms.Server.Core.Azure.Functions.Dispatcher
{
    /// <summary>
    /// DispatchController
    /// </summary>
    /// <remarks>
    /// Dispatcherは複数のイベントを受信することを前提に実装する。
    /// 一部の特殊な例を除き、以下の順序で処理を行う。
    /// (1) Event Hubのスキーマをstring型で受け取る。
    ///     このとき、stringにはEvent Hubのスキーマ全体ではなく、
    ///     Bodyプロパティが文字列として格納されることに注意する。
    /// (2) Bodyプロパティをデシリアライズして、イベントクラスの配列に変換する。
    ///     BodyプロパティはJSON配列であることに注意する。
    ///     デシリアライズに失敗した場合には、
    ///     エラーとしてBodyプロパティ自体をBlobストレージに保存する。  
    /// (3) イベントクラス配列の要素ごとに、Serviceに投げて処理を行う。
    ///     エラーが発生した場合には要素ごとにエラーをBlobストレージに保存する。
    ///     Bodyプロパティを配列ごとまとめてBlobに保存しないのは、
    ///     要素ごとにエラーが発生する/しないという状況が想定され、
    ///     正常なイベントもエラー扱いすることを防ぐためである。
    ///     Blobに保存するBodyプロパティについては、以下に注意する。
    ///     (*) Blobストレージには、エラーが発生したBodyプロパティの1要素のみを格納した
    ///         JSON配列文字列を保存する。これは、Blobストレージから取り出したデータと、
    ///         Bodyプロパティを同一のスキーマ（JSON配列）で扱うためである。
    /// </remarks>
    public class DispatchController
    {
        /// <summary>
        /// 設定
        /// </summary>
        private readonly AppSettings _settings;

        /// <summary>
        /// ディスパッチ用サービスクラス
        /// </summary>
        private readonly IDispatchService _service;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">設定</param>
        /// <param name="service">IDispatchService</param>
        public DispatchController(
            AppSettings settings,
            IDispatchService service)
        {
            Assert.IfNull(settings);
            Assert.IfNull(service);

            _settings = settings;
            _service = service;
        }

        #region Controllers

        /// <summary>
        /// DispatchDxaBillLog
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <param name="log">ロガー</param>
        [FunctionName(nameof(DispatchDxaBillLog))]
        public void DispatchDxaBillLog(
            [EventHubTrigger(Const.MessageSchemaId.MS014, Connection = Const.ConnectionString.EventHubsConnectionStringMs014)] string eventData,
            ILogger log)
        {
            log.EnterJson("EventData: {0}", new object[] { eventData });
            Result result = null;
            DispatchedEvent[] dispatchedEvents = null;
            void actionIfFailedWhenStoreUnexpected(Exception ex, string json) => log.Error(ex, nameof(Resources.CO_DSP_DDB_003), new object[] { json });

            try
            {
                try
                {
                    dispatchedEvents = GetDispatchedEvents(eventData, log);
                }
                catch (Exception e)
                {
                    // イベント情報が正常に読み解けなかった場合
                    log.Error(e, nameof(Resources.CO_DSP_DDB_006));
                    StoreUnexpectedEvent(Const.MessageSchemaId.MS014, eventData, actionIfFailedWhenStoreUnexpected);
                    return;
                }

                foreach (DispatchedEvent dispatchedEvent in dispatchedEvents)
                {
                    var messageId = dispatchedEvent?.MessageId;
                    try
                    {
                        RmsEvent rmsEvent = ConvertEventDataToRmsEvent(dispatchedEvent);

                        if (rmsEvent == null)
                        {
                            // イベントに必要なデータが不足している場合
                            log.Error(nameof(Resources.CO_DSP_DDB_006));
                            StoreUnexpectedEvent(Const.MessageSchemaId.MS014, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                            continue;
                        }

                        result = _service.StoreDxaBillLog(rmsEvent);

                        if (!result.IsSuccess())
                        {
                            StoreUnexpectedEvent(Const.MessageSchemaId.MS014, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error(e, nameof(Resources.CO_DSP_DDB_001));
                        StoreUnexpectedEvent(Const.MessageSchemaId.MS014, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                    }
                }
            }
            finally
            {
                log.LeaveJson("Response: {0}", result);
            }
        }

        /// <summary>
        /// DispatchDxaqcLog 
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <param name="log">ロガー</param>
        [FunctionName(nameof(DispatchDxaqcLog))]
        public void DispatchDxaqcLog(
            [EventHubTrigger(Const.MessageSchemaId.MS015, Connection = Const.ConnectionString.EventHubsConnectionStringMs015)] string eventData,
            ILogger log)
        {
            log.EnterJson("EventData: {0}", new object[] { eventData });
            Result result = null;
            DispatchedEvent[] dispatchedEvents = null;
            void actionIfFailedWhenStoreUnexpected(Exception ex, string json) => log.Error(ex, nameof(Resources.CO_DSP_DDL_003), new object[] { json });

            try
            {
                try
                {
                    dispatchedEvents = GetDispatchedEvents(eventData, log);
                }
                catch (Exception e)
                {
                    // イベント情報が正常に読み解けなかった場合
                    log.Error(e, nameof(Resources.CO_DSP_DDL_006));
                    StoreUnexpectedEvent(Const.MessageSchemaId.MS015, eventData, actionIfFailedWhenStoreUnexpected);
                    return;
                }

                foreach (DispatchedEvent dispatchedEvent in dispatchedEvents)
                {
                    var messageId = dispatchedEvent?.MessageId;
                    try
                    {
                        RmsEvent rmsEvent = ConvertEventDataToRmsEvent(dispatchedEvent);

                        if (rmsEvent == null)
                        {
                            // イベントに必要なデータが不足している場合
                            log.Error(nameof(Resources.CO_DSP_DDL_006));
                            StoreUnexpectedEvent(Const.MessageSchemaId.MS015, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                            continue;
                        }

                        result = _service.StoreDxaQcLog(rmsEvent);

                        if (!result.IsSuccess())
                        {
                            StoreUnexpectedEvent(Const.MessageSchemaId.MS015, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error(e, nameof(Resources.CO_DSP_DDL_001));
                        StoreUnexpectedEvent(Const.MessageSchemaId.MS015, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                    }
                }
            }
            finally
            {
                log.LeaveJson("Response: {0}", result);
            }
        }

        /// <summary>
        /// DispatchInstallResult
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <param name="log">ロガー</param>
        [FunctionName(nameof(DispatchInstallResult))]
        public void DispatchInstallResult(
            [EventHubTrigger(Const.MessageSchemaId.MS016, Connection = Const.ConnectionString.EventHubsConnectionStringMs016)] string eventData,
            ILogger log)
        {
            log.EnterJson("EventData: {0}", new object[] { eventData });
            Result result = null;
            DispatchedEvent[] dispatchedEvents = null;
            void actionIfFailedWhenStoreUnexpected(Exception ex, string json) => log.Error(ex, nameof(Resources.CO_DSP_DIR_003), new object[] { json });

            try
            {
                try
                {
                    dispatchedEvents = GetDispatchedEvents(eventData, log);
                }
                catch (Exception e)
                {
                    // イベント情報が正常に読み解けなかった場合
                    log.Error(e, nameof(Resources.CO_DSP_DIR_006));
                    StoreUnexpectedEvent(Const.MessageSchemaId.MS016, eventData, actionIfFailedWhenStoreUnexpected);
                    return;
                }

                foreach (DispatchedEvent dispatchedEvent in dispatchedEvents)
                {
                    var messageId = dispatchedEvent?.MessageId;
                    try
                    {
                        RmsEvent rmsEvent = ConvertEventDataToRmsEvent(dispatchedEvent);

                        if (rmsEvent == null)
                        {
                            // イベントに必要なデータが不足している場合
                            log.Error(nameof(Resources.CO_DSP_DIR_006));
                            StoreUnexpectedEvent(Const.MessageSchemaId.MS016, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                            continue;
                        }

                        result = _service.StoreInstallResult(rmsEvent);

                        if (!result.IsSuccess())
                        {
                            StoreUnexpectedEvent(Const.MessageSchemaId.MS016, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error(e, nameof(Resources.CO_DSP_DIR_001));
                        StoreUnexpectedEvent(Const.MessageSchemaId.MS016, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                    }
                }
            }
            finally
            {
                log.LeaveJson("Response: {0}", result);
            }
        }

        /// <summary>
        /// DispatchSoftVersion
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <param name="log">ロガー</param>
        [FunctionName(nameof(DispatchSoftVersion))]
        public void DispatchSoftVersion(
            [EventHubTrigger(Const.MessageSchemaId.MS025, Connection = Const.ConnectionString.EventHubsConnectionStringMs025)] string eventData,
            ILogger log)
        {
            log.EnterJson("EventData: {0}", new object[] { eventData });
            Result result = null;
            DispatchedEvent[] dispatchedEvents = null;
            void actionIfFailedWhenStoreUnexpected(Exception ex, string json) => log.Error(ex, nameof(Resources.CO_DSP_DSV_003), new object[] { json });

            try
            {
                try
                {
                    dispatchedEvents = GetDispatchedEvents(eventData, log);
                }
                catch (Exception e)
                {
                    // イベント情報が正常に読み解けなかった場合
                    log.Error(e, nameof(Resources.CO_DSP_DSV_006));
                    StoreUnexpectedEvent(Const.MessageSchemaId.MS025, eventData, actionIfFailedWhenStoreUnexpected);
                    return;
                }

                foreach (DispatchedEvent dispatchedEvent in dispatchedEvents)
                {
                    var messageId = dispatchedEvent?.MessageId;
                    try
                    {
                        RmsEvent rmsEvent = ConvertEventDataToRmsEvent(dispatchedEvent);

                        if (rmsEvent == null)
                        {
                            // イベントに必要なデータが不足している場合
                            log.Error(nameof(Resources.CO_DSP_DSV_006));
                            StoreUnexpectedEvent(Const.MessageSchemaId.MS025, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                            continue;
                        }

                        result = _service.StoreSoftVersion(rmsEvent);

                        if (!result.IsSuccess())
                        {
                            StoreUnexpectedEvent(Const.MessageSchemaId.MS025, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error(e, nameof(Resources.CO_DSP_DSV_001));
                        StoreUnexpectedEvent(Const.MessageSchemaId.MS025, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                    }
                }
            }
            finally
            {
                log.LeaveJson("Response: {0}", result);
            }
        }

        /// <summary>
        /// DispatchDirectoryUsage
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <param name="log">ロガー</param>
        [FunctionName(nameof(DispatchDirectoryUsage))]
        public void DispatchDirectoryUsage(
            [EventHubTrigger(Const.MessageSchemaId.MS026, Connection = Const.ConnectionString.EventHubsConnectionStringMs026)] string eventData,
            ILogger log)
        {
            log.EnterJson("EventData: {0}", new object[] { eventData });
            Result result = null;
            DispatchedEvent[] dispatchedEvents = null;
            void actionIfFailedWhenStoreUnexpected(Exception ex, string json) => log.Error(ex, nameof(Resources.CO_DSP_DDU_003), new object[] { json });

            try
            {
                try
                {
                    dispatchedEvents = GetDispatchedEvents(eventData, log);
                }
                catch (Exception e)
                {
                    // イベント情報が正常に読み解けなかった場合
                    log.Error(e, nameof(Resources.CO_DSP_DDU_006));
                    StoreUnexpectedEvent(Const.MessageSchemaId.MS026, eventData, actionIfFailedWhenStoreUnexpected);
                    return;
                }

                foreach (DispatchedEvent dispatchedEvent in dispatchedEvents)
                {
                    var messageId = dispatchedEvent?.MessageId;
                    try
                    {
                        RmsEvent rmsEvent = ConvertEventDataToRmsEvent(dispatchedEvent);

                        if (rmsEvent == null)
                        {
                            // イベントに必要なデータが不足している場合
                            log.Error(nameof(Resources.CO_DSP_DDU_006));
                            StoreUnexpectedEvent(Const.MessageSchemaId.MS026, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                            continue;
                        }

                        result = _service.StoreDirectoryUsage(rmsEvent);

                        if (!result.IsSuccess())
                        {
                            StoreUnexpectedEvent(Const.MessageSchemaId.MS026, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error(e, nameof(Resources.CO_DSP_DDU_001));
                        StoreUnexpectedEvent(Const.MessageSchemaId.MS026, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                    }
                }
            }
            finally
            {
                log.LeaveJson("Response: {0}", result);
            }
        }

        /// <summary>
        /// DispatchDiskDrive
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <param name="log">ロガー</param>
        [FunctionName(nameof(DispatchDiskDrive))]
        public void DispatchDiskDrive(
            [EventHubTrigger(Const.MessageSchemaId.MS027, Connection = Const.ConnectionString.EventHubsConnectionStringMs027)] string eventData,
            ILogger log)
        {
            log.EnterJson("EventData: {0}", new object[] { eventData });
            Result result = null;
            DispatchedEvent[] dispatchedEvents = null;
            void actionIfFailedWhenStoreUnexpected(Exception ex, string json) => log.Error(ex, nameof(Resources.CO_DSP_DDD_003), new object[] { json });

            try
            {
                try
                {
                    dispatchedEvents = GetDispatchedEvents(eventData, log);
                }
                catch (Exception e)
                {
                    // イベント情報が正常に読み解けなかった場合
                    log.Error(e, nameof(Resources.CO_DSP_DDD_006));
                    StoreUnexpectedEvent(Const.MessageSchemaId.MS027, eventData, actionIfFailedWhenStoreUnexpected);
                    return;
                }

                foreach (DispatchedEvent dispatchedEvent in dispatchedEvents)
                {
                    var messageId = dispatchedEvent?.MessageId;
                    try
                    {
                        RmsEvent rmsEvent = ConvertEventDataToRmsEvent(dispatchedEvent);

                        if (rmsEvent == null)
                        {
                            // イベントに必要なデータが不足している場合
                            log.Error(nameof(Resources.CO_DSP_DDD_006));
                            StoreUnexpectedEvent(Const.MessageSchemaId.MS027, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                            continue;
                        }

                        result = _service.StoreDiskDrive(rmsEvent);

                        if (!result.IsSuccess())
                        {
                            StoreUnexpectedEvent(Const.MessageSchemaId.MS027, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error(e, nameof(Resources.CO_DSP_DDD_001));
                        StoreUnexpectedEvent(Const.MessageSchemaId.MS027, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                    }
                }
            }
            finally
            {
                log.LeaveJson("Response: {0}", result);
            }
        }

        /// <summary>
        /// DispatchEquipmentUsage
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <param name="log">ロガー</param>
        [FunctionName(nameof(DispatchEquipmentUsage))]
        public void DispatchEquipmentUsage(
            [EventHubTrigger(Const.MessageSchemaId.MS028, Connection = Const.ConnectionString.EventHubsConnectionStringMs028)] string eventData,
            ILogger log)
        {
            log.EnterJson("EventData: {0}", new object[] { eventData });
            Result result = null;
            DispatchedEvent[] dispatchedEvents = null;
            void actionIfFailedWhenStoreUnexpected(Exception ex, string json) => log.Error(ex, nameof(Resources.CO_DSP_DEU_003), new object[] { json });

            try
            {
                try
                {
                    dispatchedEvents = GetDispatchedEvents(eventData, log);
                }
                catch (Exception e)
                {
                    // イベント情報が正常に読み解けなかった場合
                    log.Error(e, nameof(Resources.CO_DSP_DEU_006));
                    StoreUnexpectedEvent(Const.MessageSchemaId.MS028, eventData, actionIfFailedWhenStoreUnexpected);
                    return;
                }

                foreach (DispatchedEvent dispatchedEvent in dispatchedEvents)
                {
                    var messageId = dispatchedEvent?.MessageId;
                    try
                    {
                        RmsEvent rmsEvent = ConvertEventDataToRmsEvent(dispatchedEvent);

                        if (rmsEvent == null)
                        {
                            // イベントに必要なデータが不足している場合
                            log.Error(nameof(Resources.CO_DSP_DEU_006));
                            StoreUnexpectedEvent(Const.MessageSchemaId.MS028, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                            continue;
                        }

                        result = _service.StoreEquipmentUsage(rmsEvent);

                        if (!result.IsSuccess())
                        {
                            StoreUnexpectedEvent(Const.MessageSchemaId.MS028, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error(e, nameof(Resources.CO_DSP_DEU_001));
                        StoreUnexpectedEvent(Const.MessageSchemaId.MS028, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                    }
                }
            }
            finally
            {
                log.LeaveJson("Response: {0}", result);
            }
        }

        /// <summary>
        /// DispatchInventory
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <param name="log">ロガー</param>
        [FunctionName(nameof(DispatchInventory))]
        public void DispatchInventory(
            [EventHubTrigger(Const.MessageSchemaId.MS029, Connection = Const.ConnectionString.EventHubsConnectionStringMs029)] string eventData,
            ILogger log)
        {
            log.EnterJson("EventData: {0}", new object[] { eventData });
            Result result = null;
            DispatchedEvent[] dispatchedEvents = null;
            void actionIfFailedWhenStoreUnexpected(Exception ex, string json) => log.Error(ex, nameof(Resources.CO_DSP_DII_003), new object[] { json });

            try
            {
                try
                {
                    dispatchedEvents = GetDispatchedEvents(eventData, log);
                }
                catch (Exception e)
                {
                    // イベント情報が正常に読み解けなかった場合
                    log.Error(e, nameof(Resources.CO_DSP_DII_006));
                    StoreUnexpectedEvent(Const.MessageSchemaId.MS029, eventData, actionIfFailedWhenStoreUnexpected);
                    return;
                }

                foreach (DispatchedEvent dispatchedEvent in dispatchedEvents)
                {
                    var messageId = dispatchedEvent?.MessageId;
                    try
                    {
                        RmsEvent rmsEvent = ConvertEventDataToRmsEvent(dispatchedEvent);

                        if (rmsEvent == null)
                        {
                            // イベントに必要なデータが不足している場合
                            log.Error(nameof(Resources.CO_DSP_DII_006));
                            StoreUnexpectedEvent(Const.MessageSchemaId.MS029, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                            continue;
                        }

                        result = _service.StoreInventory(rmsEvent);

                        if (!result.IsSuccess())
                        {
                            StoreUnexpectedEvent(Const.MessageSchemaId.MS029, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error(e, nameof(Resources.CO_DSP_DII_001));
                        StoreUnexpectedEvent(Const.MessageSchemaId.MS029, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                    }
                }
            }
            finally
            {
                log.LeaveJson("Response: {0}", result);
            }
        }

        /// <summary>
        /// DispatchDrive
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <param name="log">ロガー</param>
        [FunctionName(nameof(DispatchDrive))]
        public void DispatchDrive(
            [EventHubTrigger(Const.MessageSchemaId.MS031, Connection = Const.ConnectionString.EventHubsConnectionStringMs031)] string eventData,
            ILogger log)
        {
            log.EnterJson("EventData: {0}", new object[] { eventData });
            Result result = null;
            DispatchedEvent[] dispatchedEvents = null;
            void actionIfFailedWhenStoreUnexpected(Exception ex, string json) => log.Error(ex, nameof(Resources.CO_DSP_DID_003), new object[] { json });

            try
            {
                try
                {
                    dispatchedEvents = GetDispatchedEvents(eventData, log);
                }
                catch (Exception e)
                {
                    // イベント情報が正常に読み解けなかった場合
                    log.Error(e, nameof(Resources.CO_DSP_DID_006));
                    StoreUnexpectedEvent(Const.MessageSchemaId.MS031, eventData, actionIfFailedWhenStoreUnexpected);
                    return;
                }

                foreach (DispatchedEvent dispatchedEvent in dispatchedEvents)
                {
                    var messageId = dispatchedEvent?.MessageId;
                    try
                    {
                        RmsEvent rmsEvent = ConvertEventDataToRmsEvent(dispatchedEvent);

                        if (rmsEvent == null)
                        {
                            // イベントに必要なデータが不足している場合
                            log.Error(nameof(Resources.CO_DSP_DID_006));
                            StoreUnexpectedEvent(Const.MessageSchemaId.MS031, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                            continue;
                        }

                        result = _service.StoreDrive(rmsEvent);

                        if (!result.IsSuccess())
                        {
                            StoreUnexpectedEvent(Const.MessageSchemaId.MS031, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error(e, nameof(Resources.CO_DSP_DID_001));
                        StoreUnexpectedEvent(Const.MessageSchemaId.MS031, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                    }
                }
            }
            finally
            {
                log.LeaveJson("Response: {0}", result);
            }
        }

        /// <summary>
        /// DispatchPlusServiceBillLog
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <param name="log">ロガー</param>
        [FunctionName(nameof(DispatchPlusServiceBillLog))]
        public void DispatchPlusServiceBillLog(
            [EventHubTrigger(Const.MessageSchemaId.MS011, Connection = Const.ConnectionString.EventHubsConnectionStringMs011)] string eventData,
            ILogger log)
        {
            log.EnterJson("EventData: {0}", new object[] { eventData });
            Result result = null;
            DispatchedEvent[] dispatchedEvents = null;
            void actionIfFailedWhenStoreUnexpected(Exception ex, string json) => log.Error(ex, nameof(Resources.CO_DSP_DPS_003), new object[] { json });

            try
            {
                try
                {
                    dispatchedEvents = GetDispatchedEvents(eventData, log);
                }
                catch (Exception e)
                {
                    // イベント情報が正常に読み解けなかった場合
                    log.Error(e, nameof(Resources.CO_DSP_DPS_006));
                    StoreUnexpectedEvent(Const.MessageSchemaId.MS011, eventData, actionIfFailedWhenStoreUnexpected);
                    return;
                }

                foreach (DispatchedEvent dispatchedEvent in dispatchedEvents)
                {
                    var messageId = dispatchedEvent?.MessageId;
                    try
                    {
                        RmsEvent rmsEvent = ConvertEventDataToRmsEvent(dispatchedEvent);

                        if (rmsEvent == null)
                        {
                            // イベントに必要なデータが不足している場合
                            log.Error(nameof(Resources.CO_DSP_DPS_006));
                            StoreUnexpectedEvent(Const.MessageSchemaId.MS011, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                            continue;
                        }

                        result = _service.StorePlusServiceBillLog(rmsEvent);

                        if (!result.IsSuccess())
                        {
                            StoreUnexpectedEvent(Const.MessageSchemaId.MS011, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error(e, nameof(Resources.CO_DSP_DPS_001));
                        StoreUnexpectedEvent(Const.MessageSchemaId.MS011, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                    }
                }
            }
            finally
            {
                log.LeaveJson("Response: {0}", result);
            }
        }

        /// <summary>
        /// DispatchParentChildConnect
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <param name="log">ロガー</param>
        [FunctionName(nameof(DispatchParentChildConnect))]
        public void DispatchParentChildConnect(
            [EventHubTrigger(Const.MessageSchemaId.MS030, Connection = Const.ConnectionString.EventHubsConnectionStringMs030)] string eventData,
            ILogger log)
        {
            log.EnterJson("EventData: {0}", new object[] { eventData });
            Result result = null;
            DispatchedEvent[] dispatchedEvents = null;
            void actionIfFailedWhenStoreUnexpected(Exception ex, string json) => log.Error(ex, nameof(Resources.CO_DSP_DPC_003), new object[] { json });

            try
            {
                try
                {
                    dispatchedEvents = GetDispatchedEvents(eventData, log);
                }
                catch (Exception e)
                {
                    // イベント情報が正常に読み解けなかった場合
                    log.Error(e, nameof(Resources.CO_DSP_DPC_006));
                    StoreUnexpectedEvent(Const.MessageSchemaId.MS030, eventData, actionIfFailedWhenStoreUnexpected);
                    return;
                }

                foreach (DispatchedEvent dispatchedEvent in dispatchedEvents)
                {
                    var messageId = dispatchedEvent?.MessageId;
                    try
                    {
                        RmsEvent rmsEvent = ConvertEventDataToRmsEvent(dispatchedEvent);

                        if (rmsEvent == null)
                        {
                            // イベントに必要なデータが不足している場合
                            log.Error(nameof(Resources.CO_DSP_DPC_006));
                            StoreUnexpectedEvent(Const.MessageSchemaId.MS030, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                            continue;
                        }

                        result = _service.StoreParentChildConnect(rmsEvent);

                        if (!result.IsSuccess())
                        {
                            StoreUnexpectedEvent(Const.MessageSchemaId.MS030, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error(e, nameof(Resources.CO_DSP_DPC_001));
                        StoreUnexpectedEvent(Const.MessageSchemaId.MS030, dispatchedEvent.RawBody, actionIfFailedWhenStoreUnexpected, messageId);
                    }
                }
            }
            finally
            {
                log.LeaveJson("Response: {0}", result);
            }
        }

        /// <summary>
        /// DispatchDeviceConnected
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <param name="log">ロガー</param>
        /// <returns>非同期処理タスク</returns>
        [FunctionName(nameof(DispatchDeviceConnected))]
        public async Task DispatchDeviceConnected(
            [EventHubTrigger(Const.EventHubNames.DeviceConnected, Connection = Const.ConnectionString.EventHubsConnectionStringDeviceConnected)] string eventData,
            ILogger log)
        {
            log.EnterJson("EventData: {0}", new object[] { eventData });
            Result result = null;
            DeviceConnectionEvent[] events = null;

            void actionIfFailedWhenStoreUnexpected(Exception ex, string json) => log.Error(ex, nameof(Resources.CO_DSP_DDC_020), new object[] { json });

            try
            {
                try
                {
                    events = GetDeviceConnectionEvents(eventData);
                }
                catch (Exception e)
                {
                    // イベント情報が正常に読み解けなかった場合
                    log.Error(e, nameof(Resources.CO_DSP_DDC_016));
                    StoreUnexpectedEvent(Const.EventHubNames.DeviceConnected, eventData, actionIfFailedWhenStoreUnexpected);
                    return;
                }

                foreach (DeviceConnectionEvent deviceConnectionEvent in events)
                {
                    try
                    {
                        if (!deviceConnectionEvent.HasNecessary())
                        {
                            log.Error(nameof(Resources.CO_DSP_DDC_016));
                            StoreUnexpectedEvent(Const.EventHubNames.DeviceConnected, deviceConnectionEvent.RawBody, actionIfFailedWhenStoreUnexpected);
                            continue;
                        }

                        Guid edgeId = deviceConnectionEvent.EdgeId;
                        DateTime eventTime = deviceConnectionEvent.EventTime;
                        result = await _service.StoreDeviceConnected(edgeId, eventTime);

                        if (!result.IsSuccess())
                        {
                            // DeviceConnectedイベントにはメッセージIDが存在しない
                            StoreUnexpectedEvent(Const.EventHubNames.DeviceConnected, deviceConnectionEvent.RawBody, actionIfFailedWhenStoreUnexpected);
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error(e, nameof(Resources.CO_DSP_DDC_013));
                        StoreUnexpectedEvent(Const.EventHubNames.DeviceConnected, deviceConnectionEvent.RawBody, actionIfFailedWhenStoreUnexpected);
                    }
                }
            }
            finally
            {
                log.LeaveJson("Response: {0}", result);
            }
        }

        /// <summary>
        /// DispatchDeviceDisconnected
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <param name="log">ロガー</param>
        [FunctionName(nameof(DispatchDeviceDisconnected))]
        public void DispatchDeviceDisconnected(
            [EventHubTrigger(Const.EventHubNames.DeviceDisconnected, Connection = Const.ConnectionString.EventHubsConnectionStringDeviceDisconnected)] string eventData,
            ILogger log)
        {
            log.EnterJson("EventData: {0}", new object[] { eventData });
            Result result = null;
            DeviceConnectionEvent[] events = null;

            void actionIfFailedWhenStoreUnexpected(Exception ex, string json) => log.Error(ex, nameof(Resources.CO_DSP_DDV_003), new object[] { json });

            try
            {
                try
                {
                    events = GetDeviceConnectionEvents(eventData);
                }
                catch (Exception e)
                {
                    // イベント情報が正常に読み解けなかった場合
                    log.Error(e, nameof(Resources.CO_DSP_DDV_006));
                    StoreUnexpectedEvent(Const.EventHubNames.DeviceDisconnected, eventData, actionIfFailedWhenStoreUnexpected);
                    return;
                }

                foreach (DeviceConnectionEvent deviceConnectionEvent in events)
                {
                    try
                    {
                        if (!deviceConnectionEvent.HasNecessary())
                        {
                            log.Error(nameof(Resources.CO_DSP_DDV_006));
                            StoreUnexpectedEvent(Const.EventHubNames.DeviceDisconnected, deviceConnectionEvent.RawBody, actionIfFailedWhenStoreUnexpected);
                            continue;
                        }

                        Guid edgeId = deviceConnectionEvent.EdgeId;
                        DateTime eventTime = deviceConnectionEvent.EventTime;
                        result = _service.StoreDeviceDisconnected(edgeId, eventTime);

                        if (!result.IsSuccess())
                        {
                            // DeviceDisconnectedイベントにはメッセージIDが存在しない
                            StoreUnexpectedEvent(Const.EventHubNames.DeviceDisconnected, deviceConnectionEvent.RawBody, actionIfFailedWhenStoreUnexpected);
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error(e, nameof(Resources.CO_DSP_DDV_001));
                        StoreUnexpectedEvent(Const.EventHubNames.DeviceDisconnected, deviceConnectionEvent.RawBody, actionIfFailedWhenStoreUnexpected);
                    }
                }
            }
            finally
            {
                log.LeaveJson("Response: {0}", result);
            }
        }

        /// <summary>
        /// DispatchTwinChanged
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <param name="log">ロガー</param>
        /// <remarks>
        /// デバイスツイン変更イベントはEventGridサブスクリプション経由ではなく、
        /// メッセージ ルーティングの機能によりEvent Hubsに配信する。
        /// Microsoft社からの実装例に倣い、
        /// デバイスツイン変更イベントはEventData配列を受け取るインターフェースとする。
        /// EventDataのBodyプロパティは、JSON配列ではなくJSONである。
        /// </remarks>
        [FunctionName(nameof(DispatchTwinChanged))]
        public void DispatchTwinChanged(
            [EventHubTrigger(Const.EventHubNames.TwinChanged, Connection = Const.ConnectionString.EventHubsConnectionStringTwinChanged)] EventData[] eventData,
            ILogger log)
        {
            log.EnterJson("EventData: {0}", new object[] { eventData });
            Result result = null;

            void actionIfFailedWhenStoreUnexpected(Exception ex, string json) => log.Error(ex, nameof(Resources.CO_DSP_DTC_003), new object[] { json });

            try
            {
                foreach (EventData eachEventData in eventData)
                {
                    RmsEvent dispatchEvent = null;
                    string rawBody = null;

                    try
                    {
                        // ログ出力用のJSON文字列を取得する
                        rawBody = GetRawBodyForDeviceTwinChanged(eachEventData);

                        // イベントクラスに変換
                        dispatchEvent = ConvertTwinChanged(eachEventData);
                    }
                    catch (Exception e)
                    {
                        // EventDataがnullまたはEdgeIDをEventDataから取得できない
                        log.Error(e, nameof(Resources.CO_DSP_DTC_006));
                        StoreUnexpectedEvent(Const.EventHubNames.TwinChanged, rawBody, actionIfFailedWhenStoreUnexpected);
                        continue;  // returnせずに次のEventを処理する
                    }

                    try
                    {
                        result = _service.StoreTwinChanged(dispatchEvent);

                        if (!result.IsSuccess())
                        {
                            StoreUnexpectedEvent(Const.EventHubNames.TwinChanged, rawBody, actionIfFailedWhenStoreUnexpected);
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error(e, nameof(Resources.CO_DSP_DTC_001));
                        StoreUnexpectedEvent(Const.EventHubNames.TwinChanged, rawBody, actionIfFailedWhenStoreUnexpected);
                    }
                }
            }
            finally
            {
                log.LeaveJson("Response: {0}", result);
            }
        }

        #endregion

        #region 異常/失敗メッセージ保存処理

        /// <summary>
        /// 異常メッセージまたは失敗メッセージを保存する。
        /// </summary>
        /// <param name="messageSchemaId">メッセージスキーマID</param>
        /// <param name="data">異常メッセージの場合はメッセージ全体、失敗メッセージの場合はRawBody</param>
        /// <param name="actionIfError">失敗時処理</param>
        /// <param name="messageId">メッセージID。null許可。</param>
        private void StoreUnexpectedEvent(string messageSchemaId, string data, Action<Exception, string> actionIfError, string messageId = null)
        {
            try
            {
                _service.StoreUnexpectedMessage(new UnexpectedMessage(messageSchemaId, data, messageId));
            }
            catch (Exception ex)
            {
                actionIfError(ex, data);
            }
        }

        #endregion

        #region Converters

        /// <summary>
        /// EventDataから取得したRowBodyをDispatchedEventに変換する
        /// </summary>
        /// <param name="body">EventDataのBodyプロパティ</param>
        /// <param name="logger">ロガー</param>
        /// <returns>DispatchedEvent配列</returns>
        private DispatchedEvent[] GetDispatchedEvents(string body, ILogger logger)
        {
            return DispatchedEvent.DeserializeIfInvalidThrowEx(body, logger);
        }

        /// <summary>
        /// IoTHubによるデバイス接続/切断EventDataをDeviceConnectionEventに変換する
        /// </summary>
        /// <param name="body">EventDataのBodyプロパティ</param>
        /// <returns>DeviceConnectionEvent配列</returns>
        private DeviceConnectionEvent[] GetDeviceConnectionEvents(string body)
        {
            return DeviceConnectionEvent.DeserializeIfInvalidThrowEx(body);
        }

        /// <summary>
        /// DispatchedEventをRmsEventに変換する。必要なデータが不足している場合nullを返す。
        /// </summary>
        /// <param name="eventData">EventData</param>
        /// <returns>RmsEvent。必要な値が不足している場合、null。</returns>
        private RmsEvent ConvertEventDataToRmsEvent(DispatchedEvent eventData)
        {
            if (!eventData.HasNecessary())
            {
                return null;
            }

            return new RmsEvent()
            {
                EdgeId = eventData.EdgeId,
                MessageId = eventData.MessageId,
                MessageDateTime = eventData.Enqueuedtime,
                MessageBody = eventData.Body,
            };
        }

        /// <summary>
        /// DeviceTwinChangedイベントデータJson文字列を取得する
        /// </summary>
        /// <param name="eventData">EventData</param>
        /// <returns>Json文字列</returns>
        private string GetRawBodyForDeviceTwinChanged(EventData eventData)
        {
            return JsonConvert.SerializeObject(new object[] { eventData });
        }

        /// <summary>
        /// IoTHubによるデバイスツイン更新EventDataを変換する
        /// </summary>
        /// <param name="eventData">EventData</param>
        /// <returns>RmsEvent</returns>
        /// <remarks>
        /// - EventDataがnullの場合はArgumentNullExceptionを投げる
        /// - EventData#Bodyをデシリアライズできない場合ArgumentExceptionを投げる
        /// - EdgeIDをSystemPropertiesから取得できない場合はFormatExceptionを投げる
        /// </remarks>
        private RmsEvent ConvertTwinChanged(EventData eventData)
        {
            // Body以下に必要な情報が格納されている
            string body = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);

            // Bodyをデシリアライズして必要な情報を抽出
            DeviceTwinEvent deviceTwinEvent = DeviceTwinEvent.DeserializeIfInvalidThrowEx(body);

            return new RmsEvent()
            {
                //// EdgeIDを取得
                EdgeId = DeviceTwinEvent.GetEdgeId(eventData),
                //// MessageId
                //// MessageDataTime
                /// 他のDispatcherと合わせるため、この時点ではBodyは文字列にしておく。
                MessageBody = JsonConvert.SerializeObject(deviceTwinEvent.Reported)
            };
        }

        #endregion
    }
}