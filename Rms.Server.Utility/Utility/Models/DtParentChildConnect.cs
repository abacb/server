using System;
using System.Collections.Generic;
using System.Text;

namespace Rms.Server.Utility.Utility.Models
{
    /// <summary>
    /// DT_PARENT_CHILD_CONNECTの拡張
    /// </summary>
    public partial class DtParentChildConnect
    {
        /// <summary>
        /// 機器UID
        /// </summary>
        public string EquipmentUid { get; set; }

        /// <summary>
        /// 機種コード
        /// </summary>
        public string TypeCode { get; set; }

        /// <summary>
        /// 温度センサ
        /// </summary>
        public string TemperatureSensor { get; set; }

        /// <summary>
        /// DXA
        /// </summary>
        public string Dxa { get; set; }

        /// <summary>
        /// 通信断経過日数
        /// </summary>
        public int DisconnectionDays { get; set; }

        /// <summary>
        /// 最終通信日
        /// </summary>
        public DateTime? LastConnectDateTime { get; set; }

        /// <summary>
        /// アラーム判定日時
        /// </summary>
        public string AlarmJudgementTime { get; set; }
    }
}
