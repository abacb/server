namespace Rms.Server.Core.Utility.Models.Entites
{
    using System;

    /// <summary>
    /// 親機フラグ=trueの場合の
    /// メッセージ - 親子間通信データテーブル変換用中間データ構造
    /// </summary>
    public partial class DtParentChildConnectFromParent
    {
        /// <summary>
        /// 親端末UID
        /// </summary>
        public string ParentDeviceUid { get; set; }

        /// <summary>
        /// 子端末UID
        /// </summary>
        public string ChildDeviceUid { get; set; }

        /// <summary>
        /// 親機確認結果
        /// </summary>
        public bool? ParentResult { get; set; }
    
        /// <summary>
        /// 親機確認日時
        /// </summary>
        public DateTime? ParentConfirmDatetime { get; set; }

        /// <summary>
        /// 親機最終通信日時
        /// </summary>
        public DateTime? ParentLastConnectDatetime { get; set; }
    }
}
