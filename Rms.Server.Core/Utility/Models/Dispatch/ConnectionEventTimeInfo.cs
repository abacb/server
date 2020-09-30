using System;
using System.Collections.Generic;
using System.Text;

namespace Rms.Server.Core.Utility.Models.Dispatch
{
    /// <summary>
    /// 接続/切断イベント時刻情報
    /// </summary>
    public class ConnectionEventTimeInfo
    {
        /// <summary>
        /// 接続/切断を示すコード
        /// </summary>
        /// <remarks>
        /// Utiliy.Const.ConnectStatusに定義された文字列
        /// </remarks>
        public string Status { get; set; } 

        /// <summary>
        /// イベント日時
        /// </summary>
        public DateTime EventTime { get; set; }

        /// <summary>
        /// 初回接続かどうかを示すフラグ
        /// </summary>
        public bool IsFirstConnection { get; set; }

        /// <summary>
        /// DBに設定された接続更新日時と比較して新しいイベントかどうかを示すフラグ
        /// </summary>
        /// <remarks>
        /// 切断イベントが初回接続イベントよりも先に来た場合には、更新処理は行わない。
        /// このフラグは切断イベントの上記判定も含んでおり、
        /// falseである場合にはDB更新処理は行わないものとする。
        /// </remarks>
        public bool IsNewerEvent { get; set; }
    }
}
