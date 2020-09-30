using Microsoft.Extensions.Logging;
using Rms.Server.Core.Abstraction.Models;
using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Core.Service.Models;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Core.Utility.Extensions;
using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Utility.Models.Entites;
using Rms.Server.Core.Utility.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rms.Server.Core.Service.Services
{
    /// <summary>
    /// CleanBlobService
    /// </summary>
    public class CleanBlobService : ICleanBlobService
    {
        /// <summary>
        /// 設定
        /// </summary>
        private readonly AppSettings _settings;

        /// <summary>
        /// ロガー
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// PrimaryRepository
        /// </summary>
        private readonly IPrimaryRepository _primaryBlobRepository;

        /// <summary>
        /// DtDeviceFileRepository
        /// </summary>
        private readonly IDtDeviceFileRepository _dtDeviceFileRepository;

        /// <summary>
        /// TimeProvider
        /// </summary>
        private readonly ITimeProvider _timeProvider;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">設定</param>
        /// <param name="logger">ロガー</param>
        /// <param name="primaryRepository">プライマリファイルリポジトリ</param>
        /// <param name="dtDeviceFileRepository">デバイスファイルリポジトリ</param>
        /// <param name="timeProvider">TimeProvider</param>
        public CleanBlobService(
            AppSettings settings,
            ILogger<CleanBlobService> logger,
            IPrimaryRepository primaryRepository,
            IDtDeviceFileRepository dtDeviceFileRepository,
            ITimeProvider timeProvider)
        {
            Assert.IfNull(settings);
            Assert.IfNull(logger);
            Assert.IfNull(primaryRepository);
            Assert.IfNull(dtDeviceFileRepository);
            Assert.IfNull(timeProvider);

            _settings = settings;
            _logger = logger;
            _primaryBlobRepository = primaryRepository;
            _dtDeviceFileRepository = dtDeviceFileRepository;
            _timeProvider = timeProvider;
        }

        /// <summary>
        /// DeleteFileメソッド内の処理ステータス
        /// </summary>
        private enum DeleteFileStatus
        {
            /// <summary>
            /// DeleteDtDeviceFile呼び出し中
            /// </summary>
            DeleteDtDeviceFile = 0,

            /// <summary>
            /// Delete呼び出し中
            /// </summary>
            Delete = 1,
        }

        /// <summary>
        /// 一定期間より古いファイルを削除する
        /// </summary>
        public void Clean()
        {
            _logger.Enter();

            try
            {
                // 削除設定を取得する（ファイル削除設定一覧のwrapperを取得する）
                var targetSettings = _settings.GetConfigs(CleanBlobSetting.KeyPrefix);
                IEnumerable<CleanBlobSetting> settings =
                    targetSettings.Select(x =>
                    {
                        try
                        {
                            return CleanBlobSetting.Create(x.Key, x.Value);
                        }
                        catch (RmsInvalidAppSettingException e)
                        {
                            _logger.Error(e, nameof(Resources.CO_BLC_BLC_002), new object[] { e.Message });
                            return null;
                        }
                    })
                    .Where(x => x != null);

                if (!settings.Any())
                {
                    _logger.Error(nameof(Resources.CO_BLC_BLC_002), new object[] { "settings is empty." });
                    return;
                }

                foreach (var setting in settings)
                {
                    DateTime threshold = CreateThreshold(setting);

                    // Sq1.1: 対象パス以下のファイル情報一覧を取得する
                    if (!GetDeletableDevices(setting, threshold, out IEnumerable<DtDeviceFile> deviceFiles))
                    {
                        continue;
                    }

                    foreach (var file in deviceFiles)
                    {
                        // Sq1.2: 一定期間より古いファイル情報を削除する
                        // Sq1.3: 一定期間より古いファイルを削除する
                        DeleteFile(file);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, nameof(Resources.CO_BLC_BLC_001));
            }
            finally
            {
                _logger.Leave();
            }
        }

        /// <summary>
        /// ファイル情報とファイルを削除する
        /// </summary>
        /// <param name="file">削除ファイル</param>
        /// <returns>true: 成功、false:失敗</returns>
        private bool DeleteFile(DtDeviceFile file)
        {
            DeleteFileStatus status = DeleteFileStatus.DeleteDtDeviceFile;

            try
            {
                // Sq1.2: 一定期間より古いファイル情報を削除する
                if (_dtDeviceFileRepository.DeleteDtDeviceFile(file.Sid) == null)
                {
                    // 削除対象を取得してから、削除するまでの間に、別口で削除された場合ここにくる。
                    // レアケースのため意図しないサーバーエラーとする。ただし何があったかは分かるように例外を入れておく。
                    _logger.Error(new RmsException(string.Format("削除対象のファイル情報が存在しません。(SID = {0})", file.Sid)), nameof(Resources.CO_BLC_BLC_001));
                    return false;
                }

                status = DeleteFileStatus.Delete;

                var archiveFile = ArchiveFile.From(file);

                // Sq1.3: 一定期間より古いファイルを削除する
                _primaryBlobRepository.Delete(archiveFile);

                _logger.Info(nameof(Resources.CO_BLC_BLC_007), new object[] { file.Container, file.FilePath, file.UpdateDatetime });
                return true;
            }
            catch (Exception e)
            {
                switch (status)
                {
                    case DeleteFileStatus.DeleteDtDeviceFile:
                        // ログ出力して次ファイルを処理
                        _logger.Warn(e, nameof(Resources.CO_BLC_BLC_005), new object[] { file.Container, file.FilePath, file.UpdateDatetime });
                        return false;
                    case DeleteFileStatus.Delete:
                    default:
                        // ログ出力して次ファイルを処理
                        // 削除に失敗した場合は、監視運用で処理する
                        _logger.Warn(e, nameof(Resources.CO_BLC_BLC_006), new object[] { file.Container, file.FilePath, file.UpdateDatetime });
                        return false;
                }
            }
        }

        /// <summary>
        /// 削除対象のデバイスファイルを取得する
        /// </summary>
        /// <param name="setting">BlobClean設定</param>
        /// <param name="threshold">閾値</param>
        /// <param name="deviceFiles">削除対象のデバイスファイル</param>
        /// <returns>true: 取得成功、false:取得エラー</returns>
        private bool GetDeletableDevices(CleanBlobSetting setting, DateTime threshold, out IEnumerable<DtDeviceFile> deviceFiles)
        {
            deviceFiles = null;

            try
            {
                deviceFiles = _dtDeviceFileRepository.FindByFilePathStartingWithAndUpdateDatetimeLessThan(setting.ContainerName, setting.FilePathPrefix, threshold);
                if (deviceFiles == null)
                {
                    // 処理自体は続行する。
                    _logger.Info(nameof(Resources.CO_BLC_BLC_004), new object[] { setting.ContainerName, setting.FilePathPrefix, threshold });
                    return false;
                }
            }
            catch (Exception ex)
            {
                // 処理自体は続行する。
                _logger.Error(ex, nameof(Resources.CO_BLC_BLC_003), new object[] { setting.ContainerName, setting.FilePathPrefix, threshold });
                return false;
            }

            return true;
        }

        /// <summary>
        /// 閾値を作成する
        /// </summary>
        /// <param name="setting">設定</param>
        /// <returns>閾値</returns>
        private DateTime CreateThreshold(CleanBlobSetting setting)
        {
            // 年月まででそれ以下は考慮外という仕様
            DateTime thresholdMonth = _timeProvider.UtcNow.AddMonths(-setting.RetentionPeriodMonth);
            return new DateTime(thresholdMonth.Year, thresholdMonth.Month, 1);
        }
    }
}
