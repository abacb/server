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

namespace Rms.Server.Operation.Azure.Functions.AlarmRegister
{
    /// <summary>
    /// AlarmRegister
    /// </summary>
    public class AlarmRegisterController
    {
        /// <summary>
        /// 設定
        /// </summary>
        private readonly OperationAppSettings _settings;

        /// <summary>
        /// AlarmRegisterサービスクラス
        /// </summary>
        private readonly IAlarmRegisterService _service;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">設定</param>
        /// <param name="service">AlarmRegisterサービスクラス</param>
        public AlarmRegisterController(OperationAppSettings settings, IAlarmRegisterService service)
        {
            Assert.IfNull(settings);
            Assert.IfNull(service);

            _settings = settings;
            _service = service;
        }

        /// <summary>
        /// アラーム情報をキューから取得しDBへの登録およびメールキューへの追加を行う
        /// </summary>
        /// <remarks>sd 05-1.アラーム登録</remarks>
        /// <param name="queueItem">アラーム情報</param>
        /// <param name="log">ロガー</param>
        [FunctionName("AlarmRegister")]
        public void DequeueAlarmInfo([QueueTrigger("alarm", Connection = "ConnectionString")]string queueItem, ILogger log)
        {
            log.EnterJson("{0}", queueItem);

            string messageId = null;
            try
            {
                // アプリケーション設定で送信元メールアドレスが設定されていない場合、実行時にエラーとする
                if (string.IsNullOrEmpty(_settings.AlarmMailAddressFrom))
                {
                    throw new RmsInvalidAppSettingException($"{nameof(_settings.AlarmMailAddressFrom)} is required.");
                }

                AlarmInfo alarmInfo = JsonConvert.DeserializeObject<AlarmInfo>(queueItem);

                messageId = alarmInfo?.MessageId;

                // バリデーション
                Validator.ValidateObject(alarmInfo, new ValidationContext(alarmInfo, null, null));

                // 同一メッセージに対するアラームが再度アラームキューへ追加される可能性があるため、アラームデータをDBに登録する際にメッセージIDによる登録確認を行う。
                bool? existSameMessageIdAlarm = _service.ExistsSameMessageIdAlarm(messageId);
                if (existSameMessageIdAlarm == true)
                {
                    // 同一メッセージIDのアラームが既にDBに登録済み
                    log.Info(nameof(Resources.OP_ALR_ALR_004), new object[] { messageId });
                    return;
                }
                else if (existSameMessageIdAlarm == false)
                {
                    // Sq1.1.2: アラーム設定を取得する
                    // Sq1.1.3: メール送信先を取得する
                    if (_service.ReadDtAlarmConfig(alarmInfo, out DtEquipment equipment, out DtAlarmConfig alarmConfig))
                    {
                        // Sq1.1.4: メール間隔日数を確認する
                        if (_service.NeedsMailSending(messageId, alarmConfig, alarmInfo.AlarmDefId, out bool needMail))
                        {
                            bool sentMail = false;

                            if (needMail)
                            {
                                // Sq1.1.5: メールキューを登録する
                                sentMail = _service.CreateAndEnqueueAlarmInfo(messageId, alarmInfo, alarmConfig, equipment);
                                if (sentMail)
                                {
                                    // メールキュー登録完了
                                    log.Info(nameof(Resources.OP_ALR_ALR_008), new object[] { messageId });
                                }
                            }

                            if (!needMail || sentMail)
                            {
                                // Sq1.1.1: アラームデータを保存する
                                // エラーとなった場合はログの情報を基に手動でアラームデータを保存する
                                if (_service.RegistAlarmInfo(messageId, needMail, alarmInfo, equipment))
                                {
                                    // アラームデータ保存完了
                                    log.Info(nameof(Resources.OP_ALR_ALR_010), new object[] { messageId });
                                    return;
                                }
                            }
                        }
                    }
                }

                // 失敗した場合はFailureストレージに書き込み
                _service.UpdateToFailureStorage(messageId, queueItem);
            }
            catch (Exception e) when (e is ValidationException || e is Newtonsoft.Json.JsonSerializationException || e is Newtonsoft.Json.JsonReaderException)
            {
                // キューフォーマット異常
                log.Error(e, nameof(Resources.OP_ALR_ALR_002), new object[] { messageId, e.Message });

                // 失敗した場合はFailureストレージに書き込み
                _service.UpdateToFailureStorage(messageId, queueItem);
            }
            catch (RmsInvalidAppSettingException e)
            {
                log.Error(e, nameof(Resources.OP_ALR_ALR_012), new object[] { e.Message });

                // 失敗した場合はFailureストレージに書き込み
                _service.UpdateToFailureStorage(messageId, queueItem);
            }
            catch (Exception e)
            {
                log.Error(e, nameof(Resources.OP_ALR_ALR_001), new object[] { messageId });

                // 失敗した場合はFailureストレージに書き込み
                _service.UpdateToFailureStorage(messageId, queueItem);
            }
            finally
            {
                log.Leave();
            }
        }
    }
}
