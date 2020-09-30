using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Rms.Server.Utility.DBAccessor.Models
{
    public partial class RmsDbContext : DbContext
    {
        public RmsDbContext()
        {
        }

        public RmsDbContext(DbContextOptions<RmsDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DtAlarmDefCalibrationPremonitor> DtAlarmDefCalibrationPremonitor { get; set; }
        public virtual DbSet<DtAlarmDefConnectionMonitor> DtAlarmDefConnectionMonitor { get; set; }
        public virtual DbSet<DtAlarmDefDirectoryUsageMonitor> DtAlarmDefDirectoryUsageMonitor { get; set; }
        public virtual DbSet<DtAlarmDefFailureMonitor> DtAlarmDefFailureMonitor { get; set; }
        public virtual DbSet<DtAlarmDefFailurePremonitor> DtAlarmDefFailurePremonitor { get; set; }
        public virtual DbSet<DtAlarmDefInstallResultMonitor> DtAlarmDefInstallResultMonitor { get; set; }
        public virtual DbSet<DtAlarmDefPanelDefectPremonitor> DtAlarmDefPanelDefectPremonitor { get; set; }
        public virtual DbSet<DtAlarmDefPanelDischargeBreakdownPremonitor> DtAlarmDefPanelDischargeBreakdownPremonitor { get; set; }
        public virtual DbSet<DtAlarmDefTemperatureSensorLogMonitor> DtAlarmDefTemperatureSensorLogMonitor { get; set; }
        public virtual DbSet<DtAlarmDefTubeCurrentDeteriorationPremonitor> DtAlarmDefTubeCurrentDeteriorationPremonitor { get; set; }
        public virtual DbSet<DtAlarmDefTubeDeteriorationPremonitor> DtAlarmDefTubeDeteriorationPremonitor { get; set; }
        public virtual DbSet<DtAlarmSmartPremonitor> DtAlarmSmartPremonitor { get; set; }
        public virtual DbSet<DtAlmilogAnalysisConfig> DtAlmilogAnalysisConfig { get; set; }
        public virtual DbSet<DtAlmilogAnalysisResult> DtAlmilogAnalysisResult { get; set; }
        public virtual DbSet<DtAlmilogPremonitor> DtAlmilogPremonitor { get; set; }
        public virtual DbSet<DtBloclogAnalysisConfig> DtBloclogAnalysisConfig { get; set; }
        public virtual DbSet<DtBloclogAnalysisResult> DtBloclogAnalysisResult { get; set; }
        public virtual DbSet<DtSmartAnalysisResult> DtSmartAnalysisResult { get; set; }
        public virtual DbSet<DtDevice> DtDevice { get; set; }
        public virtual DbSet<DtInventory> DtInventory { get; set; }
        public virtual DbSet<DtParentChildConnect> DtParentChildConnect { get; set; }
        public virtual DbSet<MtConnectStatus> MtConnectStatus { get; set; }
        public virtual DbSet<MtInstallType> MtInstallType { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
////#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseLoggerFactory(LoggerFactory).UseSqlServer(_appSettings.PrimaryDbConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DtAlarmDefCalibrationPremonitor>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_ALARM_DEF_CALIBRATION_PREMONITOR", "utility");

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.AlarmDescription)
                    .HasColumnName("ALARM_DESCRIPTION")
                    .HasMaxLength(1024);

                entity.Property(e => e.AlarmLevel).HasColumnName("ALARM_LEVEL");

                entity.Property(e => e.AlarmTitle)
                    .HasColumnName("ALARM_TITLE")
                    .HasMaxLength(200);

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.ErrorCode)
                    .IsRequired()
                    .HasColumnName("ERROR_CODE")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.TypeCode)
                    .IsRequired()
                    .HasColumnName("TYPE_CODE")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");
            });

            modelBuilder.Entity<DtAlarmDefConnectionMonitor>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_ALARM_DEF_CONNECTION_MONITOR", "utility");

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.AlarmDescription)
                    .HasColumnName("ALARM_DESCRIPTION")
                    .HasMaxLength(1024);

                entity.Property(e => e.AlarmLevel).HasColumnName("ALARM_LEVEL");

                entity.Property(e => e.AlarmTitle)
                    .HasColumnName("ALARM_TITLE")
                    .HasMaxLength(200);

                entity.Property(e => e.AnalysisResultErrorCode)
                    .IsRequired()
                    .HasColumnName("ANALYSIS_RESULT_ERROR_CODE")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.ErrorCode)
                    .IsRequired()
                    .HasColumnName("ERROR_CODE")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Target)
                    .IsRequired()
                    .HasColumnName("TARGET")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.TypeCode)
                    .IsRequired()
                    .HasColumnName("TYPE_CODE")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");

                entity.Property(e => e.ValueEqualFrom).HasColumnName("VALUE_EQUAL_FROM");

                entity.Property(e => e.ValueEqualTo).HasColumnName("VALUE_EQUAL_TO");

                entity.Property(e => e.ValueFrom).HasColumnName("VALUE_FROM");

                entity.Property(e => e.ValueTo).HasColumnName("VALUE_TO");
            });

            modelBuilder.Entity<DtAlarmDefDirectoryUsageMonitor>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_ALARM_DEF_DIRECTORY_USAGE_MONITOR", "utility");

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.AlarmDescription)
                    .HasColumnName("ALARM_DESCRIPTION")
                    .HasMaxLength(1024);

                entity.Property(e => e.AlarmLevel).HasColumnName("ALARM_LEVEL");

                entity.Property(e => e.AlarmTitle)
                    .HasColumnName("ALARM_TITLE")
                    .HasMaxLength(200);

                entity.Property(e => e.AnalysisResultErrorCode)
                    .IsRequired()
                    .HasColumnName("ANALYSIS_RESULT_ERROR_CODE")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.DirectoryPath)
                    .IsRequired()
                    .HasColumnName("DIRECTORY_PATH")
                    .HasMaxLength(300);

                entity.Property(e => e.Size).HasColumnName("SIZE");

                entity.Property(e => e.TypeCode)
                    .IsRequired()
                    .HasColumnName("TYPE_CODE")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");
            });

            modelBuilder.Entity<DtAlarmDefFailureMonitor>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_ALARM_DEF_FAILURE_MONITOR", "utility");

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.AlarmDescription)
                    .HasColumnName("ALARM_DESCRIPTION")
                    .HasMaxLength(1024);

                entity.Property(e => e.AlarmLevel).HasColumnName("ALARM_LEVEL");

                entity.Property(e => e.AlarmTitle)
                    .HasColumnName("ALARM_TITLE")
                    .HasMaxLength(200);

                entity.Property(e => e.AnalysisText)
                    .HasColumnName("ANALYSIS_TEXT")
                    .HasMaxLength(300);

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.ErrorCode)
                    .HasColumnName("ERROR_CODE")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.TypeCode)
                    .IsRequired()
                    .HasColumnName("TYPE_CODE")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");
            });

            modelBuilder.Entity<DtAlarmDefFailurePremonitor>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_ALARM_DEF_FAILURE_PREMONITOR", "utility");

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.AlarmDescription)
                    .HasColumnName("ALARM_DESCRIPTION")
                    .HasMaxLength(1024);

                entity.Property(e => e.AlarmLevel).HasColumnName("ALARM_LEVEL");

                entity.Property(e => e.AlarmTitle)
                    .HasColumnName("ALARM_TITLE")
                    .HasMaxLength(200);

                entity.Property(e => e.AnalysisText)
                    .HasColumnName("ANALYSIS_TEXT")
                    .HasMaxLength(300);

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.ErrorCode)
                    .HasColumnName("ERROR_CODE")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.TypeCode)
                    .IsRequired()
                    .HasColumnName("TYPE_CODE")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");
            });

            modelBuilder.Entity<DtAlarmDefInstallResultMonitor>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_ALARM_DEF_INSTALL_RESULT_MONITOR", "utility");

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.AlarmDescription)
                    .HasColumnName("ALARM_DESCRIPTION")
                    .HasMaxLength(1024);

                entity.Property(e => e.AlarmLevel).HasColumnName("ALARM_LEVEL");

                entity.Property(e => e.AlarmTitle)
                    .HasColumnName("ALARM_TITLE")
                    .HasMaxLength(200);

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.ErrorCode)
                    .HasColumnName("ERROR_CODE")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.IsAuto).HasColumnName("IS_AUTO");

                entity.Property(e => e.IsSuccess).HasColumnName("IS_SUCCESS");

                entity.Property(e => e.Process)
                    .HasColumnName("PROCESS")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.TypeCode)
                    .IsRequired()
                    .HasColumnName("TYPE_CODE")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");
            });

            modelBuilder.Entity<DtAlarmDefPanelDefectPremonitor>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_ALARM_DEF_PANEL_DEFECT_PREMONITOR", "utility");

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.AlarmDescription)
                    .HasColumnName("ALARM_DESCRIPTION")
                    .HasMaxLength(1024);

                entity.Property(e => e.AlarmLevel).HasColumnName("ALARM_LEVEL");

                entity.Property(e => e.AlarmTitle)
                    .HasColumnName("ALARM_TITLE")
                    .HasMaxLength(200);

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.ErrorCode)
                    .IsRequired()
                    .HasColumnName("ERROR_CODE")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.TypeCode)
                    .IsRequired()
                    .HasColumnName("TYPE_CODE")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");
            });

            modelBuilder.Entity<DtAlarmDefPanelDischargeBreakdownPremonitor>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_ALARM_DEF_PANEL_DISCHARGE_BREAKDOWN_PREMONITOR", "utility");

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.AlarmDescription)
                    .HasColumnName("ALARM_DESCRIPTION")
                    .HasMaxLength(1024);

                entity.Property(e => e.AlarmLevel).HasColumnName("ALARM_LEVEL");

                entity.Property(e => e.AlarmTitle)
                    .HasColumnName("ALARM_TITLE")
                    .HasMaxLength(200);

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.ErrorCode)
                    .IsRequired()
                    .HasColumnName("ERROR_CODE")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.TypeCode)
                    .IsRequired()
                    .HasColumnName("TYPE_CODE")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");
            });

            modelBuilder.Entity<DtAlarmDefTemperatureSensorLogMonitor>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_ALARM_DEF_TEMPERATURE_SENSOR_LOG_MONITOR", "utility");

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.AlarmDescription)
                    .HasColumnName("ALARM_DESCRIPTION")
                    .HasMaxLength(1024);

                entity.Property(e => e.AlarmLevel).HasColumnName("ALARM_LEVEL");

                entity.Property(e => e.AlarmTitle)
                    .HasColumnName("ALARM_TITLE")
                    .HasMaxLength(200);

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.ErrorCode)
                    .HasColumnName("ERROR_CODE")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.TypeCode)
                    .IsRequired()
                    .HasColumnName("TYPE_CODE")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");
            });

            modelBuilder.Entity<DtAlarmDefTubeCurrentDeteriorationPremonitor>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_ALARM_DEF_TUBE_CURRENT_DETERIORATION_PREMONITOR", "utility");

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.AlarmDescription)
                    .HasColumnName("ALARM_DESCRIPTION")
                    .HasMaxLength(1024);

                entity.Property(e => e.AlarmLevel).HasColumnName("ALARM_LEVEL");

                entity.Property(e => e.AlarmTitle)
                    .HasColumnName("ALARM_TITLE")
                    .HasMaxLength(200);

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.ErrorCode)
                    .IsRequired()
                    .HasColumnName("ERROR_CODE")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.TypeCode)
                    .IsRequired()
                    .HasColumnName("TYPE_CODE")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");
            });

            modelBuilder.Entity<DtAlarmDefTubeDeteriorationPremonitor>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_ALARM_DEF_TUBE_DETERIORATION_PREMONITOR", "utility");

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.AlarmDescription)
                    .HasColumnName("ALARM_DESCRIPTION")
                    .HasMaxLength(1024);

                entity.Property(e => e.AlarmLevel).HasColumnName("ALARM_LEVEL");

                entity.Property(e => e.AlarmTitle)
                    .HasColumnName("ALARM_TITLE")
                    .HasMaxLength(200);

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.ErrorCode)
                    .IsRequired()
                    .HasColumnName("ERROR_CODE")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.TypeCode)
                    .IsRequired()
                    .HasColumnName("TYPE_CODE")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");
            });

            modelBuilder.Entity<DtAlarmSmartPremonitor>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_ALARM_SMART_PREMONITOR", "utility");

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.AlarmDescription)
                    .HasColumnName("ALARM_DESCRIPTION")
                    .HasMaxLength(1024);

                entity.Property(e => e.AlarmLevel).HasColumnName("ALARM_LEVEL");

                entity.Property(e => e.AlarmTitle)
                    .HasColumnName("ALARM_TITLE")
                    .HasMaxLength(200);

                entity.Property(e => e.AnalysisResultErrorCode)
                    .IsRequired()
                    .HasColumnName("ANALYSIS_RESULT_ERROR_CODE")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ChangeCountThreshold)
                    .IsRequired()
                    .HasColumnName("CHANGE_COUNT_THRESHOLD")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.SmartId)
                    .IsRequired()
                    .HasColumnName("SMART_ID")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");
            });

            modelBuilder.Entity<DtAlmilogAnalysisConfig>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_ALMILOG_ANALYSIS_CONFIG", "utility");

                entity.HasIndex(e => new { e.DetectorName, e.IsNormalized })
                    .HasName("UNQ_DT_ALMILOG_ANALYSIS_CONFIG_DETECTOR_NAME_IS_NORMALIZED")
                    .IsUnique();

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.AreaStandardData)
                    .IsRequired()
                    .HasColumnName("AREA_STANDARD_DATA")
                    .HasMaxLength(700);

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.DetectorName)
                    .IsRequired()
                    .HasColumnName("DETECTOR_NAME")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.HighVoltageAreaValue).HasColumnName("HIGH_VOLTAGE_AREA_VALUE");

                entity.Property(e => e.IsNormalized).HasColumnName("IS_NORMALIZED");

                entity.Property(e => e.LowVoltageAreaValue).HasColumnName("LOW_VOLTAGE_AREA_VALUE");

                entity.Property(e => e.MaxSlopeValue).HasColumnName("MAX_SLOPE_VALUE");

                entity.Property(e => e.MiddleSlopeValue).HasColumnName("MIDDLE_SLOPE_VALUE");

                entity.Property(e => e.MinSlopeValue).HasColumnName("MIN_SLOPE_VALUE");

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");
            });

            modelBuilder.Entity<DtAlmilogAnalysisResult>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_ALMILOG_ANALYSIS_RESULT", "utility");

                entity.HasIndex(e => e.LogFileName)
                    .HasName("UNQ_DT_ALMILOG_ANALYSIS_RESULT_LOG_FILE_NAME")
                    .IsUnique();

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.AlmilogMonth)
                    .HasColumnName("ALMILOG_MONTH")
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.AnalysisResult)
                    .HasColumnName("ANALYSIS_RESULT")
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.CalculateAreaValue).HasColumnName("CALCULATE_AREA_VALUE");

                entity.Property(e => e.CalculateInclinationValue).HasColumnName("CALCULATE_INCLINATION_VALUE");

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.DetectorId)
                    .HasColumnName("DETECTOR_ID")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.DetectorName)
                    .HasColumnName("DETECTOR_NAME")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.EquipmentUid)
                    .IsRequired()
                    .HasColumnName("EQUIPMENT_UID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.FileNameNo).HasColumnName("FILE_NAME_NO");

                entity.Property(e => e.GpValue).HasColumnName("GP_VALUE");

                entity.Property(e => e.ImageFileName)
                    .HasColumnName("IMAGE_FILE_NAME")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.IsAlarmJudged).HasColumnName("IS_ALARM_JUDGED");

                entity.Property(e => e.IsBillTarget).HasColumnName("IS_BILL_TARGET");

                entity.Property(e => e.LogFileName)
                    .HasColumnName("LOG_FILE_NAME")
                    .HasMaxLength(64);

                entity.Property(e => e.MaxInclinationValue).HasColumnName("MAX_INCLINATION_VALUE");

                entity.Property(e => e.MinInclinationValue).HasColumnName("MIN_INCLINATION_VALUE");

                entity.Property(e => e.ReverseResult)
                    .HasColumnName("REVERSE_RESULT")
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.ReverseResultInclination).HasColumnName(@"REVERSE_RESULT_INCLINATION");

                entity.Property(e => e.StandardAreaValue).HasColumnName("STANDARD_AREA_VALUE");
            });

            modelBuilder.Entity<DtAlmilogPremonitor>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_ALMILOG_PREMONITOR", "utility");

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.AlarmDescription)
                    .HasColumnName("ALARM_DESCRIPTION")
                    .HasMaxLength(1024);

                entity.Property(e => e.AlarmLevel).HasColumnName("ALARM_LEVEL");

                entity.Property(e => e.AlarmTitle)
                    .HasColumnName("ALARM_TITLE")
                    .HasMaxLength(200);

                entity.Property(e => e.AnalysisResultErrorCode)
                    .IsRequired()
                    .HasColumnName("ANALYSIS_RESULT_ERROR_CODE")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.DetectorName)
                    .IsRequired()
                    .HasColumnName("DETECTOR_NAME")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.JudgeResult)
                    .IsRequired()
                    .HasColumnName("JUDGE_RESULT")
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");
            });

            modelBuilder.Entity<DtBloclogAnalysisConfig>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_BLOCLOG_ANALYSIS_CONFIG", "utility");

                entity.HasIndex(e => e.IsNormalized)
                    .HasName("UNQ_DT_BLOCLOG_ANALYSIS_CONFIG_IS_NORMALIZED")
                    .IsUnique();

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.AlsStandardValue).HasColumnName("ALS_STANDARD_VALUE");

                entity.Property(e => e.BottomUnevennessSkipValue).HasColumnName("BOTTOM_UNEVENNESS_SKIP_VALUE");

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.IsNormalized).HasColumnName("IS_NORMALIZED");

                entity.Property(e => e.McvStandardValue).HasColumnName("MCV_STANDARD_VALUE");

                entity.Property(e => e.ScvStandardValue1).HasColumnName("SCV_STANDARD_VALUE_1");

                entity.Property(e => e.ScvStandardValue2).HasColumnName("SCV_STANDARD_VALUE_2");

                entity.Property(e => e.TopUnevennessSkipValue).HasColumnName("TOP_UNEVENNESS_SKIP_VALUE");

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");
            });

            modelBuilder.Entity<DtBloclogAnalysisResult>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_BLOCLOG_ANALYSIS_RESULT", "utility");

                entity.HasIndex(e => e.LogFileName)
                    .HasName("UNQ_DT_BLOCLOG_ANALYSIS_RESULT_LOG_FILE_NAME")
                    .IsUnique();

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.BloclogMonth)
                    .HasColumnName("BLOCLOG_MONTH")
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.DetectorId)
                    .HasColumnName("DETECTOR_ID")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.DetectorName)
                    .HasColumnName("DETECTOR_NAME")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.EquipmentUid)
                    .IsRequired()
                    .HasColumnName("EQUIPMENT_UID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.FileNameNo).HasColumnName("FILE_NAME_NO");

                entity.Property(e => e.GpValue).HasColumnName("GP_VALUE");

                entity.Property(e => e.ImageFileName)
                    .HasColumnName("IMAGE_FILE_NAME")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.ImageSize).HasColumnName("IMAGE_SIZE");

                entity.Property(e => e.ImageType).HasColumnName("IMAGE_TYPE");

                entity.Property(e => e.IsBillTarget).HasColumnName("IS_BILL_TARGET");

                entity.Property(e => e.LogFileName)
                    .HasColumnName("LOG_FILE_NAME")
                    .HasMaxLength(64);

                entity.Property(e => e.ShadingResult)
                    .HasColumnName("SHADING_RESULT")
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.ShadingResultMcv).HasColumnName("SHADING_RESULT_MCV");

                entity.Property(e => e.ShadingResultMcvSv).HasColumnName("SHADING_RESULT_MCV_SV");

                entity.Property(e => e.ShadingResultScv).HasColumnName("SHADING_RESULT_SCV");

                entity.Property(e => e.ShadingResultScvSv1).HasColumnName("SHADING_RESULT_SCV_SV_1");

                entity.Property(e => e.ShadingResultScvSv2).HasColumnName("SHADING_RESULT_SCV_SV_2");
            });

            modelBuilder.Entity<DtSmartAnalysisResult>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_SMART_ANALYSIS_RESULT", "utility");

                entity.HasIndex(e => new { e.EquipmentUid, e.DiskSerialNumber })
                    .HasName("UNQ_DT_SMART_ANALYSIS_RESULT_EQUIPMENT_UID_DISK_SERIAL_NUMBER")
                    .IsUnique();

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.C5ChangesThreshhold).HasColumnName("C5_CHANGES_THRESHHOLD");

                entity.Property(e => e.C5ChangesThreshholdLastDatetime).HasColumnName("C5_CHANGES_THRESHHOLD_LAST_DATETIME");

                entity.Property(e => e.C5ChangesThreshholdOverCount).HasColumnName("C5_CHANGES_THRESHHOLD_OVER_COUNT");

                entity.Property(e => e.C5RawData).HasColumnName("C5_RAW_DATA");

                entity.Property(e => e.C5RawDataChanges).HasColumnName("C5_RAW_DATA_CHANGES");

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.DiskSerialNumber)
                    .IsRequired()
                    .HasColumnName("DISK_SERIAL_NUMBER")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.EquipmentUid)
                    .IsRequired()
                    .HasColumnName("EQUIPMENT_UID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");
            });

            modelBuilder.Entity<DtDevice>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_DEVICE", "utility");

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.ConnectStartDatetime).HasColumnName("CONNECT_START_DATETIME");

                entity.Property(e => e.ConnectStatusSid).HasColumnName("CONNECT_STATUS_SID");

                entity.Property(e => e.ConnectUpdateDatetime).HasColumnName("CONNECT_UPDATE_DATETIME");

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.EdgeId).HasColumnName("EDGE_ID");

                entity.Property(e => e.EquipmentModelSid).HasColumnName("EQUIPMENT_MODEL_SID");

                entity.Property(e => e.EquipmentUid)
                    .HasColumnName("EQUIPMENT_UID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.InstallTypeSid).HasColumnName("INSTALL_TYPE_SID");

                entity.Property(e => e.RemoteConnectUid)
                    .HasColumnName("REMOTE_CONNECT_UID")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.RmsSoftVersion)
                    .HasColumnName("RMS_SOFT_VERSION")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");
            });

            modelBuilder.Entity<DtInventory>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_INVENTORY", "utility");

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.CollectDatetime).HasColumnName("COLLECT_DATETIME");

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.DetailInfo).HasColumnName("DETAIL_INFO");

                entity.Property(e => e.DeviceSid).HasColumnName("DEVICE_SID");

                entity.Property(e => e.IsLatest).HasColumnName("IS_LATEST");

                entity.Property(e => e.MessageId)
                    .HasColumnName("MESSAGE_ID")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.SourceEquipmentUid)
                    .HasColumnName("SOURCE_EQUIPMENT_UID")
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DtParentChildConnect>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_PARENT_CHILD_CONNECT", "utility");

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.ChildConfirmDatetime).HasColumnName("CHILD_CONFIRM_DATETIME");

                entity.Property(e => e.ChildDeviceSid).HasColumnName("CHILD_DEVICE_SID");

                entity.Property(e => e.ChildLastConnectDatetime).HasColumnName("CHILD_LAST_CONNECT_DATETIME");

                entity.Property(e => e.ChildResult).HasColumnName("CHILD_RESULT");

                entity.Property(e => e.CollectDatetime).HasColumnName("COLLECT_DATETIME");

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.ParentConfirmDatetime).HasColumnName("PARENT_CONFIRM_DATETIME");

                entity.Property(e => e.ParentDeviceSid).HasColumnName("PARENT_DEVICE_SID");

                entity.Property(e => e.ParentLastConnectDatetime).HasColumnName("PARENT_LAST_CONNECT_DATETIME");

                entity.Property(e => e.ParentResult).HasColumnName("PARENT_RESULT");

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");
            });

            modelBuilder.Entity<MtConnectStatus>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("MT_CONNECT_STATUS", "utility");

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("CODE")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.Description)
                    .HasColumnName("DESCRIPTION")
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<MtInstallType>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("MT_INSTALL_TYPE", "utility");

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("CODE")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.Description)
                    .HasColumnName("DESCRIPTION")
                    .HasMaxLength(128);

                entity.Property(e => e.EquipmentTypeSid).HasColumnName("EQUIPMENT_TYPE_SID");
            });
        }
    }
}
