using Rms.Server.Core.Service.Models;
using RmsRms.Server.Core.Azure.Functions.WebApi.Dto;
using System;

namespace Rms.Server.Core.Azure.Functions.WebApi.Converter
{
    /// <summary>
    /// DeviceRemoteRequestDtoクラスの拡張クラス
    /// </summary>
    public static class DeviceRemoteRequestDtoExtensions
    {
        /// <summary>
        /// リクエストのDtoクラスパラメータをUtilityのパラメータに変換する
        /// </summary>
        /// <param name="dto">Dtoクラスパラメータ</param>
        /// <param name="deviceId">デバイスID</param>
        /// <returns>Utilityパラメータ</returns>
        public static RequestRemote ConvertDtoToUtility(this DeviceRemoteRequestDto dto, long deviceId)
        {
            // HACK: Serviceのモデルに変換しているが、リポジトリかUtilityのモデルにする必要があるかも。
            return new RequestRemote()
            {
                DeviceId = deviceId,
                SessionCode = dto.SessionCode
            };
        }
    }
}
