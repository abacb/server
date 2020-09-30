using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Extensions;
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
    /// 構造は以下のDeviceTelemetoryを前提とする。
    /// https://docs.microsoft.com/ja-jp/azure/event-grid/event-schema-iot-hub
    /// </remarks>
    public class DispatchedEvent
    {
        /// <summary>
        /// Data
        /// </summary>
        [JsonProperty("data")]
        public IoTHubEvent Data { get; set; }

        /// <summary>
        /// メッセージID
        /// </summary>
        [JsonIgnore]
        public string MessageId => GetMessageId();

        /// <summary>
        /// メッセージ日時。参照前に安全性を確認すること。
        /// </summary>
        [JsonIgnore]
        public DateTime Enqueuedtime => GetEnqueuedtime().Value;

        /// <summary>
        /// エッジID。参照前に安全性を確認すること。
        /// </summary>
        [JsonIgnore]
        public Guid EdgeId => GetEdgeId().Value;

        /// <summary>
        /// Body
        /// </summary>
        [JsonIgnore]
        public string Body => GetBody()?.ToStringJson();

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
        /// <param name="log">ロガー</param>
        /// <returns>DispatchedEvent</returns>
        public static DispatchedEvent[] DeserializeIfInvalidThrowEx(string body, ILogger log)
        {
            DispatchedEvent[] dispatchedEvents = JsonConvert.DeserializeObject<DispatchedEvent[]>(body);

            if (dispatchedEvents.Length == 0)
            {
                throw new ArgumentException(string.Format("Bodyの配列が空です。{0}", body));
            }

            // Bodyを文字列リストにパースした結果をRawBodyに格納する
            object[] rawBodies = JsonConvert.DeserializeObject<object[]>(body);

            if (rawBodies.Length != dispatchedEvents.Length)
            {
                throw new ArgumentException(string.Format("Bodyの配列サイズが一致しません。{0}", body));
            }

            for (int i = 0; i < rawBodies.Length; i++)
            {
                // RawBodyはエラー時にBlobに保存する
                // Blobから取り出したデータをBodyプロパティと同一のスキーマ（JSON配列）で扱うため、
                // 一度配列に変換してからシリアライズする
                dispatchedEvents[i].RawBody = JsonConvert.SerializeObject(new object[] { rawBodies[i] });
            }

            return dispatchedEvents;
        }

        /// <summary>
        /// 正常性検査
        /// </summary>
        /// <returns>正常: true 異常:false</returns>
        public bool HasNecessary()
        {
            if (GetMessageId() != null &&
                GetEnqueuedtime() != null &&
                GetEdgeId() != null &&
                GetBody() != null &&
                RawBody != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// メッセージID取得
        /// </summary>
        /// <returns>メッセージID</returns>
        private string GetMessageId()
        {
            return Data?.SystemProperty?.MessageId;
        }

        /// <summary>
        /// メッセージ日時取得
        /// </summary>
        /// <returns>メッセージ日時</returns>
        private DateTime? GetEnqueuedtime()
        {
            return Data?.SystemProperty?.Enqueuedtime;
        }

        /// <summary>
        /// エッジID取得
        /// </summary>
        /// <returns>エッジID</returns>
        private Guid? GetEdgeId()
        {
            return Data?.SystemProperty?.EdgeId;
        }

        /// <summary>
        /// Body取得
        /// </summary>
        /// <returns>Body</returns>
        private object GetBody()
        {
            return Data?.Body;
        }

        /// <summary>
        /// IoTHubEvent
        /// </summary>
        public class IoTHubEvent
        {
            /// <summary>
            /// SystemProperty
            /// </summary>
            [JsonProperty("systemProperties")]
            public SystemProperty SystemProperty { get; set; }

            /// <summary>
            /// Body
            /// </summary>
            [JsonProperty("body")]
            public object Body { get; set; }
        }

        /// <summary>
        /// SystemProperty
        /// </summary>
        public class SystemProperty
        {
            /// <summary>
            /// MessageId
            /// </summary>
            [JsonProperty("message-id")]
            public string MessageId { get; set; }

            /// <summary>
            /// Enqueuedtime
            /// </summary>
            [JsonProperty("iothub-enqueuedtime")]
            public DateTime? Enqueuedtime { get; set; }

            /// <summary>
            /// EdgeId
            /// </summary>
            [JsonProperty("iothub-connection-device-id")]
            public Guid? EdgeId { get; set; }
        }
    }
}
