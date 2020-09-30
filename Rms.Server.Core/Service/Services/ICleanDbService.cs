namespace Rms.Server.Core.Service.Services
{
    /// <summary>
    /// ICleanDbService
    /// </summary>
    public interface ICleanDbService
    {
        /// <summary>
        /// 不要データの削除
        /// </summary>
        void Clean();
    }
}
