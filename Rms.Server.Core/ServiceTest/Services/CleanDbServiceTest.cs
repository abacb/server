using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rms.Server.Core.Service.Services;
using System.Collections.Generic;
using TestHelper;

namespace ServiceTest
{
    [TestClass]
    public class CleanDbServiceTest
    {
        /// <summary>
        /// 削除対象クラスを1つ指定して実際にDbCleanerで削除されるかを確認する
        /// </summary>
        [TestMethod]
        public void Onepass()
        {
            // DbSetup
            DbTestHelper.DeleteAll();
            var dataOnDb = DbTestHelper.CreateMasterTables();
            dataOnDb = DbTestHelper.CreateDeliveries(dataOnDb);
            dataOnDb = DbTestHelper.CreateDevices(dataOnDb);

            // Target Create
            var builder = new TestDiProviderBuilder();
            builder.ServiceCollection.AddTransient<CleanDbService>();
            var provider = builder.Build();
            var service = provider.GetService<CleanDbService>();

            // 削除対象Insert
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("DeviceSid", dataOnDb.GetDtDeviceSid());
            dic.Add("CreateDatetime", "2019/12/31 23:59:59");
            dic.Add("IsLatest", true);
            DbTestHelper.ExecuteInsertSqlCommand("DtInventory", dic);

            /* SSMSなどで登録されているかチェック */

            // DbCleaner削除対象設定
            var diBuilder = new TestDiProviderBuilder();
            diBuilder.AddConfigure("DbCleanTarget_DtInventory", "1");

            // DbCleaner実行
            service.Clean();

            /* SSMSなどで削除されているかチェック */
        }
    }
}
