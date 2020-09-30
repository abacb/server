using Microsoft.Extensions.Logging;
using Rms.Server.Core.Abstraction.Models;
using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Core.Utility.Extensions;
using Rms.Server.Core.Utility.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Rms.Server.Core.Service.Services
{
    /// <summary>
    /// BlobIndexer
    /// </summary>
    public class IndexBlobService : IIndexBlobService
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
        /// CollectingRepository
        /// </summary>
        private readonly ICollectingRepository _collectingRepository;

        /// <summary>
        /// PrimaryRepository
        /// </summary>
        private readonly IPrimaryRepository _primaryRepository;

        /// <summary>
        /// IDtDeviceFileRepository
        /// </summary>
        private readonly IDtDeviceFileRepository _dtDeviceFileRepository;

        /// <summary>
        /// IDtDeviceRepository
        /// </summary>
        private readonly IDtDeviceRepository _dtDeviceRepository;

        /// <summary>
        /// TimeProvider
        /// </summary>
        private readonly ITimeProvider _timeProvider;

        /// <summary>
        /// IndexBlobService
        /// </summary>
        /// <param name="settings">アプリケーション設定</param>
        /// <param name="logger">ロガー</param>
        /// <param name="collectingRepository">ICollectingRepository</param>
        /// <param name="primaryRepository">IPrimaryRepository</param>
        /// <param name="dtDeviceFileRepository">IDtDeviceFileRepository</param>
        /// <param name="dtDeviceRepository">IDtDeviceRepository</param>
        /// <param name="timeProvider">ITimeProvider</param>
        public IndexBlobService(
            AppSettings settings,
            ILogger<IndexBlobService> logger,
            ICollectingRepository collectingRepository,
            IPrimaryRepository primaryRepository,
            IDtDeviceFileRepository dtDeviceFileRepository,
            IDtDeviceRepository dtDeviceRepository,
            ITimeProvider timeProvider)
        {
            Assert.IfNull(settings);
            Assert.IfNull(logger);
            Assert.IfNull(collectingRepository);
            Assert.IfNull(primaryRepository);
            Assert.IfNull(dtDeviceFileRepository);
            Assert.IfNull(dtDeviceRepository);
            Assert.IfNull(timeProvider);

            _settings = settings;
            _logger = logger;
            _collectingRepository = collectingRepository;
            _primaryRepository = primaryRepository;
            _dtDeviceFileRepository = dtDeviceFileRepository;
            _dtDeviceRepository = dtDeviceRepository;
            _timeProvider = timeProvider;
        }

        /// <summary>
        /// Indexメソッド内の処理ステータス
        /// </summary>
        private enum IndexStatus
        {
            /// <summary>
            /// Copy呼び出し中
            /// </summary>
            Copy = 0,

            /// <summary>
            /// Delete呼び出し中
            /// </summary>
            Delete,

            /// <summary>
            /// CopyToPrimary呼び出し中
            /// </summary>
            CopyToPrimary,
        }

        /// <summary>
        /// CollectingBlobに配置されたファイル群をPrimaryBlobに適切に配置する。
        /// </summary>
        /// <remarks>sd 02-7.BLOBインデックス作成</remarks>
        public void Index()
        {
            _logger.Enter();

            try
            {
                IEnumerable<CollectedFile> collectedFiles;
                try
                {
                    // ファイル一覧を取得する
                    var directory = new ArchiveDirectory()
                    {
                        ContainerName = _settings.CollectingBlobContainerNameCollect
                    };

                    // Sq2.1: ファイルを取得する
                    collectedFiles =
                        _collectingRepository.GetArchiveFiles(directory)
                        .Select(x => new CollectedFile(x, _settings, _timeProvider));
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, nameof(Resources.CO_BLI_BLI_003), new object[] { _settings.CollectingBlobContainerNameCollect });
                    return;
                }

                foreach (CollectedFile collectFile in collectedFiles)
                {
                    IndexStatus status = IndexStatus.Copy;

                    try
                    {
                        // メタデータが想定と異なる場合
                        // file作成時刻が既定年月日以上(一定時間以上処理されないケース)
                        if (!collectFile.IsValid())
                        {
                            // Sq2.2: 異常ファイルの集積コンテナにファイルを保存する
                            _collectingRepository.Copy(collectFile.SourceArchiveFile, collectFile.MakeUnknownFile());

                            // Sq2.3: ファイルを削除する（CollectingBlobのファイルを削除する。）
                            status = IndexStatus.Delete;
                            _collectingRepository.Delete(collectFile.SourceArchiveFile);
                            continue;
                        }

                        // メタデータからfileのコピー先を生成する。
                        var fileOnPrimaryBlob = collectFile.MakeClassifiedFile();

                        // Sq2.4: ファイルを保存する
                        status = IndexStatus.CopyToPrimary;
                        _collectingRepository.CopyToPrimary(collectFile.SourceArchiveFile, fileOnPrimaryBlob);

                        // Sq2.5: ファイル情報を保存する
                        if (!TryAddDtDeviceFile(collectFile, fileOnPrimaryBlob))
                        {
                            // Sq2.6: ファイルを削除する（PrimaryBlobのファイルを削除）
                            TryDeleteFromCoreMain(fileOnPrimaryBlob);
                            continue;
                        }

                        // Sq2.7: ファイルを削除する（ファイルの移動を完了したとしてCollectingBlobのファイルを削除する。）
                        status = IndexStatus.Delete;
                        _collectingRepository.Delete(collectFile.SourceArchiveFile);

                        _logger.Info(nameof(Resources.CO_BLI_BLI_009), new object[] { collectFile.SourceArchiveFile.ContainerName, collectFile.SourceArchiveFile.FilePath });
                    }
                    catch (Exception ex)
                    {
                        switch (status)
                        {
                            case IndexStatus.Delete:
                                _logger.Error(ex, nameof(Resources.CO_BLI_BLI_005), new object[] { collectFile.SourceArchiveFile.ContainerName, collectFile.SourceArchiveFile.FilePath });
                                break;
                            case IndexStatus.CopyToPrimary:
                                _logger.Error(ex, nameof(Resources.CO_BLI_BLI_006), new object[] { collectFile.SourceArchiveFile.ContainerName, collectFile.SourceArchiveFile.FilePath });
                                break;
                            case IndexStatus.Copy:
                            default:
                                _logger.Error(ex, nameof(Resources.CO_BLI_BLI_004), new object[] { collectFile.SourceArchiveFile.ContainerName, collectFile.SourceArchiveFile.FilePath });
                                break;
                        }
                    }
                }
            }
            finally
            {
                _logger.Leave();
            }
        }

        /// <summary>
        /// DtDeviceFileにレコードを追加する。
        /// </summary>
        /// <param name="collectFile">CollectingBlob上の収集ファイル</param>
        /// <param name="fileOnPrimaryBlob">PrimaryBlob上の収集ファイル</param>
        /// <returns>true: 成功 false: 失敗</returns>
        private bool TryAddDtDeviceFile(CollectedFile collectFile, ArchiveFile fileOnPrimaryBlob)
        {
            try
            {
                // DBにファイル情報を保存する(ファイルURL、メタデータ)
                // まずsys_ownerから、端末情報を取得する。
                // ・この値は機器UIDかエッジIDを使用する。
                var device = _dtDeviceRepository.ReadDtDevice(collectFile.Owner);
                if (device == null)
                {
                    throw new Exception(string.Format("Device[{0}]が見つかりませんでした。", collectFile.Owner));
                }

                // 端末ファイルデータを作る
                var dtDeviceFile = new Utility.Models.Entites.DtDeviceFile()
                {
                    DeviceSid = device.Sid,
                    //// DB上は発生元機器UIDだが、端末のUIDとは限らないので、ここではOwnerの値を設定する。
                    SourceEquipmentUid = collectFile.Owner,
                    Container = fileOnPrimaryBlob.ContainerName,
                    FilePath = fileOnPrimaryBlob.FilePath,
                    CollectDatetime = collectFile.CreatedAt,
                    DtDeviceFileAttribute = fileOnPrimaryBlob.MetaData.Select(ToDtDeviceFileAttribute).ToArray()
                };

                _dtDeviceFileRepository.CreateOrUpdateDtDeviceFile(dtDeviceFile);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, nameof(Resources.CO_BLI_BLI_007), new object[] { fileOnPrimaryBlob.ContainerName, fileOnPrimaryBlob.FilePath });
                return false;
            }
        }

        /// <summary>
        /// PrimaryBlobからのファイル削除処理を行う。
        /// </summary>
        /// <param name="archiveFile">削除対象ファイル</param>
        /// <returns>true: 削除成功 false: 削除失敗</returns>
        private bool TryDeleteFromCoreMain(ArchiveFile archiveFile)
        {
            try
            {
                _primaryRepository.Delete(archiveFile);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, nameof(Resources.CO_BLI_BLI_008), new object[] { archiveFile.ContainerName, archiveFile.FilePath });
                return false;
            }
        }

        /// <summary>
        /// KeyValue値をDtDeviceFileAttributeに変換する。
        /// </summary>
        /// <param name="pair">Key: メタデータのキー Value: メタデータの値</param>
        /// <returns>DtDeviceFileAttribute</returns>
        private Utility.Models.Entites.DtDeviceFileAttribute ToDtDeviceFileAttribute(KeyValuePair<string, string> pair)
        {
            return new Utility.Models.Entites.DtDeviceFileAttribute()
            {
                Name = pair.Key,
                Value = Uri.UnescapeDataString(pair.Value)  // DBに格納する前にメタデータはアンエスケープする
            };
        }

        /// <summary>
        /// 収集ファイル
        /// </summary>
        public class CollectedFile
        {
            /// <summary>
            /// 設定
            /// </summary>
            private readonly AppSettings _settings;

            /// <summary>
            /// TimeProvider
            /// </summary>
            private readonly ITimeProvider _timeProvider;

            /// <summary>
            /// 異常ファイルパス
            /// </summary>
            private readonly string formatUnknownFilePath = "{0}/{1}";

            /// <summary>
            /// 収集ファイル
            /// </summary>
            /// <param name="file">ファイル情報</param>
            /// <param name="settings">設定</param>
            /// <param name="timeProvider">TimeProvider</param>
            public CollectedFile(
                ArchiveFile file,
                AppSettings settings,
                ITimeProvider timeProvider)
            {
                SourceArchiveFile = file;
                _settings = settings;
                _timeProvider = timeProvider;

                SetPropertiesFromMetadata(file);
            }

            /// <summary>
            /// 元ファイル
            /// </summary>
            public ArchiveFile SourceArchiveFile { get; private set; }

            /// <summary>
            /// コンテナ名
            /// </summary>
            public string ContainerName { get; private set; }

            /// <summary>
            /// オーナー
            /// </summary>
            public string Owner { get; private set; }

            /// <summary>
            /// 作成時間
            /// </summary>
            public DateTime CreatedAt { get; private set; }

            /// <summary>
            /// サブディレクトリ
            /// </summary>
            public string SubDirectory { get; private set; }

            /// <summary>
            /// 元ファイルが正常かどうか
            /// </summary>
            /// <returns>true:正常 false:異常</returns>
            public bool IsValid()
            {
                // 異常ファイル判定にはメタデータから取得した時刻情報ではなく
                // BlobのPropertiesが持つファイル生成時刻情報を使う
                DateTime createAt = SourceArchiveFile.CreatedAt;

                if (string.IsNullOrWhiteSpace(ContainerName) ||
                    string.IsNullOrWhiteSpace(Owner) ||
                    createAt == default(DateTime))
                {
                    return false;
                }

                if (HasMoreThanDays(createAt))
                {
                    return false;
                }

                return true;
            }

            /// <summary>
            /// 分類済みファイル情報の作成
            /// </summary>
            /// <returns>分類済みファイル情報</returns>
            public ArchiveFile MakeClassifiedFile()
            {
                string classifiedFilePath = MakeClassifiedFilePath();
                var classifiedFile = new ArchiveFile()
                {
                    ContainerName = ContainerName,
                    FilePath = classifiedFilePath,
                    CreatedAt = CreatedAt.ToUniversalTime()
                };

                // メタデータ設定
                classifiedFile.SetMetaData(SourceArchiveFile.MetaData);
                return classifiedFile;
            }

            /// <summary>
            /// 異常ファイルクラスオブジェクトを作成する
            /// </summary>
            /// <returns>異常ファイルクラスオブジェクト</returns>
            public ArchiveFile MakeUnknownFile()
            {
                // 異常ファイル集積コンテナ用のファイルを作る
                ArchiveFile unknownFile = SourceArchiveFile.Copy();

                unknownFile.ContainerName = _settings.CollectingBlobContainerNameUnknown;
                var utcnow = _timeProvider.UtcNow;
                unknownFile.FilePath =
                    string.Format(
                        formatUnknownFilePath,
                        utcnow.ToString("yyyyMMddHHmmssfff"),
                        unknownFile.FilePath);
                unknownFile.CreatedAt = utcnow;
                return unknownFile;
            }

            /// <summary>
            /// ArchiveFileのメタデータから、プロパティの各値を設定する。
            /// </summary>
            /// <param name="file">ファイル情報</param>
            private void SetPropertiesFromMetadata(ArchiveFile file)
            {
                if (file.MetaData.TryGetValue("sys_container", out var containerName))
                {
                    ContainerName = containerName;
                }

                if (file.MetaData.TryGetValue("sys_owner", out var owner))
                {
                    Owner = owner;
                }

                if (file.MetaData.TryGetValue("sys_file_datetime", out string createdAtStr) &&
                    DateTime.TryParse(createdAtStr, out DateTime createdAt))
                {
                    CreatedAt = createdAt.ToUniversalTime();
                }

                if (file.MetaData.TryGetValue("sys_sub_directory", out var subDirectory))
                {
                    SubDirectory = subDirectory;
                }
            }

            /// <summary>
            /// ファイル作成後規定日数を超過しているかどうか
            /// </summary>
            /// <param name="createdAt">作成日時</param>
            /// <returns>true: 日数を超過している</returns>
            private bool HasMoreThanDays(DateTime createdAt)
            {
                return createdAt.AddDays(_settings.BlobIndexerKeepFileDays) < _timeProvider.UtcNow;
            }

            /// <summary>
            /// 分類済みファイルパスを作成する
            /// </summary>
            /// <returns>分類済みファイルパス</returns>
            private string MakeClassifiedFilePath()
            {
                var excludPath = $"{ContainerName}/";
                return Regex.Replace(SourceArchiveFile.FilePath, $"^{Regex.Escape(excludPath)}", string.Empty);
            }
        }
    }
}
