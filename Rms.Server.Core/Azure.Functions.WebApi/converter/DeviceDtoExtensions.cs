using Rms.Server.Core.Azure.Functions.WebApi.Dto;
using Rms.Server.Core.Service.Models;
using Rms.Server.Core.Utility.Models.Entites;
using System.Collections.Generic;
using System.Linq;

namespace Rms.Server.Core.Azure.Functions.WebApi.Converter
{
    /// <summary>
    /// DeviceDto�֘A�N���X�̊g���N���X
    /// </summary>
    public static class DeviceDtoExtensions
    {
        /// <summary>
        /// �ǉ����N�G�X�g��Dto�N���X�p�����[�^��Utility�̃p�����[�^�ɕϊ�����
        /// </summary>
        /// <param name="dto">Dto�N���X�p�����[�^</param>
        /// <returns>Utility�p�����[�^</returns>
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
        /// �X�V���N�G�X�g��Dto�N���X�p�����[�^��Utility�̃p�����[�^�ɕϊ�����
        /// </summary>
        /// <param name="dto">Dto�N���X�p�����[�^</param>
        /// <returns>Utility�p�����[�^</returns>
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
        /// Utility���f�������X�|���XDTO�ɕϊ�����
        /// </summary>
        /// <param name="utilParam">Utility�̃p�����[�^</param>
        /// <returns>���X�|���X�pDto</returns>
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
        /// �@��o�^DTO��ݒu�ݒ�C���X�^���X�ɕϊ�����(�G�b�WID������)
        /// </summary>
        /// <param name="dto">�@��o�^DTO</param>
        /// <returns>�ݒu�ݒ�C���X�^���X(�G�b�WID������)</returns>
        public static InstallBaseConfig ConvertDtoToInstallBaseConfig(this DeviceAddRequestDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            // ���g�̋@����
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

            // �e�@�̋@����
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

            // �q�@�̋@����
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

            // �ݒu�ݒ�t�@�C�����ɂ܂Ƃ߂ĕԋp
            return new InstallBaseConfig()
            {
                OwnConfig = ownConfig,
                ParentConfig = parentConfig,
                ChildrenConfig = childrenConfg
            };
        }

        /// <summary>
        /// �@��X�VDTO��ݒu�ݒ�C���X�^���X�ɕϊ�����(�G�b�WID�E�@��UID������)
        /// </summary>
        /// <param name="dto">�@��X�VDTO</param>
        /// <returns>�ݒu�ݒ�C���X�^���X(�G�b�WID�E�@��UID������)</returns>
        public static InstallBaseConfig ConvertDtoToInstallBaseConfig(this DeviceUpdateRequestDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            // ���g�̋@����
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

            // �e�@�̋@����
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

            // �q�@�̋@����
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

            // �ݒu�ݒ�t�@�C�����ɂ܂Ƃ߂ĕԋp
            return new InstallBaseConfig()
            {
                OwnConfig = ownConfig,
                ParentConfig = parentConfig,
                ChildrenConfig = childrenConfg
            };
        }

        /// <summary>
        /// �C���X�g�[���^�C�v��Utility���f������DTO���f���ɕϊ�����
        /// </summary>
        /// <param name="sid">�C���X�g�[���^�C�vSID</param>
        /// <param name="code">�C���X�g�[���^�C�v�R�[�h</param>
        /// <returns>DTO���f���̃C���X�g�[���^�C�v</returns>
        private static InstallTypeMasterDto ConvertUtilityInstallTypeMasterToDto(long sid, string code)
        {
            return new InstallTypeMasterDto()
            {
                InstallTypeSid = sid,
                InstallTypeCode = code
            };
        }

        /// <summary>
        /// �@��^����Utility���f������DTO���f���ɕϊ�����
        /// </summary>
        /// <param name="sid">�@��^��SID</param>
        /// <param name="code">�@��^���R�[�h</param>
        /// <returns>DTO���f���̋@��^��</returns>
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
