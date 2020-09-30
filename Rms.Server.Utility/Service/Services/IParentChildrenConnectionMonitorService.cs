using Rms.Server.Utility.Utility.Models;
using System;
using System.Collections.Generic;

namespace Rms.Server.Utility.Service.Services
{
    /// <summary>
    /// 親子間通信データテーブル監視サービスのインターフェース
    /// </summary>
    public interface IParentChildrenConnectionMonitorService
    {
        /// <summary>
        /// 接続状況を取得する
        /// </summary>
        /// <param name="parentChildConnects">親子間通信データ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool ReadParentChildConnect(out IEnumerable<DtParentChildConnect> parentChildConnects);

        /// <summary>
        /// アラーム定義を取得する
        /// </summary>
        /// <param name="parentChildConnects">親子間通信データ</param>
        /// <param name="alarmJudgementTargets">親子間通信データと対応するアラーム定義</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool ReadAlarmDefinition(IEnumerable<DtParentChildConnect> parentChildConnects, out List<Tuple<DtParentChildConnect, DtAlarmDefConnectionMonitor>> alarmJudgementTargets);

        /// <summary>
        /// アラーム生成対象データを作成する
        /// </summary>
        /// <param name="alarmJudgementTargets">アラーム判定対象データ</param>
        /// <param name="alarmCreationTargets">アラーム生成対象データ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool CreateAlarmCreationTarget(IReadOnlyCollection<Tuple<DtParentChildConnect, DtAlarmDefConnectionMonitor>> alarmJudgementTargets, out List<Tuple<DtParentChildConnect, DtAlarmDefConnectionMonitor>> alarmCreationTargets);

        /// <summary>
        /// アラーム情報を作成しQueueStorageへ登録する
        /// </summary>
        /// <param name="alarmCreationTargets">アラーム対象データ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool CreateAndEnqueueAlarmInfo(List<Tuple<DtParentChildConnect, DtAlarmDefConnectionMonitor>> alarmCreationTargets);
    }
}
