using Newtonsoft.Json;
using System.Collections.Generic;

namespace Rms.Server.Core.Service.Models
{
    /// <summary>
    /// 設置設定
    /// </summary>
    [JsonObject]
    public class InstallBaseConfig
    {
        /// <summary>
        /// 自機の設置設定
        /// </summary>
        [JsonProperty("own")]
        public InstallBaseDeviceConfig OwnConfig { get; set; }

        /// <summary>
        /// 親機の設置設定
        /// </summary>
        [JsonProperty("parent")]
        public InstallBaseDeviceConfig ParentConfig { get; set; }

        /// <summary>
        /// 子機の設置設定
        /// </summary>
        [JsonProperty("children")]
        public List<InstallBaseDeviceConfig> ChildrenConfig { get; set; }
    }
}