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
    /// 配信グループのWebAPI
    /// </summary>
    public class DeliveryGroupsController
    {
        /// <summary>
        /// 設定ファイル
        /// </summary>
        private readonly AppSettings _settings;

        /// <summary>
        /// 配信グループ用サービスクラス
        /// </summary>
        private readonly IDeliveryGroupService _service;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">設定</param>
        /// <param name="service">配信グループ用サービスクラス</param>
        public DeliveryGroupsController(AppSettings settings, IDeliveryGroupService service)
        {
            Assert.IfNull(settings);
            Assert.IfNull(service);

            _settings = settings;
            _service = service;
        }

        /// <summary>
        /// 配信グループを追加する
        /// </summary>
        /// <remarks>認証は、Headerにx-functions-keyかqueryにcodeのどちらかに関数キー設定する。セキュリティ上、codeは使用しないこと。</remarks>
        /// <param name="request">配信グループ情報</param>
        /// <param name="log">logger</param>
        /// <returns>追加された配信グループ情報</returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DeliveryGroupResponseDto))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [RequestHttpHeader("x-functions-key", isRequired: false)]
        [FunctionName(nameof(PostDeliveryGroup))]
        public IActionResult PostDeliveryGroup(
            [RequestBodyType(typeof(DeliveryGroupAddRequestDto), "配信グループ作成リクエスト")]
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "deliverygroups")]
            DeliveryGroupAddRequestDto request,
            ILogger log)
        {
            log.EnterJson("DeliveryGroupAddRequestDto: {0}", request);
            ActionResult response = null;

            try
            {
                // リクエストパラメータチェック
                if (RequestValidator.IsBadRequestParameter(request, out string message))
                {
                    log.Error(nameof(Resources.CO_API_DGC_002), new object[] { message });
                    return new BadRequestObjectResult(string.Empty);
                }

                var utilParam = request.ConvertDtoToUtility();
                var resultParam = _service.Create(utilParam);

                // Resultから返却ステータスを作成
                response = SqlResultConverter.ConvertToActionResult(
                    resultParam.ResultCode,
                    resultParam.Message,
                    resultParam.Entity.ConvertUtilityToResponseDto());
            }
            catch (Exception e)
            {
                log.Error(e, nameof(Resources.CO_API_DGC_001));
                response = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                log.LeaveJson("Response: {0}", response);
            }

            return response;
        }

        /// <summary>
        /// 配信グループを更新する
        /// </summary>
        /// <remarks>
        /// 対象のステータスが「配信前」以外の場合、403エラーを返す。
        /// 認証は、Headerにx-functions-keyかqueryにcodeのどちらかに関数キー設定する。セキュリティ上、codeは使用しないこと。
        /// </remarks>
        /// <param name="request">配信グループ情報</param>
        /// <param name="deliveryGroupId">配信グループID</param>
        /// <param name="log">logger</param>
        /// <returns>更新された配信グループ情報</returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DeliveryGroupResponseDto))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [RequestHttpHeader("x-functions-key", isRequired: false)]
        [FunctionName(nameof(PutDeliveryGroup))]
        public IActionResult PutDeliveryGroup(
            [RequestBodyType(typeof(DeliveryGroupUpdateRequestDto), "配信グループ更新リクエスト")]
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "deliverygroups/{deliveryGroupId}")]
            DeliveryGroupUpdateRequestDto request,
            long deliveryGroupId,
            ILogger log)
        {
            log.EnterJson($"{nameof(deliveryGroupId)}: {deliveryGroupId}, {typeof(DeliveryGroupUpdateRequestDto)}: {{0}}", request);
            ActionResult response = null;

            try
            {
                // リクエストパラメータチェック
                if (RequestValidator.IsBadRequestParameter(request, out string message))
                {
                    log.Error(nameof(Resources.CO_API_DGU_002), new object[] { message });
                    return new BadRequestObjectResult(string.Empty);
                }

                var utilParam = request.ConvertDtoToUtility(deliveryGroupId);
                var resultParam = _service.Update(utilParam);

                // Resultから返却ステータスを作成
                response = SqlResultConverter.ConvertToActionResult(
                    resultParam.ResultCode,
                    resultParam.Message,
                    resultParam.Entity.ConvertUtilityToResponseDto());
            }
            catch (Exception e)
            {
                log.Error(e, nameof(Resources.CO_API_DGU_001));
                response = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                log.LeaveJson("Response: {0}", response);
            }

            return response;
        }

        /// <summary>
        /// 配信グループを削除する
        /// </summary>
        /// <remarks>
        /// 対象のステータスが「配信前」以外の場合、403エラーを返す。
        /// 認証は、Headerにx-functions-keyかqueryにcodeのどちらかに関数キー設定する。セキュリティ上、codeは使用しないこと。
        /// </remarks>
        /// <param name="request">リクエスト</param>
        /// <param name="deliveryGroupId">配信グループID</param>
        /// <param name="rowVersion">楽観的同時制御用のバージョン番号</param>
        /// <param name="log">logger</param>
        /// <returns>結果</returns>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [RequestHttpHeader("x-functions-key", isRequired: false)]
        [FunctionName(nameof(DeleteDeliveryGroup))]
        public IActionResult DeleteDeliveryGroup(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "deliverygroups/{deliveryGroupId}/{rowVersion}")]
            HttpRequest request,
            long deliveryGroupId,
            long rowVersion,
            ILogger log)
        {
            log.Enter($"{nameof(deliveryGroupId)}: {deliveryGroupId}, {nameof(rowVersion)}: {rowVersion}");
            ActionResult response = null;

            try
            {
                // サービス以下の機能を利用してDBにリクエストデータを追加する
                var resultParam = _service.Delete(deliveryGroupId, WebApiHelper.ConvertLongToByteArray(rowVersion));

                // Resultから返却ステータスを作成（レスポンスボディは空）
                response = SqlResultConverter.ConvertToActionResult(resultParam.ResultCode, resultParam.Message);
            }
            catch (Exception e)
            {
                log.Error(e, nameof(Resources.CO_API_DGD_001));
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
