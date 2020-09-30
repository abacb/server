using Rms.Server.Core.Azure.Functions.WebApi.Dto;
using Rms.Server.Core.Azure.Functions.WebApi.Utility;
using Rms.Server.Core.Utility.Models.Entites;
using System.Collections.Generic;
using System.Linq;
using static Rms.Server.Core.Azure.Functions.WebApi.Dto.DeliveryFileStatusUpdateRequestDto;

namespace Rms.Server.Core.Azure.Functions.WebApi.Converter
{
    /// <summary>
    /// DeliveryFileDto関連クラスの拡張クラス
    /// </summary>
    public static class DeliveryFileDtoExtensions
    {
        // HACK: 関連クラスをまとめているため雑多になっている

        /// <summary>
        /// 配信ファイル追加リクエストDTOをUtilityモデルに変換する
        /// </summary>
        /// <param name="dto">Dtoクラスパラメータ</param>
        /// <returns>Utilityパラメータ</returns>
        public static DtDeliveryFile ConvertDtoToUtility(this DeliveryFileAddRequestDto dto)
        {
            return new DtDeliveryFile()
            {
                // Sid = dto.Sid,
                DeliveryFileTypeSid = dto.DeliveryFileType.DeliveryFileTypeSid.Value,
                FilePath = dto.FilePath,
                DtDeliveryModel = dto.EquipmentModels == null ? new DtDeliveryModel[0] : dto.EquipmentModels.ToModelDtos().ToArray(),
                ////InstallTypeSid = dto.InstallType?.InstallTypeId,
                Version = dto.Version,
                InstallableVersion = dto.InstallableVersion,
                Description = dto.Description,
                InformationId = dto.InformationId,
                //// IsCanceled
                //// CreateDatetime
                //// UpdateDatetime
                //// RowVersion = dto.RowVersion これはCreateなので不要
                //// MtDeliveryFileType
                //// DtDeliveryGroup
            };
        }

        /// <summary>
        /// 配信ファイル更新リクエストDTOをUtilityモデルに変換する
        /// </summary>
        /// <param name="dto">Dtoクラスパラメータ</param>
        /// <param name="deliveryFileId">配信ファイルデータID</param>
        /// <returns>Utilityパラメータ</returns>
        public static DtDeliveryFile ConvertDtoToUtility(this DeliveryFileUpdateRequestDto dto, long deliveryFileId)
        {
            return new DtDeliveryFile()
            {
                Sid = deliveryFileId,
                DeliveryFileTypeSid = dto.DeliveryFileType.DeliveryFileTypeSid.Value,
                //// FilePath = dto.FilePath,
                ////InstallTypeSid = dto.InstallType.InstallTypeId,
                DtDeliveryModel = dto.EquipmentModels == null ? null : dto.EquipmentModels.ToModelDtos().ToArray(),
                Version = dto.Version,
                InstallableVersion = dto.InstallableVersion,
                Description = dto.Description,
                InformationId = dto.InformationId,
                //// IsCanceled
                //// CreateDatetime
                //// UpdateDatetime
                RowVersion = WebApiHelper.ConvertLongToByteArray(dto.RowVersion.Value)
                //// MtDeliveryFileType
                //// DtDeliveryGroup
            };
        }

        /// <summary>
        /// 中止フラグ更新リクエストのDtoクラスパラメータをUtilityのパラメータに変換する
        /// </summary>
        /// <param name="dto">Dtoクラスパラメータ</param>
        /// <param name="deliveryFileId">配信ファイルID</param>
        /// <returns>Utilityパラメータ</returns>
        public static DtDeliveryFile ConvertDtoToUtility(this DeliveryFileStatusUpdateRequestDto dto, long deliveryFileId)
        {
            return new DtDeliveryFile()
            {
                Sid = deliveryFileId,
                IsCanceled = dto.DeliveryStatus.Equals(RequestDeliveryStatus.Stop) ? true : false,
                RowVersion = WebApiHelper.ConvertLongToByteArray(dto.RowVersion.Value)
            };
        }

        /// <summary>
        /// UtilityのパラメータをレスポンスDtoに変換する
        /// </summary>
        /// <param name="utilParam">Utilityのパラメータ</param>
        /// <returns>レスポンス用Dto</returns>
        public static DeliveryFileResponseDto ConvertUtilityToResponseDto(this DtDeliveryFile utilParam)
        {
            return new DeliveryFileResponseDto()
            {
                Sid = utilParam.Sid,
                DeliveryFileTypeSid = utilParam.DeliveryFileTypeSid,
                FilePath = utilParam.FilePath,
                EquipmentModels = utilParam.DtDeliveryModel.ToDtDeliveryModels(),
                InstallTypeSid = utilParam.InstallTypeSid,
                Version = utilParam.Version,
                InstallableVersion = utilParam.InstallableVersion,
                Description = utilParam.Description,
                InformationId = utilParam.InformationId,
                IsCanceled = utilParam.IsCanceled,
                CreateDatetime = utilParam.CreateDatetime,
                UpdateDatetime = utilParam.UpdateDatetime,
                RowVersion = WebApiHelper.ConvertByteArrayToLong(utilParam.RowVersion)
            };
        }

        /// <summary>
        /// 型式DTOをUtilityモデルに変換する
        /// </summary>
        /// <param name="dtos">DTO</param>
        /// <returns>Utilityパラメータ</returns>
        public static IEnumerable<DtDeliveryModel> ToModelDtos(this IEnumerable<ModelMasterDto> dtos)
        {
            if (dtos == null)
            {
                return null;
            }

            return dtos.Select(x => new DtDeliveryModel() { EquipmentModelSid = x.ModelSid.Value, });
        }

        /// <summary>
        /// Utilityのパラメータを型式DTOに変換する
        /// </summary>
        /// <param name="models">Utilityのパラメタ</param>
        /// <returns>配信ファイル型式DTO</returns>
        public static IEnumerable<DeliveryModelDto> ToDtDeliveryModels(this IEnumerable<DtDeliveryModel> models)
        {
            if (models == null)
            {
                return null;
            }

            return models.Select(x => new DeliveryModelDto()
            {
                Sid = x.Sid,
                EquipmentModelSid = x.EquipmentModelSid,
                CreateDatetime = x.CreateDatetime
            });
        }
    }
}
