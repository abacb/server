using Rms.Server.Core.Utility.Models.Entites;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// MT_INSTALL_RESULT_STATUSテーブルのリポジトリ
    /// </summary>
    public interface IMtInstallResultStatusRepository
    {
        /// <summary>
        /// 引数に指定したMtInstallResultStatusをMT_INSTALL_RESULT_STATUSテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <returns>処理結果</returns>
        MtInstallResultStatus CreateMtInstallResultStatus(MtInstallResultStatus inData);

        /// <summary>
        /// MT_INSTALL_RESULT_STATUSテーブルからMtInstallResultStatusを取得する
        /// </summary>
        /// <param name="code">取得するデータのCode</param>
        /// <returns>取得したデータ</returns>
        MtInstallResultStatus ReadMtInstallResultStatus(string code);
    }
}
