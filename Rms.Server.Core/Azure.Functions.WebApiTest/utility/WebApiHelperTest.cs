using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rms.Server.Core.Azure.Functions.WebApi.Utility;
using System;
using TestHelper;

namespace Azure.Functions.WebApiTest.Utility
{
    [TestClass()]
    public class WebApiHelperTest
    {
        /*
        /// <summary>
        /// TryFunctionAndGetActionResultメソッドにnullを渡す
        /// </summary>
        [DataTestMethod]
        [DataRow(false, false)]
        [DataRow(false, true)]
        [DataRow(true, false)]
        public void TryFunctionAndGetActionResultNullParamTest(bool isNotNullFunction, bool isNotNullLogger)
        {
            var logger = isNotNullLogger ? UnitTestHelper.CreateLogger<WebApiHelperTest>() : null;
            ActionResult result = null;
            if (isNotNullFunction)
            {
                result = WebApiHelper.TryFunctionAndGetActionResult(() => { return new OkObjectResult("hoge"); }, logger) ;
            }
            else
            {
                result = WebApiHelper.TryFunctionAndGetActionResult(null, logger);
            }

            // パラメータにnullがあればnullが返る
            Assert.IsNull(result);
        }

        /// <summary>
        /// TryFunctionAndGetActionResultメソッドにExceptionを返すメソッドを渡す
        /// </summary>
        [DataTestMethod()]
        public void TryFunctionAndGetActionResultThrowExceptionTest()
        {
            var logger = UnitTestHelper.CreateLogger<WebApiHelperTest>();
            ActionResult result = WebApiHelper.TryFunctionAndGetActionResult(() => { throw new Exception(); }, logger);

            // Exception時は500エラーが返る
            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            var statusCodeResult = result as StatusCodeResult;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }

        /// <summary>
        /// TryFunctionAndGetActionResultメソッドに正常なActionResultを返すメソッドを渡す
        /// </summary>
        [DataTestMethod()]
        public void TryFunctionAndGetActionResultValidTest()
        {
            var logger = UnitTestHelper.CreateLogger<WebApiHelperTest>();
            ActionResult result = WebApiHelper.TryFunctionAndGetActionResult(() => { return new OkObjectResult("hoge"); }, logger);

            // OKが返ること
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
        */

        /// <summary>
        /// ConvertLongToByteArrayメソッドに変換する値を渡す
        /// </summary>
        [DataTestMethod]
        [DataRow(0, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 })]
        [DataRow(long.MaxValue, new byte[] { 127, 255, 255, 255, 255, 255, 255, 255 })]
        [DataRow(long.MinValue, new byte[] { 128, 0, 0, 0, 0, 0, 0, 0 })]
        public void ConvertLongToByteArrayValidTest(long inParam, byte[] expectedByteArray)
        {
            byte[] result = WebApiHelper.ConvertLongToByteArray(inParam);

            // 期待するバイト配列(ビッグエンディアン形式)が返ること
            CollectionAssert.AreEqual(expectedByteArray, result);
        }


        /// <summary>
        /// ConvertByteArrayToLongメソッドにnullを渡す
        /// </summary>
        [DataTestMethod()]
        public void ConvertByteArrayToLongValidTest()
        {
            long result = WebApiHelper.ConvertByteArrayToLong(null);

            // 0が返ること
            Assert.AreEqual(0, result);
        }

        /// <summary>
        /// ConvertByteArrayToLongメソッドに変換するバイト配列を渡す
        /// </summary>
        [DataTestMethod]
        [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0)]
        [DataRow(new byte[] { 127, 255, 255, 255, 255, 255, 255, 255 }, long.MaxValue)]
        [DataRow(new byte[] { 128, 0, 0, 0, 0, 0, 0, 0 }, long.MinValue)]
        public void ConvertByteArrayToLongValidTest(byte[] inParam, long expectedValue)
        {
            byte[] beforeInParam = new byte[inParam.Length];
            inParam.CopyTo(beforeInParam, 0);
            long result = WebApiHelper.ConvertByteArrayToLong(inParam);

            // 期待する値が返ること
            Assert.AreEqual(expectedValue, result);
            // 副作用は発生していないこと
            CollectionAssert.AreEqual(beforeInParam, inParam);
        }
    }
}