using Rms.Server.Core.Abstraction.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// IFailureRepository
    /// </summary>
    public interface IFailureRepository
    {
        /// <summary>
        /// FailureBlobにファイルをアップロードする
        /// </summary>
        /// <param name="file">アップロードするファイルの情報</param>
        /// <param name="contents">アップロードするファイルの内容</param>
        /// <param name="withSerialNumber">ファイル名末尾に連番を付与する場合はtrue、そうでない場合はfalseを指定する。</param>
        void Upload(ArchiveFile file, string contents, bool withSerialNumber = false);
    }
}
