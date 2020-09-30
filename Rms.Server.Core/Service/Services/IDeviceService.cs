using Rms.Server.Core.Service.Models;
using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Utility.Models.Entites;
using System.Threading.Tasks;

namespace Rms.Server.Core.Service.Services
{
    /// <summary>
    /// IDeviceService
    /// </summary>
    public interface IDeviceService
    {
        /// <summary>
        /// 機器情報を保存する
        /// </summary>
        /// <remarks>sd 04-01.機器登録</remarks>
        /// <param name="utilParam">Utility型機器情報</param>
        /// <param name="baseConfig">設置設定情報</param>
        /// <returns>結果付きDB保存済み機器情報</returns>
        Result<DtDevice> Create(DtDevice utilParam, InstallBaseConfig baseConfig);

        /// <summary>
        /// 機器情報を更新する
        /// </summary>
        /// <remarks>sd 04-01.機器登録</remarks>
        /// <param name="utilParam">>Utility型機器情報</param>
        /// <param name="baseConfig">設置設定情報</param>
        /// <returns>結果付きDB保存済み機器情報</returns>
        Result<DtDevice> Update(DtDevice utilParam, InstallBaseConfig baseConfig);

        /// <summary>
        /// デバイスにリモート接続をリクエストする
        /// </summary>
        /// <param name="request">リクエスト</param>
        /// <returns>結果</returns>
        Task<Result> RequestRemoteAsync(RequestRemote request);
    }
}
