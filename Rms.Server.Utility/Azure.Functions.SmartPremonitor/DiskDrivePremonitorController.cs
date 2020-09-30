using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Utility.Service.Services;
using Rms.Server.Utility.Utility;
using Rms.Server.Utility.Utility.Extensions;
using Rms.Server.Utility.Utility.Models;
using Rms.Server.Utility.Utility.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Utility.Azure.Functions.DiskDrivePremonitor
{
    /// <summary>
    /// �f�B�X�N�h���C�u�\���Ď��A�v��
    /// </summary>
    public class DiskDrivePremonitorController
    {
        /// <summary>
        /// �C�x���g�n�u���i���b�Z�[�W�X�L�[�}ID�j
        /// </summary>
        private const string EventHubName = "ms-027";

        /// <summary>
        /// �ݒ�
        /// </summary>
        private readonly UtilityAppSettings _settings;

        /// <summary>
        /// �f�B�X�N�h���C�u�\���Ď��T�[�r�X�N���X
        /// </summary>
        private readonly IDiskDrivePremonitorService _service;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="settings">�ݒ�</param>
        /// <param name="service">�f�B�X�N�h���C�u�\���Ď��T�[�r�X�N���X</param>
        public DiskDrivePremonitorController(UtilityAppSettings settings, IDiskDrivePremonitorService service)
        {
            Assert.IfNull(settings);
            Assert.IfNull(service);

            _settings = settings;
            _service = service;
        }

        /// <summary>
        /// �f�B�X�N�h���C�u����M����
        /// </summary>
        /// <remarks>�f�B�X�N�h���C�u�\���Ď�</remarks>
        /// <param name="message">���b�Z�[�W</param>
        /// <param name="log">���K�[</param>
        [FunctionName("DiskDrivePremonitor")]
        public void ReceiveDiskDrive([EventHubTrigger(EventHubName, Connection = "ConnectionString")] string message, ILogger log)
        {
            log.EnterJson("{0}", message);
            try
            {
                // �A�v���P�[�V�����ݒ��System���ESubSystem�����ݒ肳��Ă��Ȃ��ꍇ�A���s���ɃG���[�Ƃ���
                if (string.IsNullOrEmpty(_settings.SystemName))
                {
                    throw new RmsInvalidAppSettingException($"{nameof(_settings.SystemName)} is required.");
                }

                if (string.IsNullOrEmpty(_settings.SubSystemName))
                {
                    throw new RmsInvalidAppSettingException($"{nameof(_settings.SubSystemName)} is required.");
                }

                var eventSchemaList = JsonConvert.DeserializeObject<List<AzureEventGridEventSchema>>(message);
                foreach (var eventSchema in eventSchemaList)
                {
                    string messageId = null;
                    try
                    {
                        var deviceTelemetry = JsonConvert.DeserializeObject<IotHubDeviceTelemetry>(eventSchema.Data.ToStringJson());
                        messageId = deviceTelemetry?.SystemProperties?.MessageId;
                        var diskDrive = JsonConvert.DeserializeObject<DiskDrive>(deviceTelemetry.Body.ToStringJson());

                        // �o���f�[�V����
                        Validator.ValidateObject(diskDrive, new ValidationContext(diskDrive, null, null));

                        // ��M���b�Z�[�W����ID=C5��SMART���������擾
                        if (_service.ReadSmartAttirubteInfoC5(diskDrive, messageId, out DiskDrive.SmartAttributeInfoSchema smartAttributeInfoC5))
                        {
                            // �f�B�X�N�h���C�u�\���Ď��A���[����`���擾
                            if (_service.ReadAlarmDefinition(messageId, out DtAlarmSmartPremonitor alarmDef))
                            {
                                // �o�^�ς݂�SMART��͌��ʂ��擾
                                if (_service.ReadSmartAnalysisResult(diskDrive, messageId, out DtSmartAnalysisResult analysisResult))
                                {
                                    // �A���[������Ɖ�͌��ʂ̍X�V���s��
                                    if (_service.JudgeAlarmCreationAndUpdateSmartAnalysisResult(diskDrive, messageId, analysisResult, alarmDef, smartAttributeInfoC5, out bool needsAlarmCreation))
                                    {
                                        if (!needsAlarmCreation)
                                        {
                                            log.Info(nameof(Resources.UT_DDP_DDP_006), new object[] { messageId });
                                            continue;
                                        }
                                        else
                                        {
                                            // �A���[���L���[�𐶐�����
                                            // �L���[��o�^����
                                            if (_service.CreateAndEnqueueAlarmInfo(diskDrive, smartAttributeInfoC5, messageId, analysisResult, alarmDef))
                                            {
                                                log.Info(nameof(Resources.UT_DDP_DDP_009), new object[] { messageId });
                                                continue;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Newtonsoft.Json.JsonReaderException e)
                    {
                        // ��M���b�Z�[�W�t�H�[�}�b�g�ُ�i��{�݌v�� 5.1.2.4 �G���[�����j
                        log.Error(e, nameof(Resources.UT_DDP_DDP_002), new object[] { messageId, e.Message });
                    }
                    catch (ValidationException e)
                    {
                        // ��M���b�Z�[�W�t�H�[�}�b�g�ُ�i��{�݌v�� 5.1.2.4 �G���[�����j
                        log.Error(e, nameof(Resources.UT_DDP_DDP_002), new object[] { messageId, e.Message });
                    }
                    catch (Exception e)
                    {
                        log.Error(e, nameof(Resources.UT_DDP_DDP_001), new object[] { messageId });
                    }

                    // ���s�����ꍇ��Failure�X�g���[�W�ɏ�������
                    _service.UpdateToFailureStorage(EventHubName, messageId, JsonConvert.SerializeObject(new AzureEventGridEventSchema[] { eventSchema }));
                }
            }
            catch (RmsInvalidAppSettingException e)
            {
                log.Error(e, nameof(Resources.UT_DDP_DDP_011), new object[] { e.Message });

                // ���s�����ꍇ��Failure�X�g���[�W�ɏ�������
                _service.UpdateToFailureStorage(EventHubName, null, message);
            }
            catch (Exception e)
            {
                log.Error(e, nameof(Resources.UT_DDP_DDP_001), new object[] { null });

                // ���s�����ꍇ��Failure�X�g���[�W�ɏ�������
                _service.UpdateToFailureStorage(EventHubName, null, message);
            }
            finally
            {
                log.LeaveJson("{0}", message);
            }
        }
    }
}
