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

namespace Rms.Server.Utility.Azure.Functions.DipAlmiLogAnalyzer
{
    /// <summary>
    /// �����A���~�X���[�v���O��̓A�v��
    /// </summary>
    public class DipAlmiLogAnalyzerController
    {
        /// <summary>
        /// �C�x���g�n�u���i���b�Z�[�W�X�L�[�}ID�j
        /// </summary>
        private const string EventHubName = "ms-012";

        /// <summary>
        /// �����A���~�X���[�v���O��̓T�[�r�X
        /// </summary>
        private readonly IDipAlmiLogAnalyzerService _service;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="service">�����A���~�X���[�v���O��̓T�[�r�X�N���X</param>
        public DipAlmiLogAnalyzerController(IDipAlmiLogAnalyzerService service)
        {
            Assert.IfNull(service);

            _service = service;
        }

        /// <summary>
        /// �����A���~�X���[�v���O����M����
        /// </summary>
        /// <remarks>sd 07-03.�A���~���O���</remarks>
        /// <param name="message">���b�Z�[�W</param>
        /// <param name="log">���K�[</param>
        [FunctionName("DipAlmiLogAnalyzer")]
        public void ReceiveDipAlmiSlopeLog([EventHubTrigger(EventHubName, Connection = "ConnectionString")] string message, ILogger log)
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
                        var dipAlmiSlopeLog = JsonConvert.DeserializeObject<DipAlmiSlopeLog>(deviceTelemetry.Body.ToStringJson());

                        // �o���f�[�V����
                        Validator.ValidateObject(dipAlmiSlopeLog, new ValidationContext(dipAlmiSlopeLog, null, null));

                        // ��͂��s���O�Ƀ��b�Z�[�W�̃��O�t�@�C���������Ƃ�DB�̊����f�[�^�Ɣ�r
                        if (_service.CheckDuplicateAnalysisReuslt(dipAlmiSlopeLog.LogFileName, messageId))
                        {
                            // Sq1.1.1: �A���~�X���[�v���O����͂���
                            if (_service.AnalyzeDipAlmiLog(dipAlmiSlopeLog, messageId, out AlmiLogAnalysisData _analysisData, out AlmiLogAnalysisResult _analysisResult))
                            {
                                // Sq1.1.2: ��͌��ʂ�ۑ�����
                                if (_service.RegistAlmiLogAnalysisResultToDb(dipAlmiSlopeLog, messageId, _analysisData, _analysisResult, out DtAlmilogAnalysisResult model))
                                {
                                    // ��͌��ʕۑ�����
                                    log.Info(nameof(Resources.UT_DAA_DAA_007), new object[] { messageId });
                                    continue;
                                }
                            }
                        }
                    }
                    catch (ValidationException e)
                    {
                        // ��M���b�Z�[�W�t�H�[�}�b�g�ُ�i��{�݌v�� 5.1.13.1 �G���[�����j
                        log.Error(e, nameof(Resources.UT_DAA_DAA_002), new object[] { messageId, e.Message });
                    }
                    catch (Exception e)
                    {
                        log.Error(e, nameof(Resources.UT_DAA_DAA_001), new object[] { messageId });
                    }

                    // ���s�����ꍇ��Failure�X�g���[�W�ɏ�������
                    _service.UpdateToFailureStorage(EventHubName, messageId, JsonConvert.SerializeObject(new AzureEventGridEventSchema[] { eventSchema }));
                }
            }
            catch (Exception e)
            {
                log.Error(e, nameof(Resources.UT_DAA_DAA_001), new object[] { null });

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
