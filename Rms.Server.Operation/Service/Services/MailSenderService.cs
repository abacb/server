using Microsoft.Extensions.Logging;
using Rms.Server.Core.Abstraction.Models;
using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Operation.Abstraction.Repositories;
using Rms.Server.Operation.Utility;
using Rms.Server.Operation.Utility.Extensions;
using Rms.Server.Operation.Utility.Models;
using Rms.Server.Operation.Utility.Properties;
using System;
using System.Threading.Tasks;

namespace Rms.Server.Operation.Service.Services
{
    /// <summary>
    /// MailSender Service
    /// </summary>
    public class MailSenderService : IMailSenderService
    {
        /// <summary>
        /// 設定
        /// </summary>
        private readonly OperationAppSettings _settings;

        /// <summary>
        /// ロガー
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// DateTimeの提供元
        /// </summary>
        private readonly ITimeProvider _timeProvider;

        /// <summary>
        /// メールリポジトリ
        /// </summary>
        private readonly IMailRepository _mailRepository;

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
        /// <param name="mailRepository">メールリポジトリ</param>
        /// <param name="failureRepository">Failureストレージリポジトリ</param>
        public MailSenderService(
            OperationAppSettings settings,
            ILogger<MailSenderService> logger,
            ITimeProvider timeProvider,
            IMailRepository mailRepository,
            IFailureRepository failureRepository)
        {
            Assert.IfNull(settings);
            Assert.IfNull(logger);
            Assert.IfNull(timeProvider);
            Assert.IfNull(mailRepository);
            Assert.IfNull(failureRepository);

            _settings = settings;
            _logger = logger;
            _timeProvider = timeProvider;
            _mailRepository = mailRepository;
            _failureRepository = failureRepository;
        }

        /// <summary>
        /// メールを送信する
        /// </summary>
        /// <param name="mailInfo">メール情報</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        public async Task<bool> SendMail(MailInfo mailInfo)
        {
            _logger.EnterJson("{0}", new { mailInfo });

            try
            {
                // Sq1.1.2: メール送信を依頼する
                await _mailRepository.SendMail(mailInfo);

                return true;
            }
            catch (RmsException e)
            {
                // メール送信依頼失敗（基本設計書 5.4.4 エラー処理）
                _logger.Error(e, nameof(Resources.OP_MLS_MLS_002));
                return false;
            }
            finally
            {
                _logger.Leave();
            }
        }

        /// <summary>
        /// Failureストレージに再送用メッセージをアップロードする
        /// </summary>
        /// <param name="message">メッセージ</param>
        public void UpdateToFailureStorage(string message)
        {
            try
            {
                _logger.EnterJson("{0}", new { message });

                DateTime now = _timeProvider.UtcNow;

                // ファイル情報
                ArchiveFile file = new ArchiveFile() { ContainerName = _settings.FailureBlobContainerName, CreatedAt = now };
                file.FilePath = string.Format(Utility.Const.FailureBlobFilenameFormat, now);

                // アップロード
                _failureRepository.Upload(file, message);
            }
            catch (RmsException e)
            {
                // Blobストレージへの保存処理に失敗した場合、メッセージ内容をログに出力して終了する。
                _logger.Error(e, nameof(Resources.OP_MLS_MLS_004), new object[] { message });
            }
            finally
            {
                _logger.Leave();
            }
        }
    }
}
