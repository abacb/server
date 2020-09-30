using Rms.Server.Operation.Utility.Models;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Rms.Server.Operation.Abstraction.Repositories
{
    /// <summary>
    /// メールリポジトリインターフェース
    /// </summary>
    public interface IMailRepository
    {
        /// <summary>
        /// メールを送信する
        /// </summary>
        /// <param name="mailInfo">メール情報</param>
        /// <returns>メール送信結果（HTTPステータスコードとレスポンスボディ）</returns>
        Task<KeyValuePair<HttpStatusCode, string>> SendMail(MailInfo mailInfo);
    }
}
