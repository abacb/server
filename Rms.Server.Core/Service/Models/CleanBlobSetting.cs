using Rms.Server.Core.Utility.Exceptions;
using System;
using System.Text.RegularExpressions;

namespace Rms.Server.Core.Service.Models
{
    /// <summary>
    /// CleanBlobで設定ファイルの解釈を行うクラス
    /// </summary>
    public class CleanBlobSetting
    {
        /// <summary>
        /// 設定ファイルから本機能に関連するキーのPrefix
        /// </summary>
        public const string KeyPrefix = "BlobCleanTarget_";

        /// <summary>
        /// 正規表現上のコンテナ名の変数名
        /// </summary>
        private const string VarNameContainer = "container";

        /// <summary>
        /// 正規表現上のファイルパスの変数名
        /// </summary>
        private const string VarNameFilePrefix = "path";

        /// <summary>
        /// 正規表現上のコンテナ名とファイルパスのセパレータ
        /// </summary>
        private const string Separater = "_";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private CleanBlobSetting()
        {
        }

        /// <summary>
        /// コンテナ名
        /// </summary>
        public string ContainerName { get; private set; }

        /// <summary>
        /// ファイルパスのPrefix
        /// </summary>
        public string FilePathPrefix { get; private set; }

        /// <summary>
        /// 保持期間
        /// </summary>
        public int RetentionPeriodMonth { get; private set; }

        /// <summary>
        /// {コンテナ名}_{ファイルパス}
        /// </summary>
        public string ContainerAndFilePath
        {
            get => $"{ContainerName}{Separater}{FilePathPrefix}";
        }

        /// <summary>
        /// KeyとValueのペアを文字列解析し、条件に合致する場合、その内容から本クラスのインスタンスを生成する。
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <returns>CleanBlobSetting</returns>
        public static CleanBlobSetting Create(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new RmsInvalidAppSettingException($"{nameof(key)} is required.");
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new RmsInvalidAppSettingException($"{nameof(value)} is required.");
            }

            if (!int.TryParse(value, out int month) || month <= 0)
            {
                throw new RmsInvalidAppSettingException($"{key} is invalid format.");
            }

            var containerNameAndPath = GetContainerNameAndFilePath(key);
            if (containerNameAndPath == null)
            {
                throw new RmsInvalidAppSettingException($"{key} is invalid format.");
            }

            return new CleanBlobSetting()
            {
                ContainerName = containerNameAndPath.Item1,
                FilePathPrefix = containerNameAndPath.Item2,
                RetentionPeriodMonth = month
            };
        }

        /// <summary>
        /// 文字列を解析し、コンテナ名とファイルパスのペアを生成する。
        /// フォーマットは以下とみなす。これに合致しない場合nullを返す。
        /// {BlobCleanTarget_}{コンナ名}_{ファイルパス}
        /// {BlobCleanTarget_}{コンテナ名}_
        /// {BlobCleanTarget_}{コンテナ名}
        /// </summary>
        /// <param name="text">文字列</param>
        /// <returns>{コンテナ名,ファイルパス}</returns>
        private static Tuple<string, string> GetContainerNameAndFilePath(string text)
        {
            // 大文字小文字の区別はしない(Azure Blobの仕様)
            Regex r =
                new Regex(
                    $@"^{KeyPrefix}(?<{VarNameContainer}>[^{Separater}\s]+)(_(?<{VarNameFilePrefix}>.*)|)$",
                    RegexOptions.IgnoreCase);

            // 正規表現と一致する対象を検索
            Match m = r.Match(text);
            if (m.Success)
            {
                var containerName = m.Groups[VarNameContainer].Value;
                var filePath = m.Groups[VarNameFilePrefix].Value?.Trim();

                return new Tuple<string, string>(containerName, filePath);
            }

            return null;
        }
    }
}
