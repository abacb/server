////using Microsoft.ApplicationInsights.Channel;
////using Microsoft.ApplicationInsights.DataContracts;
////using Microsoft.ApplicationInsights.Extensibility;
////using Microsoft.AspNetCore.Http;
////using System.Linq;
////using System.Net.Http.Headers;

////namespace Rms.Server.Core.Azure.Functions.WebApi.Utility
////{
////　　//// #7205全リクエスト情報の表示に関連するコード
////　　//https://lychee.mtlan.meta.co.jp/redmine/issues/7205#note-14
////　　//20200416定例にて全リクエスト情報のAzureでの監視は、対応しないことになったため
////　　//https://github.com/microsoft/ApplicationInsights-dotnet/issues/1152 を参考にコーディングしたが、効果を確認できなかった。(WebApi/StartUp/FunctionAppStartup.cs)
////    
////    /// <summary>
////    /// Httpヘッダをテレメトリに送信する
////    /// </summary>
////    /// <remarks>
////    /// https://blog.shibayan.jp/entry/20190405/1554459340
////    /// </remarks>
////    public class HeaderTelemetryInitializer : ITelemetryInitializer
////    {
////        //// クラスの使用者側で送信対象を仕込めるようにしたい時用にコメントアウトしとく。 
////        ////public List<string> RequestHeaders { get; set; }
////        ////public List<string> ResponseHeaders { get; set; }

////        ////public HeaderTelemetryInitializer()
////        ////{
////        ////    RequestHeaders = new List<string>();
////        ////    ResponseHeaders = new List<string>();
////        ////}

////       /// <summary>
////        /// 指定されたオブジェクトのプロパティを初期化する
////        /// </summary>
////        /// <param name="telemetry">テレメトリー</param>
////        public void Initialize(ITelemetry telemetry)
////        {
////            if (!(telemetry is DependencyTelemetry dependency))
////            {
////                return;
////            }

////            if (dependency.TryGetOperationDetail("HttpRequest", out var details) && details is HttpRequest request)
////            {
////                foreach (var (key, value) in request.Headers)
////                {
////                    dependency.Properties.Add(key, value.FirstOrDefault());
////                }
////            }
////            else if (dependency.TryGetOperationDetail("HttpRequestHeaders", out details) && details is HttpRequestHeaders headers)
////            {
////                foreach (var (key, value) in headers)
////                {
////                    dependency.Properties.Add(key, value.FirstOrDefault());
////                }
////            }
////            else
////            {
////                return;
////            }
////        }
////    }
////}
