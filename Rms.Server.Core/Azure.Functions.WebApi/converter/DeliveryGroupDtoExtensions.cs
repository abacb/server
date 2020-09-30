using Rms.Server.Core.Azure.Functions.WebApi.Dto;
using Rms.Server.Core.Azure.Functions.WebApi.Utility;
using Rms.Server.Core.Utility.Models.Entites;
using System.Linq;

namespace Rms.Server.Core.Azure.Functions.WebApi.Converter
{
    /// <summary>
    /// DeliveryGroupDto関連クラスの拡張クラス
    /// </summary>
    public static class DeliveryGroupDtoExtensions
    {
        /// <summary>
        /// 追加リクエストのDtoクラスパラメータをUtilityのパラメータに変換する
        /// </summary>
        /// <param name="dto">Dtoクラスパラメータ</param>
        /// <returns>Utilityパラメータ</returns>
        public static DtDeliveryGroup ConvertDtoToUtility(this DeliveryGroupAddRequestDto dto)
        {
            return dto == null ?
                null :
                new DtDeliveryGroup()
            {
                //// Sid
                DeliveryFileSid = dto.DeliveryFileSid.Value,
                ////DeliveryGroupStatusSid
                Name = dto.Name,
                StartDatetime = dto.StartDatetime.Value,
                DownloadDelayTime = dto.DownloadDelayTime,
                ////CreateDatetime
                ////UpdateDatetime
                ////RowVersion
                DtDeliveryResult = dto.DeliveryDestinations.Select(x => x.ConvertDtoToUtility()).ToArray()
            };
        }

        /// <summary>
        /// 更新リクエストのDtoクラスパラメータをUtilityのパラメータに変換する
        /// </summary>
        /// <param name="dto">Dtoクラスパラメータ</param>
        /// <param name="deliveryGroupId">配信グループID</param>
        /// <returns>Utilityパラメータ</returns>
        public static DtDeliveryGroup ConvertDtoToUtility(this DeliveryGroupUpdateRequestDto dto, long deliveryGroupId)
        {
            return dto == null ?
                null :
                new DtDeliveryGroup()
            {
                Sid = deliveryGroupId,
                //// DeliveryFileSid = dto.DeliveryFileSid
                // DeliveryGroupStatusSid
                Name = dto.Name,
                StartDatetime = dto.StartDatetime.Value,
                DownloadDelayTime = dto.DownloadDelayTime,
                //// CreateDatetime
                // UpdateDatetime
                RowVersion = WebApiHelper.ConvertLongToByteArray(dto.RowVersion.Value)
                //// DtDeliveryResult = dto.DtDeliveryResult
            };
        }

        /// <summary>
        /// UtilityモデルをレスポンスDTOに変換する
        /// </summary>
        /// <param name="utilParam">Utilityのパラメータ</param>
        /// <returns>レスポンス用Dto</returns>
        public static DeliveryGroupResponseDto ConvertUtilityToResponseDto(this DtDeliveryGroup utilParam)
        {
            return utilParam == null ?
                null :
                new DeliveryGroupResponseDto()
            {
                Sid = utilParam.Sid,
                DeliveryFileSid = utilParam.DeliveryFileSid,
                DeliveryGroupStatusSid = utilParam.DeliveryGroupStatusSid,
                Name = utilParam.Name,
                StartDatetime = utilParam.StartDatetime.Value,
                DownloadDelayTime = utilParam.DownloadDelayTime,
                CreateDatetime = utilParam.CreateDatetime,
                UpdateDatetime = utilParam.UpdateDatetime,
                RowVersion = WebApiHelper.ConvertByteArrayToLong(utilParam.RowVersion),
                DeliveryDestinations = utilParam.DtDeliveryResult.Select(x => x.ConvertUtilityToResponseDto()).ToArray()
            };
        }
    }
}
