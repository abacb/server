using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Extensions;
using Rms.Server.Utility.Service.Services;
using Rms.Server.Utility.Utility;
using System;

namespace Rms.Sever.Utility.Azure.Functions.ConnectionMonitorTest
{
    /// <summary>
    /// �ʐM�Ď��A�v���e�X�g
    /// </summary>
    public class ConnectionMonitorControllerTest
    {
        /// <summary>
        /// �ݒ�
        /// </summary>
        private readonly UtilityAppSettings _settings;

        /// <summary>
        /// �e�q�ԒʐM�f�[�^�e�[�u���Ď��T�[�r�X
        /// </summary>
        private readonly IParentChildrenConnectionMonitorService _parentChildrenConnectionService;

        /// <summary>
        /// �[���f�[�^�e�[�u���Ď��T�[�r�X
        /// </summary>
        private readonly IDeviceConnectionMonitorService _deviceConnectionService;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="settings">�ݒ�</param>
        /// <param name="parentChildrenConnectionService">�e�q�ԒʐM�f�[�^�e�[�u���Ď��T�[�r�X�N���X</param>
        /// <param name="deviceConnectionService">�[���f�[�^�e�[�u���Ď��T�[�r�X�N���X</param>
        public ConnectionMonitorControllerTest(UtilityAppSettings settings, IParentChildrenConnectionMonitorService parentChildrenConnectionService, IDeviceConnectionMonitorService deviceConnectionService)
        {
            Assert.IfNull(settings);
            Assert.IfNull(parentChildrenConnectionService);
            Assert.IfNull(deviceConnectionService);

            _settings = settings;
            _parentChildrenConnectionService = parentChildrenConnectionService;
            _deviceConnectionService = deviceConnectionService;
        }

        /// <summary>
        /// �ʐM�Ď��e�X�g
        /// </summary>
        /// <param name="message">���b�Z�[�W</param>
        /// <param name="log">���K�[</param>
        [FunctionName("ConnectionMonitorTest")]
        public void ConnectionMonitorTest([EventHubTrigger("ms-010", Connection = "ConnectionString")] string message, ILogger log)
        {
            log.EnterJson("{0}", message);
            try
            {
                var result = _parentChildrenConnectionService.ReadAlarmDefinition();
            }
            catch (Exception ex)
            {
                // [TODO]:Blob�փN���C�A���g���b�Z�[�W��ۑ����鏈����ǉ�����(Controller�ł͂Ȃ�Service�Ŏ��s����?)
                log.Error(ex, "�z��O�̃G���[");
            }
            finally
            {
                log.LeaveJson("{0}", message);
            }
        }
    }
}
