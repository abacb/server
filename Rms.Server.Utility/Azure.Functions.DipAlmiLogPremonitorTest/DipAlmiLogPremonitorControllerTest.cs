using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Extensions;
using Rms.Server.Utility.Service.Services;
using Rms.Server.Utility.Utility;
using System;

namespace Rms.Server.Utility.Azure.Functions.DipAlmiLogPremonitorTest
{
    /// <summary>
    /// �����A���~�X���[�v���O�\���Ď��e�X�g
    /// </summary>
    public class DipAlmiLogPremonitorControllerTest
    {
        /// <summary>
        /// �ݒ�
        /// </summary>
        private readonly UtilityAppSettings _settings;

        /// <summary>
        /// �����A���~�X���[�v���O�\���Ď��T�[�r�X
        /// </summary>
        private readonly IDipAlmiLogPremonitorService _service;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="settings">�ݒ�</param>
        /// <param name="service">�����A���~�X���[�v���O�\���Ď��T�[�r�X�N���X</param>
        public DipAlmiLogPremonitorControllerTest(UtilityAppSettings settings, IDipAlmiLogPremonitorService service)
        {
            Assert.IfNull(settings);
            Assert.IfNull(service);

            _settings = settings;
            _service = service;
        }

        /// <summary>
        /// �����A���~�X���[�v���O�\���Ď��e�X�g
        /// </summary>
        /// <param name="message">���b�Z�[�W</param>
        /// <param name="log">���K�[</param>
        [FunctionName("DipAlmiLogPremonitorTest")]
        public void DipAlmiLogPremonitorTest([EventHubTrigger("ms-010", Connection = "ConnectionString")] string message, ILogger log)
        {
            log.EnterJson("{0}", message);
            try
            {
                var result = _service.ReadAlarmJudgementTarget();
                _service.CreateAndEnqueueAlarmInfo(result.Entity);
                _service.UpdateAlarmJudgedAnalysisResult(result.Entity);
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
