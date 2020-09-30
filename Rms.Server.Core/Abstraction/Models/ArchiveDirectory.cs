namespace Rms.Server.Core.Abstraction.Models
{
    /// <summary>
    /// ディレクトリ
    /// </summary>
    public class ArchiveDirectory
    {
        /// <summary>
        /// コンテナ名
        /// </summary>
        public string ContainerName { get; set; }

        /// <summary>
        /// ディレクトリパス
        /// </summary>
        /// <remarks>
        /// 本項目はDBへの設定ではなくAzureSDKに対して使用する。その場合SDK側でNullだと例外が発生するため、初期値として空を設定する。
        /// </remarks>
        public string DirectoryPath { get; set; } = string.Empty;
    }
}
