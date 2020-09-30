using Rms.Server.Core.Azure.Functions.WebApi.Dto;
using Rms.Server.Core.Utility.Models.Entites;

namespace Rms.Server.Core.Azure.Functions.WebApi.Converter
{
    /// <summary>
    /// InstallResultHistoryDto関連クラスの拡張クラス
    /// </summary>
    public static class InstallResultHistoryDtoExtensions
    {
        /// <summary>
        /// UtilityモデルをレスポンスDTOに変換する
        /// </summary>
        /// <param name="utilParam">Utilityパラメータ</param>
        /// <returns>Dtoクラスパラメータ</returns>
        public static InstallResultHistoryResponseDto ConvertUtilityToResponseDto(this DtInstallResult utilParam)
        {
            return utilParam == null ?
                null :
                new InstallResultHistoryResponseDto()
                {
                    Sid = utilParam.Sid,
                    DeviceSid = utilParam.DeviceSid,
                    DeliveryResultSid = utilParam.DeliveryResultSid,
                    InstallResultStatusSid = utilParam.InstallResultStatusSid,
                    SourceEquipmentUid = utilParam.SourceEquipmentUid,
                    ReleaseVersion = utilParam.ReleaseVersion,
                    BeforeVersion = utilParam.BeforeVersion,
                    AfterVervion = utilParam.AfterVervion,
                    IsSuccess = utilParam.IsSuccess,
                    ErrorCode = utilParam.ErrorCode,
                    ErrorDescription = utilParam.ErrorDescription,
                    IsAuto = utilParam.IsAuto,
                    Method = utilParam.Method,
                    Process = utilParam.Process,
                    UpdateStratDatetime = utilParam.UpdateStratDatetime,
                    UpdateEndDatetime = utilParam.UpdateEndDatetime,
                    ComputerName = utilParam.ComputerName,
                    IpAddress = utilParam.IpAddress,
                    ServerClientKind = utilParam.ServerClientKind,
                    HasRepairReport = utilParam.HasRepairReport,
                    EventDatetime = utilParam.EventDatetime,
                    CollectDatetime = utilParam.CollectDatetime,
                    MessageId = utilParam.MessageId,
                    CreateDatetime = utilParam.CreateDatetime,
                };
        }
    }
}
