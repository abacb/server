using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Rms.Server.Core.Azure.Functions.Dispatcher.Models
{
    /// <summary>
    /// イベント情報
    /// </summary>
    /// <remarks>
    /// IoTHubs - EventGrid - EventHubs経由で実際に取得されたイベントをもとに作成。
    /// 参照しないところは無駄に作成していない。
    /// 構造は以下のDeviceConnected/DeviceDisconnectedを前提とする。
    /// https://docs.microsoft.com/ja-jp/azure/event-grid/event-schema-iot-hub
    /// </remarks>
    public class DeviceConnectionEvent
    {
        /// <summary>
        /// Data
        /// </summary>
        [JsonProperty("data")]
        public IoTHubConnectionEvent Data { get; set; }

        /// <summary>
        /// EventTime
        /// </summary>
        [JsonProperty("eventTime")]
        public DateTime EventTime { get; set; }

        /// <summary>
        /// エッジID。参照前に安全性を確認すること。
        /// </summary>
        [JsonIgnore]
        public Guid EdgeId => GetEdgeId().Value;

        /// <summary>
        /// RawBody
        /// </summary>
        /// <remarks>
        /// Bodyプロパティの一要素を保持する
        /// Bodyプロパティと同一のスキーマ（JSON文字列）で扱うため、
        /// JSON配列をシリアライズしたものを保持する
        /// </remarks>
        [JsonIgnore]
        public string RawBody { get; set; }

        /// <summary>
        /// 本クラスにデシリアライズを行う。期待しているデータが含まれない場合、例外を投げる。
        /// </summary>
        /// <param name="body">json文字列</param>
        /// <returns>DispatchedEvent</returns>
        public static DeviceConnectionEvent[] DeserializeIfInvalidThrowEx(string body)
        {
            DeviceConnectionEvent[] deviceConnectionEvents = JsonConvert.DeserializeObject<DeviceConnectionEvent[]>(body);

            if (deviceConnectionEvents.Length == 0)
            {
                throw new ArgumentException(string.Format("Bodyの配列が空です。{0}", body));
            }

            // Bodyを文字列リストにパースした結果をRawBodyに格納する
            object[] rawBodies = JsonConvert.DeserializeObject<object[]>(body);

            if (rawBodies.Length != deviceConnectionEvents.Length)
            {
                throw new ArgumentException(string.Format("Bodyの配列サイズが一致しません。{0}", body));
            }

            for (int i = 0; i < rawBodies.Length; i++)
            {
                // RawBodyはエラー時にBlobに保存する
                // Blobから取り出したデータをBodyプロパティと同一のスキーマ（JSON配列）で扱うため、
                // 一度配列に変換してからシリアライズする
                deviceConnectionEvents[i].RawBody = JsonConvert.SerializeObject(new object[] { rawBodies[i] });
            }

            return deviceConnectionEvents;
        }

        /// <summary>
        /// 正常性検査
        /// </summary>
        /// <returns>正常: true 異常:false</returns>
        public bool HasNecessary()
        {
            if (GetEdgeId() != null && EventTime != null && RawBody != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// エッジID取得
        /// </summary>
        /// <returns>エッジID</returns>
        private Guid? GetEdgeId()
        {
            return Data?.EdgeId;
        }

        /// <summary>
        /// IoTHubConnectionEvent
        /// </summary>
        public class IoTHubConnectionEvent
        {
            /// <summary>
            /// DeviceId
            /// </summary>
            [JsonProperty("deviceId")]
            public Guid? EdgeId { get; set; }
        }
    }
}
