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
using System.Linq;

namespace Rms.Server.Utility.Azure.Functions.PanelDefectPremonitor
{
    /// <summary>
    /// �p�l�����ח\���Ď��A�v��
    /// </summary>
    public class PanelDefectPremonitorController
    {
        /// <summary>
        /// �C�x���g�n�u���i���b�Z�[�W�X�L�[�}ID�j
        /// </summary>
        private const string EventHubName = "ms-021";

        /// <summary>
        /// �ݒ�
        /// </summary>
        private readonly UtilityAppSettings _settings;

        /// <summary>
        /// �p�l�����ח\���Ď��T�[�r�X
        /// </summary>
        private readonly IPanelDefectPremonitorService _service;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="settings">�ݒ�</param>
        /// <param name="service">�p�l�����ח\���Ď��T�[�r�X�N���X</param>
        public PanelDefectPremonitorController(UtilityAppSettings settings, IPanelDefectPremonitorService service)
        {
            Assert.IfNull(settings);
            Assert.IfNull(service);

            _settings = settings;
            _service = service;
        }

        /// <summary>
        /// �p�l�����ח\�����ʃ��O����M����
        /// </summary>
        /// <remarks>sd 07-08.�p�l�����ח\���Ď�</remarks>
        /// <param name="message">���b�Z�[�W</param>
        /// <param name="log">���K�[</param>
        [FunctionName("PanelDefectPremonitor")]
        public void ReceivePanelDefectPredictiveResutLog([EventHubTrigger(EventHubName, Connection = "ConnectionString")] string message, ILogger log)
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
                        var panelDefectPredictiveResutLog = JsonConvert.DeserializeObject<PanelDefectPredictiveResutLog>(deviceTelemetry.Body.ToStringJson());

                        // �o���f�[�V����
                        Validator.ValidateObject(panelDefectPredictiveResutLog, new ValidationContext(panelDefectPredictiveResutLog, null, null));

                        // Sq1.1.1: �p�l�����ח\���Ď��A���[����`���擾
                        if (_service.ReadAlarmDefinition(panelDefectPredictiveResutLog, messageId, out IEnumerable<DtAlarmDefPanelDefectPremonitor> models))
                        {
                            // Sq1.1.2: �A���[�������̗v�ۂ𔻒f����
                            bool needsAlarm = models != null && models.Count() > 0;
                            if (!needsAlarm)
                            {
                                // �A���[�������s�v
                                log.Info(nameof(Resources.UT_PDP_PDP_004), new object[] { messageId });
                                continue;
                            }
                            else
                            {
                                // Sq1.1.3: �A���[���L���[�𐶐�����
                                // Sq1.1.4: �L���[��o�^����
                                if (_service.CreateAndEnqueueAlarmInfo(panelDefectPredictiveResutLog, messageId, models))
                                {
                                    // �A���[���L���[�o�^����
                                    log.Info(nameof(Resources.UT_PDP_PDP_007), new object[] { messageId });
                                    continue;
                                }
                            }
                        }
                    }
                    catch (ValidationException e)
                    {
                        // ��M���b�Z�[�W�t�H�[�}�b�g�ُ�i��{�݌v�� 5.1.2.4 �G���[�����j
                        log.Error(e, nameof(Resources.UT_PDP_PDP_002), new object[] { messageId, e.Message });
                    }
                    catch (Exception e)
                    {
                        log.Error(e, nameof(Resources.UT_PDP_PDP_001), new object[] { messageId });
                    }

                    // ���s�����ꍇ��Failure�X�g���[�W�ɏ�������
                    _service.UpdateToFailureStorage(EventHubName, messageId, JsonConvert.SerializeObject(new AzureEventGridEventSchema[] { eventSchema }));
                }
            }
            catch (RmsInvalidAppSettingException e)
            {
                log.Error(e, nameof(Resources.UT_PDP_PDP_009), new object[] { e.Message });

                // ���s�����ꍇ��Failure�X�g���[�W�ɏ�������
                _service.UpdateToFailureStorage(EventHubName, null, message);
            }
            catch (Exception e)
            {
                log.Error(e, nameof(Resources.UT_PDP_PDP_001), new object[] { null });

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
