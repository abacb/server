using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Operation.Utility.Models;
using Rms.Server.Utility.Abstraction.Repositories;
using Rms.Server.Utility.Utility;
using Rms.Server.Utility.Utility.Extensions;
using Rms.Server.Utility.Utility.Models;
using Rms.Server.Utility.Utility.Properties;
using System;
using System.Collections.Generic;

namespace Rms.Server.Utility.Service.Services
{
    /// <summary>
    /// DeviceConnectionMonitorService
    /// </summary>
    public class DeviceConnectionMonitorService : IDeviceConnectionMonitorService
    {
        /// <summary>
        /// 設定
        /// </summary>
        private readonly UtilityAppSettings _settings;

        /// <summary>
        /// ロガー
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// DateTimeの提供元
        /// </summary>
        private readonly ITimeProvider _timeProvider;

        /// <summary>
        /// 端末データテーブルリポジトリ
        /// </summary>
        private readonly IDtDeviceRepository _dtDeviceRepository;

        /// <summary>
        /// 通信監視アラーム定義リポジトリ
        /// </summary>
        private readonly IDtAlarmDefConnectionMonitorRepository _dtAlarmDefConnectionMonitorRepository;

        /// <summary>
        /// Queueリポジトリ
        /// </summary>
        private readonly IQueueRepository _queueRepository;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">設定</param>
        /// <param name="logger">ロガー</param>
        /// <param name="timeProvider">DateTimeの提供元</param>
        /// <param name="dtDeviceRepository">端末データテーブルリポジトリ</param>
        /// <param name="dtAlarmDefConnectionMonitorRepository">通信監視アラーム定義リポジトリ</param>
        /// <param name="queueRepository">Queueリポジトリ</param>
        public DeviceConnectionMonitorService(
            UtilityAppSettings settings,
            ILogger<DeviceConnectionMonitorService> logger,
            ITimeProvider timeProvider,
            IDtDeviceRepository dtDeviceRepository,
            IDtAlarmDefConnectionMonitorRepository dtAlarmDefConnectionMonitorRepository,
            IQueueRepository queueRepository)
        {
            Assert.IfNull(settings);
            Assert.IfNull(logger);
            Assert.IfNull(timeProvider);
            Assert.IfNull(dtDeviceRepository);
            Assert.IfNull(dtAlarmDefConnectionMonitorRepository);
            Assert.IfNull(queueRepository);

            _settings = settings;
            _logger = logger;
            _timeProvider = timeProvider;
            _dtDeviceRepository = dtDeviceRepository;
            _dtAlarmDefConnectionMonitorRepository = dtAlarmDefConnectionMonitorRepository;
            _queueRepository = queueRepository;
        }

        /// <summary>
        /// 接続状況を取得する
        /// </summary>
        /// <param name="devices">端末データ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        public bool ReadDeviceConnect(out IEnumerable<DtDevice> devices)
        {
            devices = null;

            try
            {
                _logger.Enter();

                // Sq1.1: サーバ／クライアント間の接続状況を取得する
                devices = _dtDeviceRepository.ReadDtDevice();
                return true;
            }
            catch (RmsException e)
            {
                // DBにアクセスできない or DBに情報が見つからない（基本設計書 5.2.1.1 エラー処理）
                _logger.Error(e, nameof(Resources.UT_DCM_DCM_002));
                return false;
            }
            finally
            {
                _logger.LeaveJson("{0}", new { devices });
            }
        }

        /// <summary>
        /// アラーム定義を取得する
        /// </summary>
        /// <param name="devices">端末データ</param>
        /// <param name="alarmJudgementTargets">端末データと対応するアラーム定義</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        public bool ReadAlarmDefinition(IEnumerable<DtDevice> devices, out List<Tuple<DtDevice, DtAlarmDefConnectionMonitor>> alarmJudgementTargets)
        {
            alarmJudgementTargets = null;
            bool result = true;

            try
            {
                _logger.EnterJson("{0}", new { devices });

                alarmJudgementTargets = new List<Tuple<DtDevice, DtAlarmDefConnectionMonitor>>();

                foreach (var device in devices)
                {
                    try
                    {
                        var targets = new List<string>();

                        if (device.TemperatureSensor == Utility.Const.InventorySettingInfoOptionOn)
                        {
                            targets.Add(Utility.Const.AlarmDefTargetTemperature);
                        }

                        if (device.Dxa == Utility.Const.InventorySettingInfoOptionOn)
                        {
                            targets.Add(Utility.Const.AlarmDefTargetDxa);
                        }

                        if (targets.Count <= 0)
                        {
                            targets.Add(string.Empty);
                        }

                        // Sq1.2: 通信監視アラーム定義を取得する
                        var alarmDefs = _dtAlarmDefConnectionMonitorRepository.ReadDtAlarmDefConnectionMonitor(device.TypeCode, targets);

                        foreach (var alarmDef in alarmDefs)
                        {
                            alarmJudgementTargets.Add(new Tuple<DtDevice, DtAlarmDefConnectionMonitor>(device, alarmDef));
                        }
                    }
                    catch (RmsException e)
                    {
                        // DBにアクセスできない（基本設計書 5.2.1.1 エラー処理）
                        _logger.Error(e, nameof(Resources.UT_DCM_DCM_003), new object[] { device.Sid });
                        result = false;
                    }
                }

                return result;
            }
            catch
            {
                throw; // 想定外（再スローする）
            }
            finally
            {
                _logger.LeaveJson("{0}", new { alarmJudgementTargets });
            }
        }

        /// <summary>
        /// アラーム生成対象データを作成する
        /// </summary>
        /// <param name="alarmJudgementTargets">アラーム判定対象データ</param>
        /// <param name="alarmCreationTargets">アラーム生成対象データ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        public bool CreateAlarmCreationTarget(IReadOnlyCollection<Tuple<DtDevice, DtAlarmDefConnectionMonitor>> alarmJudgementTargets, out List<Tuple<DtDevice, DtAlarmDefConnectionMonitor>> alarmCreationTargets)
        {
            bool result = true;
            _logger.EnterJson("{0}", new { alarmJudgementTargets });

            alarmCreationTargets = new List<Tuple<DtDevice, DtAlarmDefConnectionMonitor>>();

            foreach (var judgementTarget in alarmJudgementTargets)
            {
                try
                {
                    (var device, var alarmDef) = judgementTarget;

                    if (device.ConnectStatusCode != Utility.Const.ConnectStatusConnected && device.ConnectStatusCode != Utility.Const.ConnectStatusUnconnected)
                    {
                        if (device.ConnectUpdateDatetime != null)
                        {
                            TimeSpan disconnectedTime = _timeProvider.UtcNow - (DateTime)device.ConnectUpdateDatetime;
                            int disconnectionDays = disconnectedTime.Days;

                            if (JudgeAlarm(disconnectionDays, alarmDef))
                            {
                                var alarmTargetDevice = new DtDevice();
                                alarmTargetDevice = device;
                                alarmTargetDevice.DisconnectionDays = disconnectionDays;
                                alarmTargetDevice.LastConnectDateTime = device.ConnectUpdateDatetime;
                                alarmTargetDevice.AlarmJudgementTime = _timeProvider.UtcNow.ToString();

                                var alarmTarget = new Tuple<DtDevice, DtAlarmDefConnectionMonitor>(alarmTargetDevice, alarmDef);
                                alarmCreationTargets.Add(alarmTarget);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    // アラーム生成エラー（基本設計書 5.2.1.1 エラー処理）
                    _logger.Error(e, nameof(Resources.UT_DCM_DCM_005), new object[] { judgementTarget?.Item1?.Sid });
                    result = false;
                }
            }

            _logger.LeaveJson("{0}", new { alarmCreationTargets });
            return result;
        }

        /// <summary>
        /// アラーム情報を作成しQueueStorageへ登録する
        /// </summary>
        /// <param name="alarmCreationTargets">アラーム対象データ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        public bool CreateAndEnqueueAlarmInfo(List<Tuple<DtDevice, DtAlarmDefConnectionMonitor>> alarmCreationTargets)
        {
            bool result = true;
            _logger.EnterJson("{0}", new { alarmCreationTargets });

            foreach (var alarmTarget in alarmCreationTargets)
            {
                string message = null;
                try
                {
                    (var device, var alarmDef) = alarmTarget;

                    string alarmDescription = alarmDef.AlarmDescription;
                    if (!string.IsNullOrEmpty(alarmDescription))
                    {
                        if (alarmDescription.Contains("{0}"))
                        {
                            if (alarmDescription.Contains("{1}"))
                            {
                                alarmDescription = string.Format(alarmDescription, device.DisconnectionDays, device.LastConnectDateTime);
                            }
                            else
                            {
                                alarmDescription = string.Format(alarmDescription, device.DisconnectionDays);
                            }
                        }
                    }

                    // Sq1.4: アラームキューを生成する
                    var alarmInfo = new AlarmInfo
                    {
                        SourceEquipmentUid = device.EquipmentUid,
                        TypeCode = device.TypeCode,
                        ErrorCode = alarmDef.AnalysisResultErrorCode,
                        AlarmLevel = alarmDef.AlarmLevel,
                        AlarmTitle = alarmDef.AlarmTitle,
                        AlarmDescription = alarmDescription,
                        AlarmDatetime = _timeProvider.UtcNow.ToString(Utility.Const.AlarmQueueDateTimeFormat),
                        EventDatetime = device.AlarmJudgementTime,
                        AlarmDefId = $"{_settings.SystemName}_{_settings.SubSystemName}_{alarmDef.Sid.ToString()}",
                        MessageId = null
                    };

                    message = JsonConvert.SerializeObject(alarmInfo);

                    // Sq1.5: キューを登録する
                    _queueRepository.SendMessageToAlarmQueue(message);
                }
                catch (Exception e)
                {
                    // アラーム生成エラー or アラームキューにアラーム情報を送信できない（基本設計書 5.2.1.1 エラー処理）
                    _logger.Error(e, string.IsNullOrEmpty(message) ? nameof(Resources.UT_DCM_DCM_005) : nameof(Resources.UT_DCM_DCM_006), new object[] { alarmTarget?.Item1?.Sid });
                    result = false;
                }
            }

            _logger.Leave();
            return result;
        }

        /// <summary>
        /// アラーム判定を行う
        /// </summary>
        /// <param name="disconnectionDays">接続断日数</param>
        /// <param name="alarmDef">アラーム定義</param>
        /// <returns>アラーム対象の場合true、アラーム対象外の場合falseを返す</returns>
        private bool JudgeAlarm(int disconnectionDays, DtAlarmDefConnectionMonitor alarmDef)
        {
            bool isAlarmTarget = false;

            if (alarmDef.ValueEqualFrom == true)
            {
                if (disconnectionDays >= alarmDef.ValueFrom)
                {
                    isAlarmTarget = true;
                }
            }
            else
            {
                if (disconnectionDays > alarmDef.ValueFrom)
                {
                    isAlarmTarget = true;
                }
            }

            // 値(To)が設定された定義はアラーム定義.xlsmに存在しないが将来追加される可能性があるため検索できるようにしておく
            if (alarmDef.ValueTo != null && isAlarmTarget == true)
            {
                if (alarmDef.ValueEqualTo == true)
                {
                    if (disconnectionDays > alarmDef.ValueTo)
                    {
                        isAlarmTarget = false;
                    }
                }
                else
                {
                    if (disconnectionDays >= alarmDef.ValueTo)
                    {
                        isAlarmTarget = false;
                    }
                }
            }

            return isAlarmTarget;
        }
    }
}
