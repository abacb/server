using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Utility.Service.Services;
using Rms.Server.Utility.Utility;
using Rms.Server.Utility.Utility.Extensions;
using Rms.Server.Utility.Utility.Models;
using Rms.Server.Utility.Utility.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rms.Server.Utility.Azure.Functions.ConnectionMonitor
{
    /// <summary>
    /// 通信監視アプリ(端末データテーブル監視)
    /// </summary>
    public class DeviceConnectionMonitorController
    {
        /// <summary>
        /// 設定
        /// </summary>
        private readonly UtilityAppSettings _settings;

        /// <summary>
        /// 端末データテーブル監視サービス
        /// </summary>
        private readonly IDeviceConnectionMonitorService _service;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">設定</param>
        /// <param name="service">端末データテーブル監視サービスクラス</param>
        public DeviceConnectionMonitorController(UtilityAppSettings settings, IDeviceConnectionMonitorService service)
        {
            Assert.IfNull(settings);
            Assert.IfNull(service);

            _settings = settings;
            _service = service;
        }

        /// <summary>
        /// 端末データテーブルの監視を実行する
        /// </summary>
        /// <remarks>sd 07-11.通信監視</remarks>
        /// <param name="timerInfo">タイマー情報</param>
        /// <param name="log">ロガー</param>
        [FunctionName("DeviceConnectionMonitor")]
        public void MonitorDeviceConnectionTable([TimerTrigger("0 0 16 * * *")]TimerInfo timerInfo, ILogger log)
        {
            log.EnterJson("{0}", timerInfo);
            try
            {
                // アプリケーション設定でSystem名・SubSystem名が設定されていない場合、実行時にエラーとする
                if (string.IsNullOrEmpty(_settings.SystemName))
                {
                    throw new RmsInvalidAppSettingException($"{nameof(_settings.SystemName)} is required.");
                }

                if (string.IsNullOrEmpty(_settings.SubSystemName))
                {
                    throw new RmsInvalidAppSettingException($"{nameof(_settings.SubSystemName)} is required.");
                }

                // Sq1.1: サーバ／クライアント間の接続状況を取得する
                if (_service.ReadDeviceConnect(out IEnumerable<DtDevice> devices))
                {
                    // Sq1.2: 通信監視アラーム定義を取得する
                    _service.ReadAlarmDefinition(devices, out List<Tuple<DtDevice, DtAlarmDefConnectionMonitor>> alarmJudgementTargets);

                    // Sq1.3: アラーム生成の要否を判断する
                    bool needsAlarm = alarmJudgementTargets != null && alarmJudgementTargets.Count > 0;
                    if (!needsAlarm)
                    {
                        // アラーム生成不要
                        IEnumerable<long?> sidArray = devices == null ? new long?[] { } : devices.Select(x => x?.Sid);
                        log.Info(nameof(Resources.UT_DCM_DCM_004), new object[] { string.Join(",", sidArray) });
                        return;
                    }

                    _service.CreateAlarmCreationTarget(alarmJudgementTargets, out List<Tuple<DtDevice, DtAlarmDefConnectionMonitor>> alarmCreationTargets);

                    // Sq1.3: アラーム生成の要否を判断する
                    needsAlarm = alarmCreationTargets != null && alarmCreationTargets.Count > 0;
                    if (!needsAlarm)
                    {
                        // アラーム生成不要
                        IEnumerable<long?> sidArray = alarmJudgementTargets.Select(x => x?.Item1?.Sid);
                        log.Info(nameof(Resources.UT_DCM_DCM_004), new object[] { string.Join(",", sidArray) });
                        return;
                    }

                    // Sq1.4: アラームキューを生成する
                    // Sq1.5: キューを登録する
                    if (_service.CreateAndEnqueueAlarmInfo(alarmCreationTargets))
                    {
                        // アラームキュー登録完了
                        IEnumerable<long?> sidArray = alarmCreationTargets.Select(x => x?.Item1?.Sid);
                        log.Info(nameof(Resources.UT_DCM_DCM_007), new object[] { string.Join(",", sidArray) });
                    }
                }
            }
            catch (RmsInvalidAppSettingException e)
            {
                log.Error(e, nameof(Resources.UT_DCM_DCM_008), new object[] { e.Message });
            }
            catch (Exception e)
            {
                log.Error(e, nameof(Resources.UT_DCM_DCM_001));
            }
            finally
            {
                log.LeaveJson("{0}", timerInfo);
            }
        }
    }
}
