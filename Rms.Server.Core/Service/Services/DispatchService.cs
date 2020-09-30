using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rms.Server.Core.Abstraction.Models;
using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Core.Azure.Utility.Validations;
using Rms.Server.Core.Service.Models;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Core.Utility.Extensions;
using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Utility.Models.Dispatch;
using Rms.Server.Core.Utility.Models.Entites;
using Rms.Server.Core.Utility.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rms.Server.Core.Service.Services
{
    /// <summary>
    /// DispatchService
    /// </summary>
    public class DispatchService : IDispatchService
    {
        /// <summary>
        /// AppSettings
        /// </summary>
        private readonly AppSettings _settings;

        /// <summary>
        /// ロガー
        /// </summary>
        private readonly ILogger _logger;

        #region Repositories for Dispatchers

        /// <summary>
        /// IDtDeviceRepository
        /// </summary>
        private readonly IDtDeviceRepository _dtDeviceRepository;

        /// <summary>
        /// IDtDeliveryGroupRepository
        /// </summary>
        private readonly IDtDeliveryGroupRepository _dtDeliveryGroupRepository;

        /// <summary>
        /// IDtPlusServiceBillLogRepository
        /// </summary>
        private readonly IDtPlusServiceBillLogRepository _dtPlusServiceBillLogRepository;

        /// <summary>
        /// IDtDxaBillLogRepository
        /// </summary>
        private readonly IDtDxaBillLogRepository _dtDxaBillLogRepository;

        /// <summary>
        /// IDtDxaQcLogRepository
        /// </summary>
        private readonly IDtDxaQcLogRepository _dtDtDxaQcLogRepository;

        /// <summary>
        /// IDtInstallResultRepository
        /// </summary>
        private readonly IDtInstallResultRepository _dtInstallResultRepository;

        /// <summary>
        /// IDtSoftVersionRepository
        /// </summary>
        private readonly IDtSoftVersionRepository _dtSoftVersionRepository;

        /// <summary>
        /// IDtDirectoryUsageRepository
        /// </summary>
        private readonly IDtDirectoryUsageRepository _dtDirectoryUsageRepository;

        /// <summary>
        /// IDtDiskDriveRepository
        /// </summary>
        private readonly IDtDiskDriveRepository _dtDiskDriveRepository;

        /// <summary>
        /// IDtEquipmentUsageRepository
        /// </summary>
        private readonly IDtEquipmentUsageRepository _dtEquipmentUsageRepository;

        /// <summary>
        /// IDtInventoryRepository
        /// </summary>
        private readonly IDtInventoryRepository _dtInventoryRepository;

        /// <summary>
        /// IDtDriveRepository
        /// </summary>
        private readonly IDtDriveRepository _dtDriveRepository;

        /// <summary>
        /// IDtParentChildConnectRepository
        /// </summary>
        private readonly IDtParentChildConnectRepository _dtParentChildConnectRepository;

        /// <summary>
        /// IRequestDeviceRepository
        /// </summary>
        private readonly IRequestDeviceRepository _dtRequestDeviceRepository;

        /// <summary>
        /// IDtScriptConfigRepository
        /// </summary>
        private readonly IDtScriptConfigRepository _dtScriptConfigRepository;

        /// <summary>
        /// IDtStorageConfigRepository
        /// </summary>
        private readonly IDtStorageConfigRepository _dtStorageConfigRepository;

        /// <summary>
        /// IFailureRepository
        /// </summary>
        private readonly IFailureRepository _failureRepository;

        #endregion

        /// <summary>
        /// ITimeProvider
        /// </summary>
        private readonly ITimeProvider _timeProvider;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">設定</param>
        /// <param name="logger">ロガー</param>
        /// <param name="dtDeviceRepository">IDtDeviceRepository</param>
        /// <param name="dtDeliveryGroupRepository">IDtDeliveryGroupRepository</param>
        /// <param name="dtPlusServiceBillLogRepository">IDtPlusServiceBillLogRepository</param>
        /// <param name="dtDxaBillLogRepository">IDtDxaBillLogRepository</param>
        /// <param name="dtDxaQcLogRepository">IDtDxaQcLogRepository</param>
        /// <param name="dtInstallResultRepository">IDtInstallResultRepository</param>
        /// <param name="dtSoftVersionRepository">IDtSoftVersionRepository</param>
        /// <param name="dtDirectoryUsageRepository">IDtDirectoryUsageRepository</param>
        /// <param name="dtDiskDriveRepository">IDtDiskDriveRepository</param>
        /// <param name="dtEquipmentUsageRepository">IDtEquipmentUsageRepository</param>
        /// <param name="dtInventoryRepository">IDtInventoryRepository</param>
        /// <param name="dtDriveRepository">IDtDriveRepository</param>
        /// <param name="dtParentChildConnectRepository">IDtParentChildConnectRepository</param>
        /// <param name="dtRequestDeviceRepository">IRequestDeviceRepository</param>
        /// <param name="dtScriptConfigRepository">IDtScriptConfigRepository</param>
        /// <param name="dtStorageConfigRepository">IDtStorageConfigRepository</param>
        /// <param name="failureRepository">IFailureRepository</param>
        /// <param name="timeProvider">ITimeProvider</param>
        public DispatchService(
            AppSettings settings,
            ILogger<DispatchService> logger,
            IDtDeviceRepository dtDeviceRepository,
            IDtDeliveryGroupRepository dtDeliveryGroupRepository,
            IDtPlusServiceBillLogRepository dtPlusServiceBillLogRepository,
            IDtDxaBillLogRepository dtDxaBillLogRepository,
            IDtDxaQcLogRepository dtDxaQcLogRepository,
            IDtInstallResultRepository dtInstallResultRepository,
            IDtSoftVersionRepository dtSoftVersionRepository,
            IDtDirectoryUsageRepository dtDirectoryUsageRepository,
            IDtDiskDriveRepository dtDiskDriveRepository,
            IDtEquipmentUsageRepository dtEquipmentUsageRepository,
            IDtInventoryRepository dtInventoryRepository,
            IDtDriveRepository dtDriveRepository,
            IDtParentChildConnectRepository dtParentChildConnectRepository,
            IRequestDeviceRepository dtRequestDeviceRepository,
            IDtScriptConfigRepository dtScriptConfigRepository,
            IDtStorageConfigRepository dtStorageConfigRepository,
            IFailureRepository failureRepository,
            ITimeProvider timeProvider)
        {
            Assert.IfNull(settings);
            Assert.IfNull(logger);
            Assert.IfNull(dtDeviceRepository);
            Assert.IfNull(dtDeliveryGroupRepository);
            Assert.IfNull(dtPlusServiceBillLogRepository);
            Assert.IfNull(dtDxaBillLogRepository);
            Assert.IfNull(dtInstallResultRepository);
            Assert.IfNull(dtSoftVersionRepository);
            Assert.IfNull(dtDirectoryUsageRepository);
            Assert.IfNull(dtDiskDriveRepository);
            Assert.IfNull(dtEquipmentUsageRepository);
            Assert.IfNull(dtInventoryRepository);
            Assert.IfNull(dtDriveRepository);
            Assert.IfNull(dtParentChildConnectRepository);
            Assert.IfNull(dtParentChildConnectRepository);
            Assert.IfNull(dtRequestDeviceRepository);
            Assert.IfNull(dtScriptConfigRepository);
            Assert.IfNull(dtStorageConfigRepository);
            Assert.IfNull(failureRepository);
            Assert.IfNull(timeProvider);

            _settings = settings;
            _logger = logger;

            _dtDeviceRepository = dtDeviceRepository;
            _dtDeliveryGroupRepository = dtDeliveryGroupRepository;
            _dtPlusServiceBillLogRepository = dtPlusServiceBillLogRepository;
            _dtDxaBillLogRepository = dtDxaBillLogRepository;
            _dtDtDxaQcLogRepository = dtDxaQcLogRepository;
            _dtInstallResultRepository = dtInstallResultRepository;
            _dtSoftVersionRepository = dtSoftVersionRepository;
            _dtDirectoryUsageRepository = dtDirectoryUsageRepository;
            _dtDiskDriveRepository = dtDiskDriveRepository;
            _dtEquipmentUsageRepository = dtEquipmentUsageRepository;
            _dtInventoryRepository = dtInventoryRepository;
            _dtDriveRepository = dtDriveRepository;
            _dtParentChildConnectRepository = dtParentChildConnectRepository;
            _dtRequestDeviceRepository = dtRequestDeviceRepository;
            _dtScriptConfigRepository = dtScriptConfigRepository;
            _dtStorageConfigRepository = dtStorageConfigRepository;
            _failureRepository = failureRepository;

            _timeProvider = timeProvider;
        }

        /// <summary>
        /// IoT Hubによるデバイス接続イベント時のシーケンスステータス
        /// シーケンス図 04-06エッジ（IoT Hub）接続参照
        /// </summary>
        private enum DeviceConnectionSequenceStatus
        {
            /// <summary>未定義のステータス</summary>
            Undefined = 0,

            /// <summary>接続ステータスの更新中(Sq1.1.1.1.1)</summary>
            UpdateConnectionStatus = 1,

            /// <summary>機器が接続したIoT Hubの情報取得中(Sq1.1.1.1.2)</summary>
            GetIoTHubInfo,

            /// <summary>デバイスツイン取得中(Sq1.1.1.1.3)</summary>
            GetDeviceTwin,

            /// <summary>端末情報取得中(Sq1.1.1.1.4)</summary>
            GetDeviceInfo,

            /// <summary>スクリプト設定取得中(Sq1.1.1.1.5)</summary>
            GetScriptConfigs,

            /// <summary>ストレージ設定取得中(Sq1.1.1.1.6)</summary>
            GetStorageConfigs,

            /// <summary>デバイスツインの最新状態を取得中(Sq1.1.1.1.7)</summary>
            CheckLatestSettingsOnDeviceTwin,

            /// <summary>デバイスツインのdesiredプロパティ更新中(Sq1.1.1.1.8)</summary>
            UpdateDeviceTwinDesiredProperty,

            /// <summary>配信結果ステータス確認中(Sq1.1.1.1.9)</summary>
            CheckDeliveryResultStatus,

            /// <summary>メッセージ生成中(Sq1.1.1.1.10)</summary>
            CreateDeliveryMessage,

            /// <summary>メッセージ送信中(Sq1.1.1.1.11)</summary>
            SendMessage,

            /// <summary>配信結果ステータス更新中(Sq1.1.1.1.12)</summary>
            UpdateDeliveryResultStatus,
        }

        /// <summary>
        /// 想定と異なるイベントを集積する。発生した例外はそのまま投げる。
        /// </summary>
        /// <param name="unexpectedMessage">想定と異なるメッセージ情報</param>
        /// <returns>Result</returns>
        public Result StoreUnexpectedMessage(UnexpectedMessage unexpectedMessage)
        {
            Result result = null;
            try
            {
                _logger.Enter("In Param: MessageSchemaId:[{0}], MessageId:[{1}]", new object[] { unexpectedMessage?.MessageSchemaId, unexpectedMessage?.MessageId });

                // ディレクトリ
                // <メッセージスキーマID>/<年(yyyy)>/<月(MM)>/<日(dd)>
                // ファイルパス
                // <メッセージID>_<生成日時(yyyyMMddHHmmssfff)>.json
                // メッセージIDがない場合
                // <生成日時(yyyyMMddHHmmssfff)>.json
                DateTime now = _timeProvider.UtcNow;
                string directory = $"{unexpectedMessage.MessageSchemaId}/{now.ToString("yyyy/MM/dd")}";
                string prefixFileName = string.IsNullOrEmpty(unexpectedMessage.MessageId) ? string.Empty : $"{unexpectedMessage.MessageId}_";
                string fileName = $"{prefixFileName}{now.ToString("yyyyMMddHHmmssfff")}.json";
                var file = new ArchiveFile()
                {
                    ContainerName = _settings.FailureBlobContainerNameDispatcher,
                    CreatedAt = now,
                    FilePath = $"{directory}/{fileName}"
                };

                _failureRepository.Upload(file, unexpectedMessage.Body);
                result = new Result(ResultCode.Succeed);
            }
            finally
            {
                _logger.LeaveJson("Result Param: {0}", result);
            }

            return result;
        }

        /// <summary>
        /// DeviceParentChildConnectを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        public Result StoreParentChildConnect(RmsEvent eventData)
        {
            Result result = null;
            ParentChildConnectionInfoMessage message = null;
            long deviceId = 0;
            _logger.EnterJson("In Param: {0}", eventData);

            try
            {
                message = DeserializeMessage<ParentChildConnectionInfoMessage>(eventData);
                deviceId = GetDeviceId(eventData); // EdgeIDは使わないが、チェックは行う
            }
            catch (RmsParameterException e)
            {
                // パラメタ不正
                _logger.Error(e, nameof(Resources.CO_DSP_DPC_002), new object[] { eventData });
                result = new Result(ResultCode.ParameterError, Resources.CO_DSP_DPC_002);
            }
            catch (Exception e)
            {
                // Device情報の取得に失敗　
                _logger.Error(e, nameof(Resources.CO_DSP_DPC_004), new object[] { eventData.EdgeId });
                result = new Result(ResultCode.NotFound, Resources.CO_DSP_DPC_004);
            }

            try
            {
                if (result == null)
                {
                    DtParentChildConnect data = StoreParentChildConnect(message);
                    if (data != null)
                    {
                        // 正常に追加できました。
                        _logger.Info(nameof(Resources.CO_DSP_DPC_005), new object[] { data });
                        result = new Result(ResultCode.Succeed);
                    }
                    else
                    {
                        // メッセージの「確認日時」が既存レコードよりも古かったためテーブルを更新できなかった
                        _logger.Info(nameof(Resources.CO_DSP_DPC_009), new object[] { data });
                        result = new Result(ResultCode.Succeed);
                    }
                }
            }
            catch (RmsParameterException e)
            {
                // Repository層でValidationに失敗
                _logger.Error(e, nameof(Resources.CO_DSP_DPC_002), new object[] { eventData.MessageId });
                result = new Result(ResultCode.ParameterError, Resources.CO_DSP_DPC_002);
            }
            catch (RmsException e)
            {
                // メッセージの保存に失敗
                _logger.Error(e, nameof(Resources.CO_DSP_DPC_008), new object[] { eventData.MessageId });
                result = new Result(ResultCode.ServerEerror, Resources.CO_DSP_DPC_008);
            }
            finally
            {
                _logger.LeaveJson("Result Param: {0}", result);
            }

            return result;
        }

        /// <summary>
        /// SoftVersionを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        public Result StoreSoftVersion(RmsEvent eventData)
        {
            Result result = null;
            SoftVersionMessage message = null;
            long deviceId = 0;
            _logger.EnterJson("In Param: {0}", eventData);

            try
            {
                message = DeserializeMessage<SoftVersionMessage>(eventData);
                deviceId = GetDeviceId(eventData);
            }
            catch (RmsParameterException e)
            {
                // パラメタ不正
                _logger.Error(e, nameof(Resources.CO_DSP_DSV_002), new object[] { eventData });
                result = new Result(ResultCode.ParameterError, Resources.CO_DSP_DSV_002);
            }
            catch (Exception e)
            {
                // Device情報の取得に失敗　
                _logger.Error(e, nameof(Resources.CO_DSP_DSV_004), new object[] { eventData.EdgeId });
                result = new Result(ResultCode.NotFound, Resources.CO_DSP_DSV_004);
            }

            try
            {
                if (result == null)
                {
                    DtSoftVersion model = message.Convert(deviceId, eventData);
                    DtSoftVersion addedData = _dtSoftVersionRepository.CreateDtSoftVersionIfAlreadyMessageThrowEx(model, message.ModelCode);

                    // 正常に追加できました。
                    _logger.Info(nameof(Resources.CO_DSP_DSV_005), new object[] { addedData });
                    result = new Result(ResultCode.Succeed);
                }
            }
            catch (RmsAlreadyExistException)
            {
                // すでに処理済みのメッセージだった。
                _logger.Info(nameof(Resources.CO_DSP_DSV_007), new object[] { eventData.MessageId });

                // 既存メッセージIDの重複は、EventHubsでとりうる挙動のため正常系。
                result = new Result(ResultCode.Succeed, Resources.CO_DSP_DSV_007);
            }
            catch (RmsParameterException e)
            {
                // パラメタ不正
                _logger.Error(e, nameof(Resources.CO_DSP_DSV_002), new object[] { eventData });
                result = new Result(ResultCode.ParameterError, Resources.CO_DSP_DSV_002);
            }
            catch (RmsException e)
            {
                // メッセージの保存に失敗
                _logger.Error(e, nameof(Resources.CO_DSP_DSV_008), new object[] { eventData.MessageId });
                result = new Result(ResultCode.ServerEerror, Resources.CO_DSP_DSV_008);
            }
            finally
            {
                _logger.LeaveJson("Result Param: {0}", result);
            }

            return result;
        }

        /// <summary>
        /// DiskDriveを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        public Result StoreDiskDrive(RmsEvent eventData)
        {
            Result result = null;
            DiskDriveMessage message = null;
            long deviceId = 0;
            _logger.EnterJson("In Param: {0}", eventData);

            try
            {
                message = DeserializeMessage<DiskDriveMessage>(eventData);
                deviceId = GetDeviceId(eventData);
            }
            catch (RmsParameterException e)
            {
                // パラメタ不正
                _logger.Error(e, nameof(Resources.CO_DSP_DDD_002), new object[] { eventData });
                result = new Result(ResultCode.ParameterError, Resources.CO_DSP_DDD_002);
            }
            catch (Exception e)
            {
                // Device情報の取得に失敗　
                _logger.Error(e, nameof(Resources.CO_DSP_DDD_004), new object[] { eventData.EdgeId });
                result = new Result(ResultCode.NotFound, Resources.CO_DSP_DDD_004);
            }

            try
            {
                if (result == null)
                {
                    DtDiskDrive model = message.Convert(deviceId, eventData);
                    DtDiskDrive addedData = _dtDiskDriveRepository.CreateDtDiskDriveIfAlreadyMessageThrowEx(model);

                    // 正常に追加できました。
                    _logger.Info(nameof(Resources.CO_DSP_DDD_005), new object[] { addedData });
                    result = new Result(ResultCode.Succeed);
                }
            }
            catch (RmsAlreadyExistException)
            {
                // すでに処理済みのメッセージだった。
                _logger.Info(nameof(Resources.CO_DSP_DDD_007), new object[] { eventData.MessageId });

                // 既存メッセージIDの重複は、EventHubsでとりうる挙動のため正常系。
                result = new Result(ResultCode.Succeed, Resources.CO_DSP_DDD_007);
            }
            catch (RmsParameterException e)
            {
                // パラメタ不正
                _logger.Error(e, nameof(Resources.CO_DSP_DDD_002), new object[] { eventData });
                result = new Result(ResultCode.ParameterError, Resources.CO_DSP_DDD_002);
            }
            catch (RmsException e)
            {
                // メッセージの保存に失敗
                _logger.Error(e, nameof(Resources.CO_DSP_DDD_008), new object[] { eventData.MessageId });
                result = new Result(ResultCode.ServerEerror, Resources.CO_DSP_DDD_008);
            }
            finally
            {
                _logger.LeaveJson("Result Param: {0}", result);
            }

            return result;
        }

        /// <summary>
        /// DirectoryUsageを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        public Result StoreDirectoryUsage(RmsEvent eventData)
        {
            Result result = null;
            DirectoryUsageMessage message = null;
            long deviceId = 0;
            _logger.EnterJson("In Param: {0}", eventData);

            try
            {
                message = DeserializeMessage<DirectoryUsageMessage>(eventData);
                deviceId = GetDeviceId(eventData);
            }
            catch (RmsParameterException e)
            {
                // パラメタ不正
                _logger.Error(e, nameof(Resources.CO_DSP_DDU_002), new object[] { eventData });
                result = new Result(ResultCode.ParameterError, Resources.CO_DSP_DDU_002);
            }
            catch (Exception e)
            {
                // Device情報の取得に失敗　
                _logger.Error(e, nameof(Resources.CO_DSP_DDU_004), new object[] { eventData.EdgeId });
                result = new Result(ResultCode.NotFound, Resources.CO_DSP_DDU_004);
            }

            try
            {
                if (result == null)
                {
                    DtDirectoryUsage model = message.Convert(deviceId, eventData);
                    DtDirectoryUsage addedData = _dtDirectoryUsageRepository.CreateDtDirectoryUsageIfAlreadyMessageThrowEx(model);

                    // 正常に追加できました。
                    _logger.Info(nameof(Resources.CO_DSP_DDU_005), new object[] { addedData });
                    result = new Result(ResultCode.Succeed);
                }
            }
            catch (RmsAlreadyExistException)
            {
                // すでに処理済みのメッセージだった。
                _logger.Info(nameof(Resources.CO_DSP_DDU_007), new object[] { eventData.MessageId });

                // 既存メッセージIDの重複は、EventHubsでとりうる挙動のため正常系。
                result = new Result(ResultCode.Succeed, Resources.CO_DSP_DDU_007);
            }
            catch (RmsParameterException e)
            {
                // パラメタ不正
                _logger.Error(e, nameof(Resources.CO_DSP_DDU_002), new object[] { eventData });
                result = new Result(ResultCode.ParameterError, Resources.CO_DSP_DDU_002);
            }
            catch (RmsException e)
            {
                // メッセージの保存に失敗
                _logger.Error(e, nameof(Resources.CO_DSP_DDU_008), new object[] { eventData.MessageId });
                result = new Result(ResultCode.ServerEerror, Resources.CO_DSP_DDU_008);
            }
            finally
            {
                _logger.LeaveJson("Result Param: {0}", result);
            }

            return result;
        }

        /// <summary>
        /// Inventoryを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        public Result StoreInventory(RmsEvent eventData)
        {
            Result result = null;
            InventoryInfoMessage message = null;
            long deviceId = 0;
            _logger.EnterJson("In Param: {0}", eventData);

            try
            {
                message = DeserializeMessage<InventoryInfoMessage>(eventData);
                deviceId = GetDeviceId(eventData);
            }
            catch (RmsParameterException e)
            {
                // パラメタ不正
                _logger.Error(e, nameof(Resources.CO_DSP_DII_002), new object[] { eventData });
                result = new Result(ResultCode.ParameterError, Resources.CO_DSP_DII_002);
            }
            catch (Exception e)
            {
                // Device情報の取得に失敗　
                _logger.Error(e, nameof(Resources.CO_DSP_DII_004), new object[] { eventData.EdgeId });
                result = new Result(ResultCode.NotFound, Resources.CO_DSP_DII_004);
            }

            try
            {
                if (result == null)
                {
                    DtInventory model = message.Convert(deviceId, eventData);
                    DtInventory addedData = _dtInventoryRepository.CreateDtInventoryIfAlreadyMessageThrowEx(model);

                    // 正常に追加できました。
                    _logger.Info(nameof(Resources.CO_DSP_DII_005), new object[] { addedData });
                    result = new Result(ResultCode.Succeed);
                }
            }
            catch (RmsAlreadyExistException)
            {
                // すでに処理済みのメッセージだった。
                _logger.Info(nameof(Resources.CO_DSP_DII_007), new object[] { eventData.MessageId });

                // 既存メッセージIDの重複は、EventHubsでとりうる挙動のため正常系。
                result = new Result(ResultCode.Succeed, Resources.CO_DSP_DII_007);
            }
            catch (RmsParameterException e)
            {
                // パラメタ不正
                _logger.Error(e, nameof(Resources.CO_DSP_DII_002), new object[] { eventData });
                result = new Result(ResultCode.ParameterError, Resources.CO_DSP_DII_002);
            }
            catch (RmsException e)
            {
                // メッセージの保存に失敗
                _logger.Error(e, nameof(Resources.CO_DSP_DII_008), new object[] { eventData.MessageId });
                result = new Result(ResultCode.ServerEerror, Resources.CO_DSP_DII_008);
            }
            finally
            {
                _logger.LeaveJson("Result Param: {0}", result);
            }

            return result;
        }

        /// <summary>
        /// EquipmentUsageを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        public Result StoreEquipmentUsage(RmsEvent eventData)
        {
            Result result = null;
            UsageMessage message = null;
            long deviceId = 0;
            _logger.EnterJson("In Param: {0}", eventData);

            try
            {
                message = DeserializeMessage<UsageMessage>(eventData);
                deviceId = GetDeviceId(eventData);
            }
            catch (RmsParameterException e)
            {
                // パラメタ不正
                _logger.Error(e, nameof(Resources.CO_DSP_DEU_002), new object[] { eventData });
                result = new Result(ResultCode.ParameterError, Resources.CO_DSP_DEU_002);
            }
            catch (Exception e)
            {
                // Device情報の取得に失敗　
                _logger.Error(e, nameof(Resources.CO_DSP_DEU_004), new object[] { eventData.EdgeId });
                result = new Result(ResultCode.NotFound, Resources.CO_DSP_DEU_004);
            }

            try
            {
                if (result == null)
                {
                    DtEquipmentUsage model = message.Convert(deviceId, eventData);
                    DtEquipmentUsage addedData = _dtEquipmentUsageRepository.CreateDtEquipmentUsageIfAlreadyMessageThrowEx(model);

                    // 正常に追加できました。
                    _logger.Info(nameof(Resources.CO_DSP_DEU_005), new object[] { addedData });
                    result = new Result(ResultCode.Succeed);
                }
            }
            catch (RmsAlreadyExistException)
            {
                // すでに処理済みのメッセージだった。
                _logger.Info(nameof(Resources.CO_DSP_DEU_007), new object[] { eventData.MessageId });

                // 既存メッセージIDの重複は、EventHubsでとりうる挙動のため正常系。
                result = new Result(ResultCode.Succeed, Resources.CO_DSP_DEU_007);
            }
            catch (RmsParameterException e)
            {
                // パラメタ不正
                _logger.Error(e, nameof(Resources.CO_DSP_DEU_002), new object[] { eventData });
                result = new Result(ResultCode.ParameterError, Resources.CO_DSP_DEU_002);
            }
            catch (RmsException e)
            {
                // メッセージの保存に失敗
                _logger.Error(e, nameof(Resources.CO_DSP_DEU_008), new object[] { eventData.MessageId });
                result = new Result(ResultCode.ServerEerror, Resources.CO_DSP_DEU_008);
            }
            finally
            {
                _logger.LeaveJson("Result Param: {0}", result);
            }

            return result;
        }

        /// <summary>
        /// Driveを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        public Result StoreDrive(RmsEvent eventData)
        {
            Result result = null;
            DriveInfoMessage message = null;
            long deviceId = 0;
            _logger.EnterJson("In Param: {0}", eventData);

            try
            {
                message = DeserializeMessage<DriveInfoMessage>(eventData);
                deviceId = GetDeviceId(eventData);
            }
            catch (RmsParameterException e)
            {
                // パラメタ不正
                _logger.Error(e, nameof(Resources.CO_DSP_DID_002), new object[] { eventData });
                result = new Result(ResultCode.ParameterError, Resources.CO_DSP_DID_002);
            }
            catch (Exception e)
            {
                // Device情報の取得に失敗　
                _logger.Error(e, nameof(Resources.CO_DSP_DID_004), new object[] { eventData.EdgeId });
                result = new Result(ResultCode.NotFound, Resources.CO_DSP_DID_004);
            }

            try
            {
                if (result == null)
                {
                    DtDrive model = message.Convert(deviceId, eventData);
                    DtDrive addedData = _dtDriveRepository.CreateDtDriveIfAlreadyMessageThrowEx(model);

                    // 正常に追加できました。
                    _logger.Info(nameof(Resources.CO_DSP_DID_005), new object[] { addedData });
                    result = new Result(ResultCode.Succeed);
                }
            }
            catch (RmsAlreadyExistException)
            {
                // すでに処理済みのメッセージだった。
                _logger.Info(nameof(Resources.CO_DSP_DID_007), new object[] { eventData.MessageId });

                // 既存メッセージIDの重複は、EventHubsでとりうる挙動のため正常系。
                result = new Result(ResultCode.Succeed, Resources.CO_DSP_DID_007);
            }
            catch (RmsParameterException e)
            {
                // パラメタ不正
                _logger.Error(e, nameof(Resources.CO_DSP_DID_002), new object[] { eventData });
                result = new Result(ResultCode.ParameterError, Resources.CO_DSP_DID_002);
            }
            catch (RmsException e)
            {
                // メッセージの保存に失敗
                _logger.Error(e, nameof(Resources.CO_DSP_DID_008), new object[] { eventData.MessageId });
                result = new Result(ResultCode.ServerEerror, Resources.CO_DSP_DID_008);
            }
            finally
            {
                _logger.LeaveJson("Result Param: {0}", result);
            }

            return result;
        }

        /// <summary>
        /// InstallResultを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        public Result StoreInstallResult(RmsEvent eventData)
        {
            Result result = null;
            InstallResultMessage message = null;
            long deviceId = 0;
            _logger.EnterJson("In Param: {0}", eventData);

            try
            {
                message = DeserializeMessage<InstallResultMessage>(eventData);
                deviceId = GetDeviceId(eventData);
            }
            catch (RmsParameterException e)
            {
                // パラメタ不正
                _logger.Error(e, nameof(Resources.CO_DSP_DIR_002), new object[] { eventData });
                result = new Result(ResultCode.ParameterError, Resources.CO_DSP_DIR_002);
            }
            catch (Exception e)
            {
                // Device情報の取得に失敗　
                _logger.Error(e, nameof(Resources.CO_DSP_DIR_004), new object[] { eventData.EdgeId });
                result = new Result(ResultCode.NotFound, Resources.CO_DSP_DIR_004);
            }

            try
            {
                if (result != null)
                {
                    DtInstallResult model = message.Convert(deviceId, eventData);
                    DtInstallResult addedData = _dtInstallResultRepository.CreateDtInstallResultIfAlreadyMessageThrowEx(model, message.State);

                    if (addedData != null)
                    {
                        // 正常に追加できました。
                        _logger.Info(nameof(Resources.CO_DSP_DIR_005), new object[] { addedData });
                        result = new Result(ResultCode.Succeed);
                    }
                    else
                    {
                        // グループステータスの更新に失敗したケース
                        _logger.Error(nameof(Resources.CO_DSP_DIR_009), new object[] { eventData });
                        result = new Result(ResultCode.ParameterError, Resources.CO_DSP_DIR_009);
                    }
                }
            }
            catch (RmsAlreadyExistException)
            {
                // すでに処理済みのメッセージだった。
                _logger.Info(nameof(Resources.CO_DSP_DIR_007), new object[] { eventData.MessageId });

                // 既存メッセージIDの重複は、EventHubsでとりうる挙動のため正常系。
                result = new Result(ResultCode.Succeed, Resources.CO_DSP_DIR_007);
            }
            catch (RmsParameterException e)
            {
                // パラメタ不正
                _logger.Error(e, nameof(Resources.CO_DSP_DIR_002), new object[] { eventData });
                result = new Result(ResultCode.ParameterError, Resources.CO_DSP_DIR_002);
            }
            catch (RmsException e)
            {
                // メッセージの保存に失敗
                _logger.Error(e, nameof(Resources.CO_DSP_DIR_008), new object[] { eventData.MessageId });
                result = new Result(ResultCode.ServerEerror, Resources.CO_DSP_DIR_008);
            }
            finally
            {
                _logger.LeaveJson("Result Param: {0}", result);
            }

            return result;
        }

        /// <summary>
        /// PlusServiceBillLogを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        public Result StorePlusServiceBillLog(RmsEvent eventData)
        {
            Result result = null;
            PlusServiceBillLogMessage message = null;
            long deviceId = 0;
            _logger.EnterJson("In Param: {0}", eventData);

            try
            {
                message = DeserializeMessage<PlusServiceBillLogMessage>(eventData);
                deviceId = GetDeviceId(eventData);
            }
            catch (RmsParameterException e)
            {
                // パラメタ不正
                _logger.Error(e, nameof(Resources.CO_DSP_DPS_002), new object[] { eventData });
                result = new Result(ResultCode.ParameterError, Resources.CO_DSP_DPS_002);
            }
            catch (Exception e)
            {
                // Device情報の取得に失敗　
                _logger.Error(e, nameof(Resources.CO_DSP_DPS_004), new object[] { eventData.EdgeId });
                result = new Result(ResultCode.NotFound, Resources.CO_DSP_DPS_004);
            }

            try
            {
                if (result == null)
                {
                    DtPlusServiceBillLog model = message.Convert(deviceId, eventData);
                    DtPlusServiceBillLog addedData = _dtPlusServiceBillLogRepository.Upsert(model);

                    // 正常に追加できました。
                    _logger.Info(nameof(Resources.CO_DSP_DPS_005), new object[] { addedData });
                    result = new Result(ResultCode.Succeed);
                }
            }
            catch (RmsAlreadyExistException)
            {
                // 既存メッセージの方が新しい。
                _logger.Info(nameof(Resources.CO_DSP_DPS_007), new object[] { eventData.MessageId });

                // 遅延は、EventHubsでとりうる挙動のため正常系。
                result = new Result(ResultCode.Succeed, Resources.CO_DSP_DPS_007);
            }
            catch (RmsParameterException e)
            {
                // パラメタ不正
                _logger.Error(e, nameof(Resources.CO_DSP_DPS_002), new object[] { eventData });
                result = new Result(ResultCode.ParameterError, Resources.CO_DSP_DPS_002);
            }
            catch (RmsException e)
            {
                // メッセージの保存に失敗
                _logger.Error(e, nameof(Resources.CO_DSP_DPS_008), new object[] { eventData.MessageId });
                result = new Result(ResultCode.ServerEerror, Resources.CO_DSP_DPS_008);
            }
            finally
            {
                _logger.LeaveJson("Result Param: {0}", result);
            }

            return result;
        }

        /// <summary>
        /// DxaQcLogを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        public Result StoreDxaQcLog(RmsEvent eventData)
        {
            Result result = null;
            DxaqcLogMessage message = null;
            long deviceId = 0;
            _logger.EnterJson("In Param: {0}", eventData);

            try
            {
                message = DeserializeMessage<DxaqcLogMessage>(eventData);
                deviceId = GetDeviceId(eventData);
            }
            catch (RmsParameterException e)
            {
                // パラメタ不正
                _logger.Error(e, nameof(Resources.CO_DSP_DDL_002), new object[] { eventData });
                result = new Result(ResultCode.ParameterError, Resources.CO_DSP_DDL_002);
            }
            catch (Exception e)
            {
                // Device情報の取得に失敗
                _logger.Error(e, nameof(Resources.CO_DSP_DDL_004), new object[] { eventData.EdgeId });
                result = new Result(ResultCode.NotFound, Resources.CO_DSP_DDL_004);
            }

            try
            {
                if (result == null)
                {
                    DtDxaQcLog model = message.Convert(deviceId, eventData);
                    DtDxaQcLog addedData = _dtDtDxaQcLogRepository.CreateDtDxaQcLogIfAlreadyMessageThrowEx(model);

                    // 正常に追加できました。
                    _logger.Info(nameof(Resources.CO_DSP_DDL_005), new object[] { addedData });
                    result = new Result(ResultCode.Succeed);
                }
            }
            catch (RmsAlreadyExistException)
            {
                // すでに処理済みのメッセージだった。
                // 本項目は特別指定されたため、Warnとする。
                _logger.Warn(nameof(Resources.CO_DSP_DDL_007), new object[] { eventData.MessageId });

                // 既存メッセージIDの重複は、EventHubsでとりうる挙動のため正常系。
                result = new Result(ResultCode.Succeed, Resources.CO_DSP_DDL_007);
            }
            catch (RmsParameterException e)
            {
                // パラメタ不正
                _logger.Error(e, nameof(Resources.CO_DSP_DDL_002), new object[] { eventData });
                result = new Result(ResultCode.ParameterError, Resources.CO_DSP_DDL_002);
            }
            catch (RmsException e)
            {
                // メッセージの保存に失敗
                _logger.Error(e, nameof(Resources.CO_DSP_DDL_008), new object[] { eventData.MessageId });
                result = new Result(ResultCode.ServerEerror, Resources.CO_DSP_DDL_008);
            }
            finally
            {
                _logger.LeaveJson("Result Param: {0}", result);
            }

            return result;
        }

        /// <summary>
        /// DxaBillLogを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        public Result StoreDxaBillLog(RmsEvent eventData)
        {
            Result result = null;
            DxaBillLogMessage message = null;
            long deviceId = 0;
            _logger.EnterJson("In Param: {0}", eventData);

            try
            {
                message = DeserializeMessage<DxaBillLogMessage>(eventData);
                deviceId = GetDeviceId(eventData);
            }
            catch (RmsParameterException e)
            {
                // パラメタ不正
                _logger.Error(e, nameof(Resources.CO_DSP_DDB_002), new object[] { eventData });
                result = new Result(ResultCode.ParameterError, Resources.CO_DSP_DDB_002);
            }
            catch (Exception e)
            {
                // Device情報の取得に失敗
                _logger.Error(e, nameof(Resources.CO_DSP_DDB_004), new object[] { eventData.EdgeId });
                result = new Result(ResultCode.NotFound, Resources.CO_DSP_DDB_004);
            }

            try
            {
                if (result == null)
                {
                    DtDxaBillLog model = message.Convert(deviceId, eventData);
                    DtDxaBillLog addedData = _dtDxaBillLogRepository.Upsert(model);

                    // 正常に追加できました。
                    _logger.Info(nameof(Resources.CO_DSP_DDB_005), new object[] { addedData });
                    result = new Result(ResultCode.Succeed);
                }
            }
            catch (RmsAlreadyExistException)
            {
                // 既存メッセージの方が新しい。
                _logger.Info(nameof(Resources.CO_DSP_DDB_007), new object[] { eventData.MessageId });

                // 遅延は、EventHubsでとりうる挙動のため正常系。
                result = new Result(ResultCode.Succeed, Resources.CO_DSP_DDB_007);
            }
            catch (RmsParameterException e)
            {
                // パラメタ不正
                _logger.Error(e, nameof(Resources.CO_DSP_DDB_002), new object[] { eventData });
                result = new Result(ResultCode.ParameterError, Resources.CO_DSP_DDB_002);
            }
            catch (RmsException e)
            {
                // メッセージの保存に失敗
                _logger.Error(e, nameof(Resources.CO_DSP_DDB_008), new object[] { eventData.MessageId });
                result = new Result(ResultCode.ServerEerror, Resources.CO_DSP_DDB_008);
            }
            finally
            {
                _logger.LeaveJson("Result Param: {0}", result);
            }

            return result;
        }

        /// <summary>
        /// IoT Hubによるデバイス接続イベントを受けて処理を行う。
        /// </summary>
        /// <param name="edgeId">エッジID</param>
        /// <param name="eventTime">イベント発生日時</param>
        /// <returns>結果</returns>
        /// <remarks>
        /// シーケンスを3つの処理単位に分割してエラー処理を行う。
        /// ブロック内でエラーになった場合は次のシーケンスブロックへと移行する。
        /// </remarks>
        public async Task<Result> StoreDeviceConnected(Guid edgeId, DateTime eventTime)
        {
            Result result = null;
            long deviceId = 0;                                  // デバイスSID
            DeviceConnectionInfo deviceConnectinInfo = null;    // IoT Hub デバイス接続情報
            DeviceConnectionSequenceStatus currentStatus = DeviceConnectionSequenceStatus.Undefined;    // 実行中のシーケンス番号

            bool firstBlockSuccess = false;     // Sq1.1.1.1.1は成功したか?
            bool secondBlockSuccess = false;    // Sq1.1.1.1.2~1.1.1.1.8は成功したか?
            bool thirdBlockSuccess = false;     // Sq1.1.1.1.9~は成功したか?

            ConnectionEventTimeInfo connectionEventTimeInfo = null;

            _logger.EnterJson("In Param: {0}", edgeId);

            try
            {
                try
                {
                    // エッジIDからデバイスSIDを取得する
                    deviceId = GetDeviceId(edgeId);
                }
                catch (Exception e)
                {
                    // Device情報の取得に失敗した場合には、すべての処理を実行できないので処理は継続せずただちに終了
                    _logger.Error(e, nameof(Resources.CO_DSP_DDC_004), new object[] { edgeId });
                    return new Result(ResultCode.NotFound, Resources.CO_DSP_DDC_004);
                }

                try
                {
                    // 接続開始・更新日時が更新対象かどうか判定する
                    connectionEventTimeInfo = GetConnectionEventTimeInfo(deviceId, eventTime, true);

                    // DBのデータの方が新しい場合には以降の処理は実行しない
                    if (!connectionEventTimeInfo.IsNewerEvent)
                    {
                        _logger.Error(nameof(Resources.CO_DSP_DDC_019), new object[] { edgeId });
                        return new Result(ResultCode.ServerEerror, Resources.CO_DSP_DDC_019);
                    }
                }
                catch (Exception e)
                {
                    // 接続開始・更新日時情報の取得に失敗
                    // 指定したSIDがDBに無かったケースとなるのでNotFoundとする
                    _logger.Error(e, nameof(Resources.CO_DSP_DDC_004), new object[] { edgeId });
                    return new Result(ResultCode.NotFound, Resources.CO_DSP_DDC_004);
                }

                currentStatus = DeviceConnectionSequenceStatus.UpdateConnectionStatus;

                // Sq1.1.1.1.1
                {
                    // Sq1.1.1.1.1 接続ステータスを更新する(Connected)
                    try
                    {
                        DtDevice updatedData = _dtDeviceRepository.UpdateDeviceConnectionStatus(deviceId, connectionEventTimeInfo);
                        _logger.Info(nameof(Resources.CO_DSP_DDC_017), new object[] { updatedData });

                        // Sq1.1.1.1.1成功
                        firstBlockSuccess = true;
                    }
                    catch (Exception e)
                    {
                        // 接続ステータス更新に失敗
                        _logger.Error(e, nameof(Resources.CO_DSP_DDC_001), new object[] { edgeId });
                    }
                }

                currentStatus = DeviceConnectionSequenceStatus.GetIoTHubInfo;

                // Sq1.1.1.1.2~1.1.1.1.8
                try
                {
                    // Sq1.1.1.1.2 機器が接続中のIoT Hubを問い合わせる                
                    deviceConnectinInfo = await _dtRequestDeviceRepository.GetDeviceConnectionInfoAsync(edgeId);
                    if (deviceConnectinInfo == null)
                    {
                        throw new RmsException("指定したエッジIDを持つデバイスはIoTHub上に見つかりませんでした");
                    }

                    currentStatus = DeviceConnectionSequenceStatus.GetDeviceTwin;

                    // Sq1.1.1.1.3 機器のデバイスツインを取得する
                    Twin deviceTwin = await _dtRequestDeviceRepository.GetDeviceTwin(deviceConnectinInfo);
                    if (deviceTwin == null)
                    {
                        throw new RmsException("デバイスツインの取得に失敗しました");
                    }

                    currentStatus = DeviceConnectionSequenceStatus.GetDeviceInfo;

                    // Sq1.1.1.1.4 端末情報を取得する
                    DtDevice deviceInfo = _dtDeviceRepository.ReadDtDevice(deviceId);
                    if (deviceInfo == null)
                    {
                        throw new RmsException("端末情報の取得に失敗しました");
                    }

                    currentStatus = DeviceConnectionSequenceStatus.GetScriptConfigs;

                    // Sq1.1.1.1.5 スクリプト設定を取得するインストールタイプ
                    // インストールタイプSIDが一致するレコードをすべて取得する
                    long installTypeSid = deviceInfo.InstallTypeSid;
                    List<DtScriptConfig> scriptConfigs = _dtScriptConfigRepository.ReadDtScriptConfigs(installTypeSid);
                    if (scriptConfigs == null)
                    {
                        throw new RmsException("スクリプト設定の取得に失敗しました");
                    }

                    currentStatus = DeviceConnectionSequenceStatus.GetStorageConfigs;

                    // Sq1.1.1.1.6 ストレージ設定を取得する
                    // ストレージ設定テーブルのレコードをすべて取得する
                    List<DtStorageConfig> storageConfigs = _dtStorageConfigRepository.ReadDtStorageConfigs();
                    if (storageConfigs == null)
                    {
                        throw new RmsException("ストレージ設定の取得に失敗しました");
                    }

                    currentStatus = DeviceConnectionSequenceStatus.CheckLatestSettingsOnDeviceTwin;

                    // Sq1.1.1.1.7~1.1.1.1.8 デバイスツインのストレージおよびスクリプト設定の最新状態を確認する
                    {
                        // DBから取得したデータをメッセージクラスに格納する
                        ServerSyncMessage message = new ServerSyncMessage();
                        message.SetStorageConfigs(storageConfigs);  // 先に引数のnullチェックを行っているので必ず成功
                        message.SetScriptConfigs(scriptConfigs);    // 先に引数のnullチェックを行っているので必ず成功

                        string desiredProperty = deviceTwin.Properties.Desired.ToStringJson();
                        ServerSyncMessage desiredPropertyMessage = ServerSyncMessage.Deserialize(desiredProperty);

                        // 最新バージョン、もしくは新規に追加された設定がある場合
                        if (!ServerSyncMessage.Equals(message, desiredPropertyMessage))
                        {
                            currentStatus = DeviceConnectionSequenceStatus.UpdateDeviceTwinDesiredProperty;

                            // Sq1.1.1.1.8 デバイスツインのDesiredプロパティを更新する(ストレージ設定、スクリプト設定)
                            string jsonString = ServerSyncMessage.CreateJsonString(message);
                            await _dtRequestDeviceRepository.UpdateDeviceTwinDesiredProperties(deviceTwin, deviceConnectinInfo, jsonString);
                        }
                    }

                    // Sq1.1.1.1.2~1.1.1.1.8成功
                    secondBlockSuccess = true;
                }
                catch (Exception e)
                {
                    string error;

                    switch (currentStatus)
                    {
                        case DeviceConnectionSequenceStatus.GetIoTHubInfo:      // Sq1.1.1.1.2
                            error = nameof(Resources.CO_DSP_DDC_002);
                            break;
                        case DeviceConnectionSequenceStatus.GetDeviceTwin:      // Sq1.1.1.1.3
                            error = nameof(Resources.CO_DSP_DDC_003);
                            break;
                        case DeviceConnectionSequenceStatus.GetDeviceInfo:      // Sq1.1.1.1.4
                            error = nameof(Resources.CO_DSP_DDC_004);
                            break;
                        case DeviceConnectionSequenceStatus.GetScriptConfigs:   // Sq1.1.1.1.5
                            error = nameof(Resources.CO_DSP_DDC_005);
                            break;
                        case DeviceConnectionSequenceStatus.GetStorageConfigs:  // Sq1.1.1.1.6
                            error = nameof(Resources.CO_DSP_DDC_006);
                            break;
                        case DeviceConnectionSequenceStatus.CheckLatestSettingsOnDeviceTwin:  // Sq1.1.1.1.7
                            error = nameof(Resources.CO_DSP_DDC_007);
                            break;
                        case DeviceConnectionSequenceStatus.UpdateDeviceTwinDesiredProperty:  // Sq1.1.1.1.8
                            error = nameof(Resources.CO_DSP_DDC_008);
                            break;
                        default:
                            error = nameof(Resources.CO_DSP_DDC_013);
                            break;
                    }

                    _logger.Error(e, error, new object[] { edgeId });
                }

                currentStatus = DeviceConnectionSequenceStatus.CheckDeliveryResultStatus;

                // Sq1.1.1.1.9~
                try
                {
                    // Sq1.1.1.1.9 機器をゲートウェイ機器とする配信の配信結果ステータスを確認する
                    // 配信グループステータスが"started"であり
                    // 適用結果ステータスが"notstarted"または"messagesent"の配信グループのリストを取得する
                    List<DtDeliveryGroup> groups = _dtDeliveryGroupRepository.GetDevicesByGatewaySidNotCompletedDownload(deviceId);

                    currentStatus = DeviceConnectionSequenceStatus.CreateDeliveryMessage;

                    // DLが完了していない配信の数だけ繰り返し実行する
                    foreach (DtDeliveryGroup group in groups)
                    {
                        // Sq1.1.1.1.10 配信メッセージを生成する
                        string deliveryMessage = CreateDeliveryMessage(deviceId, group.Sid);

                        currentStatus = DeviceConnectionSequenceStatus.SendMessage;

                        // Sq1.1.1.1.11 機器にメッセージを送信する(エッジID、配信メッセージ)
                        if (deviceConnectinInfo == null)
                        {
                            throw new RmsException("メッセージ送信先のデバイスが見つかりませんでした");
                        }

                        await _dtRequestDeviceRepository.SendMessageAsync(deviceConnectinInfo, deliveryMessage);

                        currentStatus = DeviceConnectionSequenceStatus.UpdateDeliveryResultStatus;

                        // Sq1.1.1.1.12 配信結果ステータスを更新する(MessageSent)
                        DtInstallResult updatedInstallResult = _dtInstallResultRepository.UpdateInstallResultStatusToMessageSent(deviceId);

                        currentStatus = DeviceConnectionSequenceStatus.CreateDeliveryMessage;
                    }

                    currentStatus = DeviceConnectionSequenceStatus.Undefined;

                    // Sq1.1.1.1.9~成功
                    thirdBlockSuccess = true;
                }
                catch (Exception e)
                {
                    string error;

                    switch (currentStatus)
                    {
                        case DeviceConnectionSequenceStatus.CheckDeliveryResultStatus:      // Sq1.1.1.1.9
                            error = nameof(Resources.CO_DSP_DDC_009);
                            break;
                        case DeviceConnectionSequenceStatus.CreateDeliveryMessage:          // Sq1.1.1.1.10
                            error = nameof(Resources.CO_DSP_DDC_010);
                            break;
                        case DeviceConnectionSequenceStatus.SendMessage:                    // Sq1.1.1.1.11
                            error = nameof(Resources.CO_DSP_DDC_011);
                            break;
                        case DeviceConnectionSequenceStatus.UpdateDeliveryResultStatus:     // Sq1.1.1.1.12
                            error = nameof(Resources.CO_DSP_DDC_012);
                            break;
                        default:
                            error = nameof(Resources.CO_DSP_DDC_013);
                            break;
                    }

                    _logger.Error(e, error, new object[] { edgeId });
                }

                // エラーチェック
                if (firstBlockSuccess && secondBlockSuccess && thirdBlockSuccess)
                {
                    _logger.Info(nameof(Resources.CO_DSP_DDC_014), new object[] { edgeId });
                    result = new Result(ResultCode.Succeed);
                }
                else
                {
                    result = new Result(ResultCode.ServerEerror, Resources.CO_DSP_DDC_015);
                }
            }
            finally
            {
                _logger.LeaveJson("Result Param: {0}", result);
            }

            return result;
        }

        /// <summary>
        /// IoT Hubによるデバイス切断イベントを受けて端末ステータスの更新を保存する
        /// </summary>
        /// <param name="edgeId">エッジID</param>
        /// <param name="eventTime">イベント発生日時</param>
        /// <returns>結果</returns>
        public Result StoreDeviceDisconnected(Guid edgeId, DateTime eventTime)
        {
            Result result = null;
            long deviceId = 0;

            ConnectionEventTimeInfo connectionEventTimeInfo = null;

            _logger.EnterJson("EdgeId: {0}", edgeId);

            try
            {
                try
                {
                    deviceId = GetDeviceId(edgeId);
                }
                catch (Exception e)
                {
                    // Device情報の取得に失敗
                    _logger.Error(e, nameof(Resources.CO_DSP_DDV_004), new object[] { edgeId });
                    result = new Result(ResultCode.NotFound, Resources.CO_DSP_DDV_004);
                    return result;
                }

                try
                {
                    // 接続開始・更新日時が更新対象かどうか判定する
                    connectionEventTimeInfo = GetConnectionEventTimeInfo(deviceId, eventTime, false);

                    // DBのデータの方が新しい場合には以降の処理は実行しない
                    if (!connectionEventTimeInfo.IsNewerEvent)
                    {
                        _logger.Error(nameof(Resources.CO_DSP_DDV_009), new object[] { edgeId });
                        result = new Result(ResultCode.ServerEerror, Resources.CO_DSP_DDV_009);
                    }
                }
                catch (Exception e)
                {
                    // 接続開始・更新日時情報の取得に失敗
                    // 指定したSIDがDBに無かったケースとなるのでNotFoundとする
                    _logger.Error(e, nameof(Resources.CO_DSP_DDV_004), new object[] { edgeId });
                    result = new Result(ResultCode.NotFound, Resources.CO_DSP_DDV_004);
                    return result;
                }

                try
                {
                    // ステータスを切断に更新
                    // 切断イベントでは接続開始日時を更新することはないのでフラグはfalseとする
                    DtDevice updatedData = _dtDeviceRepository.UpdateDeviceConnectionStatus(deviceId, connectionEventTimeInfo);

                    // 正常に更新できました。
                    _logger.Info(nameof(Resources.CO_DSP_DDV_005), new object[] { updatedData });
                    result = new Result(ResultCode.Succeed);
                }
                catch (RmsParameterException e)
                {
                    // パラメータ不正　
                    _logger.Error(e, nameof(Resources.CO_DSP_DDV_002), new object[] { edgeId });
                    result = new Result(ResultCode.ParameterError, Resources.CO_DSP_DDV_002);
                }
                catch (RmsException e)
                {
                    // DB更新に失敗
                    _logger.Error(e, nameof(Resources.CO_DSP_DDV_008), new object[] { edgeId });
                    result = new Result(ResultCode.ServerEerror, Resources.CO_DSP_DDV_008);
                }
            }
            finally
            {
                _logger.LeaveJson("Result Param: {0}", result);
            }

            return result;
        }

        /// <summary>
        /// IoT Hubによるデバイスツイン更新イベントを受けて処理を行う端末データテーブルを更新する
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        public Result StoreTwinChanged(RmsEvent eventData)
        {
            Result result = null;
            TwinChangedMessage message = null;
            long deviceId = 0;
            _logger.EnterJson("In Param: {0}", eventData);

            try
            {
                message = DeserializeMessage<TwinChangedMessage>(eventData);
                deviceId = GetDeviceId(eventData);
            }
            catch (RmsParameterException e)
            {
                // パラメタ不正
                _logger.Error(e, nameof(Resources.CO_DSP_DTC_002), new object[] { eventData });
                result = new Result(ResultCode.ParameterError, Resources.CO_DSP_DTC_002);
            }
            catch (Exception e)
            {
                // Device情報の取得に失敗　
                _logger.Error(e, nameof(Resources.CO_DSP_DTC_004), new object[] { eventData.EdgeId });
                result = new Result(ResultCode.NotFound, Resources.CO_DSP_DTC_004);
            }

            try
            {
                if (result == null)
                {
                    DtTwinChanged model = message.Convert(eventData);
                    DtDevice addedData = _dtDeviceRepository.UpdateDeviceInfoByTwinChanged(deviceId, model);

                    // 正常に追加できました。
                    _logger.Info(nameof(Resources.CO_DSP_DTC_005), new object[] { addedData });
                    result = new Result(ResultCode.Succeed);
                }
            }
            catch (RmsParameterException e)
            {
                // パラメータ不正　
                _logger.Error(e, nameof(Resources.CO_DSP_DTC_002), new object[] { eventData });
                result = new Result(ResultCode.ParameterError, Resources.CO_DSP_DTC_002);
            }
            catch (RmsException e)
            {
                // DB更新に失敗
                _logger.Error(e, nameof(Resources.CO_DSP_DTC_008), new object[] { eventData.EdgeId });
                result = new Result(ResultCode.ServerEerror, Resources.CO_DSP_DTC_008);
            }
            finally
            {
                _logger.LeaveJson("Result Param: {0}", result);
            }

            return result;
        }

        /// <summary>
        /// イベントデータをJsonからMessageモデルクラスに変換する
        /// </summary>
        /// <typeparam name="TypeOfMessage">Messageモデルクラス</typeparam>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>変換後のMessageモデルクラス</returns>
        private TypeOfMessage DeserializeMessage<TypeOfMessage>(RmsEvent eventData)
        {
            TypeOfMessage messageData;
            try
            {
                messageData = JsonConvert.DeserializeObject<TypeOfMessage>(eventData.MessageBody);
            }
            catch (Exception e)
            {
                throw new RmsParameterException(string.Format("Invalid message [{0}].", "Request body is null"), e);
            }

            if (RequestValidator.IsBadRequestParameter(messageData, out string errorMessage))
            {
                throw new RmsParameterException(string.Format("Invalid message [{0}].", errorMessage));
            }

            return messageData;
        }

        /// <summary>
        /// デバイスSIDの取得
        /// </summary>
        /// <param name="eventData">イベント情報</param>
        /// <returns>デバイスSID</returns>
        private long GetDeviceId(RmsEvent eventData)
        {
            return GetDeviceId(eventData.EdgeId);
        }

        /// <summary>
        /// デバイスSIDの取得
        /// </summary>
        /// <param name="edgeId">エッジID</param>
        /// <returns>デバイスSID</returns>
        private long GetDeviceId(Guid edgeId)
        {
            DtDevice device = _dtDeviceRepository.ReadDtDevice(edgeId);

            if (device == null)
            {
                throw new RmsException(string.Format("Device.EdgeId[{0}]が見つかりません。", edgeId));
            }

            return device.Sid;
        }

        /// <summary>
        /// DtParentChildConnectメッセージをDBに保存する
        /// </summary>
        /// <param name="messageData">メッセージ</param>
        /// <returns>メッセージに対応するUtility.Modelsクラス。更新を行わなかった場合にはnullを返す</returns>
        private DtParentChildConnect StoreParentChildConnect(ParentChildConnectionInfoMessage messageData)
        {
            if (messageData.IsParent())
            {
                // 親フラグ=trueの場合
                DtParentChildConnectFromParent data = messageData.ConvertForParent();
                return _dtParentChildConnectRepository.Save(data);
            }
            else
            {
                // 親フラグ=falseの場合
                DtParentChildConnectFromChild data = messageData.ConvertForChild();
                return _dtParentChildConnectRepository.Save(data);
            }
        }

        /// <summary>
        /// 指定した機器を最上位機器に持つ機器に送信する配信メッセージJSON文字列を生成する
        /// </summary>
        /// <param name="gateweyDeviceSid">最上位機器SID</param>
        /// <param name="groupSid">配信グループSID</param>
        /// <returns>配信メッセージJSON文字列</returns>
        /// <remarks>配信グループ情報の取得及びシリアライズ処理で例外が発生し得る</remarks>
        private string CreateDeliveryMessage(long gateweyDeviceSid, long groupSid)
        {
            // 配信グループデータを基に、親子エンティティのデータをもった配信グループを取得する
            var includedGroup = _dtDeliveryGroupRepository.ReadDeliveryIncludedDtDeliveryGroup(groupSid);

            // メッセージオブジェクトの生成(配信対象プロパティ以外を設定、配信対象プロパティは別途設定する)
            var messageObject = RequestDelivery.CreateDeliveryMessageObject(includedGroup);

            // 配信対象プロパティを設定
            messageObject.Targets = includedGroup.DtDeliveryResult
                ?.Where(x => x.GwDeviceSid == gateweyDeviceSid)
                .Select(x => new RequestDelivery.Target() { DeliveryResultSID = x.Sid.ToString(), EquipmentUID = x.DtDevice1.EquipmentUid })
                .ToArray();

            // メッセージオブジェクトをシリアライズ
            return JsonConvert.SerializeObject(messageObject);
        }

        /// <summary>
        /// 以下の情報をまとめて呼び出しもとに返す
        /// ・イベントステータスコード（接続ステータスマスタテーブルのコード）
        /// ・初回接続か?
        /// ・DBに格納された時刻情報よりも新しいイベントか?
        /// ・ステータスが更新されるイベントか?
        /// </summary>
        /// <param name="sid">端末SID</param>
        /// <param name="eventTime">イベント時間</param>
        /// <param name="isConnectedEvent">接続イベントであればtrue、切断イベントであればfalse</param>
        /// <returns>接続イベント時刻情報</returns>
        /// <remarks>
        /// DB操作の例外は呼び出し元で処理する
        /// </remarks>
        private ConnectionEventTimeInfo GetConnectionEventTimeInfo(long sid, DateTime eventTime, bool isConnectedEvent)
        {
            // 例外が発生することを前提とするため、戻り値のnullチェックは行わない
            DtDevice device = _dtDeviceRepository.ReadDtDevice(sid);
            DateTime? startDateTime = device.ConnectStartDatetime;
            DateTime? updateDateTime = device.UpdateDatetime;

            // イベントの種別(更新後のステータスコード)
            string newConnectStatus = isConnectedEvent ? Const.ConnectStatus.Connected : Const.ConnectStatus.Disconnected;

            // 初回接続か?
            bool isFirstConnection = startDateTime == null && isConnectedEvent;

            // 新しいイベントか?
            bool isNewerEvent = !(updateDateTime != null && updateDateTime.Value > eventTime);

            // 切断イベントの場合、
            // 接続開始日時データが設定されていないときは切断イベントが接続イベントよりも先に発生したことになるため、
            // 更新を行わず終了とする
            if (!isConnectedEvent && startDateTime == null)
            {
                isNewerEvent = false;
            }

            return new ConnectionEventTimeInfo
            {
                Status = newConnectStatus,
                EventTime = eventTime,
                IsFirstConnection = isFirstConnection,
                IsNewerEvent = isNewerEvent
            };
        }
    }
}
