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
using System.Linq;

namespace Rms.Server.Utility.Service.Services
{
    /// <summary>
    /// DipAlmiLogPremonitorService
    /// </summary>
    public class DipAlmiLogPremonitorService : IDipAlmiLogPremonitorService
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
        /// アルミスロープログ解析判定結果リポジトリ
        /// </summary>
        private readonly IDtAlmilogAnalysisResultRepository _dtAlmilogAnalysisResultRepository;

        /// <summary>
        /// アルミスロープログ予兆監視アラーム定義リポジトリ
        /// </summary>
        private readonly IDtAlmilogPremonitorRepository _dtAlmilogPremonitorRepository;

        /// <summary>
        /// Queueリポジトリ
        /// </summary>
        private readonly IQueueRepository _queueRepository;

        /// <summary>
        /// アラーム通知閾値
        /// </summary>
        private readonly int _alarmCountThreshold;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">設定</param>
        /// <param name="logger">ロガー</param>
        /// <param name="timeProvider">DateTimeの提供元</param>
        /// <param name="dtAlmilogAnalysisResultRepository">アルミスロープログ解析判定結果リポジトリ</param>
        /// <param name="dtAlmilogPremonitorRepository">アルミスロープログ予兆監視アラーム定義リポジトリ</param>
        /// <param name="queueRepository">Queueリポジトリ</param>
        public DipAlmiLogPremonitorService(
            UtilityAppSettings settings,
            ILogger<DipAlmiLogPremonitorService> logger,
            ITimeProvider timeProvider,
            IDtAlmilogAnalysisResultRepository dtAlmilogAnalysisResultRepository,
            IDtAlmilogPremonitorRepository dtAlmilogPremonitorRepository,
            IQueueRepository queueRepository)
        {
            Assert.IfNull(settings);
            Assert.IfNull(logger);
            Assert.IfNull(timeProvider);
            Assert.IfNull(dtAlmilogAnalysisResultRepository);
            Assert.IfNull(dtAlmilogPremonitorRepository);
            Assert.IfNull(queueRepository);

            _settings = settings;
            _logger = logger;
            _timeProvider = timeProvider;
            _dtAlmilogAnalysisResultRepository = dtAlmilogAnalysisResultRepository;
            _dtAlmilogPremonitorRepository = dtAlmilogPremonitorRepository;
            _queueRepository = queueRepository;

            _alarmCountThreshold = int.Parse(_settings.AlarmCountThreshold);
        }

        /// <summary>
        /// 解析結果データを取得する
        /// </summary>
        /// <param name="unjudgedItems">未判定解析結果データ</param>
        /// <param name="alarmItemsList">アラーム対象解析結果データ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        public bool ReadAlarmJudgementTarget(out List<DtAlmilogAnalysisResult> unjudgedItems, out List<List<DtAlmilogAnalysisResult>> alarmItemsList)
        {
            unjudgedItems = new List<DtAlmilogAnalysisResult>();
            alarmItemsList = new List<List<DtAlmilogAnalysisResult>>();

            try
            {
                _logger.Enter();

                // Sq1.1: アルミスロープログ解析結果を取得する
                var models = _dtAlmilogAnalysisResultRepository.ReadAlarmJudgementTarget(_alarmCountThreshold);

                if (models.Any())
                {
                    unjudgedItems = models.Where(x => x.IsAlarmJudged == false).ToList();

                    // アラーム判定は機器UID、Detector名称単位で行うため判定対象データ単位にグループ化する
                    var groupedAlarmJudgementTarget = models.GroupBy(x => new { x.EquipmentUid, x.DetectorName });

                    // 連続NG数分データが存在しない場合は判定処理を実施しない
                    groupedAlarmJudgementTarget = groupedAlarmJudgementTarget.Where(x => x.Count() >= _alarmCountThreshold);

                    foreach (var group in groupedAlarmJudgementTarget)
                    {
                        // 解析NGのリスト（日付の新しい順）
                        List<DtAlmilogAnalysisResult> ngList = new List<DtAlmilogAnalysisResult>();

                        foreach (var value in group)
                        {
                            if (value.AnalysisResult == "S")
                            {
                                ngList = new List<DtAlmilogAnalysisResult>(); // NGリストをリセット
                            }
                            else
                            {
                                // アラーム判定日時を設定してから追加する
                                value.AlarmJudgementTime = _timeProvider.UtcNow.ToString();
                                ngList.Insert(0, value);
                                if (ngList.Count >= _alarmCountThreshold)
                                {
                                    // 1アラーム作成に必要となる解析結果(最新のデータを_alarmCountThreshold件)
                                    var alarmItems = ngList.Take(_alarmCountThreshold).ToList();
                                    alarmItemsList.Add(alarmItems);
                                }
                            }
                        }
                    }
                }

                return true;
            }
            catch (RmsException e)
            {
                // DBにアクセスできない（基本設計書 5.2.1.1 エラー処理）
                _logger.Error(e, nameof(Resources.UT_DAP_DAP_002));
                return false;
            }
            finally
            {
                _logger.LeaveJson("{0}", new { unjudgedItems, alarmItemsList });
            }
        }

        /// <summary>
        /// アラーム定義を取得する
        /// </summary>
        /// <param name="analysisResultsList">アラーム判定対象データ</param>
        /// <param name="models">DBから取得したパラメータ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        public bool ReadAlarmDefinition(IEnumerable<IEnumerable<DtAlmilogAnalysisResult>> analysisResultsList, out List<List<Tuple<DtAlmilogAnalysisResult, DtAlmilogPremonitor>>> models)
        {
            models = null;
            bool result = true;

            try
            {
                _logger.EnterJson("{0}", new { analysisResultsList });

                models = new List<List<Tuple<DtAlmilogAnalysisResult, DtAlmilogPremonitor>>>();

                foreach (var analysisResults in analysisResultsList)
                {
                    List<Exception> exceptions = new List<Exception>();
                    List<Tuple<DtAlmilogAnalysisResult, DtAlmilogPremonitor>> analysisInfos = new List<Tuple<DtAlmilogAnalysisResult, DtAlmilogPremonitor>>();

                    foreach (var analysisResult in analysisResults)
                    {
                        try
                        {
                            // アラーム通知対象解析結果のアラーム定義を取得する
                            // アラーム定義が存在しない、もしくは同一解析結果について複数アラーム定義が取得された場合、ロギングしてアラーム通知は行わない。（基本設計書 5.2.3.1 処理内容）
                            var premonitor = _dtAlmilogPremonitorRepository.ReadDtAlmilogPremonitor(analysisResult, false, false);
                            analysisInfos.Add(new Tuple<DtAlmilogAnalysisResult, DtAlmilogPremonitor>(analysisResult, premonitor.First()));
                        }
                        catch (RmsException e)
                        {
                            exceptions.Add(e);
                        }
                    }

                    if (exceptions.Count > 0)
                    {
                        Exception e = new AggregateException(exceptions);
                        _logger.Error(e, nameof(Resources.UT_DAP_DAP_003), new object[] { analysisResults.ElementAt(0).Sid });
                        result = false;
                    }
                    else
                    {
                        models.Add(analysisInfos);
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
                _logger.LeaveJson("{0}", new { models });
            }
        }

        /// <summary>
        /// アラーム情報を作成しQueueStorageへ登録する
        /// </summary>
        /// <param name="alarmInfosList">アラーム対象解析結果とアラーム定義のリスト</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        public bool CreateAndEnqueueAlarmInfo(IEnumerable<IEnumerable<Tuple<DtAlmilogAnalysisResult, DtAlmilogPremonitor>>> alarmInfosList)
        {
            bool result = true;
            _logger.EnterJson("{0}", new { alarmInfosList });

            foreach (var alarmInfos in alarmInfosList)
            {
                var target = alarmInfos?.ElementAt(0);
                string errorMsg = null;
                try
                {
                    List<string> errorMessages = alarmInfos.Select(x => string.Format(x.Item2.AlarmDescription, x.Item1.AlmilogMonth, x.Item1.DetectorName)).ToList();

                    // 日付の古い順→新しい順となるようにメッセージを追加する
                    errorMessages.Reverse();
                    errorMsg = string.Join("\n", errorMessages);

                    if (string.IsNullOrEmpty(errorMsg))
                    {
                        throw new RmsException("AlarmDescription is empty");
                    }
                }
                catch (Exception e)
                {
                    // アラーム生成エラー（基本設計書 5.2.1.1 エラー処理）
                    _logger.Error(e, nameof(Resources.UT_DAP_DAP_005), new object[] { target?.Item1?.Sid });
                    result = false;
                }

                // Sq1.4: アラームキューを生成する
                // Sq1.5: キューを登録する
                if (!CreateAndEnqueueAlarmInfo(target.Item1, target.Item2, errorMsg))
                {
                    result = false;
                }
            }

            _logger.Leave();
            return result;
        }

        /// <summary>
        /// アラーム判定済みデータの更新を行う
        /// </summary>
        /// <param name="alarmJudgementTarget">アラーム判定対象データ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        public bool UpdateAlarmJudgedAnalysisResult(IEnumerable<DtAlmilogAnalysisResult> alarmJudgementTarget)
        {
            int result = 0;
            try
            {
                _logger.EnterJson("{0}", new { alarmJudgementTarget });

                // 一括更新
                result = _dtAlmilogAnalysisResultRepository.UpdateIsAlarmJudgedTrue(alarmJudgementTarget.Select(x => x.Sid));

                return result > 0;
            }
            catch (RmsException e)
            {
                // アラーム生成エラー or アラームキューにアラーム情報を送信できない（基本設計書 5.2.1.1 エラー処理）
                IEnumerable<long> sidArray = alarmJudgementTarget.Select(x => x.Sid);
                _logger.Error(e, nameof(Resources.UT_DAP_DAP_008), new object[] { string.Join(",", sidArray) });
                return false;
            }
            finally
            {
                _logger.LeaveJson("{0}", new { result });
            }
        }

        /// <summary>
        /// アラーム情報を作成しQueueStorageへ登録する
        /// </summary>
        /// <param name="analysisResult">アラーム対象解析結果</param>
        /// <param name="alarmDef">アラーム定義</param>
        /// <param name="alarmDescription">アラーム説明</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        private bool CreateAndEnqueueAlarmInfo(DtAlmilogAnalysisResult analysisResult, DtAlmilogPremonitor alarmDef, string alarmDescription)
        {
            string message = null;
            try
            {
                _logger.EnterJson("{0}", new { analysisResult, alarmDef, alarmDescription });

                // Sq1.4: アラームキューを生成する
                var alarmInfo = new AlarmInfo
                {
                    SourceEquipmentUid = analysisResult.EquipmentUid,
                    TypeCode = null,
                    ErrorCode = alarmDef.AnalysisResultErrorCode,
                    AlarmLevel = alarmDef.AlarmLevel,
                    AlarmTitle = alarmDef.AlarmTitle,
                    AlarmDescription = alarmDescription,
                    AlarmDatetime = _timeProvider.UtcNow.ToString(Utility.Const.AlarmQueueDateTimeFormat),
                    EventDatetime = analysisResult.AlarmJudgementTime,
                    AlarmDefId = $"{_settings.SystemName}_{_settings.SubSystemName}_{alarmDef.Sid.ToString()}",
                    MessageId = null
                };

                message = JsonConvert.SerializeObject(alarmInfo);

                // Sq1.5: キューを登録する
                _queueRepository.SendMessageToAlarmQueue(message);
                return true;
            }
            catch (Exception e)
            {
                // アラーム生成エラー or アラームキューにアラーム情報を送信できない（基本設計書 5.2.1.1 エラー処理）
                _logger.Error(e, string.IsNullOrEmpty(message) ? nameof(Resources.UT_DAP_DAP_005) : nameof(Resources.UT_DAP_DAP_006), new object[] { analysisResult?.Sid });
                return false;
            }
            finally
            {
                _logger.LeaveJson("{0}", new { message });
            }
        }
    }
}
