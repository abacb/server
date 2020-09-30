namespace Rms.Server.Core.Azure.Functions.WebApi.Dto
{
    /// <summary>
    /// 配信汎用処理
    /// </summary>
    public class DeliveryUtil
    {
        /// <summary>
        /// 配信時の動作
        /// </summary>
        public enum DeliveryAction
        {
            /// <summary>
            /// ダウンロードのみ行う
            /// </summary>
            DownloadOnly = 0,

            /// <summary>
            /// 解凍する
            /// </summary>
            Unzip,

            /// <summary>
            /// 解凍して実行する
            /// </summary>
            UnzipAndRun
        }

        /// <summary>
        /// 配信グループステータスコード
        /// </summary>
        public enum DeliveryGroupStatusCode
        {
            /// <summary>
            /// 配信開始前
            /// </summary>
            NotStart = 0,

            /// <summary>
            /// 配信開始
            /// </summary>
            Start,

            /// <summary>
            /// 完了
            /// </summary>
            Complete,

            /// <summary>
            /// 配信中止
            /// </summary>
            Cancel
        }
    }
}
