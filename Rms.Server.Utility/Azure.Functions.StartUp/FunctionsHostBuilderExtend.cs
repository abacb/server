using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Core.Abstraction.Repositories.Blobs;
using Rms.Server.Core.Utility;
using Rms.Server.Utility.Abstraction.Repositories;
using Rms.Server.Utility.Service.Services;
using Rms.Server.Utility.Utility;

namespace Rms.Server.Utility.Azure.Functions.StartUp
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
            services.AddSingleton(_ => new UtilityAppSettings());
            services.AddSingleton<AppSettings>(s => s.GetService<UtilityAppSettings>());

            services.AddTransient<ITimeProvider, DateTimeProvider>();

            // Blob
            services.AddTransient<IFailureRepository, FailureBlobRepository>();
            services.AddTransient<FailureBlob>();

            // Polly
            services.AddSingleton(s => new BlobPolly(s.GetService<AppSettings>()));
            services.AddSingleton(s => new QueuePolly(s.GetService<UtilityAppSettings>()));

            // Service
            services.AddTransient<IFailureMonitorService, FailureMonitorService>();
            services.AddTransient<IFailurePremonitorService, FailurePremonitorService>();
            services.AddTransient<IPanelDefectPremonitorService, PanelDefectPremonitorService>();
            services.AddTransient<ITemperatureSensorMonitorService, TemperatureSensorMonitorService>();
            services.AddTransient<ITubeDeteriorationPremonitorService, TubeDeteriorationPremonitorService>();
            services.AddTransient<IPanelDischargeBreakdownPremonitorService, PanelDischargeBreakdownPremonitorService>();
            services.AddTransient<ITubeCurrentDeteriorationPremonitorService, TubeCurrentDeteriorationPremonitorService>();
            services.AddTransient<ICalibrationPremonitorService, CalibrationPremonitorService>();
            services.AddTransient<IDipAlmiLogAnalyzerService, DipAlmiLogAnalyzerService>();
            services.AddTransient<IDipBlocLogAnalyzerService, DipBlocLogAnalyzerService>();
            services.AddTransient<IDirectoryUsageMonitorService, DirectoryUsageMonitorService>();
            services.AddTransient<IInstallMonitorService, InstallMonitorService>();
            services.AddTransient<IDiskDrivePremonitorService, DiskDrivePremonitorService>();
            services.AddTransient<IParentChildrenConnectionMonitorService, ParentChildrenConnectionMonitorService>();
            services.AddTransient<IDeviceConnectionMonitorService, DeviceConnectionMonitorService>();
            services.AddTransient<IDipAlmiLogPremonitorService, DipAlmiLogPremonitorService>();
            services.AddTransient<Core.Service.Services.ICleanDbService, CleanDbService>();

            // DB
            services.AddSingleton(s => new DBPolly(s.GetService<AppSettings>()));

            // Repository
            services.AddTransient<IDtAlarmDefFailureMonitorRepository, DtAlarmDefFailureMonitorRepository>();
            services.AddTransient<IDtAlarmDefFailurePremonitorRepository, DtAlarmDefFailurePremonitorRepository>();
            services.AddTransient<IDtAlarmDefPanelDefectPremonitorRepository, DtAlarmDefPanelDefectPremonitorRepository>();
            services.AddTransient<IDtAlarmDefTemperatureSensorLogMonitorRepository, DtAlarmDefTemperatureSensorLogMonitorRepository>();
            services.AddTransient<IDtAlarmDefTubeDeteriorationPremonitorRepository, DtAlarmDefTubeDeteriorationPremonitorRepository>();
            services.AddTransient<IDtAlarmDefPanelDischargeBreakdownPremonitorRepository, DtAlarmDefPanelDischargeBreakdownPremonitorRepository>();
            services.AddTransient<IDtAlarmDefTubeCurrentDeteriorationPremonitorRepository, DtAlarmDefTubeCurrentDeteriorationPremonitorRepository>();
            services.AddTransient<IDtAlarmDefCalibrationPremonitorRepository, DtAlarmDefCalibrationPremonitorRepository>();
            services.AddTransient<IDtAlmilogAnalysisResultRepository, DtAlmilogAnalysisResultRepository>();
            services.AddTransient<IDtAlmilogAnalysisConfigRepository, DtAlmilogAnalysisConfigRepository>();
            services.AddTransient<IDtBloclogAnalysisResultRepository, DtBloclogAnalysisResultRepository>();
            services.AddTransient<IDtBloclogAnalysisConfigRepository, DtBloclogAnalysisConfigRepository>();
            services.AddTransient<IDtAlarmDefDirectoryUsageMonitorRepository, DtAlarmDefDirectoryUsageMonitorRepository>();
            services.AddTransient<IDtAlarmDefInstallResultMonitorRepository, DtAlarmDefInstallResultMonitorRepository>();
            services.AddTransient<IDtAlarmSmartPremonitorRepository, DtAlarmSmartPremonitorRepository>();
            services.AddTransient<IDtSmartAnalysisResultRepository, DtSmartAnalysisResultRepository>();
            services.AddTransient<IDtAlarmDefConnectionMonitorRepository, DtAlarmDefConnectionMonitorRepository>();
            services.AddTransient<Abstraction.Repositories.IDtParentChildConnectRepository, Abstraction.Repositories.DtParentChildConnectRepository>();
            services.AddTransient<Abstraction.Repositories.IDtDeviceRepository, Abstraction.Repositories.DtDeviceRepository>();
            services.AddTransient<IDtAlmilogPremonitorRepository, DtAlmilogPremonitorRepository>();

            services.AddTransient<IQueueRepository, QueueRepository>();

            // Repository(for DbCleaner). ICleanRepositoryの実体。
            services.AddTransient<DtAlmilogAnalysisResultRepository>();
            services.AddTransient<DtBloclogAnalysisResultRepository>();
        }
    }
}