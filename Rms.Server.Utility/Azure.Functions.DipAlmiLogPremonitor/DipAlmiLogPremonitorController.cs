using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Utility.Service.Services;
using Rms.Server.Utility.Utility;
using Rms.Server.Utility.Utility.Extensions;
using Rms.Server.Utility.Utility.Models;
using Rms.Server.Utility.Utility.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rms.Server.Utility.Azure.Functions.DipAlmiLogPremonitor
{
    /// <summary>
    /// �����A���~�X���[�v���O�\���Ď�
    /// </summary>
    public class DipAlmiLogPremonitorController
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
        public DipAlmiLogPremonitorController(UtilityAppSettings settings, IDipAlmiLogPremonitorService service)
        {
            Assert.IfNull(settings);
            Assert.IfNull(service);

            _settings = settings;
            _service = service;
        }

        /// <summary>
        /// �A���~�X���[�v���O��͔��茋�ʃf�[�^�e�[�u���̊Ď������s����
        /// </summary>
        /// <remarks>sd 07-14.�A���~���O�\���Ď�</remarks>
        /// <param name="timerInfo">�^�C�}�[���</param>
        /// <param name="log">���K�[</param>
        [FunctionName("DipAlmiLogPremonitor")]
        public void MonitorAlmilogAnalysisResultTable([TimerTrigger("0 0 16 * * *")]TimerInfo timerInfo, ILogger log)
        {
            log.EnterJson("{0}", timerInfo);
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

                // Sq1.1: �A���~�X���[�v���O��͌��ʂ��擾����
                if (_service.ReadAlarmJudgementTarget(out List<DtAlmilogAnalysisResult> unjudgedItems, out List<List<DtAlmilogAnalysisResult>> alarmItemsList))
                {
                    // Sq1.2: �A���~�X���[�v���O�\���Ď��A���[����`���擾����
                    _service.ReadAlarmDefinition(alarmItemsList, out List<List<Tuple<DtAlmilogAnalysisResult, DtAlmilogPremonitor>>> models);

                    // Sq1.3: �A���[�������̗v�ۂ𔻒f����
                    bool needsAlarm = models != null && models.Count > 0;
                    if (!needsAlarm)
                    {
                        // �A���[�������s�v
                        IEnumerable<long?> sidArray = alarmItemsList == null ? new long?[] { } : alarmItemsList.Select(x => x.FirstOrDefault()?.Sid);
                        log.Info(nameof(Resources.UT_DAP_DAP_004), new object[] { string.Join(",", sidArray) });
                        return;
                    }

                    // Sq1.4: �A���[���L���[�𐶐�����
                    // Sq1.5: �L���[��o�^����
                    if (_service.CreateAndEnqueueAlarmInfo(models))
                    {
                        // �A���[���L���[�o�^����
                        IEnumerable<long?> sidArray = models.Select(x => x.FirstOrDefault()?.Item1?.Sid);
                        log.Info(nameof(Resources.UT_DAP_DAP_007), new object[] { string.Join(",", sidArray) });
                    }

                    // Sq1.6: ���f�̎��{�ς݃t���O���X�V����
                    if (_service.UpdateAlarmJudgedAnalysisResult(unjudgedItems))
                    {
                        // ����ς݃t���O�̍X�V����
                        IEnumerable<long> sidArray = unjudgedItems.Select(x => x.Sid);
                        log.Info(nameof(Resources.UT_DAP_DAP_009), new object[] { string.Join(",", sidArray) });
                    }
                }
            }
            catch (RmsInvalidAppSettingException e)
            {
                log.Error(e, nameof(Resources.UT_DAP_DAP_010), new object[] { e.Message });
            }
            catch (Exception e)
            {
                log.Error(e, nameof(Resources.UT_DAP_DAP_001));
            }
            finally
            {
                log.LeaveJson("{0}", timerInfo);
            }
        }
    }
}
