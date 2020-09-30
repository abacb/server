using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Utility.Utility.Models;
using System.Collections.Generic;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// DT_ALMILOG_ANALYSIS_RESULTテーブルリポジトリのインターフェース
    /// </summary>
    public interface IDtAlmilogAnalysisResultRepository : IRepository
    {
        /// <summary>
        /// 引数に指定したDtAlmilogAnalysisResultをDT_ALMILOG_ANALYSIS_RESULTテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <returns>処理結果</returns>
        DtAlmilogAnalysisResult CreateDtAlmilogAnalysisResult(DtAlmilogAnalysisResult inData);

        /// <summary>
        /// DT_ALMILOG_ANALYSIS_RESULTテーブルに条件に一致するDtAlmilogAnalysisResultが存在するか確認する
        /// </summary>
        /// <param name="logFileName">ログファイル名</param>
        /// <returns>存在する場合trueを、存在しない場合falseを返す</returns>
        bool ExistDtAlmilogAnalysisResult(string logFileName);

        /// <summary>
        /// DT_ALMILOG_ANALYSIS_RESULTテーブルからアラーム判定の対象データを取得する
        /// 機器UIDとDetector名称単位で判定済みデータを連続NG数-1件、未判定データを全件取得する
        /// 取得したアラーム判定対象データは次の順番でListに格納する(連続した1次元のデータとして格納)
        ///   機器UID1、Detector名称1、判定済みデータ1、判定済みデータ2...、未判定データ1、未判定データ2...
        ///   機器UID1、Detector名称2、判定済みデータ1、...
        ///   機器UID2、Detector名称1、...
        /// </summary>
        /// <param name="alarmCountThreshold">アラーム通知閾値</param>
        /// <returns>取得したデータ</returns>
        IEnumerable<DtAlmilogAnalysisResult> ReadAlarmJudgementTarget(int alarmCountThreshold);

        /// <summary>
        /// 引数に指定したSIDのレコードを対象にDT_ALMILOG_ANALYSIS_RESULTテーブルのIS_ALARM_JUDGEDをTRUEに更新する
        /// </summary>
        /// <param name="sidList">更新対象レコードのSID</param>
        /// <returns>更新レコード数</returns>
        int UpdateIsAlarmJudgedTrue(IEnumerable<long> sidList);
    }
}
