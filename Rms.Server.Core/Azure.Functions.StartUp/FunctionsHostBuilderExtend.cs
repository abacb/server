using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Core.Abstraction.Repositories.Blobs;
using Rms.Server.Core.Service.Services;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Models.Entites;

namespace Rms.Server.Core.Azure.Functions.Startup
{
    /// <summary>
    /// FunctionsApp 共通のスタートアップ処理
    /// </summary>
    public static class FunctionsHostBuilderExtend
    {
        /// <summary>
        /// 共通のDI設定を行う。
        /// </summary>
        /// <param name="builder">ビルダー</param>
        /// <returns>IFunctionsHostBuilder</returns>
        public static IFunctionsHostBuilder AddUtility(IFunctionsHostBuilder builder)
        {
            AddUtility(builder.Services);
            return builder;
        }

        /// <summary>
        /// 共通のDI設定を行う。
        /// </summary>
        /// <param name="services">ビルダー</param>
        public static void AddUtility(IServiceCollection services)
        {
            services.AddSingleton(_ => new AppSettings());

            services.AddSingleton<ITimeProvider, DateTimeProvider>();

            // Blob
            services.AddTransient<ICollectingRepository, CollectingBlobRepository>();
            services.AddTransient<IPrimaryRepository, PrimaryBlobRepository>();
            services.AddTransient<IFailureRepository, FailureBlobRepository>();
            services.AddTransient<PrimaryBlob>();
            services.AddTransient<CollectingBlob>();
            services.AddTransient<DeliveringBlob>();
            services.AddTransient<FailureBlob>();

            // Polly
            services.AddSingleton(s => new BlobPolly(s.GetService<AppSettings>()));
            services.AddSingleton(s => new IotHubPolly(s.GetService<AppSettings>()));
            services.AddSingleton(s => new DpsPolly(s.GetService<AppSettings>()));
            services.AddSingleton(s => new DBPolly(s.GetService<AppSettings>()));

            // Service
            services.AddTransient<ICleanBlobService, CleanBlobService>();
            services.AddTransient<IDeliveryGroupService, DeliveryGroupService>();
            services.AddTransient<IDeliveryFileService, DeliveryFileService>();
            services.AddTransient<IDeviceService, DeviceService>();
            services.AddTransient<IIndexBlobService, IndexBlobService>();
            services.AddTransient<ICleanDbService, CleanDbService>();
            services.AddTransient<IDispatchService, DispatchService>();
            services.AddTransient<IDelivererService, DelivererService>();

            // Repository
            services.AddTransient<IDeliveringRepository, DeliveringBlobRepository>();
            services.AddTransient<IDtDeviceFileRepository, DtDeviceFileRepository>();
            services.AddTransient<IDtDeliveryGroupRepository, DtDeliveryGroupRepository>();
            services.AddTransient<IMtDeliveryGroupStatusRepository, MtDeliveryGroupStatusRepository>();
            services.AddTransient<IMtDeliveryFileTypeRepository, MtDeliveryFileTypeRepository>();
            services.AddTransient<IDtDeliveryFileRepository, DtDeliveryFileRepository>();
            services.AddTransient<IMtConnectStatusRepository, MtConnectStatusRepository>();
            services.AddTransient<IDtDeviceRepository, DtDeviceRepository>();
            services.AddTransient<IMtInstallResultStatusRepository, MtInstallResultStatusRepository>();
            services.AddTransient<IMtEquipmentModelRepository, MtEquipmentModelRepository>();
            services.AddTransient<IMtEquipmentTypeRepository, MtEquipmentTypeRepository>();
            services.AddTransient<IMtInstallTypeRepository, MtInstallTypeRepository>();

            services.AddTransient<IRequestDeviceRepository, RequestDeviceOnDpsRepository>();
            services.AddTransient<IDtInstallResultRepository, DtInstallResultRepository>();

            // Repository(for Dispatcher)
            services.AddTransient<IDtPlusServiceBillLogRepository, DtPlusServiceBillLogRepository>();
            services.AddTransient<IDtDxaBillLogRepository, DtDxaBillLogRepository>();
            services.AddTransient<IDtDxaQcLogRepository, DtDxaQcLogRepository>();
            services.AddTransient<IDtInstallResultRepository, DtInstallResultRepository>();
            services.AddTransient<IDtSoftVersionRepository, DtSoftVersionRepository>();
            services.AddTransient<IDtDirectoryUsageRepository, DtDirectoryUsageRepository>();
            services.AddTransient<IDtDiskDriveRepository, DtDiskDriveRepository>();
            services.AddTransient<IDtEquipmentUsageRepository, DtEquipmentUsageRepository>();
            services.AddTransient<IDtInventoryRepository, DtInventoryRepository>();
            services.AddTransient<IDtDriveRepository, DtDriveRepository>();
            services.AddTransient<IDtParentChildConnectRepository, DtParentChildConnectRepository>();
            services.AddTransient<IRequestDeviceRepository, RequestDeviceOnDpsRepository>();
            services.AddTransient<IDtScriptConfigRepository, DtScriptConfigRepository>();
            services.AddTransient<IDtStorageConfigRepository, DtStorageConfigRepository>();
            services.AddTransient<IFailureRepository, FailureBlobRepository>();
            services.AddTransient<FailureBlob>();

            // Repository(for DbCleaner). ICleanRepositoryの実体。
            services.AddTransient<DtDirectoryUsageRepository>();
            services.AddTransient<DtDiskDriveRepository>();
            services.AddTransient<DtDriveRepository>();
            services.AddTransient<DtDxaBillLogRepository>();
            services.AddTransient<DtDxaQcLogRepository>();
            services.AddTransient<DtEquipmentUsageRepository>();
            services.AddTransient<DtInstallResultRepository>();
            services.AddTransient<DtInventoryRepository>();
            services.AddTransient<DtPlusServiceBillLogRepository>();
            services.AddTransient<DtSoftVersionRepository>();
        }
    }
}