using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;

namespace TestHelper
{
    public static class UnitTestHelper
    {
        public static ILogger<T> CreateLogger<T>()
        {
            var mockLogger = new Mock<ILogger<T>>();
            var logger = mockLogger.Object;
            return logger;
        }
        public static ILogger CreateLogger()
        {
            var mockLogger = new Mock<ILogger>();
            var logger = mockLogger.Object;
            return logger;
        }
        public static Rms.Server.Core.Utility.DateTimeProvider CreateTimeProvider(DateTime atUtcNow)
        {
            var timeMock = new Mock<Rms.Server.Core.Utility.DateTimeProvider>();
            timeMock.SetupGet(tp => tp.UtcNow).Returns(atUtcNow);
            var mockTimeProvider = timeMock.Object;
            return mockTimeProvider;
        }

        public static void AssertAreEqual<T>(T expected, T actual)
        {
            Assert.IsNotNull(expected, $"{nameof(expected)} is null.");
            Assert.IsNotNull(actual, $"{nameof(actual)} is null.");

            // HACK: リフレクションのほうが親切だけど。とりあえず雑に。
            Assert.AreEqual(
                JsonConvert.SerializeObject(expected),
                JsonConvert.SerializeObject(actual),
                $"Type is {typeof(T)}.");
        }
    }
}
