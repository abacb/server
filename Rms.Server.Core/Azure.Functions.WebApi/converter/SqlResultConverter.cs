using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rms.Server.Core.Utility.Models;

namespace Rms.Server.Core.Azure.Functions.WebApi.Converter
{
    /// <summary>
    /// DB操作結果を変換するコンバータクラス
    /// </summary>
    public static class SqlResultConverter
    {
        /// <summary>
        /// DB操作結果をActionResultに変換する
        /// </summary>
        /// <typeparam name="T">Dtoクラス型</typeparam>
        /// <param name="resultCode">結果コード</param>
        /// <param name="message">結果メッセージ</param>
        /// <param name="dto">response用Dtoクラスパラメータ</param>
        /// <returns>結果コードに対応するActionResult</returns>
        public static ActionResult ConvertToActionResult<T>(ResultCode resultCode, string message, T dto)
        {
            ActionResult action;
            switch (resultCode)
            {
                case ResultCode.Succeed:
                    action = new OkObjectResult(dto);
                    break;
                case ResultCode.Conflict:
                    //// Swagger定義に合わせて、エラー時のBodyには何も設定しない
                    action = new ConflictObjectResult(string.Empty);
                    break;
                case ResultCode.DoStartedDelivery:
                    action = new StatusCodeResult(StatusCodes.Status403Forbidden); // ObjectResultに403相当のものがない
                    break;
                case ResultCode.NotFound:
                    //// Swagger定義に合わせて、エラー時のBodyには何も設定しない
                    action = new NotFoundObjectResult(string.Empty);
                    break;
                case ResultCode.ParameterError:
                    //// Swagger定義に合わせて、エラー時のBodyには何も設定しない
                    action = new BadRequestObjectResult(string.Empty);
                    break;
                default:
                    action = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                    break;
            }

            return action;
        }

        /// <summary>
        /// DB操作結果をボディなしのActionResultに変換する
        /// </summary>
        /// <param name="resultCode">結果コード</param>
        /// <param name="message">結果メッセージ</param>
        /// <returns>結果コードに対応するActionResult</returns>
        public static ActionResult ConvertToActionResult(ResultCode resultCode, string message)
        {
            // ステータスコード200のケースで、
            // 空のレスポンスボディを期待してDTOクラスにnullを渡した場合
            // 200 ではなく 204 が返るので注意
            return ConvertToActionResult(resultCode, message, new { });
        }
    }
}
