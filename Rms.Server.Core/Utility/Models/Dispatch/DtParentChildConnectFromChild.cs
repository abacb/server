namespace Rms.Server.Core.Utility.Models.Entites
{
    using System;

    /// <summary>
    /// 親機フラグ=falseの場合の
    /// メッセージ - 親子間通信データテーブル変換用中間データ構造
    /// </summary>
    public partial class DtParentChildConnectFromChild
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
        /// 子機確認結果
        /// </summary>
        public bool? ChildResult { get; set; }

        /// <summary>
        /// 子機確認日時
        /// </summary>        
        public DateTime? ChildConfirmDatetime { get; set; }

        /// <summary>
        /// 子機最終通信日時
        /// </summary>
        public DateTime? ChildLastConnectDatetime { get; set; }
    }
}
