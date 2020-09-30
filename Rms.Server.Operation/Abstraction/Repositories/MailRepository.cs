using Microsoft.Extensions.Logging;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Operation.Abstraction.Pollies;
using Rms.Server.Operation.Utility;
using Rms.Server.Operation.Utility.Extensions;
using Rms.Server.Operation.Utility.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Rms.Server.Operation.Abstraction.Repositories
{
    /// <summary>
    /// メールリポジトリ
    /// </summary>
    public class MailRepository : IMailRepository
    {
        /// <summary>ロガー</summary>
        private readonly ILogger<MailRepository> _logger;

        /// <summary>SendGrid接続用のPolly</summary>
        private readonly SendGridPolly _sendGridPolly;

        /// <summary>アプリケーション設定</summary>
        private readonly OperationAppSettings _appSettings;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="sendGridPolly">SendGrid接続用のPolly</param>
        /// <param name="appSettings">アプリケーション設定</param>
        public MailRepository(ILogger<MailRepository> logger, SendGridPolly sendGridPolly, OperationAppSettings appSettings)
        {
            Assert.IfNull(logger);
            Assert.IfNull(sendGridPolly);
            Assert.IfNull(appSettings);

            _logger = logger;
            _sendGridPolly = sendGridPolly;
            _appSettings = appSettings;
        }

        /// <summary>
        /// メールを送信する
        /// </summary>
        /// <param name="mailInfo">メール情報</param>
        /// <returns>メール送信結果（HTTPステータスコードとレスポンスボディ）</returns>
        public async Task<KeyValuePair<HttpStatusCode, string>> SendMail(MailInfo mailInfo)
        {
            _logger.EnterJson("{0}", new { mailInfo });

            int code = 0;
            string body = null;

            try
            {
                var client = new SendGridClient(_appSettings.SendGridApiKey);

                var from = new EmailAddress(mailInfo.MailAddressFrom);

                var tos = new List<EmailAddress>();
                foreach (var address in mailInfo.MailAddressTo.Split(","))
                {
                    tos.Add(new EmailAddress(address));
                }

                string mailText = string.Format(
                    _appSettings.MailTextFormat,
                    mailInfo.CustomerNumber.ToString(),
                    mailInfo.CustomerName,
                    mailInfo.EquipmentSerialNumber,
                    mailInfo.EquipmentNumber,
                    mailInfo.EquipmentName,
                    mailInfo.TypeCode,
                    mailInfo.ErrorCode,
                    mailInfo.AlarmLevel.ToString(),
                    mailInfo.EventDate,
                    mailInfo.AlarmDescription);

                var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, mailInfo.MailSubject, mailText, string.Empty, true);

                Response response = null;
                await _sendGridPolly.ExecuteAsync(async () =>
                {
                    response = await client.SendEmailAsync(msg);
                    return response;
                });

                code = (int)response.StatusCode;
                body = await response.Body?.ReadAsStringAsync();

                if (code < 200 || code > 299)
                {
                    throw new RmsException(string.Format("status error. (StatusCode = {0})", code));
                }

                return new KeyValuePair<HttpStatusCode, string>(response.StatusCode, body);
            }
            catch (Exception e)
            {
                throw new RmsException(string.Format("メール送信の要求に失敗しました。(StatusCode = {0}, ResponseBody = {1})", code, body), e);
            }
            finally
            {
                _logger.LeaveJson("{0}", new { code, body });
            }
        }
    }
}
