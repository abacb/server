using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Core.Azure.Functions.Dispatcher.Models
{
    /// <summary>
    /// イベント情報
    /// </summary>
    /// <remarks>
    /// ツイン変更イベントではEventData#Bodyに以下の形式でデータが格納される
    ///     {
    ///         "version": 1,
    ///         "properties": 
    ///         {
    ///             "reported":  { ... },
    ///             "$metadata": { ... }
    ///         }
    ///     }
    /// 参考：https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-device-twins
    /// </remarks>
    public class DeviceTwinEvent
    {
        /// <summary>
        /// 発生元機器UID
        /// </summary>
        [Required]
        [JsonProperty("properties")]
        public Properties Data { get; set; }

        /// <summary>
        /// reportedプロパティ
        /// </summary>
        [JsonIgnore]
        public JToken Reported => Data?.Reported;

        /// <summary>
        /// 本クラスにデシリアライズを行う。期待しているデータが含まれない場合、例外を投げる。
        /// </summary>
        /// <param name="body">json文字列</param>
        /// <returns>DispatchedEvent</returns>
        public static DeviceTwinEvent DeserializeIfInvalidThrowEx(string body)
        {
            DeviceTwinEvent deviceTwinEvent = JsonConvert.DeserializeObject<DeviceTwinEvent>(body);

            if (deviceTwinEvent.Reported == null)
            {
                throw new ArgumentException(string.Format("必要な情報が含まれていません。{0}", body));
            }

            return deviceTwinEvent;
        }

        /// <summary>
        /// SystemPropertiesからEdgeIDを取得する
        /// </summary>
        /// <param name="eventData">イベント情報</param>
        /// <returns>EdgeID</returns>
        /// <remarks>
        /// - EventDataがnullの場合はArgumentNullExceptionを投げる
        /// - EdgeIDをSystemPropertiesから取得できない場合はFormatExceptionを投げる
        /// </remarks>
        public static Guid GetEdgeId(EventData eventData)
        {
            if (eventData == null)
            {
                throw new ArgumentNullException("イベント情報がnullであるためエッジIDを取得できません。");
            }

            string id = string.Empty;
            foreach (KeyValuePair<string, object> property in eventData.SystemProperties)
            {
                if (property.Key.Equals("iothub-connection-device-id"))
                {
                    id = property.Value.ToString();
                    break;
                }
            }

            return Guid.Parse(id);  // throws FormatException
        }

        /// <summary>
        /// Properties
        /// </summary>
        public class Properties
        {
            /// <summary>
            /// reported
            /// </summary>
            [JsonProperty("reported")]
            public JToken Reported { get; set; }
        }
    }
}
