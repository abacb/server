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

namespace Rms.Server.Utility.Azure.Functions.DipAlmiLogPremonitor
{
    /// <summary>
    /// 骨塩アルミスロープログ予兆監視
    /// </summary>
    public class DipAlmiLogPremonitorController
    {
        /// <summary>
        /// 設定
        /// </summary>
        private readonly UtilityAppSettings _settings;

        /// <summary>
        /// 骨塩アルミスロープログ予兆監視サービス
        /// </summary>
        private readonly IDipAlmiLogPremonitorService _service;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">設定</param>
        /// <param name="service">骨塩アルミスロープログ予兆監視サービスクラス</param>
        public DipAlmiLogPremonitorController(UtilityAppSettings settings, IDipAlmiLogPremonitorService service)
        {
            Assert.IfNull(settings);
            Assert.IfNull(service);

            _settings = settings;
            _service = service;
        }

        /// <summary>
        /// アルミスロープログ解析判定結果データテーブルの監視を実行する
        /// </summary>
        /// <remarks>sd 07-14.アルミログ予兆監視</remarks>
        /// <param name="timerInfo">タイマー情報</param>
        /// <param name="log">ロガー</param>
        [FunctionName("DipAlmiLogPremonitor")]
        public void MonitorAlmilogAnalysisResultTable([TimerTrigger("0 0 16 * * *")]TimerInfo timerInfo, ILogger log)
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

                // Sq1.1: アルミスロープログ解析結果を取得する
                if (_service.ReadAlarmJudgementTarget(out List<DtAlmilogAnalysisResult> unjudgedItems, out List<List<DtAlmilogAnalysisResult>> alarmItemsList))
                {
                    // Sq1.2: アルミスロープログ予兆監視アラーム定義を取得する
                    _service.ReadAlarmDefinition(alarmItemsList, out List<List<Tuple<DtAlmilogAnalysisResult, DtAlmilogPremonitor>>> models);

                    // Sq1.3: アラーム生成の要否を判断する
                    bool needsAlarm = models != null && models.Count > 0;
                    if (!needsAlarm)
                    {
                        // アラーム生成不要
                        IEnumerable<long?> sidArray = alarmItemsList == null ? new long?[] { } : alarmItemsList.Select(x => x.FirstOrDefault()?.Sid);
                        log.Info(nameof(Resources.UT_DAP_DAP_004), new object[] { string.Join(",", sidArray) });
                        return;
                    }

                    // Sq1.4: アラームキューを生成する
                    // Sq1.5: キューを登録する
                    if (_service.CreateAndEnqueueAlarmInfo(models))
                    {
                        // アラームキュー登録完了
                        IEnumerable<long?> sidArray = models.Select(x => x.FirstOrDefault()?.Item1?.Sid);
                        log.Info(nameof(Resources.UT_DAP_DAP_007), new object[] { string.Join(",", sidArray) });
                    }

                    // Sq1.6: 判断の実施済みフラグを更新する
                    if (_service.UpdateAlarmJudgedAnalysisResult(unjudgedItems))
                    {
                        // 判定済みフラグの更新完了
                        IEnumerable<long> sidArray = unjudgedItems.Select(x => x.Sid);
                        log.Info(nameof(Resources.UT_DAP_DAP_009), new object[] { string.Join(",", sidArray) });
                    }
                }
            }
            catch (RmsInvalidAppSettingException e)
            {
                log.Error(e, nameof(Resources.UT_DAP_DAP_010), new object[] { e.Message });
            }
            catch (Exception e)
            {
                log.Error(e, nameof(Resources.UT_DAP_DAP_001));
            }
            finally
            {
                log.LeaveJson("{0}", timerInfo);
            }
        }
    }
}
