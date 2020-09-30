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

namespace Rms.Server.Utility.Azure.Functions.PanelDischargeBreakdownPremonitor
{
    /// <summary>
    /// �p�l�����d�j��\���Ď��A�v��
    /// </summary>
    public class PanelDischargeBreakdownPremonitorController
    {
        /// <summary>
        /// �C�x���g�n�u���i���b�Z�[�W�X�L�[�}ID�j
        /// </summary>
        private const string EventHubName = "ms-020";

        /// <summary>
        /// �ݒ�
        /// </summary>
        private readonly UtilityAppSettings _settings;

        /// <summary>
        /// �p�l�����d�j��\���Ď��T�[�r�X�N���X
        /// </summary>
        private readonly IPanelDischargeBreakdownPremonitorService _service;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="settings">�ݒ�</param>
        /// <param name="service">�p�l�����d�j��\���Ď��T�[�r�X�N���X</param>
        public PanelDischargeBreakdownPremonitorController(UtilityAppSettings settings, IPanelDischargeBreakdownPremonitorService service)
        {
            Assert.IfNull(settings);
            Assert.IfNull(service);

            _settings = settings;
            _service = service;
        }

        /// <summary>
        /// �p�l�����d�j��\�����ʃ��O����M����
        /// </summary>
        /// <remarks>sd 07-07.�p�l�����d�j��Ď�</remarks>
        /// <param name="message">���b�Z�[�W</param>
        /// <param name="log">���K�[</param>
        [FunctionName("PanelDischargeBreakdownPremonitor")]
        public void ReceivePanelDischargeBreakdownPredictiveResutLog([EventHubTrigger(EventHubName, Connection = "ConnectionString")] string message, ILogger log)
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
                        var panelDischargeBreakdownPredictiveResutLog = JsonConvert.DeserializeObject<PanelDischargeBreakdownPredictiveResutLog>(deviceTelemetry.Body.ToStringJson());

                        // �o���f�[�V����
                        Validator.ValidateObject(panelDischargeBreakdownPredictiveResutLog, new ValidationContext(panelDischargeBreakdownPredictiveResutLog, null, null));

                        // Sq1.1.1: �p�l�����d�j��\���Ď��A���[����`���擾
                        if (_service.ReadAlarmDefinition(panelDischargeBreakdownPredictiveResutLog, messageId, out IEnumerable<DtAlarmDefPanelDischargeBreakdownPremonitor> models))
                        {
                            // Sq1.1.2: �A���[�������̗v�ۂ𔻒f����
                            bool needsAlarm = models != null && models.Count() > 0;
                            if (!needsAlarm)
                            {
                                log.Info(nameof(Resources.UT_PBP_PBP_004), new object[] { messageId });
                                continue;
                            }
                            else
                            {
                                // Sq1.1.3: �A���[���L���[�𐶐�����
                                // Sq1.1.4: �L���[��o�^����
                                if (_service.CreateAndEnqueueAlarmInfo(panelDischargeBreakdownPredictiveResutLog, messageId, models))
                                {
                                    log.Info(nameof(Resources.UT_PBP_PBP_007), new object[] { messageId });
                                    continue;
                                }
                            }
                        }
                    }
                    catch (ValidationException e)
                    {
                        // ��M���b�Z�[�W�t�H�[�}�b�g�ُ�i��{�݌v�� 5.1.2.4 �G���[�����j
                        log.Error(e, nameof(Resources.UT_PBP_PBP_002), new object[] { messageId, e.Message });
                    }
                    catch (Exception e)
                    {
                        log.Error(e, nameof(Resources.UT_PBP_PBP_001), new object[] { messageId });
                    }

                    // ���s�����ꍇ��Failure�X�g���[�W�ɏ�������
                    _service.UpdateToFailureStorage(EventHubName, messageId, JsonConvert.SerializeObject(new AzureEventGridEventSchema[] { eventSchema }));
                }
            }
            catch (RmsInvalidAppSettingException e)
            {
                log.Error(e, nameof(Resources.UT_PBP_PBP_009), new object[] { e.Message });

                // ���s�����ꍇ��Failure�X�g���[�W�ɏ�������
                _service.UpdateToFailureStorage(EventHubName, null, message);
            }
            catch (Exception e)
            {
                log.Error(e, nameof(Resources.UT_PBP_PBP_001), new object[] { null });

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
