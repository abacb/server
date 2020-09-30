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
    /// ParentChildrenConnectionMonitorService
    /// </summary>
    public class ParentChildrenConnectionMonitorService : IParentChildrenConnectionMonitorService
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
        /// 親子間通信データテーブルリポジトリ
        /// </summary>
        private readonly IDtParentChildConnectRepository _dtParentChildConnectRepository;

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
        /// <param name="dtParentChildConnectRepository">親子間通信データテーブルリポジトリ</param>
        /// <param name="dtAlarmDefConnectionMonitorRepository">通信監視アラーム定義リポジトリ</param>
        /// <param name="queueRepository">Queueリポジトリ</param>
        public ParentChildrenConnectionMonitorService(
            UtilityAppSettings settings,
            ILogger<ParentChildrenConnectionMonitorService> logger,
            ITimeProvider timeProvider,
            IDtParentChildConnectRepository dtParentChildConnectRepository,
            IDtAlarmDefConnectionMonitorRepository dtAlarmDefConnectionMonitorRepository,
            IQueueRepository queueRepository)
        {
            Assert.IfNull(settings);
            Assert.IfNull(logger);
            Assert.IfNull(timeProvider);
            Assert.IfNull(dtParentChildConnectRepository);
            Assert.IfNull(dtAlarmDefConnectionMonitorRepository);
            Assert.IfNull(queueRepository);

            _settings = settings;
            _logger = logger;
            _timeProvider = timeProvider;
            _dtParentChildConnectRepository = dtParentChildConnectRepository;
            _dtAlarmDefConnectionMonitorRepository = dtAlarmDefConnectionMonitorRepository;
            _queueRepository = queueRepository;
        }

        /// <summary>
        /// 接続状況を取得する
        /// </summary>
        /// <param name="parentChildConnects">親子間通信データ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        public bool ReadParentChildConnect(out IEnumerable<DtParentChildConnect> parentChildConnects)
        {
            parentChildConnects = null;

            try
            {
                _logger.Enter();

                // Sq1.1: 親子機器間の接続状況を取得する
                parentChildConnects = _dtParentChildConnectRepository.ReadDtParentChildConnect();
                return true;
            }
            catch (RmsException e)
            {
                // DBにアクセスできない or DBに情報が見つからない（基本設計書 5.2.1.1 エラー処理）
                _logger.Error(e, nameof(Resources.UT_PCM_PCM_002));
                return false;
            }
            finally
            {
                _logger.LeaveJson("{0}", new { parentChildConnects });
            }
        }

        /// <summary>
        /// アラーム定義を取得する
        /// </summary>
        /// <param name="parentChildConnects">親子間通信データ</param>
        /// <param name="alarmJudgementTargets">親子間通信データと対応するアラーム定義</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        public bool ReadAlarmDefinition(IEnumerable<DtParentChildConnect> parentChildConnects, out List<Tuple<DtParentChildConnect, DtAlarmDefConnectionMonitor>> alarmJudgementTargets)
        {
            alarmJudgementTargets = null;
            bool result = true;

            try
            {
                _logger.EnterJson("{0}", new { parentChildConnects });

                alarmJudgementTargets = new List<Tuple<DtParentChildConnect, DtAlarmDefConnectionMonitor>>();

                foreach (var parentChildConnect in parentChildConnects)
                {
                    try
                    {
                        var targets = new List<string>();

                        if (parentChildConnect.TemperatureSensor == Utility.Const.InventorySettingInfoOptionOn)
                        {
                            targets.Add(Utility.Const.AlarmDefTargetTemperature);
                        }

                        if (parentChildConnect.Dxa == Utility.Const.InventorySettingInfoOptionOn)
                        {
                            targets.Add(Utility.Const.AlarmDefTargetDxa);
                        }

                        if (targets.Count <= 0)
                        {
                            targets.Add(string.Empty);
                        }

                        // Sq1.2: 通信監視アラーム定義を取得する
                        var alarmDefs = _dtAlarmDefConnectionMonitorRepository.ReadDtAlarmDefConnectionMonitor(parentChildConnect.TypeCode, targets);

                        foreach (var alarmDef in alarmDefs)
                        {
                            alarmJudgementTargets.Add(new Tuple<DtParentChildConnect, DtAlarmDefConnectionMonitor>(parentChildConnect, alarmDef));
                        }
                    }
                    catch (RmsException e)
                    {
                        // DBにアクセスできない（基本設計書 5.2.1.1 エラー処理）
                        _logger.Error(e, nameof(Resources.UT_PCM_PCM_003), new object[] { parentChildConnect.Sid });
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
        public bool CreateAlarmCreationTarget(IReadOnlyCollection<Tuple<DtParentChildConnect, DtAlarmDefConnectionMonitor>> alarmJudgementTargets, out List<Tuple<DtParentChildConnect, DtAlarmDefConnectionMonitor>> alarmCreationTargets)
        {
            bool result = true;
            _logger.EnterJson("{0}", new { alarmJudgementTargets });

            alarmCreationTargets = new List<Tuple<DtParentChildConnect, DtAlarmDefConnectionMonitor>>();

            foreach (var judgementTarget in alarmJudgementTargets)
            {
                try
                {
                    (var parentChildConnect, var alarmDef) = judgementTarget;

                    DateTime? lastConnectDateTime = null;

                    var compResult = Nullable.Compare<DateTime>(parentChildConnect.ParentLastConnectDatetime, parentChildConnect.ChildLastConnectDatetime);

                    // 親子の最終通信日時の新しい方を取得
                    if (compResult == 1)
                    {
                        lastConnectDateTime = parentChildConnect.ParentLastConnectDatetime;
                    }
                    else if (compResult == -1)
                    {
                        lastConnectDateTime = parentChildConnect.ChildLastConnectDatetime;
                    }
                    else
                    {
                        lastConnectDateTime = parentChildConnect.ParentLastConnectDatetime;
                    }

                    if (lastConnectDateTime == null)
                    {
                        // Dispatacherで親子どちらかの最終通信日時に必ず値を設定するのでともにnullという事はあり得ない
                        throw new RmsException("LastConnectDateTime is null");
                    }
                    else
                    {
                        TimeSpan disconnectedTime = _timeProvider.UtcNow - (DateTime)lastConnectDateTime;
                        int disconnectionDays = disconnectedTime.Days;

                        if (JudgeAlarm(disconnectionDays, alarmDef))
                        {
                            var alarmTargetParentChildConnect = new DtParentChildConnect();
                            alarmTargetParentChildConnect = parentChildConnect;
                            alarmTargetParentChildConnect.DisconnectionDays = disconnectionDays;
                            alarmTargetParentChildConnect.LastConnectDateTime = lastConnectDateTime;
                            alarmTargetParentChildConnect.AlarmJudgementTime = _timeProvider.UtcNow.ToString();

                            var alarmTarget = new Tuple<DtParentChildConnect, DtAlarmDefConnectionMonitor>(alarmTargetParentChildConnect, alarmDef);
                            alarmCreationTargets.Add(alarmTarget);
                        }
                    }
                }
                catch (Exception e)
                {
                    // アラーム生成エラー（基本設計書 5.2.1.1 エラー処理）
                    _logger.Error(e, nameof(Resources.UT_PCM_PCM_005), new object[] { judgementTarget?.Item1?.Sid });
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
        public bool CreateAndEnqueueAlarmInfo(List<Tuple<DtParentChildConnect, DtAlarmDefConnectionMonitor>> alarmCreationTargets)
        {
            bool result = true;
            _logger.EnterJson("{0}", new { alarmCreationTargets });

            foreach (var alarmTarget in alarmCreationTargets)
            {
                string message = null;
                try
                {
                    (var parentChildConnect, var alarmDef) = alarmTarget;

                    string alarmDescription = alarmDef.AlarmDescription;
                    if (!string.IsNullOrEmpty(alarmDescription))
                    {
                        if (alarmDescription.Contains("{0}"))
                        {
                            if (alarmDescription.Contains("{1}"))
                            {
                                alarmDescription = string.Format(alarmDescription, parentChildConnect.DisconnectionDays, parentChildConnect.LastConnectDateTime);
                            }
                            else
                            {
                                alarmDescription = string.Format(alarmDescription, parentChildConnect.DisconnectionDays);
                            }
                        }
                    }

                    // Sq1.4: アラームキューを生成する
                    var alarmInfo = new AlarmInfo
                    {
                        SourceEquipmentUid = parentChildConnect.EquipmentUid,
                        TypeCode = parentChildConnect.TypeCode,
                        ErrorCode = alarmDef.AnalysisResultErrorCode,
                        AlarmLevel = alarmDef.AlarmLevel,
                        AlarmTitle = alarmDef.AlarmTitle,
                        AlarmDescription = alarmDescription,
                        AlarmDatetime = _timeProvider.UtcNow.ToString(Utility.Const.AlarmQueueDateTimeFormat),
                        EventDatetime = parentChildConnect.AlarmJudgementTime,
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
                    _logger.Error(e, string.IsNullOrEmpty(message) ? nameof(Resources.UT_PCM_PCM_005) : nameof(Resources.UT_PCM_PCM_006), new object[] { alarmTarget?.Item1?.Sid });
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
