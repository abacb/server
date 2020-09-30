using Rms.Server.Core.Utility.Exceptions;
using System;

namespace Rms.Server.Core.Service.Models
{
    /// <summary>
    /// CleanDbで設定ファイルの解釈を行うクラス
    /// </summary>
    public class CleanDbSetting
    {
        /// <summary>設定ファイルから本機能に関連するキーのPrefix</summary>
        public const string KeyPrefix = "DbCleanTarget_";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private CleanDbSetting()
        {
        }

        /// <summary>
        /// 処理対象となるモデルクラス名
        /// </summary>
        public string ModelName { get; private set; }

        /// <summary>
        /// 処理対象となるリポジトリクラス名
        /// </summary>
        public string RepositoryFullName { get; private set; }

        /// <summary>
        /// 保持期間
        /// </summary>
        public int RetentionPeriodMonth { get; private set; }

        /// <summary>
        /// KeyとValueのペアを文字列解析し、条件に合致する場合、その内容から本クラスのインスタンスを生成する。
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="repositoryNameFormat">エンティティモデル名からリポジトリのフル名称を生成する際のフォーマット</param>
        /// <returns>CleanDbSetting</returns>
        public static CleanDbSetting Create(string key, string value, string repositoryNameFormat)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new RmsInvalidAppSettingException($"{nameof(key)} is required.");
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new RmsInvalidAppSettingException($"{nameof(value)} is required.");
            }

            if (!key.StartsWith(KeyPrefix, StringComparison.OrdinalIgnoreCase))
            {
                throw new RmsInvalidAppSettingException($"{key} is invalid format.");
            }

            if (!int.TryParse(value, out int month) || month <= 0)
            {
                throw new RmsInvalidAppSettingException($"{key} is invalid format.");
            }

            // キー名からPrefixを外して完全限定名を作成する
            string modelName = key.Substring(KeyPrefix.Length);
            string repositoryName = string.Format(repositoryNameFormat, modelName);
            return new CleanDbSetting()
            {
                ModelName = modelName,
                RepositoryFullName = repositoryName,
                RetentionPeriodMonth = month
            };
        }
    }
}
