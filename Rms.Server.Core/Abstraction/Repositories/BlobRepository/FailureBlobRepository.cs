using Microsoft.Extensions.Logging;
using Rms.Server.Core.Abstraction.Models;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Abstraction.Repositories.Blobs;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Core.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// FailureBlobRepository
    /// </summary>
    public class FailureBlobRepository : IFailureRepository
    {
        /// <summary>
        /// FailureBlobクライアント
        /// </summary>
        private readonly Blob _failureBlob;

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger _log;

        /// <summary>
        /// AppSettings
        /// </summary>
        private readonly AppSettings _settings;

        /// <summary>
        /// BlobPolly
        /// </summary>
        private readonly BlobPolly _polly;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">AppSettings</param>
        /// <param name="failureBlob">FailureBlob</param>
        /// <param name="polly">BlobPolly</param>
        /// <param name="logger">Logger</param>
        public FailureBlobRepository(
            AppSettings settings,
            FailureBlob failureBlob,
            BlobPolly polly,
            ILogger<FailureBlobRepository> logger)
        {
            Assert.IfNull(settings);
            Assert.IfNull(failureBlob);
            Assert.IfNull(logger);
            Assert.IfNull(polly);

            _settings = settings;
            _failureBlob = failureBlob;
            _log = logger;
            _polly = polly;
        }

        /// <summary>
        /// FailureBlobにファイルをアップロードする
        /// </summary>
        /// <param name="file">アップロードするファイルの情報</param>
        /// <param name="contents">アップロードするファイルの内容</param>
        /// <param name="withSerialNumber">ファイル名末尾に連番を付与する場合はtrue、そうでない場合はfalseを指定する。</param>
        public void Upload(ArchiveFile file, string contents, bool withSerialNumber = false)
        {
            _log.EnterJson("{0}", new { file, contents, withSerialNumber });

            try
            {
                using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(contents)))
                {
                    if (withSerialNumber)
                    {
                        string filepathWithoutExtension = file.FilePath.Substring(0, file.FilePath.Length - Path.GetExtension(file.FilePath).Length);

                        for (long i = 1; i < int.MaxValue; i++)
                        {
                            try
                            {
                                file.FilePath = filepathWithoutExtension + "_" + i + Path.GetExtension(file.FilePath);
                                _failureBlob.Upload(file, stream, false);
                                break;
                            }
                            catch (RmsAlreadyExistException)
                            {
                            }
                        }
                    }
                    else
                    {
                        _failureBlob.Upload(file, stream);
                    }
                }
            }
            catch (Exception ex)
            {
                string path = string.Format("{0}/{1}", file.ContainerName, file.FilePath);
                throw new RmsException(string.Format("Blobのアップロードに失敗しました（upload to {0}）", path), ex);
            }
            finally
            {
                _log.LeaveJson("{0}", new { file });
            }
        }
    }
}
