using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Core.Azure.Functions.WebApi.Controllers;
using Rms.Server.Core.Azure.Functions.WebApi.Dto;
using Rms.Server.Core.Azure.Functions.WebApi.Utility;
using Rms.Server.Core.Utility.Models;
using System;
using System.Linq;
using TestHelper;

namespace Azure.Functions.WebApiTest
{
    [TestClass]
    public class DeliveryGroupControllerTest
    {
        /// <summary>
        /// 正常なCRUD操作を期待するテスト
        /// </summary>
        [TestMethod]
        [DoNotParallelize]
        public void CRUDOperationOKTest()
        {
            // DbSetup
            DbTestHelper.DeleteAll();
            var dataOnDb = DbTestHelper.CreateMasterTables();
            dataOnDb = DbTestHelper.CreateDeliveries(dataOnDb);
            dataOnDb = DbTestHelper.CreateDevices(dataOnDb);

            // Target Create
            var builder = new TestDiProviderBuilder();
            builder.ServiceCollection.AddTransient<DeliveryGroupsController>();
            var provider = builder.Build();
            var controller = provider.GetService<DeliveryGroupsController>();

            ////////////
            // Create //
            ////////////
            var createActual = new DeliveryGroupAddRequestDto()
            {
                DeliveryFileSid = dataOnDb.GetDtDeliveryFileSid(),
                // DeliveryGroupStatusSid = dataOnDb.GetMtDeliveryGroupStatusSid(),
                Name = "UnitTest",
                StartDatetime = new DateTime(2099, 12, 31),
                // DownloadDelayTime
                DeliveryDestinations = new DeliveryResultAddRequestDto[]
                {
                    new DeliveryResultAddRequestDto()
                    {
                        DeviceSid = dataOnDb.GetDtDeviceSid(),
                        GatewayDeviceSid = dataOnDb.GetDtDeviceSid(),
                    }
                }
            };

            // OKが返るかチェック
            var createResponse = controller.PostDeliveryGroup(createActual, UnitTestHelper.CreateLogger());
            Assert.IsInstanceOfType(createResponse, typeof(OkObjectResult));

            // OKなら型変換
            var createResponseValue = (createResponse as OkObjectResult).Value as DeliveryGroupResponseDto;

            // レスポンス値チェック(追加データと変更がないこと)
            Assert.IsNotNull(createResponseValue);
            Assert.AreEqual(createActual.DeliveryFileSid, createResponseValue.DeliveryFileSid);
            Assert.AreEqual(createActual.Name, createResponseValue.Name);
            Assert.AreEqual(createActual.StartDatetime, createResponseValue.StartDatetime);
            //Assert.AreEqual(createActual.DeliveryDestinations[0].DeviceSid, createResponseValue.DeliveryDestinations[0].DeviceSid);
            //Assert.AreEqual(createActual.DeliveryDestinations[0].GatewayDeviceSid, createResponseValue.DeliveryDestinations[0].GatewayDeviceSid);


            //////////
            // Read //
            //////////
            // ReadはWebAPIがないのでリポジトリから直接確認。
            var groupRepository = provider.GetRequiredService<IDtDeliveryGroupRepository>();
            var readActual = groupRepository.ReadDtDeliveryGroup(createResponseValue.Sid);

            // 取得値チェック(追加データレスポンス値と差異がないこと)
            var entity = readActual;
            Assert.IsNotNull(entity);
            Assert.AreEqual(createResponseValue.Sid, entity.Sid);
            Assert.AreEqual(createResponseValue.DeliveryFileSid, entity.DeliveryFileSid);
            Assert.AreEqual(createResponseValue.DeliveryGroupStatusSid, entity.DeliveryGroupStatusSid);
            Assert.AreEqual(createResponseValue.Name, entity.Name);
            Assert.AreEqual(createResponseValue.StartDatetime, entity.StartDatetime);
            Assert.AreEqual(createResponseValue.DownloadDelayTime, entity.DownloadDelayTime);
            Assert.AreEqual(createResponseValue.CreateDatetime, entity.CreateDatetime);
            Assert.AreEqual(createResponseValue.UpdateDatetime, entity.UpdateDatetime);
            Assert.AreEqual(createResponseValue.RowVersion, WebApiHelper.ConvertByteArrayToLong(entity.RowVersion));
            Assert.AreEqual(createResponseValue.DeliveryDestinations[0].DeviceSid, entity.DtDeliveryResult.ToList()[0].DeviceSid);
            Assert.AreEqual(createResponseValue.DeliveryDestinations[0].GatewayDeviceSid, entity.DtDeliveryResult.ToList()[0].GwDeviceSid);

            ////////////
            // Update //
            ////////////
            var updateActual = new DeliveryGroupUpdateRequestDto()
            {
                Name = "updateExpectedName",
                StartDatetime = new DateTime(2050, 6, 1),
                // DownloadDelayTime
                RowVersion = createResponseValue.RowVersion
            };

            // OKが返るかチェック
            var updateResponse =
                controller.PutDeliveryGroup(updateActual, createResponseValue.Sid, UnitTestHelper.CreateLogger());
            Assert.IsInstanceOfType(updateResponse, typeof(OkObjectResult));

            // OKなら型変換
            var updateResponseValue = (updateResponse as OkObjectResult).Value as DeliveryGroupResponseDto;

            // レスポンス値チェック(更新データに変更がないこと)
            Assert.IsNotNull(updateResponseValue);
            Assert.AreEqual(createResponseValue.Sid, updateResponseValue.Sid);
            Assert.AreEqual(updateActual.Name, updateResponseValue.Name);
            Assert.AreEqual(updateActual.StartDatetime, updateResponseValue.StartDatetime);

            // RowVersionは変更されていること
            Assert.AreNotEqual(updateActual.RowVersion.Value, updateResponseValue.RowVersion);

            // 更新値になっているかDBデータをチェックする
            var updatedReadActual = groupRepository.ReadDtDeliveryGroup(updateResponseValue.Sid);

            // 取得値チェック(更新データと差異がないこと)
            var updatedEntity = updatedReadActual;
            Assert.IsNotNull(updatedEntity);
            Assert.AreEqual(updateResponseValue.Name, updatedEntity.Name);
            Assert.AreEqual(updateResponseValue.StartDatetime, updatedEntity.StartDatetime);
            //Assert.AreEqual(updateResponseValue.UpdatedAtUtc, updatedEntity.UpdateDatetime);
            Assert.AreEqual(updateResponseValue.RowVersion, WebApiHelper.ConvertByteArrayToLong(updatedEntity.RowVersion));

            // その他項目は作成したデータのままであること
            Assert.AreEqual(createResponseValue.Sid, updatedEntity.Sid);
            Assert.AreEqual(createResponseValue.DeliveryFileSid, updatedEntity.DeliveryFileSid);
            Assert.AreEqual(createResponseValue.DeliveryGroupStatusSid, updatedEntity.DeliveryGroupStatusSid);
            Assert.AreEqual(createResponseValue.DownloadDelayTime, updatedEntity.DownloadDelayTime);
            //Assert.AreEqual(createResponseValue.CreatedAtUtc, updatedEntity.CreateDatetime);
            //Assert.AreEqual(createResponseValue.DeliveryDestinations[0].DeviceSid, updatedEntity.DtDeliveryResult.ToList()[0].DeviceSid);
            //Assert.AreEqual(createResponseValue.DeliveryDestinations[0].GatewayDeviceSid, updatedEntity.DtDeliveryResult.ToList()[0].GwDeviceSid);


            ////////////
            // Delete //
            ////////////
            // OKが返るかチェック
            var deleteResponse =
                controller.DeleteDeliveryGroup(null, updateResponseValue.Sid, updateResponseValue.RowVersion, UnitTestHelper.CreateLogger());
            Assert.IsInstanceOfType(deleteResponse, typeof(OkObjectResult));

            // OKなら型変換
            var deleteResponseValue = (deleteResponse as OkObjectResult).Value as DeliveryGroupResponseDto;

            // レスポンス値チェック(指定データに変更がないこと)
            Assert.IsNotNull(deleteResponseValue);
            Assert.AreEqual(updateResponseValue.Sid, deleteResponseValue.Sid);
            Assert.AreEqual(updateResponseValue.RowVersion, deleteResponseValue.RowVersion);

            // NotFoundになっているかDBデータをチェックする
            var deletedReadActual = groupRepository.ReadDtDeliveryGroup(deleteResponseValue.Sid);
            Assert.IsNull(deletedReadActual);
        }

        /// <summary>
        /// 更新時に、配信グループステータスがnotstartでないと403が返ることを期待するテスト
        /// </summary>
        [TestMethod()]
        [DoNotParallelize]
        public void Update403ErrorIfStatusIsNotStarted()
        {
            // DbSetup
            DbTestHelper.DeleteAll();
            var list = DbTestHelper.CreateMasterTables();
            list = DbTestHelper.CreateDeliveries(list);
            list = DbTestHelper.CreateDevices(list);

            // Target Create
            var builder = new TestDiProviderBuilder();
            builder.ServiceCollection.AddTransient<DeliveryGroupsController>();
            var controller = builder.Build().GetService<DeliveryGroupsController>();

            // 配信グループデータを作成し、SIDを読み込む(作成・読み込み結果は別テストで判定するためここでは省略)
            var statusStarted = new DeliveryGroupAddRequestDto()
            {
                DeliveryFileSid = list.GetDtDeliveryFileSid(),
                // これがNotStartではないのが本テストのポイント。
                // HACK: この
                // DeliveryGroupStatusSid = list.GetMtDeliveryGroupStatusSid( "started" ),
                Name = "UnitTest",
                StartDatetime = new DateTime(2099, 12, 31),
                // DownloadDelayTime
                DeliveryDestinations = new DeliveryResultAddRequestDto[] { }
            };
            var createResponse =
                controller.PostDeliveryGroup(statusStarted, UnitTestHelper.CreateLogger())
                as OkObjectResult;
            var createResponseValue = createResponse.Value as DeliveryGroupResponseDto;

            // 更新処理
            var updateActual = new DeliveryGroupUpdateRequestDto()
            {
                Name = "updateExpectedName",
                StartDatetime = new DateTime(2050, 6, 1),
                // DownloadDelayTime
                RowVersion = createResponseValue.RowVersion
            };
            var actual = controller.PutDeliveryGroup(updateActual, createResponseValue.Sid, UnitTestHelper.CreateLogger());

            // 403が返るかチェック
            Assert.IsInstanceOfType(actual, typeof(StatusCodeResult));
            var statucCodeResult = actual as StatusCodeResult;
            Assert.AreEqual(StatusCodes.Status403Forbidden, statucCodeResult.StatusCode);
        }

        /// <summary>
        /// 更新時に、配信グループステータスがnotstartでないと403が返ることを期待するテスト
        /// </summary>
        [TestMethod()]
        [DoNotParallelize]
        public void Update404ErrorIfNonexistentSid()
        {
            // DbSetup
            DbTestHelper.DeleteAll();
            var list = DbTestHelper.CreateMasterTables();
            list = DbTestHelper.CreateDeliveries(list);
            list = DbTestHelper.CreateDevices(list);

            // Target Create
            var builder = new TestDiProviderBuilder();
            builder.ServiceCollection.AddTransient<DeliveryGroupsController>();
            var controller = builder.Build().GetService<DeliveryGroupsController>();

            // 配信グループデータを作成し、SIDを読み込む(作成・読み込み結果は別テストで判定するためここでは省略)
            var statusStarted = new DeliveryGroupAddRequestDto()
            {
                DeliveryFileSid = list.GetDtDeliveryFileSid(),
                // これがNotStartではないのが本テストのポイント。
                // DeliveryGroupStatusSid = list.GetMtDeliveryGroupStatusSid("started"),
                Name = "UnitTest",
                StartDatetime = new DateTime(2099, 12, 31),
                // DownloadDelayTime
                DeliveryDestinations = new DeliveryResultAddRequestDto[] { }
            };
            var createResponse =
                controller.PostDeliveryGroup(statusStarted, UnitTestHelper.CreateLogger())
                as OkObjectResult;
            var createResponseValue = createResponse.Value as DeliveryGroupResponseDto;

            // 更新処理
            var updateActual = new DeliveryGroupUpdateRequestDto()
            {
                Name = "updateExpectedName",
                StartDatetime = new DateTime(2050, 6, 1),
                // DownloadDelayTime
                RowVersion = createResponseValue.RowVersion
            };
            var actual = controller.PutDeliveryGroup(updateActual, createResponseValue.Sid + 1, UnitTestHelper.CreateLogger());

            // 404が返るかチェック
            Assert.IsInstanceOfType(actual, typeof(NotFoundObjectResult));
        }

        /// <summary>
        /// 存在しない配信グループを削除しようとしたときに404が返ることを期待するテスト
        /// </summary>
        [TestMethod()]
        [DoNotParallelize]
        public void Delete404ErrorIfNonexistentSid()
        {
            // Target Create
            var builder = new TestDiProviderBuilder();
            builder.ServiceCollection.AddTransient<DeliveryGroupsController>();
            var controller = builder.Build().GetService<DeliveryGroupsController>();

            long notExistsSid = -1; // 存在しない配信グループのSID 
            long rowVersion = 1;    // 配信ファイルが存在しない場合、RowVersionはチェックされないので任意の値でよい

            // Delete
            var response = controller.DeleteDeliveryGroup(null, notExistsSid, rowVersion, UnitTestHelper.CreateLogger());

            // ステータスが404であることが確認できれば問題ないと判定する
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(NotFoundObjectResult));
            var result = response as NotFoundObjectResult;
            Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
        }

        /// <summary>
        /// CRUD処理Controllerにnullを渡した際の動作確認
        /// </summary>
        [TestMethod]
        public void ParamNullTest()
        {
            // Target Create
            var builder = new TestDiProviderBuilder();
            builder.ServiceCollection.AddTransient<DeliveryGroupsController>();
            var controller = builder.Build().GetService<DeliveryGroupsController>();

            // BadRequestが返るかチェック
            var createResponse =
                controller.PostDeliveryGroup(null, UnitTestHelper.CreateLogger());
            Assert.IsInstanceOfType(createResponse, typeof(BadRequestObjectResult));
            var updateResponse =
                controller.PutDeliveryGroup(null, 0, UnitTestHelper.CreateLogger());
            Assert.IsInstanceOfType(updateResponse, typeof(BadRequestObjectResult));
            
            // DeleteはBadRequestを返さない
        }

        /// <summary>
        /// 必須パラメータがnullのCreateリクエスト投入時のテスト
        /// </summary>
        /// <param name="deliveryFileSid"></param>
        /// <param name="name"></param>
        /// <param name="isNotNullStartDateTime"></param>
        /// <param name="isNotNullDeliveryDestinations"></param>
        [DataTestMethod]
        [DataRow(null, "UnitTest", true, true)]
        [DataRow(1L, null, true, true)]
        [DataRow(1L, "UnitTest", false, true)]
        [DataRow(1L, "UnitTest", true, false)]
        public void CreateParameterInValidParmTest(
            long? deliveryFileSid,
            string name,
            bool isNotNullStartDateTime,
            bool isNotNullDeliveryDestinations)
        {
            // Target Create
            var builder = new TestDiProviderBuilder();
            builder.ServiceCollection.AddTransient<DeliveryGroupsController>();
            var controller = builder.Build().GetService<DeliveryGroupsController>();

            // 配信グループデータ作成パラメータ作成
            var createActual = new DeliveryGroupAddRequestDto()
            {
                DeliveryFileSid = deliveryFileSid,
                // DeliveryGroupStatusSid
                Name = name,
                StartDatetime = isNotNullStartDateTime ? (DateTime?)new DateTime(2099, 12, 31) : null,
                // DownloadDelayTime
                DeliveryDestinations = isNotNullDeliveryDestinations ? new DeliveryResultAddRequestDto[] { } : null
            };

            // 制約違反の場合はBadRequestが返ることを確認する
            var createResponse =
                controller.PostDeliveryGroup(createActual, UnitTestHelper.CreateLogger());
            Assert.IsInstanceOfType(createResponse, typeof(BadRequestObjectResult));
        }

        /// <summary>
        /// 必須パラメータがnullのUpdateリクエスト投入時のテスト
        /// </summary>
        /// <param name="deliveryGroupStatusSid"></param>
        /// <param name="name"></param>
        /// <param name="isNotNullStartDateTime"></param>
        /// <param name="rowVersion"></param>
        [DataTestMethod]
        [DataRow(null, "UnitTest", true, 1L)]
        [DataRow(1L, null, true, 1L)]
        [DataRow(1L, "UnitTest", false, 1L)]
        [DataRow(1L, "UnitTest", true, null)]
        public void UpdateParameterInValidParmTest(
            long? deliveryGroupStatusSid,
            string name,
            bool isNotNullStartDateTime,
            long? rowVersion)
        {
            // Target Create
            var builder = new TestDiProviderBuilder();
            builder.ServiceCollection.AddTransient<DeliveryGroupsController>();
            var controller = builder.Build().GetService<DeliveryGroupsController>();

            // 配信グループデータ更新パラメータ作成
            var updateActual = new DeliveryGroupUpdateRequestDto()
            {
                Name = name,
                StartDatetime = isNotNullStartDateTime ? (DateTime?)new DateTime(2050, 6, 1) : null,
                // DownloadDelayTime
                RowVersion = rowVersion
            };

            // 制約違反の場合はBadRequestが返ることを確認する
            var updateResponse =
                controller.PutDeliveryGroup(updateActual, 1, UnitTestHelper.CreateLogger());
            Assert.IsInstanceOfType(updateResponse, typeof(BadRequestObjectResult));
        }

        // 各エラー(何があったっけ)について

    }
}
