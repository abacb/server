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

namespace Rms.Server.Utility.Azure.Functions.TubeCurrentDeteriorationPremonitor
{
    /// <summary>
    /// �Ǔd���򉻗\���Ď��A�v��
    /// </summary>
    public class TubeCurrentDeteriorationPremonitorController
    {
        /// <summary>
        /// �C�x���g�n�u���i���b�Z�[�W�X�L�[�}ID�j
        /// </summary>
        private const string EventHubName = "ms-018";

        /// <summary>
        /// �ݒ�
        /// </summary>
        private readonly UtilityAppSettings _settings;

        /// <summary>
        /// �Ǔd���򉻗\���Ď��T�[�r�X
        /// </summary>
        private readonly ITubeCurrentDeteriorationPremonitorService _service;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="settings">�ݒ�</param>
        /// <param name="service">�Ǔd���򉻗\���Ď��T�[�r�X�N���X</param>
        public TubeCurrentDeteriorationPremonitorController(UtilityAppSettings settings, ITubeCurrentDeteriorationPremonitorService service)
        {
            Assert.IfNull(settings);
            Assert.IfNull(service);

            _settings = settings;
            _service = service;
        }

        /// <summary>
        /// �Ǔd���o���򉻗\�����ʃ��O����M����
        /// </summary>
        /// <remarks>sd 07-05.�Ǔd���򉻗\���Ď�</remarks>
        /// <param name="message">���b�Z�[�W</param>
        /// <param name="log">���K�[</param>
        [FunctionName("TubeCurrentDeteriorationPremonitor")]
        public void ReceiveTubeCurrentDeteriorationPredictiveResutLog([EventHubTrigger(EventHubName, Connection = "ConnectionString")] string message, ILogger log)
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
                        var tubeCurrentDeteriorationPredictiveResutLog = JsonConvert.DeserializeObject<TubeCurrentDeteriorationPredictiveResutLog>(deviceTelemetry.Body.ToStringJson());

                        // �o���f�[�V����
                        Validator.ValidateObject(tubeCurrentDeteriorationPredictiveResutLog, new ValidationContext(tubeCurrentDeteriorationPredictiveResutLog, null, null));

                        // Sq1.1.1: �Ǔd���o���򉻗\���Ď��A���[����`���擾
                        if (_service.ReadAlarmDefinition(tubeCurrentDeteriorationPredictiveResutLog, messageId, out IEnumerable<DtAlarmDefTubeCurrentDeteriorationPremonitor> models))
                        {
                            // Sq1.1.2: �A���[�������̗v�ۂ𔻒f����
                            bool needsAlarm = models != null && models.Count() > 0;
                            if (!needsAlarm)
                            {
                                // �A���[�������s�v
                                log.Info(nameof(Resources.UT_TCP_TCP_004), new object[] { messageId });
                                continue;
                            }
                            else
                            {
                                // Sq1.1.3: �A���[���L���[�𐶐�����
                                // Sq1.1.4: �L���[��o�^����
                                if (_service.CreateAndEnqueueAlarmInfo(tubeCurrentDeteriorationPredictiveResutLog, messageId, models))
                                {
                                    // �A���[���L���[�o�^����
                                    log.Info(nameof(Resources.UT_TCP_TCP_007), new object[] { messageId });
                                    continue;
                                }
                            }
                        }
                    }
                    catch (ValidationException e)
                    {
                        // ��M���b�Z�[�W�t�H�[�}�b�g�ُ�i��{�݌v�� 5.1.2.4 �G���[�����j
                        log.Error(e, nameof(Resources.UT_TCP_TCP_002), new object[] { messageId, e.Message });
                    }
                    catch (Exception e)
                    {
                        log.Error(e, nameof(Resources.UT_TCP_TCP_001), new object[] { messageId });
                    }

                    // ���s�����ꍇ��Failure�X�g���[�W�ɏ�������
                    _service.UpdateToFailureStorage(EventHubName, messageId, JsonConvert.SerializeObject(new AzureEventGridEventSchema[] { eventSchema }));
                }
            }
            catch (RmsInvalidAppSettingException e)
            {
                log.Error(e, nameof(Resources.UT_TCP_TCP_009), new object[] { e.Message });

                // ���s�����ꍇ��Failure�X�g���[�W�ɏ�������
                _service.UpdateToFailureStorage(EventHubName, null, message);
            }
            catch (Exception e)
            {
                log.Error(e, nameof(Resources.UT_TCP_TCP_001), new object[] { null });

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
