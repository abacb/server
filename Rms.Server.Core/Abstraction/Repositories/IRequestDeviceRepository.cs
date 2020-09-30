using Microsoft.Azure.Devices.Shared;
using Rms.Server.Core.Abstraction.Models;
using Rms.Server.Core.Utility.Models;
using System;
using System.Threading.Tasks;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// IRequestDeviceRepository
    /// </summary>
    public interface IRequestDeviceRepository
    {
        /// <summary>
        /// デバイス接続情報を取得する。
        /// </summary>
        /// <param name="edgeId">端末ID</param>
        /// <returns>デバイス接続情報を返す。対象デバイスが見つからない場合nullを返す。</returns>
        Task<DeviceConnectionInfo> GetDeviceConnectionInfoAsync(Guid edgeId);

        /// <summary>
        /// デバイスにメッセージを送信する
        /// </summary>
        /// <param name="deviceConnectionInfo">デバイス接続情報</param>
        /// <param name="messageBody">メッセージ</param>
        /// <remarks>エラーが発生した場合には例外を投げる</remarks>
        /// <returns>Task</returns>
        Task SendMessageAsync(DeviceConnectionInfo deviceConnectionInfo, string messageBody);

        /// <summary>
        /// デバイスツインを取得する @TBD Exception??
        /// </summary>
        /// <param name="deviceConnectionInfo">デバイス接続情報</param>
        /// <returns>デバイスツイン</returns>
        Task<Twin> GetDeviceTwin(DeviceConnectionInfo deviceConnectionInfo);

        /// <summary>
        /// デバイスツインのdesiredプロパティを更新する @TBD
        /// </summary>
        /// <param name="deviceTwin">更新対象デバイスツイン</param>
        /// <param name="deviceConnectionInfo">デバイス接続情報</param>
        /// <param name="messageBody">desiredプロパティに設定するJSON文字列</param>
        /// <returns>非同期実行タスク</returns>
        /// <remarks>
        /// - プロパティに設定するJSON文字列の正当性はService層でチェックすること
        /// - エラーが発生した場合には例外を投げる
        /// </remarks>
        Task UpdateDeviceTwinDesiredProperties(Twin deviceTwin, DeviceConnectionInfo deviceConnectionInfo, string messageBody);
    }
}
