using AzureFunctions.Extensions.Swashbuckle;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rms.Server.Core.Azure.Functions.WebApi.Controllers
{
    /// <summary>
    /// Swagger関連API
    /// </summary>
    public static class SwaggerController
    {
        /// <summary>
        /// Swagger Json
        /// </summary>
        /// <param name="req">リクエスト</param>
        /// <param name="swashBuckleClient">swashBuckleClient</param>
        /// <returns>swagger json</returns>
        [SwaggerIgnore]
        [FunctionName(nameof(Swagger))]
        public static Task<HttpResponseMessage> Swagger(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "swagger/json")]
            HttpRequestMessage req,
            [SwashBuckleClient] ISwashBuckleClient swashBuckleClient)
        {
            return Task.FromResult(swashBuckleClient.CreateSwaggerDocumentResponse(req));
        }

        /// <summary>
        /// Swagger UI
        /// </summary>
        /// <param name="req">リクエスト</param>
        /// <param name="swashBuckleClient">swashBuckleClient</param>
        /// <returns>swagger ui</returns>
        [SwaggerIgnore]
        [FunctionName(nameof(SwaggerUi))]
        public static Task<HttpResponseMessage> SwaggerUi(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "swagger/ui")]
            HttpRequestMessage req,
            [SwashBuckleClient] ISwashBuckleClient swashBuckleClient)
        {
            return Task.FromResult(swashBuckleClient.CreateSwaggerUIResponse(req, "swagger/json"));
        }
    }
}