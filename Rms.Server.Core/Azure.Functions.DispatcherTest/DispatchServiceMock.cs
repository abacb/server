using Microsoft.Extensions.Logging;
using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Core.Service.Services;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Utility.Models.Dispatch;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Azure.Functions.DispatcherTest
{
    /// <summary>
    /// DispatchService Mockクラス
    /// </summary>
    public class DispatchServiceMock : IDispatchService
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

        /// <summary>例外を発生させるメソッド</summary>
        private static Dictionary<string, bool> _exceptionMethods = new Dictionary<string, bool>();

        /// <summary>正規の処理を実行するサービス</summary>
        private static DispatchService _service = null;

        /// <summary>発生させる例外</summary>
        private Exception _exception = new System.Exception();

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
        public DispatchServiceMock(
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
        /// 初期化処理
        /// </summary>
        /// <param name="exceptionMethodName">例外を発生させるメソッド名</param>
        public void Init(string exceptionMethodName)
        {
            if (!string.IsNullOrEmpty(exceptionMethodName))
            {
                _exceptionMethods[exceptionMethodName] = true;
            }

            _service = new DispatchService(
                _settings,
                _logger as ILogger<DispatchService>,
                _dtDeviceRepository,
                _dtDeliveryGroupRepository,
                _dtPlusServiceBillLogRepository,
                _dtDxaBillLogRepository,
                _dtDtDxaQcLogRepository,
                _dtInstallResultRepository,
                _dtSoftVersionRepository,
                _dtDirectoryUsageRepository,
                _dtDiskDriveRepository,
                _dtEquipmentUsageRepository,
                _dtInventoryRepository,
                _dtDriveRepository,
                _dtParentChildConnectRepository,
                _dtRequestDeviceRepository,
                _dtScriptConfigRepository,
                _dtStorageConfigRepository,
                _failureRepository,
                _timeProvider);
        }

        /// <summary>
        /// 想定と異なるイベントを集積する。発生した例外はそのまま投げる。
        /// </summary>
        /// <param name="unexpectedMessage">想定と異なるメッセージ情報</param>
        /// <returns>Result</returns>
        public Result StoreUnexpectedMessage(UnexpectedMessage unexpectedMessage)
        {
            return _service.StoreUnexpectedMessage(unexpectedMessage);
        }

        #region デバイスイベント

        /// <summary>
        /// IoT Hubによるデバイス接続イベントを受けて処理を行う
        /// </summary>
        /// <param name="edgeId">エッジID</param>
        /// <param name="eventTime">イベント発生日時</param>
        /// <returns>結果</returns>
        public Task<Result> StoreDeviceConnected(Guid edgeId, DateTime eventTime)
        {
            return _service.StoreDeviceConnected(edgeId, eventTime);
        }

        /// <summary>
        /// IoT Hubによるデバイス切断イベントを受けて端末ステータスの更新を保存する
        /// </summary>
        /// <param name="edgeId">エッジID</param>
        /// <param name="eventTime">イベント発生日時</param>
        /// <returns>結果</returns>
        public Result StoreDeviceDisconnected(Guid edgeId, DateTime eventTime)
        {
            return _service.StoreDeviceDisconnected(edgeId, eventTime);
        }

        /// <summary>
        /// IoT Hubによるデバイスツイン更新イベントを受けて処理を行う端末データテーブルを更新する
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        public Result StoreTwinChanged(RmsEvent eventData)
        {
            return _service.StoreTwinChanged(eventData);
        }

        #endregion

        #region ストア

        /// <summary>
        /// DeviceParentChildConnectを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        public Result StoreParentChildConnect(RmsEvent eventData)
        {
            string myName = MethodBase.GetCurrentMethod().Name;

            if (_exceptionMethods.ContainsKey(myName) && _exceptionMethods[myName])
            {
                throw _exception;
            }

            return _service.StoreParentChildConnect(eventData);
        }

        /// <summary>
        /// SoftVersionを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        public Result StoreSoftVersion(RmsEvent eventData)
        {
            string myName = MethodBase.GetCurrentMethod().Name;

            if (_exceptionMethods.ContainsKey(myName) && _exceptionMethods[myName])
            {
                throw _exception;
            }

            return _service.StoreSoftVersion(eventData);
        }

        /// <summary>
        /// DiskDriveを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        public Result StoreDiskDrive(RmsEvent eventData)
        {
            string myName = MethodBase.GetCurrentMethod().Name;

            if (_exceptionMethods.ContainsKey(myName) && _exceptionMethods[myName])
            {
                throw _exception;
            }

            return _service.StoreDiskDrive(eventData);
        }

        /// <summary>
        /// DirectoryUsageを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        public Result StoreDirectoryUsage(RmsEvent eventData)
        {
            string myName = MethodBase.GetCurrentMethod().Name;

            if (_exceptionMethods.ContainsKey(myName) && _exceptionMethods[myName])
            {
                throw _exception;
            }

            return _service.StoreDirectoryUsage(eventData);
        }

        /// <summary>
        /// Inventoryを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        public Result StoreInventory(RmsEvent eventData)
        {
            string myName = MethodBase.GetCurrentMethod().Name;

            if (_exceptionMethods.ContainsKey(myName) && _exceptionMethods[myName])
            {
                throw _exception;
            }

            return _service.StoreInventory(eventData);
        }

        /// <summary>
        /// EquipmentUsageを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        public Result StoreEquipmentUsage(RmsEvent eventData)
        {
            string myName = MethodBase.GetCurrentMethod().Name;

            if (_exceptionMethods.ContainsKey(myName) && _exceptionMethods[myName])
            {
                throw _exception;
            }

            return _service.StoreEquipmentUsage(eventData);
        }

        /// <summary>
        /// Driveを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        public Result StoreDrive(RmsEvent eventData)
        {
            string myName = MethodBase.GetCurrentMethod().Name;

            if (_exceptionMethods.ContainsKey(myName) && _exceptionMethods[myName])
            {
                throw _exception;
            }

            return _service.StoreDrive(eventData);
        }

        /// <summary>
        /// PlusServiceBillLogを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        public Result StorePlusServiceBillLog(RmsEvent eventData)
        {
            string myName = MethodBase.GetCurrentMethod().Name;

            if (_exceptionMethods.ContainsKey(myName) && _exceptionMethods[myName])
            {
                throw _exception;
            }

            return _service.StorePlusServiceBillLog(eventData);
        }

        /// <summary>
        /// DxaBillLogを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        public Result StoreDxaBillLog(RmsEvent eventData)
        {
            string myName = MethodBase.GetCurrentMethod().Name;

            if (_exceptionMethods.ContainsKey(myName) && _exceptionMethods[myName])
            {
                throw _exception;
            }

            return _service.StoreDxaBillLog(eventData);
        }

        /// <summary>
        /// DxaQcLogを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        public Result StoreDxaQcLog(RmsEvent eventData)
        {
            string myName = MethodBase.GetCurrentMethod().Name;

            if (_exceptionMethods.ContainsKey(myName) && _exceptionMethods[myName])
            {
                throw _exception;
            }

            return _service.StoreDxaQcLog(eventData);
        }

        /// <summary>
        /// InstallResultを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        public Result StoreInstallResult(RmsEvent eventData)
        {
            string myName = MethodBase.GetCurrentMethod().Name;

            if (_exceptionMethods.ContainsKey(myName) && _exceptionMethods[myName])
            {
                throw _exception;
            }

            return _service.StoreInstallResult(eventData);
        }

        #endregion
    }
}
