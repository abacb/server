using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Operation.Utility.Models;

namespace Rms.Server.Operation.Abstraction.Repositories
{
    /// <summary>
    /// DT_ALARMテーブルリポジトリのインターフェース
    /// </summary>
    public interface IDtAlarmRepository : IRepository
    {
        /// <summary>
        /// 引数に指定したDtAlarmをDT_ALARMテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <returns>処理結果</returns>
        DtAlarm CreateDtAlarm(DtAlarm inData);

        /// <summary>
        /// DT_ALARMテーブルに条件に一致するDtAlarmが存在するか確認する
        /// </summary>
        /// <param name="messageId">メッセージID</param>
        /// <returns>存在する場合trueを、存在しない場合falseを返す</returns>
        bool ExistDtAlarm(string messageId);

        /// <summary>
        /// DT_ALARMテーブルからメール送信済みの最新DtAlarmを取得する
        /// </summary>
        /// <param name="alarmDefId">アラーム定義ID</param>
        /// <returns>取得したデータ</returns>
        DtAlarm ReadLatestMailSentDtAlarm(string alarmDefId);
    }
}
