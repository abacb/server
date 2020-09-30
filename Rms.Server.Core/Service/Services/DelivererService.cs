using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rms.Server.Core.Abstraction.Models;
using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Core.Service.Models;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Core.Utility.Extensions;
using Rms.Server.Core.Utility.Models.Entites;
using Rms.Server.Core.Utility.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Rms.Server.Core.Utility.Const;

namespace Rms.Server.Core.Service.Services
{
    /// <summary>
    /// DelivererService
    /// </summary>
    public class DelivererService : IDelivererService
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// 配信グループリポジトリ
        /// </summary>
        private readonly IDtDeliveryGroupRepository _deliveryGroupRepository;

        /// <summary>
        /// 端末データリポジトリ
        /// </summary>
        private readonly IDtDeviceRepository _deviceRepository;

        /// <summary>
        /// 適用結果リポジトリ
        /// </summary>
        private readonly IDtInstallResultRepository _installResultRepository;

        /// <summary>
        /// デバイス情報リポジトリ
        /// </summary>
        private readonly IRequestDeviceRepository _requestDeviceRepository;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="deliveryGroupRepository">配信グループリポジトリ</param>
        /// <param name="deviceRepository">端末データリポジトリ</param>
        /// <param name="installResultRepository">適用結果リポジトリ</param>
        /// <param name="requestDeviceRepository">デバイス情報リポジトリ</param>
        public DelivererService(
            ILogger<DelivererService> logger,
            IDtDeliveryGroupRepository deliveryGroupRepository,
            IDtDeviceRepository deviceRepository,
            IDtInstallResultRepository installResultRepository,
            IRequestDeviceRepository requestDeviceRepository)
        {
            Assert.IfNull(logger);
            Assert.IfNull(deliveryGroupRepository);
            Assert.IfNull(deviceRepository);
            Assert.IfNull(installResultRepository);
            Assert.IfNull(requestDeviceRepository);

            _logger = logger;
            _deliveryGroupRepository = deliveryGroupRepository;
            _deviceRepository = deviceRepository;
            _installResultRepository = installResultRepository;
            _requestDeviceRepository = requestDeviceRepository;
        }

        /// <summary>
        /// 配信処理開始
        /// </summary>
        public void StartDelivery()
        {
            try
            {
                _logger.Enter();

                // Sq1.1: 開始可能な配信グループを取得する(現在時刻)
                // 配信結果・配信グループステータス・配信ファイルのデータを持って取得
                var groups = ReadStartableDtDeliveryGroupWrapper();

                foreach (var group in groups)
                {
                    try
                    {
                        // Sq1.2: 開始可能な配信グループの配信結果ステータスを更新する(started)
                        var updatedGroup = UpdateDtDeliveryGroupStatusStartedWrapper(group.Sid);
                        if (updatedGroup == null)
                        {
                            continue;
                        }

                        // Sq1.3: 配信グループに属し、オンラインとなっているゲートウェイ機器を取得する
                        var onlineGatewayDevices = ReadDtDeviceOnlineGatewayWrapper(group);
                        if (!onlineGatewayDevices.Any())
                        {
                            continue;
                        }

                        // 配信グループデータを基に、親子エンティティのデータをもった配信グループを取得する
                        var includedGroup = _deliveryGroupRepository.ReadDeliveryIncludedDtDeliveryGroup(group.Sid);
                        if (includedGroup == null)
                        {
                            continue;
                        }

                        // メッセージオブジェクトの生成(配信対象以外)
                        var messageObject = RequestDelivery.CreateDeliveryMessageObject(includedGroup);

                        foreach (var gatewayDevice in onlineGatewayDevices)
                        {
                            try
                            {
                                // 配信対象の設定
                                messageObject.Targets = includedGroup.DtDeliveryResult
                                    ?.Where(x => x.GwDeviceSid == gatewayDevice.Sid)
                                    .Select(x => new RequestDelivery.Target() { DeliveryResultSID = x.Sid.ToString(), EquipmentUID = x.DtDevice1.EquipmentUid })
                                    .ToArray();

                                // Sq1.4: 配信メッセージを生成する
                                var deliveryMessage = CreateDeliveryMessageJson(messageObject, gatewayDevice);
                                if (deliveryMessage == null)
                                {
                                    continue;
                                }

                                // メッセージ送信
                                if (!SendMessageAsync(gatewayDevice, deliveryMessage).Result)
                                {
                                    // 送信失敗時は次のゲートウェイ機器の処理に移る
                                    continue;
                                }

                                // Sq1.7:ゲートウェイ機器を持つ配信機器の配信結果ステータスを更新する(messagesent)
                                var installResults = CreateDtInstallResultStatusSentWrapper(includedGroup, gatewayDevice);
                                if (!installResults.Any())
                                {
                                    continue;
                                }

                                // メッセージ送信処理の正常終了をログ出力
                                _logger.Info(nameof(Resources.CO_DLV_DLV_017), new object[] { gatewayDevice.EdgeId, gatewayDevice.EquipmentUid });
                            }
                            catch (Exception e)
                            {
                                // その他箇所でエラー
                                _logger.Error(e, nameof(Resources.CO_DLV_DLV_003), new object[] { gatewayDevice.EdgeId, gatewayDevice.EquipmentUid, e.Message });
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        // その他箇所でエラー
                        _logger.Error(e, nameof(Resources.CO_DLV_DLV_002), new object[] { group.Sid, e.Message });
                    }
                }
            }
            finally
            {
                _logger.Leave();
            }
        }

        /// <summary>
        /// 開始可能な配信グループを取得する(ラッパー)
        /// </summary>
        /// <returns>開始可能な配信グループ</returns>
        private IEnumerable<DtDeliveryGroup> ReadStartableDtDeliveryGroupWrapper()
        {
            IEnumerable<DtDeliveryGroup> models = new List<DtDeliveryGroup>();
            try
            {
                models = _deliveryGroupRepository.ReadStartableDtDeliveryGroup();
            }
            catch (Exception e)
            {
                // Sq1.1
                _logger.Error(e, nameof(Resources.CO_DLV_DLV_009), new object[] { e.Message });
            }

            return models;
        }

        /// <summary>
        /// 開始可能な配信グループの配信結果ステータスをstartedにする(ラッパー)
        /// </summary>
        /// <param name="sid">配信グループSID</param>
        /// <returns>更新されたDBデータ</returns>
        private DtDeliveryGroup UpdateDtDeliveryGroupStatusStartedWrapper(long sid)
        {
            DtDeliveryGroup model = null;
            try
            {
                model = _deliveryGroupRepository.UpdateDtDeliveryGroupStatusStarted(sid);
            }
            catch (Exception e)
            {
                // Sq1.2
                _logger.Error(e, nameof(Resources.CO_DLV_DLV_010), new object[] { sid, e.Message });
            }

            return model;
        }

        /// <summary>
        /// 配信グループに属し、オンラインとなっているゲートウェイ機器を取得する(ラッパー)
        /// </summary>
        /// <param name="group">配信グループ</param>
        /// <returns>オンラインなゲートウェイ機器</returns>
        private IEnumerable<DtDevice> ReadDtDeviceOnlineGatewayWrapper(DtDeliveryGroup group)
        {
            IEnumerable<DtDevice> models = new List<DtDevice>();
            try
            {
                models = _deviceRepository.ReadDtDeviceOnlineGateway(group);
            }
            catch (Exception e)
            {
                // Sq1.3
                _logger.Error(e, nameof(Resources.CO_DLV_DLV_011), new object[] { group.Sid, e.Message });
            }

            return models;
        }

        /// <summary>
        /// 配信メッセージを作成する
        /// </summary>
        /// <param name="messageObject">配信メッセージオブジェクト</param>
        /// <param name="gatewayDevice">ゲートウェイ機器</param>
        /// <returns>配信メッセージ</returns>
        private string CreateDeliveryMessageJson(RequestDelivery messageObject, DtDevice gatewayDevice)
        {
            string deliveryMessage = null;
            try
            {
                Assert.IfNull(messageObject);

                deliveryMessage = JsonConvert.SerializeObject(messageObject);

                _logger.Debug("message: {0}", new object[] { deliveryMessage });
            }
            catch (Exception e)
            {
                // Sq1.4
                _logger.Error(e, nameof(Resources.CO_DLV_DLV_012), new object[] { gatewayDevice.EdgeId, gatewayDevice.EquipmentUid, e.Message });
            }
            
            return deliveryMessage;
        }

        /// <summary>
        /// メッセージを送信する
        /// </summary>
        /// <param name="gatewayDevice">ゲートウェイ機器</param>
        /// <param name="deliveryMessage">配信メッセージ</param>
        /// <returns>送信成功/失敗</returns>
        private async Task<bool> SendMessageAsync(DtDevice gatewayDevice, string deliveryMessage)
        {
            // Sq1.5: ゲートウェイ機器が接続中のIoT Hubに問い合わせる
            var deveiceConnectionInfo = await GetDeviceConnectionInfoAsyncWrapper(gatewayDevice);
            if (deveiceConnectionInfo == null)
            {
                return false;
            }

            // Sq1.6:ゲートウェイ機器にメッセージを送信する
            var sendResult = await SendMessageAsyncWrapper(deveiceConnectionInfo, deliveryMessage, gatewayDevice);

            return sendResult;
        }

        /// <summary>
        /// デバイス接続情報を取得する(ラッパー)
        /// </summary>
        /// <param name="gatewayDevice">ゲートウェイ機器</param>
        /// <returns>デバイス接続情報</returns>
        private async Task<DeviceConnectionInfo> GetDeviceConnectionInfoAsyncWrapper(DtDevice gatewayDevice)
        {
            DeviceConnectionInfo deveiceConnectionInfo = null;
            try
            {
                deveiceConnectionInfo = await _requestDeviceRepository.GetDeviceConnectionInfoAsync(gatewayDevice.EdgeId);
                if (deveiceConnectionInfo == null)
                {
                    // Sq1.5(デバイスが見つからない場合)
                    throw new RmsException("IoTHubの接続文字列が見つかりません");
                }
            }
            catch (Exception e)
            {
                // Sq1.5
                _logger.Error(e, nameof(Resources.CO_DLV_DLV_013), new object[] { gatewayDevice.EdgeId, gatewayDevice.EquipmentUid, e.Message });
            }

            return deveiceConnectionInfo;
        }

        /// <summary>
        /// ゲートウェイ機器にメッセージを送信する(ラッパー)
        /// </summary>
        /// <param name="deveiceConnectionInfo">デバイス接続情報</param>
        /// <param name="deliveryMessage">配信メッセージ</param>
        /// <param name="gatewayDevice">ゲートウェイ機器</param>
        /// <returns>送信依頼処理成功/失敗</returns>
        private async Task<bool> SendMessageAsyncWrapper(DeviceConnectionInfo deveiceConnectionInfo, string deliveryMessage, DtDevice gatewayDevice)
        {
            bool ret = false;
            try
            {
                await _requestDeviceRepository.SendMessageAsync(deveiceConnectionInfo, deliveryMessage);
                ret = true;
            }
            catch (Exception e)
            {
                // Sq1.6
                _logger.Error(e, nameof(Resources.CO_DLV_DLV_015), new object[] { gatewayDevice.EdgeId, gatewayDevice.EquipmentUid, e.Message });
            }

            return ret;
        }

        /// <summary>
        /// ゲートウェイ機器を持つ配信機器の配信結果ステータスを更新する(messagesent)(ラッパー)
        /// </summary>
        /// <param name="includedGroup">配信結果情報をもつ配信グループ</param>
        /// <param name="gatewayDevice">ゲートウェイ機器</param>
        /// <returns>更新した適用結果</returns>
        private IEnumerable<DtInstallResult> CreateDtInstallResultStatusSentWrapper(DtDeliveryGroup includedGroup, DtDevice gatewayDevice)
        {
            IEnumerable<DtInstallResult> models = new List<DtInstallResult>();
            try
            {
                models = _installResultRepository.CreateDtInstallResultStatusSent(includedGroup, gatewayDevice.Sid);
            }
            catch (Exception e)
            {
                // Sq1.7
                _logger.Error(e, nameof(Resources.CO_DLV_DLV_016), new object[] { gatewayDevice.EdgeId, gatewayDevice.EquipmentUid, e.Message });
            }

            return models;
        }
    }
}
