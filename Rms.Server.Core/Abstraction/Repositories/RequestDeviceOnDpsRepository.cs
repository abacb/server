using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Provisioning.Service;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Logging;
using Rms.Server.Core.Abstraction.Models;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Core.Utility.Extensions;
using Rms.Server.Core.Utility.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// RequestDeviceOnDpsRepository
    /// </summary>
    public class RequestDeviceOnDpsRepository : IRequestDeviceRepository
    {
        /// <summary>
        /// アプリケーション設定上のIoTHub接続文字列のKey名
        /// </summary>
        private const string IoTHubNamePrefixOnAppSettings = "IotHubConnectionString_{0}";

        /// <summary>
        /// IotHubへのメッセージ送信成功ステータス
        /// </summary>
        private const int StatusOK = 200;

        /// <summary>
        /// アプリケーション設定
        /// </summary>
        private readonly AppSettings _appSettings;

        /// <summary>
        /// ロガー
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// IotHubPolly
        /// </summary>
        private readonly IotHubPolly _iotHubPolly;

        /// <summary>
        /// DpsPolly
        /// </summary>
        private readonly DpsPolly _dpsPolly;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="iotHubPolly">DpsPolly</param>
        /// <param name="dpsPolly">IotHubPolly</param>
        /// <param name="appSettings">アプリケーション設定</param>
        public RequestDeviceOnDpsRepository(
            ILogger<RequestDeviceOnDpsRepository> logger,
            IotHubPolly iotHubPolly,
            DpsPolly dpsPolly,
            AppSettings appSettings)
        {
            Assert.IfNull(logger);
            Assert.IfNull(iotHubPolly);
            Assert.IfNull(dpsPolly);
            Assert.IfNull(appSettings);

            _logger = logger;
            _iotHubPolly = iotHubPolly;
            _dpsPolly = dpsPolly;
            _appSettings = appSettings;
        }

        /// <summary>
        /// デバイスにメッセージを送信する
        /// </summary>
        /// <param name="deviceConnectionInfo">デバイス接続情報</param>
        /// <param name="messageBody">メッセージ</param>
        /// <remarks>エラーが発生した場合には例外を投げる</remarks>
        /// <returns>成功:true 失敗:falseを返す</returns>
        public async Task SendMessageAsync(DeviceConnectionInfo deviceConnectionInfo, string messageBody)
        {
            System.Diagnostics.Debug.Assert(deviceConnectionInfo != null, "deviceConnectionInfo != null");
            System.Diagnostics.Debug.Assert(deviceConnectionInfo?.EdgeId != null, "deviceConnectionInfo?.EdgeId != null");
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(deviceConnectionInfo?.IotHubConnectionString.Key), "!string.IsNullOrEmpty(deviceConnectionInfo?.IotHubConnectionString.Key)");
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(deviceConnectionInfo?.IotHubConnectionString.Value), "!string.IsNullOrEmpty(deviceConnectionInfo?.IotHubConnectionString.Value)");

            bool result = false;

            try
            {
                _logger.EnterJson("{0}", new { deviceConnectionInfo, messageBody });

                // メッセージを変換する
                var message = new Message(Encoding.UTF8.GetBytes(messageBody));
                using (var serviceClient = ServiceClient.CreateFromConnectionString(deviceConnectionInfo.IotHubConnectionString.Value))
                {
                    // メッセージの送信を依頼する
                    await _iotHubPolly.ExecuteAsync(
                        async () =>
                        {
                            await serviceClient.SendAsync(deviceConnectionInfo.EdgeId.ToString(), message);
                        });
                }

                result = true;
            }
            catch (FormatException ex)
            {
                // 設定した接続文字列のフォーマットが不正
                throw new RmsInvalidAppSettingException(ErrorMessage.InvalidFormat(deviceConnectionInfo.EdgeId, deviceConnectionInfo.IotHubConnectionString.Key), ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                // 接続文字列の認証が通らない
                throw new RmsException(ErrorMessage.UnauthroizedAccess(deviceConnectionInfo.EdgeId, deviceConnectionInfo.IotHubConnectionString.Key), ex);
            }
            catch (Exception ex)
            {
                // その他例外
                throw new RmsException(ErrorMessage.Others(deviceConnectionInfo.EdgeId, deviceConnectionInfo.IotHubConnectionString.Key), ex);
            }
            finally
            {
                _logger.LeaveJson("{0}", new { result });
            }
        }

        /// <summary>
        /// デバイスツインを取得する
        /// </summary>
        /// <param name="deviceConnectionInfo">デバイス接続情報</param>
        /// <returns>デバイスツイン</returns>
        public async Task<Twin> GetDeviceTwin(DeviceConnectionInfo deviceConnectionInfo)
        {
            System.Diagnostics.Debug.Assert(deviceConnectionInfo != null, "deviceConnectionInfo != null");
            System.Diagnostics.Debug.Assert(deviceConnectionInfo?.EdgeId != null, "deviceConnectionInfo?.EdgeId != null");
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(deviceConnectionInfo?.IotHubConnectionString.Key), "!string.IsNullOrEmpty(deviceConnectionInfo?.IotHubConnectionString.Key)");
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(deviceConnectionInfo?.IotHubConnectionString.Value), "!string.IsNullOrEmpty(deviceConnectionInfo?.IotHubConnectionString.Value)");

            Twin deviceTwin = null;

            try
            {
                _logger.EnterJson("{0}", new { deviceConnectionInfo });

                using (var registryManager = RegistryManager.CreateFromConnectionString(deviceConnectionInfo.IotHubConnectionString.Value))
                {
                    deviceTwin = await registryManager.GetTwinAsync(deviceConnectionInfo.EdgeId.ToString());
                }
            }
            catch (FormatException ex)
            {
                // 設定した接続文字列のフォーマットが不正
                throw new RmsInvalidAppSettingException(ErrorMessage.InvalidFormat(deviceConnectionInfo.EdgeId, deviceConnectionInfo.IotHubConnectionString.Key), ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                // 接続文字列の認証が通らない
                throw new RmsException(ErrorMessage.UnauthroizedAccess(deviceConnectionInfo.EdgeId, deviceConnectionInfo.IotHubConnectionString.Key), ex);
            }
            catch (Exception ex)
            {
                // その他例外
                throw new RmsException(ErrorMessage.Others(deviceConnectionInfo.EdgeId, deviceConnectionInfo.IotHubConnectionString.Key), ex);
            }
            finally
            {
                _logger.LeaveJson("{0}", new { deviceTwin });
            }

            return deviceTwin;
        }

        /// <summary>
        /// デバイスツインのdesiredプロパティを更新する
        /// </summary>
        /// <param name="deviceTwin">更新対象デバイスツイン</param>
        /// <param name="deviceConnectionInfo">デバイス接続情報</param>
        /// <param name="messageBody">desiredプロパティに設定するJSON文字列</param>
        /// <returns>非同期実行タスク</returns>
        /// <remarks>
        /// - プロパティに設定するJSON文字列の正当性はService層でチェックすること
        /// - エラーが発生した場合には例外を投げる
        /// </remarks>
        public async Task UpdateDeviceTwinDesiredProperties(Twin deviceTwin, DeviceConnectionInfo deviceConnectionInfo, string messageBody)
        {
            System.Diagnostics.Debug.Assert(deviceConnectionInfo != null, "deviceConnectionInfo != null");
            System.Diagnostics.Debug.Assert(deviceConnectionInfo?.EdgeId != null, "deviceConnectionInfo?.EdgeId != null");
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(deviceConnectionInfo?.IotHubConnectionString.Key), "!string.IsNullOrEmpty(deviceConnectionInfo?.IotHubConnectionString.Key)");
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(deviceConnectionInfo?.IotHubConnectionString.Value), "!string.IsNullOrEmpty(deviceConnectionInfo?.IotHubConnectionString.Value)");
            System.Diagnostics.Debug.Assert(deviceTwin != null, "deviceTwin != null");

            bool result = false;

            try
            {
                _logger.EnterJson("{0}", new { deviceTwin, deviceConnectionInfo });

                using (var registryManager = RegistryManager.CreateFromConnectionString(deviceConnectionInfo.IotHubConnectionString.Value))
                {
                    // メッセージの送信を依頼する
                    await _iotHubPolly.ExecuteAsync(
                        async () =>
                        {
                            await registryManager.UpdateTwinAsync(deviceTwin.DeviceId, messageBody, deviceTwin.ETag);
                        });
                }

                result = true;
            }
            catch (FormatException ex)
            {
                // 設定した接続文字列のフォーマットが不正
                throw new RmsInvalidAppSettingException(ErrorMessage.InvalidFormat(deviceConnectionInfo.EdgeId, deviceConnectionInfo.IotHubConnectionString.Key), ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                // 接続文字列の認証が通らない
                throw new RmsException(ErrorMessage.UnauthroizedAccess(deviceConnectionInfo.EdgeId, deviceConnectionInfo.IotHubConnectionString.Key), ex);
            }
            catch (Exception ex)
            {
                // その他例外
                throw new RmsException(ErrorMessage.Others(deviceConnectionInfo.EdgeId, deviceConnectionInfo.IotHubConnectionString.Key), ex);
            }
            finally
            {
                _logger.LeaveJson("{0}", new { result });
            }
        }

        /// <summary>
        /// デバイス接続情報を取得する。
        /// </summary>
        /// <param name="edgeId">端末ID</param>
        /// <returns>デバイス接続情報を返す。対象デバイスが見つからない場合nullを返す。</returns>
        public async Task<DeviceConnectionInfo> GetDeviceConnectionInfoAsync(Guid edgeId)
        {
            DeviceConnectionInfo result = null;
            string connectionStringDps = null;

            try
            {
                _logger.Enter($"{nameof(edgeId)}: {{0}}", new object[] { edgeId });

                // DPS接続文字列取得
                connectionStringDps = _appSettings.DpsConnectionString;
                if (string.IsNullOrWhiteSpace(connectionStringDps))
                {
                    throw new RmsInvalidAppSettingException("DpsConnectionString is required.");
                }

                string iotHubName = null;

                using (var service = ProvisioningServiceClient.CreateFromConnectionString(connectionStringDps))
                {
                    await _dpsPolly.ExecuteAsync(
                        async () =>
                        {
                            // 見つからなかったらProvisioningServiceClientHttpExceptionが発生して、
                            // その例外のStatusCodeにHttpStatusCode型のNotFoundになる。
                            // これ以外はとりあえず500にする。
                            DeviceRegistrationState deviceRegistrationState = await service.GetDeviceRegistrationStateAsync(edgeId.ToString());
                            iotHubName = deviceRegistrationState.AssignedHub;
                        });
                }

                string iotHubConnectionStringKey = string.Format(IoTHubNamePrefixOnAppSettings, iotHubName);

                // IoTHub接続文字列取得
                string iotHubConnectionStringValue = _appSettings.GetConnectionString(iotHubConnectionStringKey);
                if (string.IsNullOrWhiteSpace(iotHubConnectionStringValue))
                {
                    throw new RmsInvalidAppSettingException(string.Format("{0} is required.", iotHubConnectionStringKey));
                }

                result = new DeviceConnectionInfo()
                {
                    EdgeId = edgeId,
                    IotHubConnectionString = new KeyValuePair<string, string>(iotHubConnectionStringKey, iotHubConnectionStringValue),
                };
                return result;
            }
            catch (RmsException)
            {
                throw;
            }
            catch (ArgumentException ex)
            {
                // 接続文字列がフォーマットに則っていない。
                throw new RmsInvalidAppSettingException("DpsConnectionString is invalid format.", ex);
            }
            catch (ProvisioningServiceClientHttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // 対象デバイスが見つからない
                result = null;
                return result;
            }
            catch (ProvisioningServiceClientHttpException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                // 使用した接続文字列で認証が通らなかった
                throw new RmsException(string.Format("DPSで認証エラーが発生しました。({0})", connectionStringDps), ex);
            }
            catch (Exception ex)
            {
                throw new RmsException("接続先のIoT Hub", ex);
            }
            finally
            {
                _logger.LeaveJson("{0}", new { result });
            }
        }

        /// <summary>
        /// SendMessageAsync/SendDirectMessageAsync 共通のエラーメッセージ定義クラス
        /// </summary>
        private class ErrorMessage
        {
            /// <summary>
            /// 不正なフォーマットの場合のエラーメッセージ文字列を返す
            /// </summary>
            /// <param name="edgeId">EdgeId</param>
            /// <param name="target">接続文字列キー</param>
            /// <returns>エラーメッセージ</returns>
            internal static string InvalidFormat(Guid edgeId, string target)
            {
                return string.Format("{0} is invalid format. (EdgeId: {1})", target, edgeId);
            }

            /// <summary>
            /// IoT Hub認証エラーの場合のエラーメッセージ文字列を返す
            /// </summary>
            /// <param name="edgeId">EdgeId</param>
            /// <param name="connectionStringKey">接続文字列キー</param>
            /// <returns>エラーメッセージ</returns>
            internal static string UnauthroizedAccess(Guid edgeId, string connectionStringKey)
            {
                return string.Format("IoT Hubで認証エラーが発生しました。(EdgeId: {0}, Key: {1})", edgeId, connectionStringKey);
            }

            /// <summary>
            /// その他のメッセージ送信時エラー
            /// </summary>
            /// <param name="edgeId">edgeId</param>
            /// <param name="connectionStringKey">接続文字列キー</param>
            /// <remarks>EdgeIdと接続文字列キーを渡す</remarks>
            /// <returns>EdgeIdと接続文字列キー,エラーメッセージを返す</returns>
            internal static string Others(Guid edgeId, string connectionStringKey)
            {
                return string.Format("IoT Hubへのメッセージ送信時にエラーが発生しました。(EdgeId: {0}, Key: {1})", edgeId, connectionStringKey);
            }
        }
    }
}
