namespace Rms.Server.Core.Utility
{
    /// <summary>
    /// 定数クラス
    /// </summary>
    public static class Const
    {
        /// <summary>
        /// ASCIIコードでの文字(記号含む)の範囲を表す正規表現
        /// </summary>
        public const string AsciiCodeCharactersReg = @"^[\x20-\x7e]+$";

        /// <summary>
        /// 接続文字列
        /// </summary>
        /// <remarks>
        /// 必要なものだけここに定義する。例えばDispatcherで、静的文字列で直接設定する必要があるもの。
        /// </remarks>
        public static class ConnectionString
        {
            /// <summary>
            /// Event hubsの接続文字列の設定名：DispatchDeviceConnected用
            /// </summary>
            public const string EventHubsConnectionStringDeviceConnected = "EventHubsConnectionStringDeviceConnected";

            /// <summary>
            /// Event hubsの接続文字列の設定名：DispatchDeviceDisconnected用
            /// </summary>
            public const string EventHubsConnectionStringDeviceDisconnected = "EventHubsConnectionStringDeviceDisconnected";

            /// <summary>
            /// Event hubsの接続文字列の設定名：DispatchTwinChanged用
            /// </summary>
            public const string EventHubsConnectionStringTwinChanged = "EventHubsConnectionStringTwinChanged";

            /// <summary>
            /// Event hubsの接続文字列の設定名：DispatchPlusServiceBillLog用
            /// </summary>
            public const string EventHubsConnectionStringMs011 = "EventHubsConnectionStringMs011";

            /// <summary>
            /// Event hubsの接続文字列の設定名：DispatchDxaBillLog用
            /// </summary>
            public const string EventHubsConnectionStringMs014 = "EventHubsConnectionStringMs014";

            /// <summary>
            /// Event hubsの接続文字列の設定名：DispatchDxaqcLog用
            /// </summary>
            public const string EventHubsConnectionStringMs015 = "EventHubsConnectionStringMs015";

            /// <summary>
            /// Event hubsの接続文字列の設定名：DispatchInstallResult用
            /// </summary>
            public const string EventHubsConnectionStringMs016 = "EventHubsConnectionStringMs016";

            /// <summary>
            /// Event hubsの接続文字列の設定名：DispatchSoftVersion用
            /// </summary>
            public const string EventHubsConnectionStringMs025 = "EventHubsConnectionStringMs025";

            /// <summary>
            /// Event hubsの接続文字列の設定名：DispatchDirectoryUsage用
            /// </summary>
            public const string EventHubsConnectionStringMs026 = "EventHubsConnectionStringMs026";

            /// <summary>
            /// Event hubsの接続文字列の設定名：DispatchDiskDrive用
            /// </summary>
            public const string EventHubsConnectionStringMs027 = "EventHubsConnectionStringMs027";

            /// <summary>
            /// Event hubsの接続文字列の設定名：DispatchEquipmentUsage用
            /// </summary>
            public const string EventHubsConnectionStringMs028 = "EventHubsConnectionStringMs028";

            /// <summary>
            /// Event hubsの接続文字列の設定名：DispatchInventory用
            /// </summary>
            public const string EventHubsConnectionStringMs029 = "EventHubsConnectionStringMs029";

            /// <summary>
            /// Event hubsの接続文字列の設定名：DispatchParentChildConnect用
            /// </summary>
            public const string EventHubsConnectionStringMs030 = "EventHubsConnectionStringMs030";

            /// <summary>
            /// Event hubsの接続文字列の設定名：DispatchDrive用
            /// </summary>
            public const string EventHubsConnectionStringMs031 = "EventHubsConnectionStringMs031";
        }

        /// <summary>
        /// MessageSchemaId
        /// </summary>
        public static class MessageSchemaId
        {
            /// <summary>
            /// MS011
            /// </summary>
            public const string MS011 = "MS-011";

            /// <summary>
            /// MS014
            /// </summary>
            public const string MS014 = "MS-014";

            /// <summary>
            /// MS015
            /// </summary>
            public const string MS015 = "MS-015";

            /// <summary>
            /// MS016
            /// </summary>
            public const string MS016 = "MS-016";

            /// <summary>
            /// MS025
            /// </summary>
            public const string MS025 = "MS-025";

            /// <summary>
            /// MS026
            /// </summary>
            public const string MS026 = "MS-026";

            /// <summary>
            /// MS027
            /// </summary>
            public const string MS027 = "MS-027";

            /// <summary>
            /// MS028
            /// </summary>
            public const string MS028 = "MS-028";

            /// <summary>
            /// MS029
            /// </summary>
            public const string MS029 = "MS-029";

            /// <summary>
            /// MS030
            /// </summary>
            public const string MS030 = "MS-030";

            /// <summary>
            /// MS031
            /// </summary>
            public const string MS031 = "MS-031";
        }

        /// <summary>
        /// EventHubName
        /// </summary>
        /// <remarks>IoT Hub → Event Grid → Event Hubとルーティングされたメッセージを送信するEvent Hub名を定義する</remarks>
        public static class EventHubNames
        {
            /// <summary>
            /// IoT Hubによるデバイス接続イベント
            /// </summary>
            public const string DeviceConnected = "device-connected";

            /// <summary>
            /// IoT Hubによるデバイス切断イベント
            /// </summary>
            public const string DeviceDisconnected = "device-disconnected";

            /// <summary>
            /// IoT Hubによるデバイスツイン更新イベント
            /// </summary>
            public const string TwinChanged = "twin-changed";
        }

        /// <summary>
        /// 配信ファイルタイプ
        /// </summary>
        /// <remarks>配信file種別マスタテーブルのコードの値</remarks>
        public static class DeliveryFileType
        {
            /// <summary>
            /// AlSoft
            /// </summary>
            public const string AlSoft = "rms";
            
            /// <summary>
            /// HotFixConsole
            /// </summary>
            public const string HotFixConsole = "hotfix_console";

            /// <summary>
            /// HotFixHobbit
            /// </summary>
            public const string HotFixHobbit = "hotfix_hobbit";

            /// <summary>
            /// Package
            /// </summary>
            public const string Package = "package";
        }

        /// <summary>
        /// 配信グループステータス
        /// </summary>
        /// <remarks>
        /// 配信グループデータテーブルが持つ配信グループステータスが取り得るデータ。
        /// テーブル名変更に際してトレーサビリティを確保できるようにDtDeliveryGroupクラス（Utility\Models）に持たせたいが、
        /// 当該ファイルが自動生成された際に、分割クラスのクラス名が追従しないためConstsクラスに集約した。
        /// </remarks>
        public static class DeliveryGroupStatus
        {
            /// <summary>
            /// 未開始（開始前）
            /// </summary>
            public const string NotStarted = "notstarted";

            /// <summary>
            /// 開始済み
            /// </summary>
            public const string Started = "started";

            /// <summary>
            /// 完了
            /// </summary>
            public const string Completed = "completed";
        }

        /// <summary>
        /// 適用結果ステータス
        /// </summary>
        /// <remarks>
        /// 適用データテーブルが持つ適用結果ステータスが取り得るデータ
        /// テーブル名変更に際してトレーサビリティを確保できるようにDtInstallResultHistoryクラス（Utility\Models）に持たせたいが、
        /// 当該ファイルが自動生成された際に、分割クラスのクラス名が追従しないためConstsクラスに集約した。
        /// </remarks>
        public static class InstallResultStatus
        {
            /// <summary>
            /// 未開始
            /// </summary>
            public const string NotStarted = "notstarted";

            /// <summary>
            /// メッセージ送信済み
            /// </summary>
            public const string MessageSent = "messagesent";

            /// <summary>
            /// 対象外
            /// </summary>
            public const string Nottarget = "nottarget";

            /// <summary>
            /// ダウンロード済み
            /// </summary>
            public const string Downloaded = "downloaded";

            /// <summary>
            /// 配布中
            /// </summary>
            public const string Dispatching = "dispatching";

            /// <summary>
            /// 配布済み
            /// </summary>
            public const string Dispatched = "dispatched";

            /// <summary>
            /// ユーザによる適用キャンセル
            /// </summary>
            public const string Usercanceled = "usercanceled";

            /// <summary>
            /// 適用済み
            /// </summary>
            public const string Installed = "installed";
        }

        /// <summary>
        /// 接続ステータス
        /// </summary>
        /// <remarks>
        /// 端末データテーブルが持つ接続ステータスが取り得るデータ
        /// テーブル名変更に際してトレーサビリティを確保できるようにDtDeviceクラス（Utility\Models）に持たせたいが、
        /// 当該ファイルが自動生成された際に、分割クラスのクラス名が追従しないためConstsクラスに集約した。
        /// </remarks>
        public static class ConnectStatus
        {
            /// <summary>
            /// 未接続（接続履歴なし）
            /// 未接続（接続歴なし）
            /// </summary>
            public const string Unconnected = "unconnected";

            /// <summary>
            /// 接続中
            /// </summary>
            public const string Connected = "connected";

            /// <summary>
            /// 切断
            /// </summary>
            public const string Disconnected = "disconnected";
        }

        /// <summary>
        /// 機器分類
        /// </summary>
        public static class EquipmentType
        {
            /// <summary>
            /// ゲートウェイ
            /// </summary>
            public const string Gateway = "gateway";

            /// <summary>
            /// デバイス
            /// </summary>
            public const string Device = "device";

            /// <summary>
            /// ユニット
            /// </summary>
            public const string Unit = "Unit";
        }

        /// <summary>
        /// 機器分類
        /// </summary>
        public static class InstallType
        {
            /// <summary>
            /// RSPC
            /// </summary>
            public const string Rspc = "rms_rspc";

            /// <summary>
            /// Console
            /// </summary>
            public const string ConsoleRspc = "rms_console_rspc";

            /// <summary>
            /// Console（RSPCレス）
            /// </summary>
            public const string ConsoleRspcless = "rms_console_rspcless";

            /// <summary>
            /// Console MINI（単独）
            /// </summary>
            public const string ConsoleMini = "rms_console_mini";

            /// <summary>
            /// Console MINI（Hobbit連携）
            /// </summary>
            public const string ConsoleMiniHobbit = "rms_console_mini_hobbit";

            /// <summary>
            /// Hobbitサーバ
            /// </summary>
            public const string HobbitServer = "rms_hobbit_server";

            /// <summary>
            /// Hobbitクライアント
            /// </summary>
            public const string HobbitClient = "rms_hobbit_client";

            /// <summary>
            /// リモート端末
            /// </summary>
            public const string Remote = "rms_remote";
        }
    }
}
