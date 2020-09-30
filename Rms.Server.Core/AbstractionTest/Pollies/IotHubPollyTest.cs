using Microsoft.Azure.Devices.Common.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rms.Server.Core.Abstraction.Pollies;
using System;
using System.Threading.Tasks;
using TestHelper;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace AbstractionTest.Repositories
{
    /// <summary>
    /// IotHubPollyTestのテスト
    /// </summary>
    [TestClass]
    public class IotHubPollyTest
    {
        // ＊機能仕様
        // リトライしない条件①：指定のException。
        // 上記に合致しない場合リトライする。
        // リトライに際して、アプリケーション設定からリトライ回数、Delay時間を取得する。
        // これが取得できない場合、デフォルトで動作する。

        #region リトライしないテスト

        [DataTestMethod]
        [DataRow(typeof(ArgumentNullException))]
        [DataRow(typeof(OperationCanceledException))]
        [DataRow(typeof(UnauthorizedException))]
        public void IotHubPollyNoRetryExecuteTest(Type actualEx)
        {
            // Delay時間の指定は、テスト時間を縮めるため
            IotHubPolly target = CreateTestTarget(3, 1);
            var startAt = DateTime.UtcNow;
            try
            {
                // 引数を付けているのはUnauthorizedExceptionは引数つきコンストラクタしかないため。
                target.Execute(() => throw Activator.CreateInstance(actualEx, "message") as Exception);
            }
            catch (Exception)
            {
                var elapsedTime = DateTime.UtcNow - startAt;
                Assert.AreEqual(1, new TimeSpan(0, 0, 2).CompareTo(elapsedTime), $"経過時間：{elapsedTime}");
                return;
            }
            Assert.Fail();
        }

        [DataTestMethod]
        [DataRow(typeof(ArgumentNullException))]
        [DataRow(typeof(OperationCanceledException))]
        [DataRow(typeof(UnauthorizedException))]
        public async Task IotHubPollyNoRetryExecuteAsyncTest(Type actualEx)
        {
            // Delay時間の指定は、テスト時間を縮めるため
            IotHubPolly target = CreateTestTarget(3, 1);
            var startAt = DateTime.UtcNow;
            try
            {
#pragma warning disable 1998
                // 引数を付けているのはUnauthorizedExceptionは引数つきコンストラクタしかないため。
                await target.ExecuteAsync(async () => throw Activator.CreateInstance(actualEx, "message") as Exception);
#pragma warning restore 1998
            }
            catch (Exception)
            {
                var elapsedTime = DateTime.UtcNow - startAt;
                Assert.AreEqual(1, new TimeSpan(0, 0, 2).CompareTo(elapsedTime), $"経過時間：{elapsedTime}");
                return;
            }
            Assert.Fail();
        }

        #endregion リトライしないテスト

        #region リトライするテスト

        [DataTestMethod]
        [DataRow(typeof(IotHubException))]
        public void IotHubPollyRetryExecuteTest(Type actualEx)
        {
            // Delay時間の指定は、テスト時間を縮めるため
            IotHubPolly target = CreateTestTarget(1, 2);
            var startAt = DateTime.UtcNow;
            int execCount = 0;

            try
            {
                // 引数を付けているのはIotHubExceptionは引数つきコンストラクタしかないため。
                target.Execute(() =>
                {
                    execCount++;
                    throw Activator.CreateInstance(actualEx, "message") as Exception;
                });
            }
            catch (Exception)
            {
                // リトライ1回なので、2回実行
                Assert.AreEqual(2, execCount);

                // 2秒(1 * 2)以上経過しているはず。
                var elapsedTime = DateTime.UtcNow - startAt;
                Assert.AreEqual(-1, new TimeSpan(0, 0, 2).CompareTo(elapsedTime), $"経過時間：{elapsedTime}");
                return;
            }
            Assert.Fail();
        }

        [DataTestMethod]
        [DataRow(typeof(IotHubException))]
        public async Task IotHubPollyRetryExecuteAsyncTest(Type actualEx)
        {
            // Delay時間の指定は、テスト時間を縮めるため
            IotHubPolly target = CreateTestTarget(1, 2);
            var startAt = DateTime.UtcNow;
            int execCount = 0;

            try
            {
                // 引数を付けているのはIotHubExceptionは引数つきコンストラクタしかないため。
                await target.ExecuteAsync(() =>
                {
                    execCount++;
                    throw Activator.CreateInstance(actualEx, "message") as Exception;
                });
            }
            catch (Exception)
            {
                // リトライ1回なので、2回実行
                Assert.AreEqual(2, execCount);

                // 2秒(1 * 2)以上経過しているはず。
                var elapsedTime = DateTime.UtcNow - startAt;
                Assert.AreEqual(-1, new TimeSpan(0, 0, 2).CompareTo(elapsedTime), $"経過時間：{elapsedTime}");
                return;
            }
            Assert.Fail();
        }

        #endregion リトライするテスト

        #region アプリケーション設定が存在しない場合デフォルトで動作する

        [TestMethod]
        public void IotHubPollyExecuteDefaultTest()
        {
            // アプリケーション設定がない場合デフォルト設定(3回、3秒)で動作する
            IotHubPolly target = CreateTestTarget();
            var startAt = DateTime.UtcNow;
            int execCount = 0;

            try
            {
                // 引数を付けているのはUnauthorizedExceptionは引数つきコンストラクタしかないため。
                target.Execute(() =>
                {
                    execCount++;
                    throw new IotHubException("message");
                });
            }
            catch (Exception)
            {
                // 初回 + リトライ回数を期待する
                Assert.AreEqual(4, execCount);
                // 18秒(0 + 3 + 6 + 9)以上経過しているはず。
                var elapsedTime = DateTime.UtcNow - startAt;
                Assert.AreEqual(-1, new TimeSpan(0, 0, 18).CompareTo(elapsedTime), $"経過時間：{elapsedTime}");
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public async Task IotHubPollyExecuteAsyncDefaultTest()
        {
            // アプリケーション設定がない場合デフォルト設定(3回、3秒)で動作する
            IotHubPolly target = CreateTestTarget();
            var startAt = DateTime.UtcNow;
            int execCount = 0;

            try
            {
                // 引数を付けているのはUnauthorizedExceptionは引数つきコンストラクタしかないため。
                await target.ExecuteAsync(() =>
                {
                    execCount++;
                    throw new IotHubException("message");
                });
            }
            catch (Exception)
            {
                // 初回 + リトライ回数を期待する
                Assert.AreEqual(4, execCount);
                // 18秒(0 + 3 + 6 + 9)以上経過しているはず。
                var elapsedTime = DateTime.UtcNow - startAt;
                Assert.AreEqual(-1, new TimeSpan(0, 0, 18).CompareTo(elapsedTime), $"経過時間：{elapsedTime}");
                return;
            }
            Assert.Fail();
        }

        #endregion アプリケーション設定が存在しない場合デフォルトで動作する

        private IotHubPolly CreateTestTarget(int? retry = null, int? delaySeconds = null)
        {
            // DI設定
            var builder = new TestDiProviderBuilder();

            if (null != retry)
            {
                builder.AddConfigure("IotHubMaxRetryAttempts", retry.ToString());
            }
            if (null != delaySeconds)
            {
                builder.AddConfigure("IotHubDelayDeltaSeconds", delaySeconds.ToString());
            }
            var provider = builder.Build();
            return provider.GetService<IotHubPolly>();
        }
    }
}
