using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rms.Server.Core.Service.Models;

namespace ServiceTest
{
    [TestClass]
    public class CleanDbSettingTest
    {
        const string VALID_KEY_SAMPLE = "DbCleanTarget_DtAccountTb";
        const string VALID_VALUE_SAMPLE = "10";
        const string VALID_REPOSITORYNAMEFORMAT_SAMPLE = "Rms.Server.Core.Abstraction.Repositories.{0}Repository, Rms.Server.Core.Abstraction";

        /// <summary>
        /// 指定のKeyPrefixがあるkeyを渡せば、nullでないSettingインスタンスを取得できることを確認する
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expectedRepositoryFullName"></param>
        [DataTestMethod]
        [DataRow("DbCleanTarget_DtAccountTb", "Rms.Server.Core.Abstraction.Repositories.DtAccountTbRepository, Rms.Server.Core.Abstraction")]
        [DataRow("DbCleanTarget_ DtAccountTb", "Rms.Server.Core.Abstraction.Repositories. DtAccountTbRepository, Rms.Server.Core.Abstraction")]
        [DataRow("DbCleanTarget_a", "Rms.Server.Core.Abstraction.Repositories.aRepository, Rms.Server.Core.Abstraction")]
        [DataRow("DbCleanTarget_(){}[]!\"#$%&'=~|`{+*}<>?_", "Rms.Server.Core.Abstraction.Repositories.(){}[]!\"#$%&'=~|`{+*}<>?_Repository, Rms.Server.Core.Abstraction")]
        public void ValidKey(string key, string expectedRepositoryFullName)
        {
            var setting = CleanDbSetting.Create(key, VALID_VALUE_SAMPLE, VALID_REPOSITORYNAMEFORMAT_SAMPLE);
            Assert.IsNotNull(setting);
            Assert.AreEqual(expectedRepositoryFullName, setting.RepositoryFullName);
        }

        /// <summary>
        /// 指定のKeyPrefixから始まらないkeyを渡すと、nullを取得することを確認する
        /// </summary>
        /// <param name="key"></param>
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("DtAccountTb")]
        [DataRow(" DbCleanTarget_DtAccountTb")]
        [DataRow("DbClean_Target_DtAccountTb")]
        [DataRow("DbClean/Target_DtAccountTb")]
        [DataRow("DbCleanTarget _DtAccountTb")]
        public void InvalidKey(string key)
        {
            var setting = CleanDbSetting.Create(key, VALID_VALUE_SAMPLE, VALID_REPOSITORYNAMEFORMAT_SAMPLE);
            Assert.IsNull(setting);
        }

        /// <summary>
        /// 0より大きいint値を渡せば、nullでないSettingインスタンスを取得できることを確認する
        /// </summary>
        /// <param name="value"></param>
        /// <param name="expected"></param>
        [DataTestMethod]
        [DataRow("2147483647", 2147483647)] // int.MaxValue
        [DataRow("500", 500)]
        [DataRow("1", 1)]
        public void ValidValue(string value, int expected)
        {
            var setting = CleanDbSetting.Create(VALID_KEY_SAMPLE, value, VALID_REPOSITORYNAMEFORMAT_SAMPLE);
            Assert.IsNotNull(setting);
            Assert.AreEqual(expected, setting.RetentionPeriodMonth);
        }

        /// <summary>
        /// int値外だったり0以下の値を渡すと、nullを取得することを確認する
        /// </summary>
        /// <param name="value"></param>
        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        [DataRow("1.1")]
        [DataRow("2147483648")] // int.MaxValue + 1
        [DataRow("-1")]
        [DataRow("0xa")]
        [DataRow("0x00000001")]
        [DataRow("0")]
        public void InvalidValue(string value)
        {
            var setting = CleanDbSetting.Create(VALID_KEY_SAMPLE, value, VALID_REPOSITORYNAMEFORMAT_SAMPLE);
            Assert.IsNull(setting);
        }
    }
}
