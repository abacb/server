using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Rms.Server.Core.Abstraction.Models;
using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Core.Service.Models;
using Rms.Server.Core.Service.Services;
using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Utility.Models.Entites;
using System;
using System.Threading.Tasks;
using TestHelper;
using static Rms.Server.Core.Service.Services.DeviceService;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ServiceTest.Services
{
    [TestClass]
    public class DeviceServiceTest
    {
        // テスト設計
        // ・正常系
        //      ・DBから取得したエッジIDに対して処理を行う。
        //      ・設定から取得したメッセージを送る。
        //      ・引数セッションコードをその中に含む
        // ・DBにデバイスがない場合NotFound
        // ・アプリケーション設定にメッセージの元がない場合サーバーエラー。
        // ・デバイス取得時の例外はそのまま上に飛ばす
        // ・デバイスメッセージ送信時の例外はそのまま上に飛ばす

        [TestMethod]
        public async Task RequestRemoteAsync_SendSuccess_ReturnsSucceed()
        {
            // 以下の確認をしたい。
            //      ・DBから取得したエッジIDに対して処理を行う。
            //      ・設定から取得したメッセージを送る。
            //      ・引数セッションコードをその中に含む

            // 期待する結果
            Guid expectedEdgeId = Guid.NewGuid();
            string expectedSessionCode = "525215";
            string expectedRemoteParameter = "isl.exe --session-code {0}.";
            string expectedMessage = JsonConvert.SerializeObject(
                new RequestRemoteMessage(
                    expectedRemoteParameter,
                    new RequestRemote()
                    {
                        SessionCode = expectedSessionCode
                    }));
            var GotDevice = new DtDevice() { EdgeId = expectedEdgeId };

            // 渡された値
            Guid actualEdgeId = Guid.Empty;
            string actualMessage = string.Empty;

            // Target Create
            TestDiProviderBuilder builder =
                new TestDiProviderBuilder()
                .AddConfigure("RemoteParameter", expectedRemoteParameter);

            // Mock : デバイス取得に成功
            SetMock_ReadDevice_Returns(GotDevice, builder);

            // Mock : メッセージ送信に成功
            var mockOfRequestDevice = new Mock<IRequestDeviceRepository>();
            mockOfRequestDevice
                .Setup(x => x.SendMessageAsync(It.IsAny<DeviceConnectionInfo>(), It.IsAny<string>()))
                .Callback<DeviceConnectionInfo, string>((info, message) =>
                {
                    // 引数が想定された内容で渡されたことを確認する。
                    actualEdgeId = info.EdgeId;
                    actualMessage = message;
                });
            builder.ServiceCollection.AddTransient(_ => mockOfRequestDevice.Object);
            var service = builder.Build().GetService<IDeviceService>();
            var request = new RequestRemote() { SessionCode = expectedSessionCode };

            var result = await service.RequestRemoteAsync(request);

            Assert.AreEqual(ResultCode.Succeed, result.ResultCode);
            Assert.AreEqual(expectedEdgeId, actualEdgeId);
            Assert.AreEqual(expectedMessage, actualMessage);
        }

        [TestMethod]
        public async Task RequestRemoteAsync_NotExistDevice_ReturnsNotFound()
        {
            DtDevice NotFoundResult = null;

            // Target Create
            var builder = new TestDiProviderBuilder();

            // Mock : デバイス取得失敗
            SetMock_ReadDevice_Returns(NotFoundResult, builder);

            var service = builder.Build().GetService<IDeviceService>();

            var result = await service.RequestRemoteAsync(new RequestRemote());
            Assert.AreEqual(ResultCode.NotFound, result.ResultCode);
        }

        [TestMethod]
        public async Task RequestRemoteAsync_NotExistRemoteParamAtAppSettings_ReturnsServerError()
        {
            var GotDevice = new DtDevice();

            // Target Create
            TestDiProviderBuilder builder =
                new TestDiProviderBuilder();
            //.AddConfigure("RemoteParameter", "isl.exe --session-code {0}.");

            // Mock : デバイス取得に成功
            SetMock_ReadDevice_Returns(GotDevice, builder);

            var service = builder.Build().GetService<IDeviceService>();

            var result = await service.RequestRemoteAsync(new RequestRemote());
            Assert.AreEqual(ResultCode.ServerEerror, result.ResultCode);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task RequestRemoteAsync_ThrowExceptionAtReadDevice_ThrownException()
        {
            var exception = new Exception();

            // Target Create
            TestDiProviderBuilder builder =
                new TestDiProviderBuilder()
                .AddConfigure("RemoteParameter", "isl.exe --session-code {0}.");

            // Mock : デバイス取得時に例外
            SetMock_ReadDevice_Thrown(exception, builder);

            var service = builder.Build().GetService<IDeviceService>();

            await service.RequestRemoteAsync(new RequestRemote());
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task RequestRemoteAsync_ThrowExceptionAtSendMessage_ThrownException()
        {
            var exception = new Exception();
            var GotDevice = new DtDevice();

            // Target Create
            TestDiProviderBuilder builder =
                new TestDiProviderBuilder()
                .AddConfigure("RemoteParameter", "isl.exe --session-code {0}.");

            // Mock : デバイス取得に成功
            SetMock_ReadDevice_Returns(GotDevice, builder);

            // Mock : メッセージ送信時に例外
            SetMock_SendMessage_Thrown(exception, builder);

            var service = builder.Build().GetService<IDeviceService>();
            var request = new RequestRemote();

            var result = await service.RequestRemoteAsync(request);
            Assert.Fail();
        }

        private static void SetMock_ReadDevice_Returns(DtDevice result, TestDiProviderBuilder builder)
        {
            var mock = new Mock<IDtDeviceRepository>();
            mock.Setup(x => x.ReadDtDevice(It.IsAny<long>()))
                .Returns(result);
            builder.ServiceCollection.AddTransient<IDtDeviceRepository>(_ => mock.Object);
        }
        private static void SetMock_ReadDevice_Thrown(Exception ex, TestDiProviderBuilder builder)
        {
            var mock = new Mock<IDtDeviceRepository>();
            mock.Setup(x => x.ReadDtDevice(It.IsAny<long>()))
                .Throws(ex);
            builder.ServiceCollection.AddTransient<IDtDeviceRepository>(_ => mock.Object);
        }

        private static void SetMock_SendMessage_Returns(bool result, TestDiProviderBuilder builder)
        {
            var mockOfRequestDevice = new Mock<IRequestDeviceRepository>();
            mockOfRequestDevice.Setup(x => x.SendMessageAsync(It.IsAny<DeviceConnectionInfo>(), It.IsAny<string>()));
              //  .ReturnsAsync(result);
            builder.ServiceCollection.AddTransient(_ => mockOfRequestDevice.Object);
        }

        private static void SetMock_SendMessage_Thrown(Exception throwException, TestDiProviderBuilder builder)
        {
            var mockOfRequestDevice = new Mock<IRequestDeviceRepository>();
            mockOfRequestDevice.Setup(x => x.SendMessageAsync(It.IsAny<DeviceConnectionInfo>(), It.IsAny<string>()))
                .Throws(throwException);
            builder.ServiceCollection.AddTransient(_ => mockOfRequestDevice.Object);
        }
    }
}
