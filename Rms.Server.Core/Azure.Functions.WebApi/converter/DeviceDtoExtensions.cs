using Rms.Server.Core.Azure.Functions.WebApi.Dto;
using Rms.Server.Core.Service.Models;
using Rms.Server.Core.Utility.Models.Entites;
using System.Collections.Generic;
using System.Linq;

namespace Rms.Server.Core.Azure.Functions.WebApi.Converter
{
    /// <summary>
    /// DeviceDto関連クラスの拡張クラス
    /// </summary>
    public static class DeviceDtoExtensions
    {
        /// <summary>
        /// 追加リクエストのDtoクラスパラメータをUtilityのパラメータに変換する
        /// </summary>
        /// <param name="dto">Dtoクラスパラメータ</param>
        /// <returns>Utilityパラメータ</returns>
        public static DtDevice ConvertDtoToUtility(this DeviceAddRequestDto dto)
        {
            return dto == null ?
                null :
                new DtDevice()
            {
                //// Sid
                EquipmentModelSid = dto.Device.Model.ModelSid,
                InstallTypeSid = dto.Device.InstallType.InstallTypeSid.Value,
                //// ConnectStatusSid
                //// EdgeId
                EquipmentUid = dto.Device.EquipmentUid,
                //// RemoteConnectUid
                //// RmsSoftVersion
                //// ConnectStartDatetime
                //// ConnectUpdateDatetime
                //// CreateDatetime
                //// UpdateDatetime
            };
        }

        /// <summary>
        /// 更新リクエストのDtoクラスパラメータをUtilityのパラメータに変換する
        /// </summary>
        /// <param name="dto">Dtoクラスパラメータ</param>
        /// <returns>Utilityパラメータ</returns>
        public static DtDevice ConvertDtoToUtility(this DeviceUpdateRequestDto dto)
        {
            return dto == null ?
                null :
                new DtDevice()
                {
                    //// Sid
                    EquipmentModelSid = dto.Device.Model.ModelSid,
                    InstallTypeSid = dto.Device.InstallType.InstallTypeSid.Value,
                    //// ConnectStatusSid
                    //// EdgeId
                    //// EquipmentUid
                    //// RemoteConnectUid
                    //// RmsSoftVersion
                    //// ConnectStartDatetime
                    //// ConnectUpdateDatetime
                    //// CreateDatetime
                    //// UpdateDatetime
                };
        }

        /// <summary>
        /// UtilityモデルをレスポンスDTOに変換する
        /// </summary>
        /// <param name="utilParam">Utilityのパラメータ</param>
        /// <returns>レスポンス用Dto</returns>
        public static DeviceResponseDto ConvertUtilityToResponseDto(this DtDevice utilParam)
        {
            return utilParam == null ?
                null :
                new DeviceResponseDto()
            {
                Sid = utilParam.Sid,
                EdgeId = utilParam.EdgeId,
                InstallType = ConvertUtilityInstallTypeMasterToDto(utilParam.InstallTypeSid, utilParam.MtInstallType?.Code),
                EquipmentUid = utilParam.EquipmentUid,
                Models = ConvertUtilityModelMasterToDto(utilParam.EquipmentModelSid, utilParam.MtEquipmentModel?.Code),
                CreateDatetime = utilParam.CreateDatetime,
                UpdateDatetime = utilParam.UpdateDatetime,
                RemoteConnectUid = utilParam.RemoteConnectUid
            };
        }

        /// <summary>
        /// 機器登録DTOを設置設定インスタンスに変換する(エッジIDを除く)
        /// </summary>
        /// <param name="dto">機器登録DTO</param>
        /// <returns>設置設定インスタンス(エッジIDを除く)</returns>
        public static InstallBaseConfig ConvertDtoToInstallBaseConfig(this DeviceAddRequestDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            // 自身の機器情報
            InstallBaseDeviceConfig ownConfig = new InstallBaseDeviceConfig()
            {
                ////EdgeId
                InstallType = dto.Device.InstallType.InstallTypeCode,
                EquipmentUid = dto.Device.EquipmentUid,
                EquipmentName = dto.Device.EquipmentName,
                EquipmentSerialNumber = dto.Device.EquipmentSerialNumber,
                InstallFeatures = dto.Device.InstallFeatures,
                IpAddress = dto.Device.IpAddress,
                HostName = dto.Device.HostName,
                Model = dto.Device.Model.ModelCode,
                NetworkRoute = dto.Device.NetworkRoute
            };

            // 親機の機器情報
            InstallBaseDeviceConfig parentConfig = dto.Parent == null ?
                null :
                new InstallBaseDeviceConfig()
            {
                EdgeId = dto.Parent.EdgeId?.ToString(),
                InstallType = dto.Parent.InstallType?.InstallTypeCode,
                EquipmentUid = dto.Parent.EquipmentUid,
                EquipmentName = dto.Parent.EquipmentName,
                EquipmentSerialNumber = dto.Parent.EquipmentSerialNumber,
                InstallFeatures = dto.Parent.InstallFeatures,
                IpAddress = dto.Parent.IpAddress,
                HostName = dto.Parent.HostName,
                Model = dto.Parent.Model?.ModelCode,
                NetworkRoute = dto.Parent.NetworkRoute
            };

            // 子機の機器情報
            List<InstallBaseDeviceConfig> childrenConfg = dto.Children == null ?
                null :
                dto.Children.Select(
                    x => new InstallBaseDeviceConfig()
                    {
                        EdgeId = x.EdgeId?.ToString(),
                        InstallType = x.InstallType?.InstallTypeCode,
                        EquipmentUid = x.EquipmentUid,
                        EquipmentName = x.EquipmentName,
                        EquipmentSerialNumber = x.EquipmentSerialNumber,
                        InstallFeatures = x.InstallFeatures,
                        IpAddress = x.IpAddress,
                        HostName = x.HostName,
                        Model = x.Model?.ModelCode,
                        NetworkRoute = x.NetworkRoute
                    })
                .ToList();

            // 設置設定ファイル情報にまとめて返却
            return new InstallBaseConfig()
            {
                OwnConfig = ownConfig,
                ParentConfig = parentConfig,
                ChildrenConfig = childrenConfg
            };
        }

        /// <summary>
        /// 機器更新DTOを設置設定インスタンスに変換する(エッジID・機器UIDを除く)
        /// </summary>
        /// <param name="dto">機器更新DTO</param>
        /// <returns>設置設定インスタンス(エッジID・機器UIDを除く)</returns>
        public static InstallBaseConfig ConvertDtoToInstallBaseConfig(this DeviceUpdateRequestDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            // 自身の機器情報
            InstallBaseDeviceConfig ownConfig = new InstallBaseDeviceConfig()
            {
                ////EdgeId
                InstallType = dto.Device.InstallType.InstallTypeCode,
                ////DeviceUid
                EquipmentName = dto.Device.EquipmentName,
                EquipmentSerialNumber = dto.Device.EquipmentSerialNumber,
                InstallFeatures = dto.Device.InstallFeatures,
                IpAddress = dto.Device.IpAddress,
                HostName = dto.Device.HostName,
                Model = dto.Device.Model.ModelCode,
                NetworkRoute = dto.Device.NetworkRoute
            };

            // 親機の機器情報
            InstallBaseDeviceConfig parentConfig = dto.Parent == null ?
                null :
                new InstallBaseDeviceConfig()
                {
                    EdgeId = dto.Parent.EdgeId?.ToString(),
                    InstallType = dto.Parent.InstallType?.InstallTypeCode,
                    EquipmentUid = dto.Parent.EquipmentUid,
                    EquipmentName = dto.Parent.EquipmentName,
                    EquipmentSerialNumber = dto.Parent.EquipmentSerialNumber,
                    InstallFeatures = dto.Parent.InstallFeatures,
                    IpAddress = dto.Parent.IpAddress,
                    HostName = dto.Parent.HostName,
                    Model = dto.Parent.Model?.ModelCode,
                    NetworkRoute = dto.Parent.NetworkRoute
                };

            // 子機の機器情報
            List<InstallBaseDeviceConfig> childrenConfg = dto.Children == null ?
                null :
                dto.Children.Select(
                    x => new InstallBaseDeviceConfig()
                    {
                        EdgeId = x.EdgeId?.ToString(),
                        InstallType = x.InstallType?.InstallTypeCode,
                        EquipmentUid = x.EquipmentUid,
                        EquipmentName = x.EquipmentName,
                        EquipmentSerialNumber = x.EquipmentSerialNumber,
                        InstallFeatures = x.InstallFeatures,
                        IpAddress = x.IpAddress,
                        HostName = x.HostName,
                        Model = x.Model?.ModelCode,
                        NetworkRoute = x.NetworkRoute
                    })
                .ToList();

            // 設置設定ファイル情報にまとめて返却
            return new InstallBaseConfig()
            {
                OwnConfig = ownConfig,
                ParentConfig = parentConfig,
                ChildrenConfig = childrenConfg
            };
        }

        /// <summary>
        /// インストールタイプをUtilityモデルからDTOモデルに変換する
        /// </summary>
        /// <param name="sid">インストールタイプSID</param>
        /// <param name="code">インストールタイプコード</param>
        /// <returns>DTOモデルのインストールタイプ</returns>
        private static InstallTypeMasterDto ConvertUtilityInstallTypeMasterToDto(long sid, string code)
        {
            return new InstallTypeMasterDto()
            {
                InstallTypeSid = sid,
                InstallTypeCode = code
            };
        }

        /// <summary>
        /// 機器型式をUtilityモデルからDTOモデルに変換する
        /// </summary>
        /// <param name="sid">機器型式SID</param>
        /// <param name="code">機器型式コード</param>
        /// <returns>DTOモデルの機器型式</returns>
        private static ModelMasterDto ConvertUtilityModelMasterToDto(long? sid, string code)
        {
            return sid == null ?
                null :
                new ModelMasterDto()
            {
                ModelSid = sid,
                ModelCode = code
            };
        }
    }
}
