using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Utility.Models.Dispatch;
using System;
using System.Threading.Tasks;

namespace Rms.Server.Core.Service.Services
{
    /// <summary>
    /// IDispatchService
    /// </summary>
    public interface IDispatchService
    {
        /// <summary>
        /// 想定と異なるイベントを集積する。発生した例外はそのまま投げる。
        /// </summary>
        /// <param name="unexpectedMessage">想定と異なるメッセージ情報</param>
        /// <returns>Result</returns>
        Result StoreUnexpectedMessage(UnexpectedMessage unexpectedMessage);

        #region デバイスイベント

        /// <summary>
        /// IoT Hubによるデバイス接続イベントを受けて処理を行う
        /// </summary>
        /// <param name="edgeId">エッジID</param>
        /// <param name="eventTime">イベント発生日時</param>
        /// <returns>結果</returns>
        Task<Result> StoreDeviceConnected(Guid edgeId, DateTime eventTime);

        /// <summary>
        /// IoT Hubによるデバイス切断イベントを受けて端末ステータスの更新を保存する
        /// </summary>
        /// <param name="edgeId">エッジID</param>
        /// <param name="eventTime">イベント発生日時</param>
        /// <returns>結果</returns>
        Result StoreDeviceDisconnected(Guid edgeId, DateTime eventTime);

        /// <summary>
        /// IoT Hubによるデバイスツイン更新イベントを受けて処理を行う端末データテーブルを更新する
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        Result StoreTwinChanged(RmsEvent eventData);

        #endregion

        #region ストア

        /// <summary>
        /// DeviceParentChildConnectを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        Result StoreParentChildConnect(RmsEvent eventData);

        /// <summary>
        /// SoftVersionを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        Result StoreSoftVersion(RmsEvent eventData);

        /// <summary>
        /// DiskDriveを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        Result StoreDiskDrive(RmsEvent eventData);

        /// <summary>
        /// DirectoryUsageを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        Result StoreDirectoryUsage(RmsEvent eventData);

        /// <summary>
        /// Inventoryを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        Result StoreInventory(RmsEvent eventData);

        /// <summary>
        /// EquipmentUsageを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        Result StoreEquipmentUsage(RmsEvent eventData);

        /// <summary>
        /// Driveを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        Result StoreDrive(RmsEvent eventData);

        /// <summary>
        /// PlusServiceBillLogを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        Result StorePlusServiceBillLog(RmsEvent eventData);

        /// <summary>
        /// DxaBillLogを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        Result StoreDxaBillLog(RmsEvent eventData);

        /// <summary>
        /// DxaQcLogを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        Result StoreDxaQcLog(RmsEvent eventData);

        /// <summary>
        /// InstallResultを保存する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns>結果</returns>
        Result StoreInstallResult(RmsEvent eventData);

        #endregion
    }
}
