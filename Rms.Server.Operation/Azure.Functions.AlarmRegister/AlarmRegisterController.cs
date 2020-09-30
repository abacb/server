using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Operation.Service.Services;
using Rms.Server.Operation.Utility;
using Rms.Server.Operation.Utility.Extensions;
using Rms.Server.Operation.Utility.Models;
using Rms.Server.Operation.Utility.Properties;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Operation.Azure.Functions.AlarmRegister
{
    /// <summary>
    /// AlarmRegister
    /// </summary>
    public class AlarmRegisterController
    {
        /// <summary>
        /// �ݒ�
        /// </summary>
        private readonly OperationAppSettings _settings;

        /// <summary>
        /// AlarmRegister�T�[�r�X�N���X
        /// </summary>
        private readonly IAlarmRegisterService _service;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="settings">�ݒ�</param>
        /// <param name="service">AlarmRegister�T�[�r�X�N���X</param>
        public AlarmRegisterController(OperationAppSettings settings, IAlarmRegisterService service)
        {
            Assert.IfNull(settings);
            Assert.IfNull(service);

            _settings = settings;
            _service = service;
        }

        /// <summary>
        /// �A���[�������L���[����擾��DB�ւ̓o�^����у��[���L���[�ւ̒ǉ����s��
        /// </summary>
        /// <remarks>sd 05-1.�A���[���o�^</remarks>
        /// <param name="queueItem">�A���[�����</param>
        /// <param name="log">���K�[</param>
        [FunctionName("AlarmRegister")]
        public void DequeueAlarmInfo([QueueTrigger("alarm", Connection = "ConnectionString")]string queueItem, ILogger log)
        {
            log.EnterJson("{0}", queueItem);

            string messageId = null;
            try
            {
                // �A�v���P�[�V�����ݒ�ő��M�����[���A�h���X���ݒ肳��Ă��Ȃ��ꍇ�A���s���ɃG���[�Ƃ���
                if (string.IsNullOrEmpty(_settings.AlarmMailAddressFrom))
                {
                    throw new RmsInvalidAppSettingException($"{nameof(_settings.AlarmMailAddressFrom)} is required.");
                }

                AlarmInfo alarmInfo = JsonConvert.DeserializeObject<AlarmInfo>(queueItem);

                messageId = alarmInfo?.MessageId;

                // �o���f�[�V����
                Validator.ValidateObject(alarmInfo, new ValidationContext(alarmInfo, null, null));

                // ���ꃁ�b�Z�[�W�ɑ΂���A���[�����ēx�A���[���L���[�֒ǉ������\�������邽�߁A�A���[���f�[�^��DB�ɓo�^����ۂɃ��b�Z�[�WID�ɂ��o�^�m�F���s���B
                bool? existSameMessageIdAlarm = _service.ExistsSameMessageIdAlarm(messageId);
                if (existSameMessageIdAlarm == true)
                {
                    // ���ꃁ�b�Z�[�WID�̃A���[��������DB�ɓo�^�ς�
                    log.Info(nameof(Resources.OP_ALR_ALR_004), new object[] { messageId });
                    return;
                }
                else if (existSameMessageIdAlarm == false)
                {
                    // Sq1.1.2: �A���[���ݒ���擾����
                    // Sq1.1.3: ���[�����M����擾����
                    if (_service.ReadDtAlarmConfig(alarmInfo, out DtEquipment equipment, out DtAlarmConfig alarmConfig))
                    {
                        // Sq1.1.4: ���[���Ԋu�������m�F����
                        if (_service.NeedsMailSending(messageId, alarmConfig, alarmInfo.AlarmDefId, out bool needMail))
                        {
                            bool sentMail = false;

                            if (needMail)
                            {
                                // Sq1.1.5: ���[���L���[��o�^����
                                sentMail = _service.CreateAndEnqueueAlarmInfo(messageId, alarmInfo, alarmConfig, equipment);
                                if (sentMail)
                                {
                                    // ���[���L���[�o�^����
                                    log.Info(nameof(Resources.OP_ALR_ALR_008), new object[] { messageId });
                                }
                            }

                            if (!needMail || sentMail)
                            {
                                // Sq1.1.1: �A���[���f�[�^��ۑ�����
                                // �G���[�ƂȂ����ꍇ�̓��O�̏�����Ɏ蓮�ŃA���[���f�[�^��ۑ�����
                                if (_service.RegistAlarmInfo(messageId, needMail, alarmInfo, equipment))
                                {
                                    // �A���[���f�[�^�ۑ�����
                                    log.Info(nameof(Resources.OP_ALR_ALR_010), new object[] { messageId });
                                    return;
                                }
                            }
                        }
                    }
                }

                // ���s�����ꍇ��Failure�X�g���[�W�ɏ�������
                _service.UpdateToFailureStorage(messageId, queueItem);
            }
            catch (Exception e) when (e is ValidationException || e is Newtonsoft.Json.JsonSerializationException || e is Newtonsoft.Json.JsonReaderException)
            {
                // �L���[�t�H�[�}�b�g�ُ�
                log.Error(e, nameof(Resources.OP_ALR_ALR_002), new object[] { messageId, e.Message });

                // ���s�����ꍇ��Failure�X�g���[�W�ɏ�������
                _service.UpdateToFailureStorage(messageId, queueItem);
            }
            catch (RmsInvalidAppSettingException e)
            {
                log.Error(e, nameof(Resources.OP_ALR_ALR_012), new object[] { e.Message });

                // ���s�����ꍇ��Failure�X�g���[�W�ɏ�������
                _service.UpdateToFailureStorage(messageId, queueItem);
            }
            catch (Exception e)
            {
                log.Error(e, nameof(Resources.OP_ALR_ALR_001), new object[] { messageId });

                // ���s�����ꍇ��Failure�X�g���[�W�ɏ�������
                _service.UpdateToFailureStorage(messageId, queueItem);
            }
            finally
            {
                log.Leave();
            }
        }
    }
}
