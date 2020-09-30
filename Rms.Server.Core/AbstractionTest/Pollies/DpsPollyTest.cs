using Microsoft.Azure.Devices.Common.Exceptions;
using Microsoft.Azure.Devices.Provisioning.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rms.Server.Core.Abstraction.Pollies;
using System;
using System.Net;
using System.Threading.Tasks;
using TestHelper;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace AbstractionTest.Repositories
{
    /// <summary>
    /// DpsPollyTest
    /// </summary>
    [TestClass]
    public class DpsPollyTest
    {
        // ＊機能仕様
        // リトライしない条件①：ProvisioningServiceClientHttpExceptionでかつ指定のStatusCode
        // リトライしない条件②：①がinnerExceptionに含まれる
        // 上記に合致しない場合リトライする。
        // リトライに際して、アプリケーション設定からリトライ回数、Delay時間を取得する。
        // これが取得できない場合、デフォルトで動作する。

        #region リトライしないテスト

        [DataTestMethod]
        [DataRow(typeof(ProvisioningServiceClientHttpException), HttpStatusCode.BadRequest)]
        [DataRow(typeof(ProvisioningServiceClientHttpException), HttpStatusCode.Unauthorized)]
        [DataRow(typeof(ProvisioningServiceClientHttpException), HttpStatusCode.NotFound)]
        [DataRow(typeof(IotHubException), null)]
        [DataRow(typeof(ArgumentNullException), null)]
        [DataRow(typeof(ArgumentException), null)]
        public void DpsPollyNoRetryExecuteTest(Type actualEx, HttpStatusCode? code)
        {
            DpsPolly target = CreateTestTarget();
            int execCount = 0;

            try
            {
                target.Execute(() =>
                {
                    execCount++;
                    throw CreateException(actualEx, code);
                });
            }
            catch (Exception)
            {
                Assert.AreEqual(1, execCount);
                return;
            }
            Assert.Fail();
        }

        [DataTestMethod]
        [DataRow(typeof(ProvisioningServiceClientHttpException), HttpStatusCode.BadRequest)]
        [DataRow(typeof(ProvisioningServiceClientHttpException), HttpStatusCode.Unauthorized)]
        [DataRow(typeof(ProvisioningServiceClientHttpException), HttpStatusCode.NotFound)]
        [DataRow(typeof(IotHubException), null)]
        [DataRow(typeof(ArgumentNullException), null)]
        [DataRow(typeof(ArgumentException), null)]
        public async Task DpsPollyNoRetryExecuteAsyncTest(Type actualEx, HttpStatusCode? code)
        {
            DpsPolly target = CreateTestTarget();
            int execCount = 0;

            try
            {
                await target.ExecuteAsync(() =>
                {
                    execCount++;
                    throw CreateException(actualEx, code);
                });
            }
            catch (Exception)
            {
                Assert.AreEqual(1, execCount);
                return;
            }
            Assert.Fail();
        }

        [DataTestMethod]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.Unauthorized)]
        [DataRow(HttpStatusCode.NotFound)]
        public void DpsPollyNoRetryExecuteWithInnerExTest(HttpStatusCode statusCode)
        {
            DpsPolly target = CreateTestTarget();
            var executedCount = 0;
            try
            {
                target.Execute(() => {
                    executedCount++;
                    throw CreateInnerDpsException(statusCode);
                });
            }
            catch (Exception)
            {
                Assert.AreEqual(1, executedCount);
                return;
            }
            Assert.Fail();
        }

        [DataTestMethod]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.Unauthorized)]
        [DataRow(HttpStatusCode.NotFound)]
        public async Task DpsPollyNoRetryExecuteAsyncWithInnerExTest(HttpStatusCode statusCode)
        {
            DpsPolly target = CreateTestTarget();
            var executedCount = 0;
            try
            {
#pragma warning disable 1998
                await target.ExecuteAsync(async () => {
                    executedCount++;
                    throw CreateInnerDpsException(statusCode);
                });
#pragma warning restore 1998
            }
            catch (Exception)
            {
                Assert.AreEqual(1, executedCount);
                return;
            }
            Assert.Fail();
        }

        #endregion リトライしないテスト

        #region リトライするテスト

        [DataTestMethod]
        [DataRow(typeof(ProvisioningServiceClientHttpException), HttpStatusCode.Accepted)]
        [DataRow(typeof(ProvisioningServiceClientHttpException), HttpStatusCode.InternalServerError)]
        [DataRow(typeof(ProvisioningServiceClientHttpException), HttpStatusCode.LengthRequired)]
        [DataRow(typeof(ProvisioningServiceClientHttpException), HttpStatusCode.Forbidden)]
        public void DpsPollyRetryExecuteTest(Type actualEx, HttpStatusCode? code)
        {
            // Delay時間の指定は、テスト時間を縮めるため
            DpsPolly target = CreateTestTarget(1, 2);
            var startAt = DateTime.UtcNow;
            int execCount = 0;

            try
            {
                target.Execute(() =>
                {
                    execCount++;
                    throw CreateException(actualEx, code);
                });
            }
            catch (Exception)
            {
                // リトライ1回なので、2回実行
                Assert.AreEqual(2, execCount);

                // 2秒(1 * 2)以上経過しているはず。
                var elapsedTime = DateTime.UtcNow - startAt;
                // 本来なら期待結果を2にしたいが、微妙な揺らぎで1.999xになることがあるため、1.9とする。
                Assert.AreEqual(-1, TimeSpan.FromSeconds(1.9).CompareTo(elapsedTime), $"経過時間：{elapsedTime}");
                return;
            }
            Assert.Fail();
        }

        [DataTestMethod]
        [DataRow(typeof(ProvisioningServiceClientHttpException), HttpStatusCode.Accepted)]
        [DataRow(typeof(ProvisioningServiceClientHttpException), HttpStatusCode.InternalServerError)]
        [DataRow(typeof(ProvisioningServiceClientHttpException), HttpStatusCode.LengthRequired)]
        [DataRow(typeof(ProvisioningServiceClientHttpException), HttpStatusCode.Forbidden)]
        public async Task DpsPollyRetryExecuteAsyncTest(Type actualEx, HttpStatusCode? code)
        {
            // Delay時間の指定は、テスト時間を縮めるため
            DpsPolly target = CreateTestTarget(1, 2);
            var startAt = DateTime.UtcNow;
            int execCount = 0;

            try
            {
                await target.ExecuteAsync(() =>
                {
                    execCount++;
                    throw CreateException(actualEx, code);
                });
            }
            catch (Exception)
            {
                // リトライ1回なので、2回実行
                Assert.AreEqual(2, execCount);

                // 2秒(1 * 2)以上経過しているはず。
                var elapsedTime = DateTime.UtcNow - startAt;
                // 本来なら期待結果を2にしたいが、微妙な揺らぎで1.999xになることがあるため、1.9とする。
                Assert.AreEqual(-1, TimeSpan.FromSeconds(1.9).CompareTo(elapsedTime), $"経過時間：{elapsedTime}");
                return;
            }
            Assert.Fail();
        }

        #endregion

        #region アプリケーション設定が存在しない場合デフォルトで動作する

        [TestMethod]
        public void DpsPollyExecuteDefaultTest()
        {
            // アプリケーション設定がない場合デフォルト設定(3回、3秒)で動作する
            DpsPolly target = CreateTestTarget();
            var startAt = DateTime.UtcNow;
            int execCount = 0;

            try
            {
                // 引数を付けているのはUnauthorizedExceptionは引数つきコンストラクタしかないため。
                target.Execute(() =>
                {
                    execCount++;
                    throw CreateDpsException(HttpStatusCode.Forbidden);
                });
            }
            catch (Exception)
            {
                // 初回 + リトライ回数を期待する
                Assert.AreEqual(4, execCount);
                // 18秒(0 + 3 + 6 + 9)以上経過しているはず。
                var elapsedTime = DateTime.UtcNow - startAt;
                Assert.AreEqual(-1, TimeSpan.FromSeconds(18).CompareTo(elapsedTime), $"経過時間：{elapsedTime}");
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public async Task DpsPollyExecuteAsyncDefaultTest()
        {
            // アプリケーション設定がない場合デフォルト設定(3回、3秒)で動作する
            DpsPolly target = CreateTestTarget();
            var startAt = DateTime.UtcNow;
            int execCount = 0;

            try
            {
                // 引数を付けているのはUnauthorizedExceptionは引数つきコンストラクタしかないため。
                await target.ExecuteAsync(() =>
                {
                    execCount++;
                    throw CreateDpsException(HttpStatusCode.Forbidden);
                });
            }
            catch (Exception)
            {
                // 初回 + リトライ回数を期待する
                Assert.AreEqual(4, execCount);
                // 18秒(0 + 3 + 6 + 9)以上経過しているはず。
                var elapsedTime = DateTime.UtcNow - startAt;
                Assert.AreEqual(-1, TimeSpan.FromSeconds(18).CompareTo(elapsedTime), $"経過時間：{elapsedTime}");
                return;
            }
            Assert.Fail();
        }
        
        #endregion

        private DpsPolly CreateTestTarget(int? retry = null, int? delaySeconds = null)
        {
            // DI設定
            var builder = new TestDiProviderBuilder();

            if (null != retry)
            {
                builder.AddConfigure("DpsMaxRetryAttempts", retry.ToString());
            }
            if (null != delaySeconds)
            {
                builder.AddConfigure("DpsDelayDeltaSeconds", delaySeconds.ToString());
            }
            var provider = builder.Build();
            return provider.GetService<DpsPolly>();
        }

        private Exception CreateException(Type typeofEx, HttpStatusCode? code = null)
        {
            var ex = Activator.CreateInstance(typeofEx, "message") as Exception;
            if(code != null)
            {
                var property = ex.GetType().GetProperty("StatusCode");
                property.SetValue(ex, code);
            }
            return ex;
        }

        private ProvisioningServiceClientHttpException CreateDpsException(HttpStatusCode code)
        {
            var ex = new ProvisioningServiceClientHttpException();
            var property = ex.GetType().GetProperty("StatusCode");
            property.SetValue(ex, code);
            return ex;
        }

        private Exception CreateInnerDpsException(HttpStatusCode code)
        {
            return new Exception("message", CreateDpsException(code));
        }
    }
}
