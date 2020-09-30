using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rms.Server.Core.Azure.Functions.Utility;
using System;
using System.ComponentModel.DataAnnotations;

namespace Azure.Functions.Utility.Tests
{
    [TestClass()]
    public class ParameterCheckerTests
    {
        [TestMethod()]
        public void NullInstance()
        {
            ParameterCheckerTestValue param = null;
            Result<ParameterCheckerResultCode> result = ParameterChecker.HasInvalidProperty(param);
            Assert.IsTrue(result.ResultCode == ParameterCheckerResultCode.NullArgError);
        }

        [DataTestMethod]
        [DataRow(0, 0, "", "123")] // arg3, arg4最低文字数
        [DataRow(0, 0, "", "1234567890")] // arg4最大文字数
        [DataRow(0, 0, "", null)] 
        [DataRow(0, 0, "1234567890", "123")] // arg3最大文字数
        [DataRow(0, null, "", "123")]
        [DataRow(0, Int32.MaxValue, "", "123")]
        [DataRow(0, Int32.MinValue, "", "123")]
        [DataRow(Int32.MaxValue, 0, "", "123")]
        [DataRow(Int32.MinValue, 0, "", "123")]
        public void ValidParam(Nullable<int> arg1, Nullable<int> arg2, string arg3, string arg4)
        {
            ParameterCheckerTestValue param = new ParameterCheckerTestValue()
            {
                reqParam = arg1,
                notReqParam = arg2,
                max10LenReqParam = arg3,
                max10Min3LenParam = arg4
            };

            Result<ParameterCheckerResultCode> result = ParameterChecker.HasInvalidProperty(param);
            Assert.IsTrue(result.ResultCode == ParameterCheckerResultCode.Succeed);
        }

        [DataTestMethod]
        [DataRow(null, 0, "", "123", ParameterCheckerResultCode.RequiredError)]
        [DataRow(0, 0, null, "123", ParameterCheckerResultCode.RequiredError)]
        [DataRow(0, 0, "", "12345678901", ParameterCheckerResultCode.LengthError)] // arg4最大文字数 + 1
        [DataRow(0, 0, "12345678901", "123", ParameterCheckerResultCode.LengthError)] // arg3最大文字数 + 1
        [DataRow(0, 0, "", "12", ParameterCheckerResultCode.LengthError)] // arg4最小文字数 - 1
        public void InvalidParam(Nullable<int> arg1, Nullable<int> arg2, string arg3, string arg4, ParameterCheckerResultCode expected)
        {
            ParameterCheckerTestValue param = new ParameterCheckerTestValue()
            {
                reqParam = arg1,
                notReqParam = arg2,
                max10LenReqParam = arg3,
                max10Min3LenParam = arg4
            };

            Result<ParameterCheckerResultCode> result = ParameterChecker.HasInvalidProperty(param);
            Assert.IsTrue(result.ResultCode == expected);
        }

        public class ParameterCheckerTestValue
        {
            [Required]
            public Nullable<int> reqParam { get; set; }

            public Nullable<int> notReqParam { get; set; }

            [Required]
            [StringLength(10)]
            public string max10LenReqParam { get; set; }

            [StringLength(10, MinimumLength = 3)]
            public string max10Min3LenParam { get; set; }
        }
    }
}