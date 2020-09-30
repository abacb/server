using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Core.Service.Models;
using Rms.Server.Core.Service.Services;
using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Utility.Models.Dispatch;
using Rms.Server.Core.Utility.Models.Entites;
using System;
using System.Threading.Tasks;
using TestHelper;
using static Rms.Server.Core.Service.Services.DeviceService;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ServiceTest.Services
{
    [TestClass]
    public class DispatchServiceTest
    {
        // テスト設計
        // ・正常系
        //      ・DBから取得したエッジIDに対して処理を行う。
        //      ・設定から取得したメッセージを送る。
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

            var builder = new TestDiProviderBuilder();
            IDispatchService testTarget = builder.Build().GetService<IDispatchService>();
            // testTarget.StoreDXABillingLog(); 

            ////HACK:下記はWarning除け用の仮コードです。
            Guid guid = Guid.NewGuid();
            DateTime eventTime = default(DateTime);
            await testTarget.StoreDeviceConnected(guid, eventTime);
        }
    }
}
