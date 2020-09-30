using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rms.Server.Core.Utility;
using Rms.Server.Utility.Service.Services;
using Rms.Server.Utility.Utility.Extensions;
using Rms.Server.Utility.Utility.Models;
using Rms.Server.Utility.Utility.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Rms.Server.Utility.Utility.LogAnalysisDllWrapper;

namespace Rms.Server.Utility.Azure.Functions.DipBlocLogAnalyzer
{
    /// <summary>
    /// �����������O��̓A�v��
    /// </summary>
    public class DipBlocLogAnalyzerController
    {
        /// <summary>
        /// �C�x���g�n�u���i���b�Z�[�W�X�L�[�}ID�j
        /// </summary>
        private const string EventHubName = "ms-013";

        /// <summary>
        /// �����������O��̓T�[�r�X
        /// </summary>
        private readonly IDipBlocLogAnalyzerService _service;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="service">�����������O��̓T�[�r�X�N���X</param>
        public DipBlocLogAnalyzerController(IDipBlocLogAnalyzerService service)
        {
            Assert.IfNull(service);

            _service = service;
        }

        /// <summary>
        /// �����������O����M����
        /// </summary>
        /// <remarks>sd 07-04.�������O���</remarks>
        /// <param name="message">���b�Z�[�W</param>
        /// <param name="log">���K�[</param>
        [FunctionName("DipBlocLogAnalyzer")]
        public void ReceiveDipBlocLog([EventHubTrigger(EventHubName, Connection = "ConnectionString")] string message, ILogger log)
        {
            log.EnterJson("{0}", message);
            try
            {
                var eventSchemaList = JsonConvert.DeserializeObject<List<AzureEventGridEventSchema>>(message);
                foreach (var eventSchema in eventSchemaList)
                {
                    string messageId = null;
                    try
                    {
                        var deviceTelemetry = JsonConvert.DeserializeObject<IotHubDeviceTelemetry>(eventSchema.Data.ToStringJson());
                        messageId = deviceTelemetry?.SystemProperties?.MessageId;
                        var dipBlocLog = JsonConvert.DeserializeObject<DipBlocLog>(deviceTelemetry.Body.ToStringJson());

                        // �o���f�[�V����
                        Validator.ValidateObject(dipBlocLog, new ValidationContext(dipBlocLog, null, null));

                        // ��͂��s���O�Ƀ��b�Z�[�W�̃��O�t�@�C���������Ƃ�DB�̊����f�[�^�Ɣ�r
                        if (_service.CheckDuplicateAnalysisReuslt(dipBlocLog.LogFileName, messageId))
                        {
                            // Sq1.1.1: �������O����͂���
                            if (_service.AnalyzeDipBlocLog(dipBlocLog, messageId, out BlocLogAnalysisData _analysisData, out BlocLogAnalysisResult _analysisResult))
                            {
                                // Sq1.1.2: ��͌��ʂ�ۑ�����
                                if (_service.RegistBlocLogAnalysisResultToDb(dipBlocLog, messageId, _analysisData, _analysisResult, out DtBloclogAnalysisResult model))
                                {
                                    // ��͌��ʕۑ�����
                                    log.Info(nameof(Resources.UT_DBA_DBA_007), new object[] { messageId });
                                    continue;
                                }
                            }
                        }
                    }
                    catch (ValidationException e)
                    {
                        // ��M���b�Z�[�W�t�H�[�}�b�g�ُ�i��{�݌v�� 5.1.13.1 �G���[�����j
                        log.Error(e, nameof(Resources.UT_DBA_DBA_002), new object[] { messageId, e.Message });
                    }
                    catch (Exception e)
                    {
                        log.Error(e, nameof(Resources.UT_DBA_DBA_001), new object[] { messageId });
                    }

                    // ���s�����ꍇ��Failure�X�g���[�W�ɏ�������
                    _service.UpdateToFailureStorage(EventHubName, messageId, JsonConvert.SerializeObject(new AzureEventGridEventSchema[] { eventSchema }));
                }
            }
            catch (Exception e)
            {
                log.Error(e, nameof(Resources.UT_DBA_DBA_001), new object[] { null });

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
