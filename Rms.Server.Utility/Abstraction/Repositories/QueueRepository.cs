using Microsoft.Extensions.Logging;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Operation.Utility;
using Rms.Server.Utility.Utility;
using Rms.Server.Utility.Utility.Extensions;
using System;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// Queueリポジトリ
    /// </summary>
    public class QueueRepository : IQueueRepository
    {
        /// <summary>ロガー</summary>
        private readonly ILogger<QueueRepository> _logger;

        /// <summary>キュー接続用のPolly</summary>
        private readonly QueuePolly _queuePolly;

        /// <summary>アプリケーション設定</summary>
        private readonly UtilityAppSettings _appSettings;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="queuePolly">キュー接続用のPolly</param>
        /// <param name="appSettings">アプリケーション設定</param>
        public QueueRepository(ILogger<QueueRepository> logger, QueuePolly queuePolly, UtilityAppSettings appSettings)
        {
            Assert.IfNull(logger);
            Assert.IfNull(queuePolly);
            Assert.IfNull(appSettings);

            _logger = logger;
            _queuePolly = queuePolly;
            _appSettings = appSettings;
        }

        /// <summary>
        /// アラームキューへメッセージを送信する
        /// </summary>
        /// <param name="message">送信するメッセージ</param>
        public void SendMessageToAlarmQueue(string message)
        {
            try
            {
                _logger.EnterJson("{0}", new { message });

                _queuePolly.Execute(() =>
                {
                    QueueOperation.SendMessage(_appSettings.AlarmQueueName, _appSettings.QueueStorageConnectionString, message);
                });
            }
            catch (Exception e)
            {
                throw new RmsException(string.Format("アラームキューへのメッセージ送信に失敗しました。(message = {0})", message), e);
            }
            finally
            {
                _logger.Leave();
            }
        }
    }
}
