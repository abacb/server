using Rms.Server.Operation.Utility.Models;
using System.Threading.Tasks;

namespace Rms.Server.Operation.Service.Services
{
    /// <summary>
    /// MailSenderサービスのインターフェース
    /// </summary>
    public interface IMailSenderService
    {
        /// <summary>
        /// メールを送信する
        /// </summary>
        /// <param name="mailInfo">メール情報</param>
        /// <returns>成功した場合true、失敗した場合falseを返す</returns>
        Task<bool> SendMail(MailInfo mailInfo);

        /// <summary>
        /// Failureストレージに再送用メッセージをアップロードする
        /// </summary>
        /// <param name="message">メッセージ</param>
        void UpdateToFailureStorage(string message);
    }
}
