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
using System.Threading.Tasks;

namespace Rms.Server.Operation.Azure.Functions.MailSender
{
    /// <summary>
    /// MailSender
    /// </summary>
    public class MailSenderController
    {
        /// <summary>
        /// �ݒ�
        /// </summary>
        private readonly OperationAppSettings _settings;

        /// <summary>
        /// MailSender�T�[�r�X�N���X
        /// </summary>
        private readonly IMailSenderService _service;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="settings">�ݒ�</param>
        /// <param name="service">MailSender�T�[�r�X�N���X</param>
        public MailSenderController(OperationAppSettings settings, IMailSenderService service)
        {
            Assert.IfNull(settings);
            Assert.IfNull(service);

            _settings = settings;
            _service = service;
        }

        /// <summary>
        /// ���[�������L���[����擾�����[���̑��M���s��
        /// </summary>
        /// <remarks>sd 05-2.���[�����M</remarks>
        /// <param name="queueItem">���[�����</param>
        /// <param name="log">���K�[</param>
        /// <returns>���[�����M����</returns>
        [FunctionName("MailSender")]
        public async Task DequeueMailInfo([QueueTrigger("mail", Connection = "ConnectionString")]string queueItem, ILogger log)
        {
            log.EnterJson("{0}", queueItem);

            try
            {
                // �A�v���P�[�V�����ݒ�Ń��[���{���t�H�[�}�b�g���ݒ肳��Ă��Ȃ��ꍇ�A���s���ɃG���[�Ƃ���
                if (string.IsNullOrEmpty(_settings.MailTextFormat))
                {
                    throw new RmsInvalidAppSettingException($"{nameof(_settings.MailTextFormat)} is required.");
                }

                // Sq1.1.1: ���[�����M�f�[�^�𐶐�����
                MailInfo mailInfo = JsonConvert.DeserializeObject<MailInfo>(queueItem);

                // �o���f�[�V����
                Validator.ValidateObject(mailInfo, new ValidationContext(mailInfo, null, null));

                // Sq1.1.2: ���[�����M���˗�����
                if (await _service.SendMail(mailInfo))
                {
                    // ���[�����M�˗�����
                    log.Info(nameof(Resources.OP_MLS_MLS_003));
                }
                else
                {
                    // ���s�����ꍇ��Failure�X�g���[�W�ɏ�������
                    _service.UpdateToFailureStorage(queueItem);
                }
            }
            catch (ValidationException e)
            {
                // �L���[�t�H�[�}�b�g�ُ�
                log.Error(e, nameof(Resources.OP_MLS_MLS_005), new object[] { e.Message });

                // ���s�����ꍇ��Failure�X�g���[�W�ɏ�������
                _service.UpdateToFailureStorage(queueItem);
            }
            catch (RmsInvalidAppSettingException e)
            {
                log.Error(e, nameof(Resources.OP_MLS_MLS_006), new object[] { e.Message });

                // ���s�����ꍇ��Failure�X�g���[�W�ɏ�������
                _service.UpdateToFailureStorage(queueItem);
            }
            catch (Exception e)
            {
                log.Error(e, nameof(Resources.OP_MLS_MLS_001));

                // ���s�����ꍇ��Failure�X�g���[�W�ɏ�������
                _service.UpdateToFailureStorage(queueItem);
            }
            finally
            {
                log.Leave();
            }
        }
    }
}
