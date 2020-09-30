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
using System.Linq;
using static Rms.Server.Utility.Utility.LogAnalysisDllWrapper;

namespace Rms.Server.Utility.Service.Services
{
    /// <summary>
    /// DipAlmiLogAnalyzerService
    /// </summary>
    public class DipAlmiLogAnalyzerService : IDipAlmiLogAnalyzerService
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
        /// アルミスロープログ解析判定結果リポジトリ
        /// </summary>
        private readonly IDtAlmilogAnalysisResultRepository _dtAlmilogAnalysisResultRepository;

        /// <summary>
        /// アルミスロープログ解析設定リポジトリ
        /// </summary>
        private readonly IDtAlmilogAnalysisConfigRepository _dtAlmilogAnalysisConfigRepository;

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
        /// <param name="dtAlmilogAnalysisResultRepository">アルミスロープログ解析判定結果リポジトリ</param>
        /// <param name="dtAlmilogAnalysisConfigRepository">アルミスロープログ解析設定リポジトリ</param>
        /// <param name="dtBloclogAnalysisConfigRepository">ムラログ解析設定リポジトリ</param>
        /// <param name="failureRepository">Failureストレージリポジトリ</param>
        public DipAlmiLogAnalyzerService(
            UtilityAppSettings settings,
            ILogger<DipAlmiLogAnalyzerService> logger,
            ITimeProvider timeProvider,
            IDtAlmilogAnalysisResultRepository dtAlmilogAnalysisResultRepository,
            IDtAlmilogAnalysisConfigRepository dtAlmilogAnalysisConfigRepository,
            IDtBloclogAnalysisConfigRepository dtBloclogAnalysisConfigRepository,
            IFailureRepository failureRepository)
        {
            Assert.IfNull(settings);
            Assert.IfNull(logger);
            Assert.IfNull(timeProvider);
            Assert.IfNull(dtAlmilogAnalysisResultRepository);
            Assert.IfNull(dtAlmilogAnalysisConfigRepository);
            Assert.IfNull(dtBloclogAnalysisConfigRepository);
            Assert.IfNull(failureRepository);

            _settings = settings;
            _logger = logger;
            _timeProvider = timeProvider;
            _dtAlmilogAnalysisResultRepository = dtAlmilogAnalysisResultRepository;
            _dtAlmilogAnalysisConfigRepository = dtAlmilogAnalysisConfigRepository;
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

                bool isExist = _dtAlmilogAnalysisResultRepository.ExistDtAlmilogAnalysisResult(logFileName);
                if (isExist)
                {
                    // DBにすでにデータが存在する場合は解析を実行しない
                    _logger.Error(nameof(Resources.UT_DAA_DAA_004), new object[] { messageId });
                }

                return !isExist;
            }
            catch (RmsException e)
            {
                _logger.Error(e, nameof(Resources.UT_DAA_DAA_003), new object[] { messageId });
                return false;
            }
            finally
            {
                _logger.Leave();
            }
        }

        /// <summary>
        /// アルミロープログ解析を行う
        /// </summary>
        /// <param name="dipAlmiSlopeLog">骨塩アルミスロープログ</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="_analysisData">アルミスロープログ解析対象データ</param>
        /// <param name="_analysisResult">アルミスロープログ解析結果</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        public bool AnalyzeDipAlmiLog(DipAlmiSlopeLog dipAlmiSlopeLog, string messageId, out AlmiLogAnalysisData _analysisData, out AlmiLogAnalysisResult _analysisResult)
        {
            _analysisData = new AlmiLogAnalysisData();
            _analysisResult = new AlmiLogAnalysisResult();

            try
            {
                _logger.EnterJson("{0}", new { dipAlmiSlopeLog, messageId });

                int gpValue = int.Parse(dipAlmiSlopeLog.GpValue);

                if (gpValue != 0 && (gpValue < GpMinValue || GpMaxValue < gpValue))
                {
                    string message = $"GP値が正しくありません。(機器管理番号:{dipAlmiSlopeLog?.SourceEquipmentUid})(解析ログファイル名:{dipAlmiSlopeLog?.FileName})(GP値:{gpValue})";
                    throw new RmsException(message);
                }

                bool isNormalized = gpValue == 0 ? false : true;

                var almiLogConfigResult = _dtAlmilogAnalysisConfigRepository.ReadDtAlmilogAnalysisConfig(dipAlmiSlopeLog.DetectorName, isNormalized, false);
                var blocLogConfigResult = _dtBloclogAnalysisConfigRepository.ReadDtBloclogAnalysisConfig(isNormalized, false);

                CreatetAnalysisData(dipAlmiSlopeLog, almiLogConfigResult, blocLogConfigResult, out _analysisData);

                int ret = NativeMethods.AnalyzeAlmiLog(ref _analysisData, ref _analysisResult);

                if (ret != 0)
                {
                    string message = $"(機器管理番号:{dipAlmiSlopeLog?.SourceEquipmentUid})(解析ログファイル名:{dipAlmiSlopeLog?.FileName})";
                    throw new RmsException(string.Format("LogAnalysisDll.AnalyzeAlmiLog() returned {0}. {1} {2}", ret, _analysisResult.ErrorMsg, message));
                }

                return true;
            }
            catch (RmsException e)
            {
                _logger.Error(e, nameof(Resources.UT_DAA_DAA_005), new object[] { messageId });
                return false;
            }
            finally
            {
                _logger.LeaveJson("{0}", new { _analysisData, _analysisResult });
            }
        }

        /// <summary>
        /// アルミロープログ解析結果をDBに登録する
        /// </summary>
        /// <param name="dipAlmiSlopeLog">骨塩アルミスロープログ</param>
        /// <param name="messageId">メッセージID</param>
        /// <param name="_analysisData">アルミスロープログ解析対象データ</param>
        /// <param name="_analysisResult">アルミスロープログ解析結果</param>
        /// <param name="model">DBへの登録結果</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        public bool RegistAlmiLogAnalysisResultToDb(DipAlmiSlopeLog dipAlmiSlopeLog, string messageId, AlmiLogAnalysisData _analysisData, AlmiLogAnalysisResult _analysisResult, out DtAlmilogAnalysisResult model)
        {
            model = null;

            try
            {
                _logger.EnterJson("{0}", new { dipAlmiSlopeLog, messageId, _analysisData, _analysisResult });

                var dtAlmilogAnalysisResult = new DtAlmilogAnalysisResult
                {
                    EquipmentUid = dipAlmiSlopeLog.SourceEquipmentUid,
                    AnalysisResult = _analysisResult.JudgeResult,
                    CalculateInclinationValue = _analysisResult.CulSlopeValue,
                    CalculateAreaValue = _analysisResult.CulAreaValue,
                    MaxInclinationValue = _analysisData.MaxSlopeValue,
                    MinInclinationValue = _analysisData.MinSlopeValue,
                    StandardAreaValue = _analysisResult.CulStdValue,
                    AlmilogMonth = dipAlmiSlopeLog.OccurrenceYm,
                    DetectorName = dipAlmiSlopeLog.DetectorName,
                    DetectorId = dipAlmiSlopeLog.DetectorId,
                    GpValue = int.Parse(dipAlmiSlopeLog.GpValue),
                    ImageFileName = dipAlmiSlopeLog.FileName,
                    FileNameNo = short.Parse(dipAlmiSlopeLog.SNumber),
                    ReverseResult = _analysisResult.SlopeReverseResult,
                    ReverseResultInclination = _analysisResult.Inclination,
                    IsAlarmJudged = false,
                    IsBillTarget = dipAlmiSlopeLog.ServiceFlg == true ? false : true,
                    LogFileName = dipAlmiSlopeLog.LogFileName
                };

                model = _dtAlmilogAnalysisResultRepository.CreateDtAlmilogAnalysisResult(dtAlmilogAnalysisResult);

                return true;
            }
            catch (RmsParameterException e)
            {
                _logger.Error(e, nameof(Resources.UT_DAA_DAA_006), new object[] { messageId });
                return false;
            }
            catch (RmsException e)
            {
                _logger.Error(e, nameof(Resources.UT_DAA_DAA_006), new object[] { messageId });
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
                _logger.Error(e, nameof(Resources.UT_DAA_DAA_008), new object[] { messageId, message });
            }
            finally
            {
                _logger.Leave();
            }
        }

        /// <summary>
        /// 解析対象データを作成する
        /// </summary>
        /// <param name="dipAlmiSlopeLog">骨塩アルミスロープログ</param>
        /// <param name="almilogConfig">アルミスロープログ解析設定</param>
        /// <param name="bloclogConfig">ムラグ解析設定</param>
        /// <param name="_analysisData">アルミスロープログ解析対象データ</param>
        private void CreatetAnalysisData(DipAlmiSlopeLog dipAlmiSlopeLog, DtAlmilogAnalysisConfig almilogConfig, DtBloclogAnalysisConfig bloclogConfig, out AlmiLogAnalysisData _analysisData)
        {
            _analysisData = new AlmiLogAnalysisData();

            try
            {
                _logger.EnterJson("{0}", new { dipAlmiSlopeLog, almilogConfig, bloclogConfig });

                var areaStandardDataConfgValue = almilogConfig.AreaStandardData.Split(',').Select(x => int.Parse(x)).ToArray();

                // バリデーション
                if (areaStandardDataConfgValue.Length != AlmiAreaDataNum)
                {
                    // 個数が異なる場合は解析が実施できないためエラー
                    throw new RmsException("アルミスロープログ解析設定の面積算出用データの要素数が不正です");
                }

                var luminanceValue = new int[AnalysisDataMax];
                for (int i = 0; i < dipAlmiSlopeLog.LuminanceValue.Length; i++)
                {
                    luminanceValue[i] = int.Parse(dipAlmiSlopeLog.LuminanceValue[i]);
                }

                // AreaStandardDataのデータ数はAlmiAreaDataNum固定だがCモジュールへデータを渡す都合上サイズはAnalysisDataMax必要
                var areaStandardData = new int[AnalysisDataMax];
                for (int i = 0; i < areaStandardDataConfgValue.Length; i++)
                {
                    areaStandardData[i] = areaStandardDataConfgValue[i];
                }

                _analysisData = new AlmiLogAnalysisData()
                {
                    LuminanceValue = luminanceValue,
                    LuminanceValueCount = dipAlmiSlopeLog.LuminanceValue.Length,
                    MinSlopeValue = almilogConfig.MinSlopeValue,
                    MiddleSlopeValue = almilogConfig.MiddleSlopeValue,
                    MaxSlopeValue = almilogConfig.MaxSlopeValue,
                    LowVoltageAreaValue = almilogConfig.LowVoltageAreaValue,
                    HighVoltageAreaValue = almilogConfig.HighVoltageAreaValue,
                    AreaStandardData = areaStandardData,
                    AlmiAreaDataNum = AlmiAreaDataNum,
                    AlsStandardValue = bloclogConfig.AlsStandardValue
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
