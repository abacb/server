using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rms.Server.Core.Azure.Functions.WebApi.Controllers;
using Rms.Server.Core.Service.Services;
using RmsRms.Server.Core.Azure.Functions.WebApi.Dto;
using System.Threading.Tasks;
using TestHelper;
using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Service.Models;
using Microsoft.AspNetCore.Http;

namespace Azure.Functions.WebApiTest
{
    // [本テストのスコープ]
    // Controllerまで。
    // Service以下のテストはそちらで行う。
    // 本機能はCRUDほど単純でなく一気通貫でテストすると複雑になる懸念があったため、層毎にテストを行う。
    // （そうするとController層のテストの存在意義が微妙だが、まあ一応残す）

    [TestClass]
    public class DevicesControllerTest
    {
        /// <summary>
        /// 必須パラメータが不正の場合BadRequestを返す
        /// </summary>
        /// <param name="sessionCode">セッションコード</param>
        [DataTestMethod]
        [DataRow(null)]
        public async Task PostDeviceRemote_InvalidParm_ReturnsBadRequest(
            string sessionCode)
        {
            // Target Create
            var builder = new TestDiProviderBuilder();
            builder.ServiceCollection.AddTransient<DevicesController>();
            var controller = builder.Build().GetService<DevicesController>();

            // 配信グループデータ更新パラメータ作成
            var dto = new DeviceRemoteRequestDto()
            {
                SessionCode = sessionCode
            };

            // 制約違反の場合はBadRequestが返ることを確認する
            var updateResponse = await controller.PostDeviceRemoteAsync(
                dto,
                100,
                UnitTestHelper.CreateLogger());

            Assert.IsInstanceOfType(updateResponse, typeof(BadRequestObjectResult));
        }

        /// <summary>
        /// 存在しないデバイスIDが指定された場合NotFoundを返す
        /// </summary>
        [TestMethod]
        public async Task PostDeviceRemote_NotFoundDevice_ReturnsNotFound()
        {
            // Target Create
            var builder = new TestDiProviderBuilder();
            builder.ServiceCollection.AddTransient<DevicesController>();

            // Mock
            var mockReturnSucceed = new Mock<IDeviceService>();
            mockReturnSucceed.Setup(x => x.RequestRemoteAsync(It.IsAny<RequestRemote>()))
                .ReturnsAsync(new Result(ResultCode.NotFound));
            builder.ServiceCollection.AddTransient<IDeviceService>(_ => mockReturnSucceed.Object);
            var controller = builder.Build().GetService<DevicesController>();

            var notFoundDviceId = 100;

            // 配信グループデータ更新パラメータ作成
            var validDto = new DeviceRemoteRequestDto()
            {
                SessionCode = "100"
            };

            // 制約違反の場合はBadRequestが返ることを確認する
            var updateResponse = await controller.PostDeviceRemoteAsync(
                validDto,
                notFoundDviceId,
                UnitTestHelper.CreateLogger());

            Assert.IsInstanceOfType(updateResponse, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task PostDeviceRemote_Success_ReturnsSuccess()
        {
            // Target Create
            var builder = new TestDiProviderBuilder();
            builder.ServiceCollection.AddTransient<DevicesController>();

            // Mock
            var mockReturnSucceed = new Mock<IDeviceService>();
            mockReturnSucceed.Setup( x => x.RequestRemoteAsync( It.IsAny<RequestRemote>() ))
                .ReturnsAsync(new Result(ResultCode.Succeed));
            builder.ServiceCollection.AddTransient<IDeviceService>(_ => mockReturnSucceed.Object);

            var controller = builder.Build().GetService<DevicesController>();
            var dviceId = 100;

            // 配信グループデータ更新パラメータ作成
            var validDto = new DeviceRemoteRequestDto()
            {
                SessionCode = "100"
            };

            var updateResponse = await controller.PostDeviceRemoteAsync(
                validDto,
                dviceId,
                UnitTestHelper.CreateLogger());

            Assert.IsInstanceOfType(updateResponse, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task PostDeviceRemote_ExceptionThrown_Returns500Error()
        {
            // Target Create
            var builder = new TestDiProviderBuilder();
            builder.ServiceCollection.AddTransient<DevicesController>();

            // Mock
            var mockReturnSucceed = new Mock<IDeviceService>();
            mockReturnSucceed.Setup(x => x.RequestRemoteAsync(It.IsAny<RequestRemote>()))
                .ThrowsAsync(new System.Exception());
            builder.ServiceCollection.AddTransient<IDeviceService>(_ => mockReturnSucceed.Object);

            var controller = builder.Build().GetService<DevicesController>();
            var dviceId = 100;

            // 配信グループデータ更新パラメータ作成
            var validDto = new DeviceRemoteRequestDto()
            {
                SessionCode = "100"
            };

            var response = await controller.PostDeviceRemoteAsync(
                validDto,
                dviceId,
                UnitTestHelper.CreateLogger());

            Assert.IsInstanceOfType(response, typeof(StatusCodeResult));
            Assert.AreEqual(StatusCodes.Status500InternalServerError,(response as StatusCodeResult).StatusCode);
        }
    }
}
