using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
using System.Globalization;

namespace Rms.Server.Operation.Service.Services
{
    /// <summary>
    /// AlarmRegister Service
    /// </summary>
    public class AlarmRegisterService : IAlarmRegisterService
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
        /// アラームリポジトリ
        /// </summary>
        private readonly IDtAlarmRepository _dtAlarmRepository;

        /// <summary>
        /// 機器リポジトリ
        /// </summary>
        private readonly IDtEquipmentRepository _dtEquipmentRepository;

        /// <summary>
        /// アラーム設定リポジトリ
        /// </summary>
        private readonly IDtAlarmConfigRepository _dtAlarmConfigRepository;

        /// <summary>
        /// Queueリポジトリ
        /// </summary>
        private readonly IQueueRepository _queueRepository;

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
        /// <param name="dtAlarmRepository">アラームリポジトリ</param>
        /// <param name="dtEquipmentRepository">機器リポジトリ</param>
        /// <param name="dtAlarmConfigRepository">アラーム設定リポジトリ</param>
        /// <param name="queueRepository">Queueリポジトリ</param>
        /// <param name="failureRepository">Failureストレージリポジトリ</param>
        public AlarmRegisterService(
            OperationAppSettings settings,
            ILogger<AlarmRegisterService> logger,
            ITimeProvider timeProvider,
            IDtAlarmRepository dtAlarmRepository,
            IDtEquipmentRepository dtEquipmentRepository,
            IDtAlarmConfigRepository dtAlarmConfigRepository,
            IQueueRepository queueRepository,
            IFailureRepository failureRepository)
        {
            Assert.IfNull(settings);
            Assert.IfNull(logger);
            Assert.IfNull(timeProvider);
            Assert.IfNull(dtAlarmRepository);
            Assert.IfNull(dtEquipmentRepository);
            Assert.IfNull(dtAlarmConfigRepository);
            Assert.IfNull(queueRepository);
            Assert.IfNull(failureRepository);

            _settings = settings;
            _logger = logger;
            _timeProvider = timeProvider;
            _dtAlarmRepository = dtAlarmRepository;
            _dtEquipmentRepository = dtEquipmentRepository;
            _dtAlarmConfigRepository = dtAlarmConfigRepository;
            _queueRepository = queueRepository;
            _failureRepository = failureRepository;
        }

        /// <summary>
        /// 同一メッセージIDのアラームが登録されているか確認する
        /// </summary>
        /// <param name="messageId">メッセージID</param>
        /// <returns>アラームが未登録の場合falseを、アラームが登録済みの場合trueを、処理に失敗した場合nullを返す。</returns>
        public bool? ExistsSameMessageIdAlarm(string messageId)
        {
            bool? result = null;

            try
            {
                _logger.EnterJson("{0}", new { messageId });

                if (string.IsNullOrEmpty(messageId))
                {
                    // メッセージIDが設定されていない場合(DB監視等)は未登録を返す
                    result = false;
                }
                else
                {
                    result = _dtAlarmRepository.ExistDtAlarm(messageId);
                }

                return result;
            }
            catch (RmsException e)
            {
                // アラーム設定取得失敗（基本設計書 5.3.4 エラー処理）
                _logger.Error(e, nameof(Resources.OP_ALR_ALR_003), new object[] { messageId });
                result = null;
                return result;
            }
            finally
            {
                _logger.LeaveJson("{0}", new { result });
            }
        }

        /// <summary>
        /// アラーム設定を取得する
        /// </summary>
        /// <param name="alarmInfo">アラーム情報</param>
        /// <param name="equipment">機器データ</param>
        /// <param name="alarmConfig">アラーム設定</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        public bool ReadDtAlarmConfig(AlarmInfo alarmInfo, out DtEquipment equipment, out DtAlarmConfig alarmConfig)
        {
            equipment = null;
            alarmConfig = null;

            try
            {
                _logger.EnterJson("{0}", alarmInfo);

                // Sq1.1.2: アラーム設定を取得する
                // Sq1.1.3: メール送信先を取得する
                equipment = _dtEquipmentRepository.ReadDtEquipment(alarmInfo.SourceEquipmentUid, false);
                alarmConfig = _dtAlarmConfigRepository.ReadDtAlarmConfig(alarmInfo.AlarmLevel, false);

                return true;
            }
            catch (RmsException e)
            {
                // アラーム設定取得失敗（基本設計書 5.3.4 エラー処理）
                _logger.Error(e, nameof(Resources.OP_ALR_ALR_005), new object[] { alarmInfo?.MessageId });
                return false;
            }
            finally
            {
                _logger.LeaveJson("{0}", new { equipment, alarmConfig });
            }
        }

        /// <summary>
        /// メール送信が必要か判定する
        /// </summary>
        /// <param name="messageId">メッセージID</param>
        /// <param name="alarmConfig">アラーム設定</param>
        /// <param name="alarmDefId">アラーム定義ID</param>
        /// <param name="needMail">メール送信が必要な場合true、不要な場合false</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        public bool NeedsMailSending(string messageId, DtAlarmConfig alarmConfig, string alarmDefId, out bool needMail)
        {
            needMail = false;

            try
            {
                _logger.EnterJson("{0}", new { messageId, alarmConfig, alarmDefId });

                if (string.IsNullOrEmpty(alarmConfig.MailAddress))
                {
                    // メール送信先が設定されていない
                }
                else
                {
                    // メール送信先がある場合

                    // アラームデータから同一アラーム定義IDで送信済みの最新データを取得
                    var latestMailSentAlarm = _dtAlarmRepository.ReadLatestMailSentDtAlarm(alarmDefId);

                    if (latestMailSentAlarm == null)
                    {
                        // 同一アラーム定義IDで送信済みのデータが存在しない
                        needMail = true;
                    }
                    else if (alarmConfig.MailSendingInterval == null || alarmConfig.MailSendingInterval == 0)
                    {
                        // メール送信間隔が設定されていない
                        needMail = true;
                    }
                    else
                    {
                        // Sq1.1.4: メール間隔日数を確認する
                        var nextAlarmSendingDay = latestMailSentAlarm.CreateDatetime.AddDays((double)alarmConfig.MailSendingInterval);

                        if (_timeProvider.UtcNow > nextAlarmSendingDay)
                        {
                            // メール送信間隔日数が経過している場合
                            needMail = true;
                        }
                    }
                }

                return true;
            }
            catch (RmsException e)
            {
                // アラーム設定取得失敗（基本設計書 5.3.4 エラー処理）
                _logger.Error(e, nameof(Resources.OP_ALR_ALR_006), new object[] { messageId });
                needMail = false;
                return false;
            }
            finally
            {
                _logger.LeaveJson("{0}", new { needMail });
            }
        }

        /// <summary>
        /// アラームをDBに登録する
        /// </summary>
        /// <param name="messageId">メッセージID</param>
        /// <param name="hasMail">メールありフラグ</param>
        /// <param name="alarmInfo">アラーム情報</param>
        /// <param name="equipment">機器データ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        public bool RegistAlarmInfo(string messageId, bool hasMail, AlarmInfo alarmInfo, DtEquipment equipment)
        {
            try
            {
                _logger.EnterJson("{0}", new { messageId, hasMail, alarmInfo, equipment });

                var dtAlarm = new DtAlarm
                {
                    EquipmentSid = equipment.Sid,
                    TypeCode = alarmInfo.TypeCode,
                    ErrorCode = alarmInfo.ErrorCode,
                    AlarmLevel = alarmInfo.AlarmLevel,
                    AlarmTitle = alarmInfo.AlarmTitle,
                    AlarmDescription = alarmInfo.AlarmDescription,
                    AlarmDatetime = DateTime.Parse(alarmInfo.AlarmDatetime, null, DateTimeStyles.RoundtripKind),
                    AlarmDefId = alarmInfo.AlarmDefId,
                    EventDatetime = DateTime.Parse(alarmInfo.EventDatetime, null, DateTimeStyles.RoundtripKind),
                    HasMail = hasMail,
                    MessageId = messageId
                };

                _dtAlarmRepository.CreateDtAlarm(dtAlarm);
                return true;
            }
            catch (RmsException e)
            {
                var dtAlarm = new
                {
                    EquipmentSid = equipment?.Sid,
                    TypeCode = alarmInfo?.TypeCode,
                    ErrorCode = alarmInfo?.ErrorCode,
                    AlarmLevel = alarmInfo?.AlarmLevel,
                    AlarmTitle = alarmInfo?.AlarmTitle,
                    AlarmDescription = alarmInfo?.AlarmDescription,
                    AlarmDatetime = alarmInfo?.AlarmDatetime,
                    AlarmDefId = alarmInfo?.AlarmDefId,
                    EventDatetime = alarmInfo?.EventDatetime,
                    HasMail = hasMail,
                    MessageId = messageId
                };

                // アラームデータ保存失敗（基本設計書 5.3.4 エラー処理）
                _logger.Error(e, nameof(Resources.OP_ALR_ALR_009), new object[] { JsonConvert.SerializeObject(dtAlarm), messageId });
                return false;
            }
            finally
            {
                _logger.Leave();
            }
        }

        /// <summary>
        /// メール送信情報を作成しQueueStorageへ登録する
        /// </summary>
        /// <param name="messageId">メッセージID</param>
        /// <param name="alarmInfo">アラーム情報</param>
        /// <param name="alarmConfig">アラーム設定</param>
        /// <param name="equipment">機器データ</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        public bool CreateAndEnqueueAlarmInfo(string messageId, AlarmInfo alarmInfo, DtAlarmConfig alarmConfig, DtEquipment equipment)
        {
            try
            {
                _logger.EnterJson("{0}", new { alarmInfo, alarmConfig, equipment });

                var mailInfo = new MailInfo
                {
                    MailAddressTo = alarmConfig.MailAddress,
                    MailAddressFrom = _settings.AlarmMailAddressFrom,
                    MailSubject = alarmInfo.AlarmTitle,
                    CustomerNumber = equipment.DtInstallBase.CustomerNumber,
                    CustomerName = equipment.DtInstallBase.CustomerName,
                    EquipmentSerialNumber = equipment.DtInstallBase.EquipmentSerialNumber,
                    EquipmentNumber = equipment.DtInstallBase.EquipmentNumber,
                    EquipmentName = equipment.DtInstallBase.EquipmentName,
                    TypeCode = alarmInfo.TypeCode,
                    ErrorCode = alarmInfo.ErrorCode,
                    AlarmLevel = alarmInfo.AlarmLevel,
                    EventDate = alarmInfo.EventDatetime,
                    AlarmDescription = alarmInfo.AlarmDescription
                };

                string message = JsonConvert.SerializeObject(mailInfo);
                _queueRepository.SendMessageToMailQueue(message);

                return true;
            }
            catch (RmsException e)
            {
                // メールキュー登録失敗（基本設計書 5.3.4 エラー処理）
                _logger.Error(e, nameof(Resources.OP_ALR_ALR_007), new object[] { messageId });
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
        /// <param name="messageId">メッセージID</param>
        /// <param name="message">メッセージ</param>
        public void UpdateToFailureStorage(string messageId, string message)
        {
            try
            {
                _logger.EnterJson("{0}", new { messageId, message });

                DateTime now = _timeProvider.UtcNow;

                // ファイル情報
                ArchiveFile file = new ArchiveFile() { ContainerName = _settings.FailureBlobContainerName, CreatedAt = now };
                file.FilePath = string.Format(Utility.Const.FailureBlobFilenameFormat, now);

                // アップロード
                _failureRepository.Upload(file, message, true);
            }
            catch (RmsException e)
            {
                // Blobストレージへの保存処理に失敗した場合、メッセージ内容をログに出力して終了する。
                _logger.Error(e, nameof(Resources.OP_ALR_ALR_011), new object[] { messageId, message });
            }
            finally
            {
                _logger.Leave();
            }
        }
    }
}
