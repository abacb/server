namespace Rms.Server.Core.Utility.Models.Entites
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 端末設定同期
    /// IoT Hubによるデバイスツイン更新イベントデータ
    /// </summary>
    public partial class DtTwinChanged
    {
        /// <summary>
        /// リモート接続UID
        /// </summary>
        public string RemoteConnectionUid { get; set; }

        /// <summary>
        /// RMSソフトバージョン
        /// </summary>
        public string SoftVersion { get; set; }
    }
}
