using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Operation.Service.Services;
using Rms.Server.Operation.Utility;
using Rms.Server.Operation.Utility.Extensions;
using Rms.Server.Operation.Utility.Models;
using Rms.Server.Operation.Utility.Properties;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Rms.Server.Operation.Azure.Functions.MailSender
{
    /// <summary>
    /// MailSender
    /// </summary>
    public class MailSenderController
    {
        /// <summary>
        /// 設定
        /// </summary>
        private readonly OperationAppSettings _settings;

        /// <summary>
        /// MailSenderサービスクラス
        /// </summary>
        private readonly IMailSenderService _service;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">設定</param>
        /// <param name="service">MailSenderサービスクラス</param>
        public MailSenderController(OperationAppSettings settings, IMailSenderService service)
        {
            Assert.IfNull(settings);
            Assert.IfNull(service);

            _settings = settings;
            _service = service;
        }

        /// <summary>
        /// メール情報をキューから取得しメールの送信を行う
        /// </summary>
        /// <remarks>sd 05-2.メール送信</remarks>
        /// <param name="queueItem">メール情報</param>
        /// <param name="log">ロガー</param>
        /// <returns>メール送信結果</returns>
        [FunctionName("MailSender")]
        public async Task DequeueMailInfo([QueueTrigger("mail", Connection = "ConnectionString")]string queueItem, ILogger log)
        {
            log.EnterJson("{0}", queueItem);

            try
            {
                // アプリケーション設定でメール本文フォーマットが設定されていない場合、実行時にエラーとする
                if (string.IsNullOrEmpty(_settings.MailTextFormat))
                {
                    throw new RmsInvalidAppSettingException($"{nameof(_settings.MailTextFormat)} is required.");
                }

                // Sq1.1.1: メール送信データを生成する
                MailInfo mailInfo = JsonConvert.DeserializeObject<MailInfo>(queueItem);

                // バリデーション
                Validator.ValidateObject(mailInfo, new ValidationContext(mailInfo, null, null));

                // Sq1.1.2: メール送信を依頼する
                if (await _service.SendMail(mailInfo))
                {
                    // メール送信依頼完了
                    log.Info(nameof(Resources.OP_MLS_MLS_003));
                }
                else
                {
                    // 失敗した場合はFailureストレージに書き込み
                    _service.UpdateToFailureStorage(queueItem);
                }
            }
            catch (ValidationException e)
            {
                // キューフォーマット異常
                log.Error(e, nameof(Resources.OP_MLS_MLS_005), new object[] { e.Message });

                // 失敗した場合はFailureストレージに書き込み
                _service.UpdateToFailureStorage(queueItem);
            }
            catch (RmsInvalidAppSettingException e)
            {
                log.Error(e, nameof(Resources.OP_MLS_MLS_006), new object[] { e.Message });

                // 失敗した場合はFailureストレージに書き込み
                _service.UpdateToFailureStorage(queueItem);
            }
            catch (Exception e)
            {
                log.Error(e, nameof(Resources.OP_MLS_MLS_001));

                // 失敗した場合はFailureストレージに書き込み
                _service.UpdateToFailureStorage(queueItem);
            }
            finally
            {
                log.Leave();
            }
        }
    }
}
