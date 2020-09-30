using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Core.Azure.Functions.WebApi.Controllers;
using Rms.Server.Core.Azure.Functions.WebApi.Dto;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Models;
using System;
using System.Linq;
using TestHelper;
using Rms.Server.Core.Azure.Functions.WebApi.Converter;
using Microsoft.AspNetCore.Http;
using Rms.Server.Core.Azure.Functions.WebApi.Utility;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Rms.Server.Core.Utility.Models.Entites;
using System.Collections.Generic;

namespace Azure.Functions.WebApiTest
{
    [TestClass]
    public class DeliveryFilesControllerTest
    {
        [TestMethod]
        [DoNotParallelize]
        public void CRUDOperationTest()
        {
            // TestInitialize
            DbTestHelper.DeleteAll();
            var dataOnDb = DbTestHelper.CreateMasterTables();
            dataOnDb = DbTestHelper.CreateDevices(dataOnDb);

            // Target Create
            var builder = new TestDiProviderBuilder();
            builder.ServiceCollection.AddTransient<DeliveryFilesController>();

            var createdDatetime = new DateTime(2030, 1, 1);
            builder.ServiceCollection.AddSingleton<ITimeProvider>(UnitTestHelper.CreateTimeProvider(createdDatetime));
            var provider = builder.Build();
            var controller = provider.GetService<DeliveryFilesController>();

            ////////////
            // Create //
            ////////////
            var requestCreate = new DeliveryFileAddRequestDto()
            {
                DeliveryFileType = 
                    new DeliveryFileTypeMasterDto()
                    {
                        DeliveryFileTypeSid = dataOnDb.Get<MtDeliveryFileType>()[0].Sid,
                        DeliveryFileTypeCode = dataOnDb.Get<MtDeliveryFileType>()[0].Code
                    },
                FilePath = "filepath",
                EquipmentModels = new List<ModelMasterDto>() {
                    new ModelMasterDto(){
                        ModelSid = dataOnDb.Get<MtEquipmentModel>()[0].Sid,
                        ModelCode = dataOnDb.Get<MtEquipmentModel>()[0].Code
                    },
                    new ModelMasterDto(){
                        ModelSid = dataOnDb.Get<MtEquipmentModel>()[1].Sid,
                        ModelCode = dataOnDb.Get<MtEquipmentModel>()[1].Code
                    },
                },
                InstallType =
                    new InstallTypeMasterDto()
                    {
                        InstallTypeSid = dataOnDb.Get<MtInstallType>()[0].Sid,
                        InstallTypeCode = dataOnDb.Get<MtInstallType>()[0].Code
                    },
                Version = "v3.0",
                InstallableVersion = "v1.0,v2.0",
                Description = "description",
                InformationId = "200"
            };

            long targetDeliveryFileSid;
            var response =
                controller.PostDeliveryFile(requestCreate, UnitTestHelper.CreateLogger())
                as OkObjectResult;

            Assert.IsNotNull(response);
            var responseCreate = response.Value as DeliveryFileResponseDto;
            targetDeliveryFileSid = responseCreate.Sid;
            long latestRowVersion = responseCreate.RowVersion.Value;

            // リクエストとレスポンスの比較
            Assert.IsTrue(responseCreate.Sid > 0);
            Assert.AreEqual(requestCreate.DeliveryFileType.DeliveryFileTypeSid, responseCreate.DeliveryFileTypeSid);
            Assert.AreEqual(requestCreate.FilePath, responseCreate.FilePath);
            AssertResponse(requestCreate.EquipmentModels, responseCreate.EquipmentModels, responseCreate.CreateDatetime.Value);
            Assert.AreEqual(requestCreate.InstallType.InstallTypeSid, responseCreate.InstallTypeSid);

            Assert.AreEqual(requestCreate.Version, responseCreate.Version);
            Assert.AreEqual(requestCreate.InstallableVersion, responseCreate.InstallableVersion);
            Assert.AreEqual(requestCreate.Description, responseCreate.Description);
            Assert.AreEqual(requestCreate.InformationId, responseCreate.InformationId);

            Assert.IsFalse(responseCreate.IsCanceled.HasValue);
            Assert.IsNotNull(responseCreate.RowVersion);
            Assert.AreEqual(createdDatetime, responseCreate.CreateDatetime);
            Assert.AreEqual(createdDatetime, responseCreate.UpdateDatetime);

            // ==== Read
            // ReadはWebAPIがないのでリポジトリから直接確認。
            var repository = provider.GetRequiredService<IDtDeliveryFileRepository>();
            var readCreated = repository.ReadDtDeliveryFile(targetDeliveryFileSid);

            latestRowVersion = AssertResponse(responseCreate, readCreated);

            // === Update
            // DateTimeProviderを再注入し、更新時に更新されることを確認する。
            var updatedDateTime = new DateTime(2050, 3, 1);
            builder.ServiceCollection.AddSingleton<ITimeProvider>(UnitTestHelper.CreateTimeProvider(updatedDateTime));
            provider = builder.Build();
            controller = provider.GetService<DeliveryFilesController>();

            // 更新データ作成
            var requestUpdated = new DeliveryFileUpdateRequestDto()
            {
                DeliveryFileType = 
                    new DeliveryFileTypeMasterDto()
                    {
                        DeliveryFileTypeSid = dataOnDb.Get<MtDeliveryFileType>()[1].Sid,
                        DeliveryFileTypeCode = dataOnDb.Get<MtDeliveryFileType>()[1].Code
                    },
                EquipmentModels = new List<ModelMasterDto>() {
                    new ModelMasterDto(){
                        ModelSid = dataOnDb.Get<MtEquipmentModel>()[2].Sid,
                        ModelCode = dataOnDb.Get<MtEquipmentModel>()[2].Code
                    }
                },
                InstallType =
                    new InstallTypeMasterDto()
                    {
                        InstallTypeSid = dataOnDb.Get<MtInstallType>()[1].Sid,
                        InstallTypeCode = dataOnDb.Get<MtInstallType>()[1].Code
                    },
                Version = "v4.0",
                InstallableVersion = "v3.0,v2.0",
                Description = "updated description",
                InformationId = "300",
                RowVersion = latestRowVersion
            };
            var responseUpdated =
                controller.PutDeliveryFile(requestUpdated, targetDeliveryFileSid, UnitTestHelper.CreateLogger())
                as OkObjectResult;

            Assert.IsNotNull(responseUpdated);
            var responseUpdatedDto = responseUpdated.Value as DeliveryFileResponseDto;

            Assert.AreEqual(targetDeliveryFileSid, responseUpdatedDto.Sid);
            Assert.AreEqual(requestUpdated.DeliveryFileType.DeliveryFileTypeSid, responseUpdatedDto.DeliveryFileTypeSid);

            AssertResponse(requestUpdated.EquipmentModels, responseUpdatedDto.EquipmentModels, responseUpdatedDto.UpdateDatetime.Value);

            Assert.AreEqual(requestUpdated.InstallType.InstallTypeSid, responseUpdatedDto.InstallTypeSid);
            Assert.AreEqual(requestUpdated.Version, responseUpdatedDto.Version);
            Assert.AreEqual(requestUpdated.InstallableVersion, responseUpdatedDto.InstallableVersion);
            Assert.AreEqual(requestUpdated.Description, responseUpdatedDto.Description);
            Assert.AreEqual(requestUpdated.InformationId, responseUpdatedDto.InformationId);

            Assert.IsFalse(responseUpdatedDto.IsCanceled.HasValue);
            Assert.IsNotNull(responseUpdatedDto.RowVersion);
            Assert.AreEqual(createdDatetime, responseUpdatedDto.CreateDatetime);
            Assert.AreEqual(updatedDateTime, responseUpdatedDto.UpdateDatetime);

            latestRowVersion = responseUpdatedDto.RowVersion.Value;

            // ==== Read
            var readUpdated = repository.ReadDtDeliveryFile(targetDeliveryFileSid);
            latestRowVersion = AssertResponse(responseUpdatedDto, readUpdated);

            // === Delete
            var deletedResponse = controller.DeleteDeliveryFile(
                null,
                targetDeliveryFileSid,
                latestRowVersion,
                UnitTestHelper.CreateLogger()) as OkObjectResult;

            Assert.IsNotNull(deletedResponse);
            var deleteResponse = deletedResponse.Value as DeliveryFileResponseDto;
            Assert.IsNotNull(deleteResponse);

            // HACK: Deleteの戻り値が一致するかどうかのテスト
            // 子テーブルの削除に失敗する。db側にon delete cascadeを追加する。

            // 取得して存在しないことを確認
            var readResult = repository.ReadDtDeliveryFile(deleteResponse.Sid);
            Assert.IsNull(readResult);
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
            var dataOnDb = DbTestHelper.CreateMasterTables();
            dataOnDb = DbTestHelper.CreateDeliveries(dataOnDb);
            dataOnDb = DbTestHelper.CreateDevices(dataOnDb);

            // Target Create
            var builder = new TestDiProviderBuilder();
            builder.ServiceCollection.AddTransient<DeliveryGroupsController>();
            builder.ServiceCollection.AddTransient<DeliveryFilesController>();
            var provider = builder.Build();

            // Init
            // 配信グループデータを作成し、SIDを読み込む(作成・読み込み結果は別テストで判定するためここでは省略)
            var targetDeliveryFileSid = dataOnDb.GetDtDeliveryFileSid();
            var delGroupController = provider.GetService<DeliveryGroupsController>();

            // 配信グループデータを複数作成する。
            // これがNotStartではないのが本テストのポイント。
            DeliveryGroupAddRequestDto statusStarted = CreateDeliveryGroupAddRequestDto(dataOnDb, targetDeliveryFileSid, "started");
            Assert.IsNotNull(delGroupController.PostDeliveryGroup(statusStarted, UnitTestHelper.CreateLogger()) as OkObjectResult);

            statusStarted = CreateDeliveryGroupAddRequestDto(dataOnDb, targetDeliveryFileSid, Const.DeliveryGroupStatus.NotStarted);
            Assert.IsNotNull(delGroupController.PostDeliveryGroup(statusStarted, UnitTestHelper.CreateLogger()) as OkObjectResult);

            statusStarted = CreateDeliveryGroupAddRequestDto(dataOnDb, targetDeliveryFileSid, Const.DeliveryGroupStatus.NotStarted);
            Assert.IsNotNull(delGroupController.PostDeliveryGroup(statusStarted, UnitTestHelper.CreateLogger()) as OkObjectResult);

            // 更新処理
            // 対象の配信ファイル情報はDbSetupで作成済み
            var createdDeliveryFile = dataOnDb.Get<DtDeliveryFile>().First();
            var rowVersionAtCreated = WebApiHelper.ConvertByteArrayToLong(createdDeliveryFile.RowVersion);

            var requestUpdated = new DeliveryFileUpdateRequestDto()
            {
                DeliveryFileType =
                    new DeliveryFileTypeMasterDto()
                    {
                        DeliveryFileTypeSid = dataOnDb.Get<MtDeliveryFileType>()[0].Sid,
                        DeliveryFileTypeCode = dataOnDb.Get<MtDeliveryFileType>()[0].Code
                    },
                // 本テストの本筋ではないため、コメントアウトのまま。
                //EquipmentModel = "UpdatedHOGEHOGE500",
                //InstallType = "UpdatedType1",
                Version = "v4.0",
                InstallableVersion = "v3.0,v2.0",
                Description = "updated description",
                InformationId = "300",
                RowVersion = rowVersionAtCreated
            };

            var actual = provider
                .GetService<DeliveryFilesController>()
                .PutDeliveryFile(requestUpdated, targetDeliveryFileSid, UnitTestHelper.CreateLogger());

            // 403が返るかチェック
            Assert.IsInstanceOfType(actual, typeof(StatusCodeResult));
            var statucCodeResult = actual as StatusCodeResult;
            Assert.AreEqual(StatusCodes.Status403Forbidden, statucCodeResult.StatusCode);

            // 更新されていないことを確認
            var repository = provider.GetRequiredService<IDtDeliveryFileRepository>();
            var readUpdated = repository.ReadDtDeliveryFile(targetDeliveryFileSid);

            Assert.IsNotNull(readUpdated);

            // RowVersionが更新されていなければ問題ないと判定する。
            var readResponseDto = readUpdated.ConvertUtilityToResponseDto();
            Assert.AreEqual(rowVersionAtCreated, readResponseDto.RowVersion.Value);
        }


        /// <summary>
        /// 存在しない配信ファイルを削除しようとしたときに404が返ることを期待するテスト
        /// </summary>
        [TestMethod()]
        [DoNotParallelize]
        public void Delete404ErrorIfDeliveryFileNotFound()
        {
            // 存在しないレコードを指定するテストなのでDBのセットアップ処理は不要

            // Target Create
            var builder = new TestDiProviderBuilder();
            builder.ServiceCollection.AddTransient<DeliveryFilesController>();
            var provider = builder.Build();
            var controller = provider.GetService<DeliveryFilesController>();

            long notExistsSid = -1; // 存在しない配信ファイルのSID 
            long rowVersion = 1;    // 配信ファイルが存在しない場合、RowVersionはチェックされないので任意の値でよい

            // Delete
            var response = controller.DeleteDeliveryFile(null, notExistsSid, rowVersion, UnitTestHelper.CreateLogger());

            // ステータスが404であることが確認できれば問題ないと判定する
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(NotFoundObjectResult));
            var result = response as NotFoundObjectResult;
            Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
        }

        /// <summary>
        /// DeliveryGroupAddRequestDtoの作成
        /// </summary>
        /// <param name="dataOnDb"></param>
        /// <param name="targetDeliveryFileSid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        private static DeliveryGroupAddRequestDto CreateDeliveryGroupAddRequestDto(InstancesOnDb dataOnDb, long targetDeliveryFileSid, string status)
        {
            var statusStarted = new DeliveryGroupAddRequestDto()
            {
                DeliveryFileSid = targetDeliveryFileSid,
                // DeliveryGroupStatusSid = dataOnDb.GetMtDeliveryGroupStatusSid(status),
                Name = "UnitTest",
                StartDatetime = new DateTime(2099, 12, 31),
                // DownloadDelayTime
                DeliveryDestinations = new DeliveryResultAddRequestDto[] { }
            };
            return statusStarted;
        }

        /// <summary>
        /// Assertを行う。
        /// </summary>
        /// <param name="response"></param>
        /// <param name="readResult"></param>
        /// <returns></returns>
        private static long AssertResponse(DeliveryFileResponseDto response, DtDeliveryFile readResult)
        {
            Assert.IsNotNull(readResult);
            DeliveryFileResponseDto readResponseDto = readResult.ConvertUtilityToResponseDto();

            Assert.AreEqual(response.Sid, readResponseDto.Sid);
            Assert.AreEqual(response.DeliveryFileTypeSid, readResponseDto.DeliveryFileTypeSid);
            Assert.AreEqual(response.FilePath, readResponseDto.FilePath);

            // モデル同士の比較
            var expectedEquipments = response.EquipmentModels;
            var actualEquipments = readResult.DtDeliveryModel;
            var expectedCreateAt = readResponseDto.UpdateDatetime;

            Assert.IsNotNull(expectedEquipments);
            Assert.IsNotNull(actualEquipments);
            Assert.AreEqual(expectedEquipments.Count(), actualEquipments.Count());
            foreach (var expectedModel in expectedEquipments)
            {
                var actualParts = actualEquipments.FirstOrDefault(x => x.Sid == expectedModel.EquipmentModelSid );
                Assert.IsNotNull(actualParts);
                Assert.IsTrue(actualParts.DeliveryFileSid > 0);
                Assert.AreEqual(expectedCreateAt, actualParts.CreateDatetime);
            }

            Assert.AreEqual(response.InstallTypeSid, readResponseDto.InstallTypeSid);
            Assert.AreEqual(response.Version, readResponseDto.Version);
            Assert.AreEqual(response.InstallableVersion, readResponseDto.InstallableVersion);
            Assert.AreEqual(response.Description, readResponseDto.Description);
            Assert.AreEqual(response.InformationId, readResponseDto.InformationId);

            Assert.AreEqual(response.IsCanceled, readResponseDto.IsCanceled);
            Assert.AreEqual(response.RowVersion, readResponseDto.RowVersion);
            Assert.AreEqual(response.CreateDatetime, readResponseDto.CreateDatetime);
            Assert.AreEqual(response.UpdateDatetime, readResponseDto.UpdateDatetime);

            return readResponseDto.RowVersion.Value;
        }

        private static void AssertResponse(IEnumerable<ModelMasterDto> expected, IEnumerable<DeliveryModelDto> actual, DateTime expectedCreateAt)
        {
            Assert.IsNotNull(expected);
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Count(), actual.Count());

            foreach(ModelMasterDto expectedModel in expected)
            {
                DeliveryModelDto actualParts = actual.FirstOrDefault( x => expectedModel.ModelSid == x.EquipmentModelSid );
                Assert.IsNotNull(actualParts);
                Assert.IsTrue(actualParts.EquipmentModelSid > 0);
                Assert.AreEqual(expectedCreateAt, actualParts.CreateDatetime);
            }
        }

        /// <summary>
        /// CRUD処理Controllerにnullを渡した際の動作確認
        /// </summary>
        [TestMethod]
        public void BadRequestIfRequestNull()
        {
            // Target Create
            var builder = new TestDiProviderBuilder();
            builder.ServiceCollection.AddTransient<DeliveryFilesController>();
            var controller = builder.Build().GetService<DeliveryFilesController>();

            // BadRequestが返るかチェック
            var createResponse = controller.PostDeliveryFile(null, UnitTestHelper.CreateLogger());
            Assert.IsInstanceOfType(createResponse, typeof(BadRequestObjectResult));

            var updateResponse = controller.PutDeliveryFile(null, 0, UnitTestHelper.CreateLogger());
            Assert.IsInstanceOfType(updateResponse, typeof(BadRequestObjectResult));

            var updateStatusResponse = controller.PutDeliveryFileStatus(null, 0, UnitTestHelper.CreateLogger());
            Assert.IsInstanceOfType(updateStatusResponse, typeof(BadRequestObjectResult));

            // DeleteはBadRequestを返さない
        }

        /// <summary>
        /// 必須パラメータがnullのCreateリクエスト投入時のテスト
        /// </summary>
        /// <param name="deliveryFileSid"></param>
        /// <param name="groupStatusSid"></param>
        /// <param name="name"></param>
        /// <param name="isNotNullStartDateTime"></param>
        /// <param name="isNotNullDeliveryDestinations"></param>
        [Ignore("仕様が固まっていないためいったんIgnore")]
        [DataTestMethod]
        [DataRow(null, "filepath", "equipmentModel", "installType", "version", "installableVersion", "description", "informationId")]
        [DataRow(1L, null, "equipmentModel", "installType", "version", "installableVersion", "description", "informationId")]
        [DataRow(1L, "filepath", null, "installType", "version", "installableVersion", "description", "informationId")]
        [DataRow(1L, "filepath", "equipmentModel", null, "version", "installableVersion", "description", "informationId")]
        [DataRow(1L, "filepath", "equipmentModel", "installType", null, "installableVersion", "description", "informationId")]
        [DataRow(1L, "filepath", "equipmentModel", "installType", "version", null, "description", "informationId")]
        [DataRow(1L, "filepath", "equipmentModel", "installType", "version", "installableVersion", null, "informationId")]
        [DataRow(1L, "filepath", "equipmentModel", "installType", "version", "installableVersion", "description", null)]
        public void CreateParameterInValidParmTest(
            long? deliveryFileTypeSid,
            string filePath,
            string equipmentModel,
            string installType,
            string version,
            string installableVersion,
            string description,
            string informationId)
        {
            // Target Create
            var builder = new TestDiProviderBuilder();
            builder.ServiceCollection.AddTransient<DeliveryFilesController>();
            var controller = builder.Build().GetService<DeliveryFilesController>();

            // 配信グループデータ作成パラメータ作成
            var createActual = new DeliveryFileAddRequestDto()
            {
                DeliveryFileType = 
                    deliveryFileTypeSid == null ?
                    null :
                    new DeliveryFileTypeMasterDto()
                    {
                        DeliveryFileTypeSid = deliveryFileTypeSid,
                        DeliveryFileTypeCode = "tekitou"
                    },
                FilePath = filePath,
                // HACK: 本当はTypeによって要不要が異なるので、その観点でテストを分けるべき。
                // EquipmentModels = new List<MtEquipmentModel>(){
                // HACK: 本当はTypeによって要不要が異なるので、その観点でテストを分けるべき。
                // InstallType = installType,
                Version = version,
                InstallableVersion = installableVersion,
                Description = description,
                InformationId = informationId
            };

            // 制約違反の場合はBadRequestが返ることを確認する
            var createResponse = controller.PostDeliveryFile(createActual, UnitTestHelper.CreateLogger());
            Assert.IsInstanceOfType(createResponse, typeof(BadRequestObjectResult));
        }

        // HACK: Updateパラメタチェック

        // 各エラーのチェック（あと何があったけ）

    }
}
