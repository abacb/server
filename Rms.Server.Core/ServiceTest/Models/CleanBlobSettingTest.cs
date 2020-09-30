using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rms.Server.Core.Service.Models;
using Rms.Server.Core.Utility.Exceptions;

namespace ServiceTest
{
    [TestClass]
    public class CleanBlobSettingTest
    {
        [DataTestMethod]
        [DataRow("BlobCleanTarget_container", "container", "")]
        [DataRow("BlobCleanTarget_container_", "container", "")]
        [DataRow("BlobCleanTarget_container_   ", "container", "")]
        [DataRow("BlobCleanTarget_container_filepath", "container", "filepath")]
        [DataRow("BlobCleanTarget_container_filepath_filepath", "container", "filepath_filepath")]
        [DataRow("BlobCleanTarget_cont/.ainer_filepath", "cont/.ainer", "filepath")]
        [DataRow("BlobCleanTarget_container_delivery/files/userA/data", "container", "delivery/files/userA/data")]
        public void ValidKey(string Key, string ExpectedContainerName, string ExpectedFilePathPrefix)
        {
            var setting = CleanBlobSetting.Create(Key, "10");
            Assert.IsNotNull(setting);
            Assert.AreEqual(ExpectedContainerName, setting.ContainerName);
            Assert.AreEqual(ExpectedFilePathPrefix, setting.FilePathPrefix);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        [DataRow("BlobCleanTarget_@ _filepath")]
        [DataRow("Blob_Clean_Target_container_filepath")]
        [DataRow("Blob/CleanTarget_container_filepath")]
        [DataRow(" BlobCleanTarget_container_filepath")]
        [DataRow("BlobCleanTarget _container_filepath")]
        public void InvalidContainerName(string ConfigKey)
        {
            try
            {
                var setting = CleanBlobSetting.Create(ConfigKey, "1");
            }
            catch(RmsInvalidAppSettingException)
            {
                return;
            }
            Assert.Fail();
        }

        [DataTestMethod]
        [DataRow("2147483647", 2147483647)]
        [DataRow("500", 500)]
        [DataRow("1", 1)]
        public void ValidValue(string Value, int Expected)
        {
            var setting = CleanBlobSetting.Create("BlobCleanTarget_container_filepath", Value);
            Assert.AreEqual(Expected, setting.RetentionPeriodMonth);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        [DataRow("1.1")]
        [DataRow("2147483648")] // int.MaxValue + 1
        [DataRow("-1")]
        [DataRow("0xa")]
        [DataRow("0")]
        public void InvalidValue(string Value)
        {
            try
            {
                var setting = CleanBlobSetting.Create("BlobCleanTarget_container_filepath", Value);
            }
            catch (RmsInvalidAppSettingException)
            {
                return;
            }
            Assert.Fail();
        }
    }
}
