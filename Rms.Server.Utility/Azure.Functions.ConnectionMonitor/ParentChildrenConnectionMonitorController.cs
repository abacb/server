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

namespace Rms.Server.Utility.Azure.Functions.ConnectionMonitor
{
    /// <summary>
    /// �ʐM�Ď��A�v��(�e�q�ԒʐM�f�[�^�e�[�u���Ď�)
    /// </summary>
    public class ParentChildrenConnectionMonitorController
    {
        /// <summary>
        /// �ݒ�
        /// </summary>
        private readonly UtilityAppSettings _settings;

        /// <summary>
        /// �e�q�ԒʐM�f�[�^�e�[�u���Ď��T�[�r�X
        /// </summary>
        private readonly IParentChildrenConnectionMonitorService _service;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="settings">�ݒ�</param>
        /// <param name="service">�e�q�ԒʐM�f�[�^�e�[�u���Ď��T�[�r�X�N���X</param>
        public ParentChildrenConnectionMonitorController(UtilityAppSettings settings, IParentChildrenConnectionMonitorService service)
        {
            Assert.IfNull(settings);
            Assert.IfNull(service);

            _settings = settings;
            _service = service;
        }

        /// <summary>
        /// �e�q�ԒʐM�f�[�^�e�[�u��(Core)�̊Ď������s����
        /// </summary>
        /// <remarks>sd 07-11.�ʐM�Ď�</remarks>
        /// <param name="timerInfo">�^�C�}�[���</param>
        /// <param name="log">���K�[</param>
        [FunctionName("ParentChildrenConnectionMonitor")]
        public void MonitorParentChildConnectTable([TimerTrigger("0 0 16 * * *")]TimerInfo timerInfo, ILogger log)
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

                // Sq1.1: �e�q�@��Ԃ̐ڑ��󋵂��擾����
                if (_service.ReadParentChildConnect(out IEnumerable<DtParentChildConnect> parentChildConnects))
                {
                    // Sq1.2: �ʐM�Ď��A���[����`���擾����
                    _service.ReadAlarmDefinition(parentChildConnects, out List<Tuple<DtParentChildConnect, DtAlarmDefConnectionMonitor>> alarmJudgementTargets);

                    // Sq1.3: �A���[�������̗v�ۂ𔻒f����
                    bool needsAlarm = alarmJudgementTargets != null && alarmJudgementTargets.Count > 0;
                    if (!needsAlarm)
                    {
                        // �A���[�������s�v
                        IEnumerable<long?> sidArray = parentChildConnects == null ? new long?[] { } : parentChildConnects.Select(x => x?.Sid);
                        log.Info(nameof(Resources.UT_PCM_PCM_004), new object[] { string.Join(",", sidArray) });
                        return;
                    }

                    _service.CreateAlarmCreationTarget(alarmJudgementTargets, out List<Tuple<DtParentChildConnect, DtAlarmDefConnectionMonitor>> alarmCreationTargets);

                    // Sq1.3: �A���[�������̗v�ۂ𔻒f����
                    needsAlarm = alarmCreationTargets != null && alarmCreationTargets.Count > 0;
                    if (!needsAlarm)
                    {
                        // �A���[�������s�v
                        IEnumerable<long?> sidArray = alarmJudgementTargets.Select(x => x?.Item1?.Sid);
                        log.Info(nameof(Resources.UT_PCM_PCM_004), new object[] { string.Join(",", sidArray) });
                        return;
                    }

                    // Sq1.4: �A���[���L���[�𐶐�����
                    // Sq1.5: �L���[��o�^����
                    if (_service.CreateAndEnqueueAlarmInfo(alarmCreationTargets))
                    {
                        // �A���[���L���[�o�^����
                        IEnumerable<long?> sidArray = alarmCreationTargets.Select(x => x?.Item1?.Sid);
                        log.Info(nameof(Resources.UT_PCM_PCM_007), new object[] { string.Join(",", sidArray) });
                    }
                }
            }
            catch (RmsInvalidAppSettingException e)
            {
                log.Error(e, nameof(Resources.UT_PCM_PCM_008), new object[] { e.Message });
            }
            catch (Exception e)
            {
                log.Error(e, nameof(Resources.UT_PCM_PCM_001));
            }
            finally
            {
                log.LeaveJson("{0}", timerInfo);
            }
        }
    }
}
