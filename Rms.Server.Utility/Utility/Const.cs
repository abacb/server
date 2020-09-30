using System;
using System.Collections.Generic;
using System.Text;

namespace Rms.Server.Utility.Utility
{
    /// <summary>
    /// 定数クラス
    /// </summary>
    public static class Const
    {
        /// <summary>
        /// Failureストレージにアップロードするファイルのファイルパスのフォーマット
        /// </summary>
        public const string FailureBlobFilenameFormat = "{0}/{2:yyyy}/{2:MM}/{2:dd}/{1}_{2:yyyyMMddHHmmssfff}.json";

        /// <summary>
        /// Failureストレージにアップロードするファイルのファイルパスのフォーマット（メッセージIDが無い場合）
        /// </summary>
        public const string FailureBlobFilenameFormatWithoutMessageId = "{0}/{2:yyyy}/{2:MM}/{2:dd}/{2:yyyyMMddHHmmssfff}.json";

        /// <summary>
        /// アラームキューの日付形式データのフォーマット
        /// </summary>
        public const string AlarmQueueDateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss'Z'";

        /// <summary>
        /// インベントリ設定詳細情報 ACTIVE LINEソフトウェア名
        /// </summary>
        public const string InventoryAlSoftwareName = "ALSoftware";

        /// <summary>
        /// インベントリ設定詳細情報 設定情報名
        /// </summary>
        public const string InventorySettingInfoName = "SettingInfo";

        /// <summary>
        /// インベントリ設定詳細情報 オプション名
        /// </summary>
        public const string InventoryOptionName = "Option";

        /// <summary>
        /// インベントリ設定詳細情報 温度監視名
        /// </summary>
        public const string InventoryTemperatureSensorName = "TemperatureSensor";

        /// <summary>
        /// インベントリ設定詳細情報 DXA名
        /// </summary>
        public const string InventoryDxaName = "DXA";

        /// <summary>
        /// インベントリ設定詳細情報オプション ON
        /// </summary>
        public const string InventorySettingInfoOptionOn = "ON";

        /// <summary>
        /// 接続ステータス 接続中
        /// </summary>
        public const string ConnectStatusConnected = "connected";

        /// <summary>
        /// 接続ステータス 未接続
        /// </summary>
        public const string ConnectStatusUnconnected = "unconnected";

        /// <summary>
        /// 通信監視アラーム定義監視対象温度センサ
        /// </summary>
        public const string AlarmDefTargetTemperature = "temperature";

        /// <summary>
        /// 通信監視アラーム定義監視対象DXA
        /// </summary>
        public const string AlarmDefTargetDxa = "dxa";
    }
}
