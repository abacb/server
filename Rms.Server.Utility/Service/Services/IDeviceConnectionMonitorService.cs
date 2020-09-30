using Rms.Server.Utility.Utility.Models;
using System;
using System.Collections.Generic;

namespace Rms.Server.Utility.Service.Services
{
    /// <summary>
    /// 端末データテーブル監視サービスのインターフェース
    /// </summary>
    public interface IDeviceConnectionMonitorService
    {
        /// <summary>
        /// 接続状況を取得する
        /// </summary>
        /// <param name="devices">端末データ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool ReadDeviceConnect(out IEnumerable<DtDevice> devices);

        /// <summary>
        /// アラーム定義を取得する
        /// </summary>
        /// <param name="devices">端末データ</param>
        /// <param name="alarmJudgementTargets">端末データと対応するアラーム定義</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool ReadAlarmDefinition(IEnumerable<DtDevice> devices, out List<Tuple<DtDevice, DtAlarmDefConnectionMonitor>> alarmJudgementTargets);

        /// <summary>
        /// アラーム生成対象データを作成する
        /// </summary>
        /// <param name="alarmJudgementTargets">アラーム判定対象データ</param>
        /// <param name="alarmCreationTargets">アラーム生成対象データ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool CreateAlarmCreationTarget(IReadOnlyCollection<Tuple<DtDevice, DtAlarmDefConnectionMonitor>> alarmJudgementTargets, out List<Tuple<DtDevice, DtAlarmDefConnectionMonitor>> alarmCreationTargets);

        /// <summary>
        /// アラーム情報を作成しQueueStorageへ登録する
        /// </summary>
        /// <param name="alarmCreationTargets">アラーム対象データ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        bool CreateAndEnqueueAlarmInfo(List<Tuple<DtDevice, DtAlarmDefConnectionMonitor>> alarmCreationTargets);
    }
}
