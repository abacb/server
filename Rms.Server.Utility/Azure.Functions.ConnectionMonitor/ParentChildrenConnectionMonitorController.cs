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
    /// 通信監視アプリ(親子間通信データテーブル監視)
    /// </summary>
    public class ParentChildrenConnectionMonitorController
    {
        /// <summary>
        /// 設定
        /// </summary>
        private readonly UtilityAppSettings _settings;

        /// <summary>
        /// 親子間通信データテーブル監視サービス
        /// </summary>
        private readonly IParentChildrenConnectionMonitorService _service;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">設定</param>
        /// <param name="service">親子間通信データテーブル監視サービスクラス</param>
        public ParentChildrenConnectionMonitorController(UtilityAppSettings settings, IParentChildrenConnectionMonitorService service)
        {
            Assert.IfNull(settings);
            Assert.IfNull(service);

            _settings = settings;
            _service = service;
        }

        /// <summary>
        /// 親子間通信データテーブル(Core)の監視を実行する
        /// </summary>
        /// <remarks>sd 07-11.通信監視</remarks>
        /// <param name="timerInfo">タイマー情報</param>
        /// <param name="log">ロガー</param>
        [FunctionName("ParentChildrenConnectionMonitor")]
        public void MonitorParentChildConnectTable([TimerTrigger("0 0 16 * * *")]TimerInfo timerInfo, ILogger log)
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

                // Sq1.1: 親子機器間の接続状況を取得する
                if (_service.ReadParentChildConnect(out IEnumerable<DtParentChildConnect> parentChildConnects))
                {
                    // Sq1.2: 通信監視アラーム定義を取得する
                    _service.ReadAlarmDefinition(parentChildConnects, out List<Tuple<DtParentChildConnect, DtAlarmDefConnectionMonitor>> alarmJudgementTargets);

                    // Sq1.3: アラーム生成の要否を判断する
                    bool needsAlarm = alarmJudgementTargets != null && alarmJudgementTargets.Count > 0;
                    if (!needsAlarm)
                    {
                        // アラーム生成不要
                        IEnumerable<long?> sidArray = parentChildConnects == null ? new long?[] { } : parentChildConnects.Select(x => x?.Sid);
                        log.Info(nameof(Resources.UT_PCM_PCM_004), new object[] { string.Join(",", sidArray) });
                        return;
                    }

                    _service.CreateAlarmCreationTarget(alarmJudgementTargets, out List<Tuple<DtParentChildConnect, DtAlarmDefConnectionMonitor>> alarmCreationTargets);

                    // Sq1.3: アラーム生成の要否を判断する
                    needsAlarm = alarmCreationTargets != null && alarmCreationTargets.Count > 0;
                    if (!needsAlarm)
                    {
                        // アラーム生成不要
                        IEnumerable<long?> sidArray = alarmJudgementTargets.Select(x => x?.Item1?.Sid);
                        log.Info(nameof(Resources.UT_PCM_PCM_004), new object[] { string.Join(",", sidArray) });
                        return;
                    }

                    // Sq1.4: アラームキューを生成する
                    // Sq1.5: キューを登録する
                    if (_service.CreateAndEnqueueAlarmInfo(alarmCreationTargets))
                    {
                        // アラームキュー登録完了
                        IEnumerable<long?> sidArray = alarmCreationTargets.Select(x => x?.Item1?.Sid);
                        log.Info(nameof(Resources.UT_PCM_PCM_007), new object[] { string.Join(",", sidArray) });
                    }
                }
            }
            catch (RmsInvalidAppSettingException e)
            {
                log.Error(e, nameof(Resources.UT_PCM_PCM_008), new object[] { e.Message });
            }
            catch (Exception e)
            {
                log.Error(e, nameof(Resources.UT_PCM_PCM_001));
            }
            finally
            {
                log.LeaveJson("{0}", timerInfo);
            }
        }
    }
}
