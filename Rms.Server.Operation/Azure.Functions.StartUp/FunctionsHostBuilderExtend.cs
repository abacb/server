using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Core.Abstraction.Repositories.Blobs;
using Rms.Server.Core.Utility;
using Rms.Server.Operation.Abstraction.Pollies;
using Rms.Server.Operation.Abstraction.Repositories;
using Rms.Server.Operation.Service.Services;
using Rms.Server.Operation.Utility;

namespace Rms.Server.Operation.Azure.Functions.StartUp
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
            services.AddSingleton(_ => new OperationAppSettings());
            services.AddSingleton<AppSettings>(s => s.GetService<OperationAppSettings>());

            services.AddTransient<ITimeProvider, DateTimeProvider>();

            // Blob
            services.AddTransient<IFailureRepository, FailureBlobRepository>();
            services.AddTransient<FailureBlob>();

            // Polly
            services.AddSingleton(s => new BlobPolly(s.GetService<AppSettings>()));
            services.AddSingleton(s => new QueuePolly(s.GetService<OperationAppSettings>()));
            services.AddSingleton(s => new SendGridPolly(s.GetService<OperationAppSettings>()));

            // Service
            services.AddTransient<IAlarmRegisterService, AlarmRegisterService>();
            services.AddTransient<IMailSenderService, MailSenderService>();
            services.AddTransient<Core.Service.Services.ICleanDbService, CleanDbService>();

            // DB
            services.AddSingleton(s => new DBPolly(s.GetService<AppSettings>()));

            // Repository
            services.AddTransient<IDtAlarmRepository, DtAlarmRepository>();
            services.AddTransient<IDtEquipmentRepository, DtEquipmentRepository>();
            services.AddTransient<IDtInstallBaseRepository, DtInstallBaseRepository>();
            services.AddTransient<IDtAlarmConfigRepository, DtAlarmConfigRepository>();
            services.AddTransient<IQueueRepository, QueueRepository>();
            services.AddTransient<IMailRepository, MailRepository>();

            // Repository(for DbCleaner). ICleanRepositoryの実体。
            services.AddTransient<DtAlarmRepository>();
        }
    }
}
