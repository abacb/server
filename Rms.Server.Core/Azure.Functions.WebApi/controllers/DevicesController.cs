using AzureFunctions.Extensions.Swashbuckle.Attribute;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Rms.Server.Core.Azure.Functions.WebApi.Converter;
using Rms.Server.Core.Azure.Functions.WebApi.Dto;
using Rms.Server.Core.Azure.Functions.WebApi.Utility;
using Rms.Server.Core.Azure.Utility.Validations;
using Rms.Server.Core.Service.Services;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Extensions;
using Rms.Server.Core.Utility.Properties;
using RmsRms.Server.Core.Azure.Functions.WebApi.Dto;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Rms.Server.Core.Azure.Functions.WebApi.Controllers
{
    /// <summary>
    /// 機器データのWebAPI
    /// </summary>
    public class DevicesController
    {
        /// <summary>
        /// 設定
        /// </summary>
        private readonly AppSettings _settings;

        /// <summary>
        /// DeviceService
        /// </summary>
        private readonly IDeviceService _deviceService;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">設定</param>
        /// <param name="deviceService">deviceService</param>
        public DevicesController(
            AppSettings settings,
            IDeviceService deviceService)
        {
            Assert.IfNull(settings);
            Assert.IfNull(deviceService);

            _settings = settings;
            _deviceService = deviceService;
        }

        /// <summary>
        /// デバイスを追加する
        /// </summary>
        /// <remarks>認証は、Headerにx-functions-keyかqueryにcodeのどちらかに関数キー設定する。セキュリティ上、codeは使用しないこと。</remarks>
        /// <param name="request">デバイス追加リクエスト</param>
        /// <param name="log">ロガー</param>
        /// <returns>追加された配信ファイル情報</returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DeviceResponseDto))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [RequestHttpHeader("x-functions-key", isRequired: false)]
        [FunctionName(nameof(PostDevice))]
        public IActionResult PostDevice(
            [RequestBodyType(typeof(DeviceAddRequestDto), "デバイス作成リクエスト")]
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "devices")]
            DeviceAddRequestDto request,
            ILogger log)
        {
            log.EnterJson("DeviceAddRequestDto: {0}", request);
            ActionResult response = null;

            try
            {
                // リクエストパラメータチェック
                string message = string.Empty;
                if (RequestValidator.IsBadRequestParameter(request, out message))
                {
                    log.Error(nameof(Resources.CO_API_DVC_002), new object[] { message });
                    return new BadRequestObjectResult(string.Empty);
                }

                // DTOを設置設定インスタンスに変換する
                var baseConfig = request.ConvertDtoToInstallBaseConfig();

                // DTOをUtility型に変換する
                var utilParam = request.ConvertDtoToUtility();

                // 作成実行
                var resultParam = _deviceService.Create(utilParam, baseConfig);

                // Resultから返却ステータスを作成
                return SqlResultConverter.ConvertToActionResult(
                    resultParam.ResultCode,
                    resultParam.Message,
                    resultParam.Entity.ConvertUtilityToResponseDto());
            }
            catch (Exception e)
            {
                log.Error(e, nameof(Resources.CO_API_DVC_001));
                response = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                log.Leave($"Response Status: {response}");
            }

            return response;
        }

        /// <summary>
        /// デバイスを更新する
        /// </summary>
        /// <remarks>認証は、Headerにx-functions-keyかqueryにcodeのどちらかに関数キー設定する。セキュリティ上、codeは使用しないこと。</remarks>
        /// <param name="request">デバイス更新リクエスト</param>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="log">ロガー</param>
        /// <returns>更新された配信ファイル情報</returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DeviceResponseDto))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [RequestHttpHeader("x-functions-key", isRequired: false)]
        [FunctionName(nameof(PutDevice))]
        public IActionResult PutDevice(
            [RequestBodyType(typeof(DeviceUpdateRequestDto), "デバイス更新リクエスト")]
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "devices/{deviceId}")]
            DeviceUpdateRequestDto request,
            long deviceId,
            ILogger log)
        {
            log.EnterJson("DeviceUpdateRequestDto: {0}", request);
            ActionResult response = null;

            try
            {
                // リクエストパラメータチェック
                string message = string.Empty;
                if (RequestValidator.IsBadRequestParameter(request, out message))
                {
                    log.Error(nameof(Resources.CO_API_DVU_002), new object[] { message });
                    return new BadRequestObjectResult(string.Empty);
                }

                // DTOを設置設定インスタンスに変換する
                var baseConfig = request.ConvertDtoToInstallBaseConfig();

                // DTOをUtility型に変換する(SIDも含める)
                var utilParam = request.ConvertDtoToUtility();
                utilParam.Sid = deviceId;

                // 更新実行
                var resultParam = _deviceService.Update(utilParam, baseConfig);

                // Resultから返却ステータスを作成
                return SqlResultConverter.ConvertToActionResult(
                    resultParam.ResultCode,
                    resultParam.Message,
                    resultParam.Entity.ConvertUtilityToResponseDto());
            }
            catch (Exception e)
            {
                log.Error(e, nameof(Resources.CO_API_DVU_001));
                response = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                log.Leave($"Response Status: {response}");
            }

            return response;
        }

        /// <summary>
        /// 機器に対してリモート接続を開始する
        /// </summary>
        /// <remarks>認証は、Headerにx-functions-keyかqueryにcodeのどちらかに関数キー設定する。セキュリティ上、codeは使用しないこと。</remarks>
        /// <param name="request">リモート接続開始リクエスト</param>
        /// <param name="deviceId">端末データID</param>
        /// <param name="log">ロガー</param>
        /// <returns>結果</returns>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [RequestHttpHeader("x-functions-key", isRequired: false)]
        [FunctionName("PostDeviceRemote")]
        public async Task<IActionResult> PostDeviceRemoteAsync(
            [RequestBodyType(typeof(DeviceRemoteRequestDto), "リモート接続開始リクエスト")]
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "devices/{deviceId}/remote")]
            DeviceRemoteRequestDto request,
            long deviceId,
            ILogger log)
        {
            log.Enter($"{nameof(request)}: {{0}}, {nameof(deviceId)}: {{1}}", new object[] { request, deviceId });
            ActionResult response = null;

            try
            {
                // リクエストパラメータチェック
                if (RequestValidator.IsBadRequestParameter(request, out var message))
                {
                    log.Error(nameof(Resources.CO_API_DRC_002), new object[] { message });
                    return new BadRequestObjectResult(string.Empty);
                }

                var requestRemote = request.ConvertDtoToUtility(deviceId);
                var resultParam = await _deviceService.RequestRemoteAsync(requestRemote);

                // Resultから返却ステータスを作成（レスポンスボディは空）
                response = SqlResultConverter.ConvertToActionResult(resultParam.ResultCode, resultParam.Message);
            }
            catch (Exception e)
            {
                log.Error(e, nameof(Resources.CO_API_DRC_001));
                response = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                log.Leave($"Response Status: {response}");
            }

            return response;
        }
    }
}
