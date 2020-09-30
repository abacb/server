using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Models.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Rms.Server.Core.Abstraction.Models
{
    /// <summary>
    /// ファイル情報
    /// </summary>
    public class ArchiveFile
    {
        /// <summary>
        /// コンテナ名
        /// </summary>
        /// <remarks>命名規則については「https://docs.microsoft.com/ja-jp/azure/azure-resource-manager/management/resource-name-rules#microsoftstorage」を参照</remarks>
        [Required(ErrorMessage = "ContainerName is required.")]
        [MinLength(3, ErrorMessage = "ContainerName length should be between 3 and 63.")]
        [MaxLength(63, ErrorMessage = "ContainerName length should be between 3 and 63.")]
        [RegularExpression("^[a-z0-9]+(-[a-z0-9]+)*$", ErrorMessage = "ContainerName is invalid.")]
        public string ContainerName { get; set; }

        /// <summary>
        /// ファイルパス
        /// </summary>
        /// <remarks>命名規則については「https://docs.microsoft.com/ja-jp/azure/azure-resource-manager/management/resource-name-rules#microsoftstorage」を参照</remarks>
        [Required(ErrorMessage = "FilePath is required.")]
        [MinLength(1, ErrorMessage = "FilePath length should be between 1 and 1024.")]
        [MaxLength(1024, ErrorMessage = "FilePath length should be between 1 and 1024.")]
        [RegularExpression("^[^/|\\][^|]*[^/|.\\]$", ErrorMessage = "FilePath is invalid.")]
        public string FilePath { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// メタデータ
        /// </summary>
        /// <remarks>
        /// Blobのメタデータはhttpで受けるため大文字小文字を区別しない
        /// </remarks>
        public IDictionary<string, string> MetaData { get; } =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 変換する
        /// </summary>
        /// <param name="file">DtDeviceFile</param>
        /// <returns>ArchiveFile</returns>
        public static ArchiveFile From(DtDeviceFile file)
        {
            Assert.IfNull(file);
            var archiveFile = new ArchiveFile()
            {
                ContainerName = file.Container,
                FilePath = file.FilePath
            };

            IDictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var attribute in file.DtDeviceFileAttribute)
            {
                archiveFile.SetMetaData(attribute);
            }

            return archiveFile;
        }

        /// <summary>
        /// コピーを作成する
        /// </summary>
        /// <returns>ArchiveFile</returns>
        public ArchiveFile Copy()
        {
            var archiveFile = new ArchiveFile()
            {
                ContainerName = ContainerName,
                FilePath = FilePath,
                CreatedAt = CreatedAt
            };
            archiveFile.SetMetaData(MetaData);
            return archiveFile;
        }

        /// <summary>
        /// メタデータを設定する。
        /// </summary>
        /// <param name="metaData">メタデータ</param>
        /// <returns>アーカイブファイル</returns>
        /// <remarks>外から直接設定しないのは、DictionaryをAzure Blobの仕様に合わせて大文字小文字区別しないようにするため。</remarks>
        public ArchiveFile SetMetaData(IDictionary<string, string> metaData)
        {
            foreach (var inputMetaData in metaData)
            {
                this.MetaData[inputMetaData.Key] = inputMetaData.Value;
            }

            return this;
        }

        /// <summary>
        /// メタデータを設定する。
        /// </summary>
        /// <param name="fileAttr">ファイル属性</param>
        /// <remarks>
        /// 外から直接設定しないのは、DictionaryをAzure Blobの仕様に合わせて大文字小文字区別しないようにするため。
        /// </remarks>
        private void SetMetaData(DtDeviceFileAttribute fileAttr)
        {
            if (fileAttr != null && !string.IsNullOrWhiteSpace(fileAttr.Name))
            {
                MetaData[fileAttr.Name] = fileAttr.Value;
            }
        }
    }
}
