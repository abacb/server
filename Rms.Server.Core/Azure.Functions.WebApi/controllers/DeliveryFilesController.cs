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
using System;
using System.Net;

namespace Rms.Server.Core.Azure.Functions.WebApi.Controllers
{
    /// <summary>
    /// 配信ファイルのWebAPI
    /// </summary>
    public class DeliveryFilesController
    {
        /// <summary>
        /// 設定
        /// </summary>
        private readonly AppSettings _settings;

        /// <summary>
        /// 配信ファイル用サービスクラス
        /// </summary>
        private readonly IDeliveryFileService _service;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">設定</param>
        /// <param name="service">配信ファイル用サービスクラス</param>
        public DeliveryFilesController(AppSettings settings, IDeliveryFileService service)
        {
            Assert.IfNull(settings);
            Assert.IfNull(service);

            _settings = settings;
            _service = service;
        }

        /// <summary>
        /// 配信ファイルを追加する
        /// </summary>
        /// <remarks>認証は、Headerにx-functions-keyかqueryにcodeのどちらかに関数キー設定する。セキュリティ上、codeは使用しないこと。</remarks>
        /// <param name="request">配信ファイル情報</param>
        /// <param name="log">ロガー</param>
        /// <returns>追加された配信ファイル情報</returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DeliveryFileResponseDto))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [RequestHttpHeader("x-functions-key", isRequired: false)]
        [FunctionName(nameof(PostDeliveryFile))]
        public IActionResult PostDeliveryFile(
            [RequestBodyType(typeof(DeliveryFileAddRequestDto), "配信ファイル作成リクエスト")]
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "deliveryfiles")]
            DeliveryFileAddRequestDto request,
            ILogger log)
        {
            log.EnterJson("DeliveryFileAddRequestDto: {0}", request);
            ActionResult response = null;

            try
            {
                DeliveryFileAddRequestDto requestByType = RequestConverter.Convert(request);    // リクエストオブジェクトがnullの場合にはnullが返る
                // リクエストパラメータチェック
                if (RequestValidator.IsBadRequestParameter(requestByType, out string message))
                {
                    log.Error(nameof(Resources.CO_API_DFC_002), new object[] { message });
                    return new BadRequestObjectResult(string.Empty);
                }

                var utilParam = requestByType.ConvertDtoToUtility();
                var resultParam = _service.Create(utilParam);

                // Resultから返却ステータスを作成
                response = SqlResultConverter.ConvertToActionResult(
                    resultParam.ResultCode,
                    resultParam.Message,
                    resultParam.Entity.ConvertUtilityToResponseDto());
            }
            catch (Exception e)
            {
                log.Error(e, nameof(Resources.CO_API_DFC_001));
                response = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                log.LeaveJson("Response: {0}", response);
            }

            return response;
        }

        /// <summary>
        /// 配信ファイルを更新する
        /// </summary>
        /// <remarks>
        /// 対象配信ファイルに紐づく配信グループのステータスが、1つ以上「配信前」以外の場合、403エラーを返す。
        /// 認証は、Headerにx-functions-keyかqueryにcodeのどちらかに関数キー設定する。セキュリティ上、codeは使用しないこと。
        /// </remarks>
        /// <param name="request">配信ファイル情報</param>
        /// <param name="deliveryFileId">配信ファイルID</param>
        /// <param name="log">ロガー</param>
        /// <returns>更新された配信ファイル情報</returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DeliveryFileResponseDto))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [RequestHttpHeader("x-functions-key", isRequired: false)]
        [FunctionName(nameof(PutDeliveryFile))]
        public IActionResult PutDeliveryFile(
            [RequestBodyType(typeof(DeliveryFileUpdateRequestDto), "配信ファイル更新リクエスト")]
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "deliveryfiles/{deliveryFileId}")]
            DeliveryFileUpdateRequestDto request,
            long deliveryFileId,
            ILogger log)
        {
            log.EnterJson($"{nameof(deliveryFileId)}: {deliveryFileId}, {typeof(DeliveryFileUpdateRequestDto)}: {{0}}", request);
            ActionResult response = null;

            try
            {
                DeliveryFileUpdateRequestDto requestByType = RequestConverter.Convert(request);    // リクエストオブジェクトがnullの場合にはnullが返る
                // リクエストパラメータチェック
                if (RequestValidator.IsBadRequestParameter(request, out string message))
                {
                    log.Error(nameof(Resources.CO_API_DFU_002), new object[] { message });
                    return new BadRequestObjectResult(string.Empty);
                }

                var utilParam = requestByType.ConvertDtoToUtility(deliveryFileId);
                var resultParam = _service.Update(utilParam);

                // Resultから返却ステータスを作成
                response = SqlResultConverter.ConvertToActionResult(
                    resultParam.ResultCode,
                    resultParam.Message,
                    resultParam.Entity.ConvertUtilityToResponseDto());
            }
            catch (Exception e)
            {
                log.Error(e, nameof(Resources.CO_API_DFU_001));
                response = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                log.LeaveJson("Response: {0}", response);
            }

            return response;
        }

        /// <summary>
        /// 配信ファイルを削除する
        /// </summary>
        /// <remarks>
        /// 対象配信ファイルに紐づく配信グループのステータスが、1つ以上「配信前」以外の場合、403エラーを返す。
        /// 認証は、Headerにx-functions-keyかqueryにcodeのどちらかに関数キー設定する。セキュリティ上、codeは使用しないこと。
        /// </remarks>
        /// <param name="request">配信ファイル削除情報</param>
        /// <param name="deliveryFileId">配信ファイルID</param>
        /// <param name="rowVersion">楽観的同時制御用のバージョン番号</param>
        /// <param name="log">ロガー</param>
        /// <returns>結果</returns>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [RequestHttpHeader("x-functions-key", isRequired: false)]
        [FunctionName(nameof(DeleteDeliveryFile))]
        public IActionResult DeleteDeliveryFile(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "deliveryfiles/{deliveryFileId}/{rowVersion}")]
            HttpRequest request,
            long deliveryFileId,
            long rowVersion,
            ILogger log)
        {
            log.Enter($"{nameof(deliveryFileId)}: {deliveryFileId}, {nameof(rowVersion)}: {rowVersion}");
            ActionResult response = null;

            try
            {
                // サービス以下の機能を利用してDBにリクエストデータを追加する
                var resultParam = _service.Delete(deliveryFileId, WebApiHelper.ConvertLongToByteArray(rowVersion));

                // Resultから返却ステータスを作成（レスポンスボディは空）
                response = SqlResultConverter.ConvertToActionResult(resultParam.ResultCode, resultParam.Message);
            }
            catch (Exception e)
            {
                log.Error(e, nameof(Resources.CO_API_DFD_001));
                response = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                log.LeaveJson("Response: {0}", response);
            }

            return response;
        }

        /// <summary>
        /// 配信ファイルの配信を中止/開始する
        /// </summary>
        /// <remarks>認証は、Headerにx-functions-keyかqueryにcodeのどちらかに関数キー設定する。セキュリティ上、codeは使用しないこと。</remarks>
        /// <param name="request">リクエスト</param>
        /// <param name="deliveryFileId">配信ファイルID</param>
        /// <param name="log">ロガー</param>
        /// <returns>結果</returns>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [RequestHttpHeader("x-functions-key", isRequired: false)]
        [FunctionName(nameof(PutDeliveryFileStatus))]
        public IActionResult PutDeliveryFileStatus(
            [RequestBodyType(typeof(DeliveryFileStatusUpdateRequestDto), "配信ファイル更新リクエスト")]
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "deliveryfiles/{deliveryFileId}/status")]
            DeliveryFileStatusUpdateRequestDto request,
            long deliveryFileId,
            ILogger log)
        {
            log.EnterJson("Request: {0}", request.ToStringProperties());
            ActionResult response = null;

            try
            {
                // リクエストパラメータチェック
                string message = string.Empty;
                if (RequestValidator.IsBadRequestParameter(request, out message))
                {
                    log.Error(nameof(Resources.CO_API_DSU_002), new object[] { message });
                    return new BadRequestObjectResult(string.Empty);
                }

                var utilParam = request.ConvertDtoToUtility(deliveryFileId);
                var resultParam = _service.PutCancelFlag(utilParam);

                // Resultから返却ステータスを作成（レスポンスボディは空）
                response = SqlResultConverter.ConvertToActionResult(resultParam.ResultCode, resultParam.Message);
            }
            catch (Exception e)
            {
                log.Error(e, nameof(Resources.CO_API_DSU_001));
                response = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                log.LeaveJson("Response: {0}", response);
            }

            return response;
        }
    }
}
