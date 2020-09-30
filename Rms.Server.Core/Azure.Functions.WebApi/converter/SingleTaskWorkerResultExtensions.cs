using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rms.Server.Core.Azure.Service.Workers;
using System;

namespace Rms.Server.Core.Azure.Functions.WebApi.Converter
{
    /// <summary>
    /// SingleTaskWorkerResultの拡張メソッド
    /// </summary>
    public static class SingleTaskWorkerResultExtensions
    {
        /// <summary>
        /// HttpControllerの結果に変換する
        /// </summary>
        /// <param name="result">結果</param>
        /// <returns>IActionResult</returns>
        public static IActionResult ToActionResult(this SingleTaskWorker.Result result)
        {
            switch (result)
            {
                case SingleTaskWorker.Result.Started:
                    return new OkObjectResult($"Start task, {DateTime.Now.ToShortTimeString()}");
                case SingleTaskWorker.Result.AlreadyRunning:
                    return new OkObjectResult($"Already started task, {DateTime.Now.ToShortTimeString()}");
                case SingleTaskWorker.Result.Error:
                default:
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
