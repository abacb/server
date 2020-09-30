using Rms.Server.Core.Azure.Functions.WebApi.Dto;
using Rms.Server.Core.Utility.Models.Entites;
using System.Linq;

namespace Rms.Server.Core.Azure.Functions.WebApi.Converter
{
    /// <summary>
    /// DeliveryGroupDto関連クラスの拡張クラス
    /// </summary>
    public static class DeliveryResultDtoExtensions
    {
        /// <summary>
        /// 配信ファイル追加リクエストDTOをUtilityモデルに変換する
        /// </summary>
        /// <param name="dto">Dtoクラスパラメータ</param>
        /// <returns>Utilityパラメータ</returns>
        public static DtDeliveryResult ConvertDtoToUtility(this DeliveryResultAddRequestDto dto)
        {
            return dto == null ?
                null :
                new DtDeliveryResult()
            {
                ////Sid
                DeviceSid = dto.DeviceSid.Value,
                GwDeviceSid = dto.GatewayDeviceSid.Value,
                ////DeliveryGroupSid
                ////CreatedDatetime
            };
        }

        /// <summary>
        /// UtilityモデルをレスポンスDTOに変換する
        /// </summary>
        /// <param name="utilParam">Utilityパラメータ</param>
        /// <returns>Dtoクラスパラメータ</returns>
        public static DeliveryResultResponseDto ConvertUtilityToResponseDto(this DtDeliveryResult utilParam)
        {
            return utilParam == null ?
                null :
                new DeliveryResultResponseDto()
                {
                    Sid = utilParam.Sid,
                    DeviceSid = utilParam.DeviceSid,
                    GatewayDeviceSid = utilParam.GwDeviceSid,
                    ////DeliveryGroupSid
                    CreateDatetime = utilParam.CreateDatetime,
                    InstallResultHistories = utilParam.DtInstallResult.Select(x => x.ConvertUtilityToResponseDto()).ToArray()
            };
        }
    }
}
