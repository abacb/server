using Microsoft.Extensions.Logging;
using Rms.Server.Core.Abstraction.Models;
using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Utility.Abstraction.Repositories;
using Rms.Server.Utility.Utility;
using Rms.Server.Utility.Utility.Extensions;
using Rms.Server.Utility.Utility.Models;
using Rms.Server.Utility.Utility.Properties;
using System;
using static Rms.Server.Utility.Utility.LogAnalysisDllWrapper;

namespace Rms.Server.Utility.Service.Services
{
    /// <summary>
    /// DipBlocLogAnalyzerService
    /// </summary>
    public class DipBlocLogAnalyzerService : IDipBlocLogAnalyzerService
    {
        /// <summary>
        /// 設定
        /// </summary>
        private readonly UtilityAppSettings _settings;

        /// <summary>
        /// ロガー
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// DateTimeの提供元
        /// </summary>
        private readonly ITimeProvider _timeProvider;

        /// <summary>
        /// ムラログ解析判定結果リポジトリ
        /// </summary>
        private readonly IDtBloclogAnalysisResultRepository _dtBloclogAnalysisResultRepository;

        /// <summary>
        /// ムラログ解析設定リポジトリ
        /// </summary>
        private readonly IDtBloclogAnalysisConfigRepository _dtBloclogAnalysisConfigRepository;

        /// <summary>
        /// Failureストレージリポジトリ
        /// </summary>
        private readonly IFailureRepository _failureRepository;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">設定</param>
        /// <param name="logger">ロガー</param>
        /// <param name="timeProvider">DateTimeの提供元</param>
        /// <param name="dtBloclogAnalysisResultRepository">ムラログ解析判定結果リポジトリ</param>
        /// <param name="dtBloclogAnalysisConfigRepository">ムラログ解析設定リポジトリ</param>
        /// <param name="failureRepository">Failureストレージリポジトリ</param>
        public DipBlocLogAnalyzerService(
            UtilityAppSettings settings,
            ILogger<DipBlocLogAnalyzerService> logger,
            ITimeProvider timeProvider,
            IDtBloclogAnalysisResultRepository dtBloclogAnalysisResultRepository,
            IDtBloclogAnalysisConfigRepository dtBloclogAnalysisConfigRepository,
            IFailureRepository failureRepository)
        {
            Assert.IfNull(settings);
            Assert.IfNull(logger);
            Assert.IfNull(timeProvider);
            Assert.IfNull(dtBloclogAnalysisResultRepository);
            Assert.IfNull(dtBloclogAnalysisConfigRepository);
            Assert.IfNull(failureRepository);

            _settings = settings;
            _logger = logger;
            _timeProvider = timeProvider;
            _dtBloclogAnalysisResultRepository = dtBloclogAnalysisResultRepository;
            _dtBloclogAnalysisConfigRepository = dtBloclogAnalysisConfigRepository;
            _failureRepository = failureRepository;
        }

        /// <summary>
        /// 同一ファイル名の解析結果が登録されているか確認する
        /// </summary>
        /// <param name="logFileName">ログファイル名</param>
        /// <param name="messageId">メッセージID</param>
        /// <returns>解析結果が未登録の場合trueを返す。解析結果が登録済み、あるいは、処理に失敗した場合falseを返す。</returns>
        public bool CheckDuplicateAnalysisReuslt(string logFileName, string messageId)
        {
            try
            {
                _logger.EnterJson("{0}", new { logFileName, messageId });

                bool isExist = _dtBloclogAnalysisResultRepository.ExistDtBloclogAnalysisResult(logFileName);
                if (isExist)
                {
                    // DBにすでにデータが存在する場合は解析を実行しない
                    _logger.Error(nameof(Resources.UT_DBA_DBA_004), new object[] { messageId });
                }

                return !isExist;
            }
            catch (RmsException e)
            {
                _logger.Error(e, nameof(Resources.UT_DBA_DBA_003), new object[] { messageId });
                return false;
            }
            finally
            {
                _logger.Leave();
            }
        }

        /// <summary>
        /// ムラログ解析を行う
        /// </summary>
        /// <param name="dipBlocLog">骨塩ムラログ</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="_analysisData">ムラログ解析対象データ</param>
        /// <param name="_analysisResult">ムラログ解析結果</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        public bool AnalyzeDipBlocLog(DipBlocLog dipBlocLog, string messageId, out BlocLogAnalysisData _analysisData, out BlocLogAnalysisResult _analysisResult)
        {
            _analysisData = new BlocLogAnalysisData();
            _analysisResult = new BlocLogAnalysisResult();

            try
            {
                _logger.EnterJson("{0}", new { dipBlocLog, messageId });

                int gpValue = int.Parse(dipBlocLog.GpValue);

                if (gpValue != 0 && (gpValue < GpMinValue || GpMaxValue < gpValue))
                {
                    string message = $"GP値が正しくありません。(機器管理番号:{dipBlocLog?.SourceEquipmentUid})(解析ログファイル名:{dipBlocLog?.FileName})(GP値:{gpValue})";
                    throw new RmsException(message);
                }

                bool isNormalized = gpValue == 0 ? false : true;

                var blocLogConfigResult = _dtBloclogAnalysisConfigRepository.ReadDtBloclogAnalysisConfig(isNormalized, false);

                CreatetAnalysisData(dipBlocLog, blocLogConfigResult, out _analysisData);

                int ret = NativeMethods.AnalyzeBlocLog(ref _analysisData, ref _analysisResult);

                if (ret != 0)
                {
                    string message = $"(機器管理番号:{dipBlocLog?.SourceEquipmentUid})(解析ログファイル名:{dipBlocLog?.FileName})";
                    throw new RmsException(string.Format("LogAnalysisDll.AnalyzeBlocLog() returned {0}. {1} {2}", ret, _analysisResult.ErrorMsg, message));
                }

                return true;
            }
            catch (RmsException e)
            {
                _logger.Error(e, nameof(Resources.UT_DBA_DBA_005), new object[] { messageId });
                return false;
            }
            finally
            {
                _logger.LeaveJson("{0}", new { _analysisData, _analysisResult });
            }
        }

        /// <summary>
        /// ムラログ解析結果をDBに登録する
        /// </summary>
        /// <param name="dipBlocLog">骨塩ムラログ</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="_analysisData">ムラログ解析対象データ</param>
        /// <param name="_analysisResult">ムラログ解析結果</param>
        /// <param name="model">DBへの登録結果</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        public bool RegistBlocLogAnalysisResultToDb(DipBlocLog dipBlocLog, string messageId, BlocLogAnalysisData _analysisData, BlocLogAnalysisResult _analysisResult, out DtBloclogAnalysisResult model)
        {
            model = null;

            try
            {
                _logger.EnterJson("{0}", new { dipBlocLog, messageId, _analysisData, _analysisResult });

                var dtBloclogAnalysisResult = new DtBloclogAnalysisResult
                {
                    EquipmentUid = dipBlocLog.SourceEquipmentUid,
                    BloclogMonth = dipBlocLog.OccurrenceYm,
                    DetectorName = dipBlocLog.DetectorName,
                    DetectorId = dipBlocLog.DetectorId,
                    GpValue = int.Parse(dipBlocLog.GpValue),
                    ImageFileName = dipBlocLog.FileName,
                    FileNameNo = short.Parse(dipBlocLog.SNumber),
                    ShadingResult = _analysisResult.UnevenResult,
                    ShadingResultMcv = _analysisResult.Mcv,
                    ShadingResultScv = _analysisResult.Scv,
                    ShadingResultMcvSv = _analysisData.McvStandardValue,
                    ShadingResultScvSv1 = _analysisData.ScvStandardValue1,
                    ShadingResultScvSv2 = _analysisData.ScvStandardValue2,
                    ImageType = _analysisResult.ImageClassification == 0 ? false : true,
                    ImageSize = _analysisResult.PixelSize,
                    IsBillTarget = dipBlocLog.ServiceFlg == true ? false : true,
                    LogFileName = dipBlocLog.LogFileName
                };

                model = _dtBloclogAnalysisResultRepository.CreateDtBloclogAnalysisResult(dtBloclogAnalysisResult);

                return true;
            }
            catch (RmsParameterException e)
            {
                _logger.Error(e, nameof(Resources.UT_DBA_DBA_006), new object[] { messageId });
                return false;
            }
            catch (RmsException e)
            {
                _logger.Error(e, nameof(Resources.UT_DBA_DBA_006), new object[] { messageId });
                return false;
            }
            finally
            {
                _logger.LeaveJson("{0}", new { model });
            }
        }

        /// <summary>
        /// Failureストレージに再送用メッセージをアップロードする
        /// </summary>
        /// <param name="messageSchemaId">メッセージスキーマID</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="message">メッセージ</param>
        public void UpdateToFailureStorage(string messageSchemaId, string messageId, string message)
        {
            try
            {
                _logger.EnterJson("{0}", new { messageSchemaId, messageId, message });

                DateTime now = _timeProvider.UtcNow;
                bool noMessageId = string.IsNullOrEmpty(messageId);

                // ファイル情報
                ArchiveFile file = new ArchiveFile() { ContainerName = _settings.FailureBlobContainerName, CreatedAt = now };
                if (noMessageId)
                {
                    file.FilePath = string.Format(Utility.Const.FailureBlobFilenameFormatWithoutMessageId, messageSchemaId, messageId, now);
                }
                else
                {
                    file.FilePath = string.Format(Utility.Const.FailureBlobFilenameFormat, messageSchemaId, messageId, now);
                }

                // アップロード
                _failureRepository.Upload(file, message, noMessageId);
            }
            catch (RmsException e)
            {
                // Blobストレージへの保存処理に失敗した場合、メッセージ内容をログに出力して終了する。
                _logger.Error(e, nameof(Resources.UT_DBA_DBA_008), new object[] { messageId, message });
            }
            finally
            {
                _logger.Leave();
            }
        }

        /// <summary>
        /// 解析対象データを作成する
        /// </summary>
        /// <param name="dipBlocLog">骨塩ムラログ</param>
        /// <param name="bloclogConfig">ムラグ解析設定</param>
        /// <param name="_analysisData">ムラログ解析対象データ</param>
        private void CreatetAnalysisData(DipBlocLog dipBlocLog, DtBloclogAnalysisConfig bloclogConfig, out BlocLogAnalysisData _analysisData)
        {
            _analysisData = new BlocLogAnalysisData();

            try
            {
                _logger.EnterJson("{0}", new { dipBlocLog, bloclogConfig });

                var profileValue = new double[AnalysisDataMax];
                for (int i = 0; i < dipBlocLog.ProfileValue.Length; i++)
                {
                    profileValue[i] = double.Parse(dipBlocLog.ProfileValue[i]);
                }

                _analysisData = new BlocLogAnalysisData()
                {
                    ProfileValue = profileValue,
                    ProfileValueCount = dipBlocLog.ProfileValue.Length,
                    GpValue = int.Parse(dipBlocLog.GpValue),
                    TopUnevennessSkipValue = bloclogConfig.TopUnevennessSkipValue,
                    BottomUnevennessSkipValue = bloclogConfig.BottomUnevennessSkipValue,
                    McvStandardValue = bloclogConfig.McvStandardValue,
                    ScvStandardValue1 = bloclogConfig.ScvStandardValue1,
                    ScvStandardValue2 = bloclogConfig.ScvStandardValue2
                };
            }
            catch
            {
                throw; // ログ出力はpublicメソッドにお任せ
            }
            finally
            {
                _logger.LeaveJson("{0}", new { _analysisData });
            }
        }
    }
}
