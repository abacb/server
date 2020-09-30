using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Rms.Server.Core.DBAccessor.Models
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

        public virtual DbSet<DtDeliveryFile> DtDeliveryFile { get; set; }
        public virtual DbSet<DtDeliveryGroup> DtDeliveryGroup { get; set; }
        public virtual DbSet<DtDeliveryModel> DtDeliveryModel { get; set; }
        public virtual DbSet<DtDeliveryResult> DtDeliveryResult { get; set; }
        public virtual DbSet<DtDevice> DtDevice { get; set; }
        public virtual DbSet<DtDeviceFile> DtDeviceFile { get; set; }
        public virtual DbSet<DtDeviceFileAttribute> DtDeviceFileAttribute { get; set; }
        public virtual DbSet<DtDirectoryUsage> DtDirectoryUsage { get; set; }
        public virtual DbSet<DtDiskDrive> DtDiskDrive { get; set; }
        public virtual DbSet<DtDrive> DtDrive { get; set; }
        public virtual DbSet<DtDxaBillLog> DtDxaBillLog { get; set; }
        public virtual DbSet<DtDxaQcLog> DtDxaQcLog { get; set; }
        public virtual DbSet<DtEquipmentUsage> DtEquipmentUsage { get; set; }
        public virtual DbSet<DtInstallResult> DtInstallResult { get; set; }
        public virtual DbSet<DtInventory> DtInventory { get; set; }
        public virtual DbSet<DtParentChildConnect> DtParentChildConnect { get; set; }
        public virtual DbSet<DtPlusServiceBillLog> DtPlusServiceBillLog { get; set; }
        public virtual DbSet<DtScriptConfig> DtScriptConfig { get; set; }
        public virtual DbSet<DtSoftVersion> DtSoftVersion { get; set; }
        public virtual DbSet<DtStorageConfig> DtStorageConfig { get; set; }
        public virtual DbSet<MtConnectStatus> MtConnectStatus { get; set; }
        public virtual DbSet<MtDeliveryFileType> MtDeliveryFileType { get; set; }
        public virtual DbSet<MtDeliveryGroupStatus> MtDeliveryGroupStatus { get; set; }
        public virtual DbSet<MtEquipmentModel> MtEquipmentModel { get; set; }
        public virtual DbSet<MtEquipmentType> MtEquipmentType { get; set; }
        public virtual DbSet<MtInstallResultStatus> MtInstallResultStatus { get; set; }
        public virtual DbSet<MtInstallType> MtInstallType { get; set; }
        public virtual DbSet<MtSoftVersionConvert> MtSoftVersionConvert { get; set; }

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
            modelBuilder.Entity<DtDeliveryFile>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_DELIVERY_FILE", "core");

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.DeliveryFileTypeSid).HasColumnName("DELIVERY_FILE_TYPE_SID");

                entity.Property(e => e.Description)
                    .HasColumnName("DESCRIPTION")
                    .HasMaxLength(200);

                entity.Property(e => e.FilePath)
                    .HasColumnName("FILE_PATH")
                    .HasMaxLength(300);

                entity.Property(e => e.InformationId)
                    .HasColumnName("INFORMATION_ID")
                    .HasMaxLength(45);

                entity.Property(e => e.InstallTypeSid).HasColumnName("INSTALL_TYPE_SID");

                entity.Property(e => e.InstallableVersion)
                    .HasColumnName("INSTALLABLE_VERSION")
                    .HasMaxLength(600)
                    .IsUnicode(false);

                entity.Property(e => e.IsCanceled).HasColumnName("IS_CANCELED");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .HasColumnName("ROW_VERSION")
                    .IsRowVersion();

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");

                entity.Property(e => e.Version)
                    .HasColumnName("VERSION")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.DeliveryFileTypeS)
                    .WithMany(p => p.DtDeliveryFile)
                    .HasForeignKey(d => d.DeliveryFileTypeSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DT_DELIVERY_FILE_FK1");

                entity.HasOne(d => d.InstallTypeS)
                    .WithMany(p => p.DtDeliveryFile)
                    .HasForeignKey(d => d.InstallTypeSid)
                    .HasConstraintName("DT_DELIVERY_FILE_FK2");
            });

            modelBuilder.Entity<DtDeliveryGroup>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_DELIVERY_GROUP", "core");

                entity.HasIndex(e => e.Name)
                    .HasName("UNQ_DT_DELIVERY_GROUP_NAME")
                    .IsUnique();

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.DeliveryFileSid).HasColumnName("DELIVERY_FILE_SID");

                entity.Property(e => e.DeliveryGroupStatusSid).HasColumnName("DELIVERY_GROUP_STATUS_SID");

                entity.Property(e => e.DownloadDelayTime).HasColumnName("DOWNLOAD_DELAY_TIME");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("NAME")
                    .HasMaxLength(100);

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .HasColumnName("ROW_VERSION")
                    .IsRowVersion();

                entity.Property(e => e.StartDatetime).HasColumnName("START_DATETIME");

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");

                entity.HasOne(d => d.DeliveryFileS)
                    .WithMany(p => p.DtDeliveryGroup)
                    .HasForeignKey(d => d.DeliveryFileSid)
                    .HasConstraintName("DT_DELIVERY_GROUP_FK1");

                entity.HasOne(d => d.DeliveryGroupStatusS)
                    .WithMany(p => p.DtDeliveryGroup)
                    .HasForeignKey(d => d.DeliveryGroupStatusSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DT_DELIVERY_GROUP_FK2");
            });

            modelBuilder.Entity<DtDeliveryModel>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_DELIVERY_MODEL", "core");

                entity.HasIndex(e => new { e.DeliveryFileSid, e.EquipmentModelSid })
                    .HasName("UNQ_DT_DELIVERY_MODEL_DELIVERY_FILE_EQUIPMENT_MODEL")
                    .IsUnique();

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.DeliveryFileSid).HasColumnName("DELIVERY_FILE_SID");

                entity.Property(e => e.EquipmentModelSid).HasColumnName("EQUIPMENT_MODEL_SID");

                entity.HasOne(d => d.DeliveryFileS)
                    .WithMany(p => p.DtDeliveryModel)
                    .HasForeignKey(d => d.DeliveryFileSid)
                    .HasConstraintName("DT_DELIVERY_MODEL_FK1");

                entity.HasOne(d => d.EquipmentModelS)
                    .WithMany(p => p.DtDeliveryModel)
                    .HasForeignKey(d => d.EquipmentModelSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DT_DELIVERY_MODEL_FK2");
            });

            modelBuilder.Entity<DtDeliveryResult>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_DELIVERY_RESULT", "core");

                entity.HasIndex(e => new { e.DeviceSid, e.GwDeviceSid, e.DeliveryGroupSid })
                    .HasName("UNQ_DT_DELIVERY_RESULT_DEVICE_GW_DELIVERY_GROUP")
                    .IsUnique();

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.DeliveryGroupSid).HasColumnName("DELIVERY_GROUP_SID");

                entity.Property(e => e.DeviceSid).HasColumnName("DEVICE_SID");

                entity.Property(e => e.GwDeviceSid).HasColumnName("GW_DEVICE_SID");

                entity.HasOne(d => d.DeliveryGroupS)
                    .WithMany(p => p.DtDeliveryResult)
                    .HasForeignKey(d => d.DeliveryGroupSid)
                    .HasConstraintName("DT_DELIVERY_RESULT_FK2");

                entity.HasOne(d => d.DeviceS)
                    .WithMany(p => p.DtDeliveryResultDeviceS)
                    .HasForeignKey(d => d.DeviceSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DT_DELIVERY_RESULT_FK3");

                entity.HasOne(d => d.GwDeviceS)
                    .WithMany(p => p.DtDeliveryResultGwDeviceS)
                    .HasForeignKey(d => d.GwDeviceSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DT_DELIVERY_RESULT_FK1");
            });

            modelBuilder.Entity<DtDevice>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_DEVICE", "core");

                entity.HasIndex(e => e.EdgeId)
                    .HasName("UNQ_DT_DEVICE_EDGE_ID")
                    .IsUnique();

                entity.HasIndex(e => e.EquipmentUid)
                    .HasName("IDX_DT_DEVICE_EQUIPMENT_UID")
                    .IsUnique()
                    .HasFilter("([EQUIPMENT_UID] IS NOT NULL)");

                entity.HasIndex(e => e.RemoteConnectUid)
                    .HasName("IDX_DT_DEVICE_REMOTE_CONNECT_UID")
                    .IsUnique()
                    .HasFilter("([REMOTE_CONNECT_UID] IS NOT NULL)");

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

                entity.HasOne(d => d.ConnectStatusS)
                    .WithMany(p => p.DtDevice)
                    .HasForeignKey(d => d.ConnectStatusSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DT_DEVICE_FK1");

                entity.HasOne(d => d.EquipmentModelS)
                    .WithMany(p => p.DtDevice)
                    .HasForeignKey(d => d.EquipmentModelSid)
                    .HasConstraintName("DT_DEVICE_FK3");

                entity.HasOne(d => d.InstallTypeS)
                    .WithMany(p => p.DtDevice)
                    .HasForeignKey(d => d.InstallTypeSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DT_DEVICE_FK2");
            });

            modelBuilder.Entity<DtDeviceFile>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_DEVICE_FILE", "core");

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.CollectDatetime).HasColumnName("COLLECT_DATETIME");

                entity.Property(e => e.Container)
                    .IsRequired()
                    .HasColumnName("CONTAINER")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.DeviceSid).HasColumnName("DEVICE_SID");

                entity.Property(e => e.FilePath)
                    .IsRequired()
                    .HasColumnName("FILE_PATH")
                    .HasMaxLength(1024);

                entity.Property(e => e.SourceEquipmentUid)
                    .HasColumnName("SOURCE_EQUIPMENT_UID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");

                entity.HasOne(d => d.DeviceS)
                    .WithMany(p => p.DtDeviceFile)
                    .HasForeignKey(d => d.DeviceSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DT_DEVICE_FILE_FK1");
            });

            modelBuilder.Entity<DtDeviceFileAttribute>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_DEVICE_FILE_ATTRIBUTE", "core");

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.DeviceFileSid).HasColumnName("DEVICE_FILE_SID");

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(256);

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");

                entity.Property(e => e.Value).HasColumnName("VALUE");

                entity.HasOne(d => d.DeviceFileS)
                    .WithMany(p => p.DtDeviceFileAttribute)
                    .HasForeignKey(d => d.DeviceFileSid)
                    .HasConstraintName("DT_DEVICE_FILE_ATTRIBUTE_FK1");
            });

            modelBuilder.Entity<DtDirectoryUsage>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_DIRECTORY_USAGE", "core");

                entity.HasIndex(e => e.MessageId)
                    .HasName("UNQ_DT_DIRECTORY_USAGE_MESSAGE_ID")
                    .IsUnique();

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.CollectDatetime).HasColumnName("COLLECT_DATETIME");

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.DetailInfo).HasColumnName("DETAIL_INFO");

                entity.Property(e => e.DeviceSid).HasColumnName("DEVICE_SID");

                entity.Property(e => e.MessageId)
                    .HasColumnName("MESSAGE_ID")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.SourceEquipmentUid)
                    .HasColumnName("SOURCE_EQUIPMENT_UID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.DeviceS)
                    .WithMany(p => p.DtDirectoryUsage)
                    .HasForeignKey(d => d.DeviceSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DT_DIRECTORY_USAGE_FK1");
            });

            modelBuilder.Entity<DtDiskDrive>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_DISK_DRIVE", "core");

                entity.HasIndex(e => e.MessageId)
                    .HasName("UNQ_DT_DISK_DRIVE_MESSAGE_ID")
                    .IsUnique();

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.CollectDatetime).HasColumnName("COLLECT_DATETIME");

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.DeviceSid).HasColumnName("DEVICE_SID");

                entity.Property(e => e.InterfaceType)
                    .HasColumnName("INTERFACE_TYPE")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.MediaType)
                    .HasColumnName("MEDIA_TYPE")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.MessageId)
                    .HasColumnName("MESSAGE_ID")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.Model)
                    .HasColumnName("MODEL")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SerialNumber)
                    .HasColumnName("SERIAL_NUMBER")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.SmartAttributeInfo).HasColumnName("SMART_ATTRIBUTE_INFO");

                entity.Property(e => e.SourceEquipmentUid)
                    .HasColumnName("SOURCE_EQUIPMENT_UID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.DeviceS)
                    .WithMany(p => p.DtDiskDrive)
                    .HasForeignKey(d => d.DeviceSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DT_DISK_DRIVE_FK1");
            });

            modelBuilder.Entity<DtDrive>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_DRIVE", "core");

                entity.HasIndex(e => e.MessageId)
                    .HasName("UNQ_DT_DRIVE_MESSAGE_ID")
                    .IsUnique();

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.CollectDatetime).HasColumnName("COLLECT_DATETIME");

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.DetailInfo).HasColumnName("DETAIL_INFO");

                entity.Property(e => e.DeviceSid).HasColumnName("DEVICE_SID");

                entity.Property(e => e.MessageId)
                    .HasColumnName("MESSAGE_ID")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.SourceEquipmentUid)
                    .HasColumnName("SOURCE_EQUIPMENT_UID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.DeviceS)
                    .WithMany(p => p.DtDrive)
                    .HasForeignKey(d => d.DeviceSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DT_DRIVE_FK1");
            });

            modelBuilder.Entity<DtDxaBillLog>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_DXA_BILL_LOG", "core");

                entity.HasIndex(e => new { e.StudyInstanceUid, e.TypeName })
                    .HasName("UNQ_DT_DXA_BILL_LOG_STUDYUID_TNAME")
                    .IsUnique();

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.CollectDatetime).HasColumnName("COLLECT_DATETIME");

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.DeviceSid).HasColumnName("DEVICE_SID");

                entity.Property(e => e.MeasureDatetime).HasColumnName("MEASURE_DATETIME");

                entity.Property(e => e.OptionDxa).HasColumnName("OPTION_DXA");

                entity.Property(e => e.PatientId)
                    .HasColumnName("PATIENT_ID")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.ServiceMode).HasColumnName("SERVICE_MODE");

                entity.Property(e => e.SoueceEquipmentUid)
                    .HasColumnName("SOUECE_EQUIPMENT_UID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.StudyDatetime).HasColumnName("STUDY_DATETIME");

                entity.Property(e => e.StudyInstanceUid)
                    .HasColumnName("STUDY_INSTANCE_UID")
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.TypeName).HasColumnName("TYPE_NAME");

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");

                entity.HasOne(d => d.DeviceS)
                    .WithMany(p => p.DtDxaBillLog)
                    .HasForeignKey(d => d.DeviceSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DT_DXA_BILL_LOG_FK1");
            });

            modelBuilder.Entity<DtDxaQcLog>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_DXA_QC_LOG", "core");

                entity.HasIndex(e => e.MessageId)
                    .HasName("UNQ_DT_DXA_QC_LOG_MESSAGE_ID")
                    .IsUnique();

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.AreaDxaATest).HasColumnName("AREA_DXA_A_TEST");

                entity.Property(e => e.AreaDxaBTest).HasColumnName("AREA_DXA_B_TEST");

                entity.Property(e => e.AreaDxaCTest).HasColumnName("AREA_DXA_C_TEST");

                entity.Property(e => e.BmcDxaATest).HasColumnName("BMC_DXA_A_TEST");

                entity.Property(e => e.BmcDxaBTest).HasColumnName("BMC_DXA_B_TEST");

                entity.Property(e => e.BmcDxaCTest).HasColumnName("BMC_DXA_C_TEST");

                entity.Property(e => e.BmdDxaABasvalTest).HasColumnName("BMD_DXA_A_BASVAL_TEST");

                entity.Property(e => e.BmdDxaATest).HasColumnName("BMD_DXA_A_TEST");

                entity.Property(e => e.BmdDxaBBasvalTest).HasColumnName("BMD_DXA_B_BASVAL_TEST");

                entity.Property(e => e.BmdDxaBTest).HasColumnName("BMD_DXA_B_TEST");

                entity.Property(e => e.BmdDxaCBasvalTest).HasColumnName("BMD_DXA_C_BASVAL_TEST");

                entity.Property(e => e.BmdDxaCTest).HasColumnName("BMD_DXA_C_TEST");

                entity.Property(e => e.BmdLinearityTest).HasColumnName("BMD_LINEARITY_TEST");

                entity.Property(e => e.CollectDatetime).HasColumnName("COLLECT_DATETIME");

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.DeviceSid).HasColumnName("DEVICE_SID");

                entity.Property(e => e.MeasureDatetime).HasColumnName("MEASURE_DATETIME");

                entity.Property(e => e.MessageId)
                    .HasColumnName("MESSAGE_ID")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.PanelSerialId)
                    .HasColumnName("PANEL_SERIAL_ID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.QcResult).HasColumnName("QC_RESULT");

                entity.Property(e => e.QlCsiBoneABasvalTest).HasColumnName("QL_CSI_BONE_A_BASVAL_TEST");

                entity.Property(e => e.QlCsiBoneATest).HasColumnName("QL_CSI_BONE_A_TEST");

                entity.Property(e => e.QlCsiBoneBBasvalTest).HasColumnName("QL_CSI_BONE_B_BASVAL_TEST");

                entity.Property(e => e.QlCsiBoneBTest).HasColumnName("QL_CSI_BONE_B_TEST");

                entity.Property(e => e.QlCsiBoneCBasvalTest).HasColumnName("QL_CSI_BONE_C_BASVAL_TEST");

                entity.Property(e => e.QlCsiBoneCTest).HasColumnName("QL_CSI_BONE_C_TEST");

                entity.Property(e => e.QlCsiSoftLaBasvalTest).HasColumnName("QL_CSI_SOFT_LA_BASVAL_TEST");

                entity.Property(e => e.QlCsiSoftLaTest).HasColumnName("QL_CSI_SOFT_LA_TEST");

                entity.Property(e => e.QlCsiSoftLbBasvalTest).HasColumnName("QL_CSI_SOFT_LB_BASVAL_TEST");

                entity.Property(e => e.QlCsiSoftLbTest).HasColumnName("QL_CSI_SOFT_LB_TEST");

                entity.Property(e => e.QlCsiSoftLcBasvalTest).HasColumnName("QL_CSI_SOFT_LC_BASVAL_TEST");

                entity.Property(e => e.QlCsiSoftLcTest).HasColumnName("QL_CSI_SOFT_LC_TEST");

                entity.Property(e => e.QlCsiSoftRaBasvalTest).HasColumnName("QL_CSI_SOFT_RA_BASVAL_TEST");

                entity.Property(e => e.QlCsiSoftRaTest).HasColumnName("QL_CSI_SOFT_RA_TEST");

                entity.Property(e => e.QlCsiSoftRbBasvalTest).HasColumnName("QL_CSI_SOFT_RB_BASVAL_TEST");

                entity.Property(e => e.QlCsiSoftRbTest).HasColumnName("QL_CSI_SOFT_RB_TEST");

                entity.Property(e => e.QlCsiSoftRcBasvalTest).HasColumnName("QL_CSI_SOFT_RC_BASVAL_TEST");

                entity.Property(e => e.QlCsiSoftRcTest).HasColumnName("QL_CSI_SOFT_RC_TEST");

                entity.Property(e => e.QlDxaBoneABasvalTest).HasColumnName("QL_DXA_BONE_A_BASVAL_TEST");

                entity.Property(e => e.QlDxaBoneATest).HasColumnName("QL_DXA_BONE_A_TEST");

                entity.Property(e => e.QlDxaBoneBBasvalTest).HasColumnName("QL_DXA_BONE_B_BASVAL_TEST");

                entity.Property(e => e.QlDxaBoneBTest).HasColumnName("QL_DXA_BONE_B_TEST");

                entity.Property(e => e.QlDxaBoneCBasvalTest).HasColumnName("QL_DXA_BONE_C_BASVAL_TEST");

                entity.Property(e => e.QlDxaBoneCTest).HasColumnName("QL_DXA_BONE_C_TEST");

                entity.Property(e => e.QlDxaSoftLaBasvalTest).HasColumnName("QL_DXA_SOFT_LA_BASVAL_TEST");

                entity.Property(e => e.QlDxaSoftLaTest).HasColumnName("QL_DXA_SOFT_LA_TEST");

                entity.Property(e => e.QlDxaSoftLbBasvalTest).HasColumnName("QL_DXA_SOFT_LB_BASVAL_TEST");

                entity.Property(e => e.QlDxaSoftLbTest).HasColumnName("QL_DXA_SOFT_LB_TEST");

                entity.Property(e => e.QlDxaSoftLcBasvalTest).HasColumnName("QL_DXA_SOFT_LC_BASVAL_TEST");

                entity.Property(e => e.QlDxaSoftLcTest).HasColumnName("QL_DXA_SOFT_LC_TEST");

                entity.Property(e => e.QlDxaSoftRaBasvalTest).HasColumnName("QL_DXA_SOFT_RA_BASVAL_TEST");

                entity.Property(e => e.QlDxaSoftRaTest).HasColumnName("QL_DXA_SOFT_RA_TEST");

                entity.Property(e => e.QlDxaSoftRbBasvalTest).HasColumnName("QL_DXA_SOFT_RB_BASVAL_TEST");

                entity.Property(e => e.QlDxaSoftRbTest).HasColumnName("QL_DXA_SOFT_RB_TEST");

                entity.Property(e => e.QlDxaSoftRcBasvalTest).HasColumnName("QL_DXA_SOFT_RC_BASVAL_TEST");

                entity.Property(e => e.QlDxaSoftRcTest).HasColumnName("QL_DXA_SOFT_RC_TEST");

                entity.Property(e => e.QlGosBoneABasvalTest).HasColumnName("QL_GOS_BONE_A_BASVAL_TEST");

                entity.Property(e => e.QlGosBoneATest).HasColumnName("QL_GOS_BONE_A_TEST");

                entity.Property(e => e.QlGosBoneBBasvalTest).HasColumnName("QL_GOS_BONE_B_BASVAL_TEST");

                entity.Property(e => e.QlGosBoneBTest).HasColumnName("QL_GOS_BONE_B_TEST");

                entity.Property(e => e.QlGosBoneCBasvalTest).HasColumnName("QL_GOS_BONE_C_BASVAL_TEST");

                entity.Property(e => e.QlGosBoneCTest).HasColumnName("QL_GOS_BONE_C_TEST");

                entity.Property(e => e.QlGosSoftLaBasvalTest).HasColumnName("QL_GOS_SOFT_LA_BASVAL_TEST");

                entity.Property(e => e.QlGosSoftLaTest).HasColumnName("QL_GOS_SOFT_LA_TEST");

                entity.Property(e => e.QlGosSoftLbBasvalTest).HasColumnName("QL_GOS_SOFT_LB_BASVAL_TEST");

                entity.Property(e => e.QlGosSoftLbTest).HasColumnName("QL_GOS_SOFT_LB_TEST");

                entity.Property(e => e.QlGosSoftLcBasvalTest).HasColumnName("QL_GOS_SOFT_LC_BASVAL_TEST");

                entity.Property(e => e.QlGosSoftLcTest).HasColumnName("QL_GOS_SOFT_LC_TEST");

                entity.Property(e => e.QlGosSoftRaBasvalTest).HasColumnName("QL_GOS_SOFT_RA_BASVAL_TEST");

                entity.Property(e => e.QlGosSoftRaTest).HasColumnName("QL_GOS_SOFT_RA_TEST");

                entity.Property(e => e.QlGosSoftRbBasvalTest).HasColumnName("QL_GOS_SOFT_RB_BASVAL_TEST");

                entity.Property(e => e.QlGosSoftRbTest).HasColumnName("QL_GOS_SOFT_RB_TEST");

                entity.Property(e => e.QlGosSoftRcBasvalTest).HasColumnName("QL_GOS_SOFT_RC_BASVAL_TEST");

                entity.Property(e => e.QlGosSoftRcTest).HasColumnName("QL_GOS_SOFT_RC_TEST");

                entity.Property(e => e.SopInstanceUid)
                    .HasColumnName("SOP_INSTANCE_UID")
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.SourceEquipmentUid)
                    .HasColumnName("SOURCE_EQUIPMENT_UID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.StdCsiBoneABasvalTest).HasColumnName("STD_CSI_BONE_A_BASVAL_TEST");

                entity.Property(e => e.StdCsiBoneATest).HasColumnName("STD_CSI_BONE_A_TEST");

                entity.Property(e => e.StdCsiBoneBBasvalTest).HasColumnName("STD_CSI_BONE_B_BASVAL_TEST");

                entity.Property(e => e.StdCsiBoneBTest).HasColumnName("STD_CSI_BONE_B_TEST");

                entity.Property(e => e.StdCsiBoneCBasvalTest).HasColumnName("STD_CSI_BONE_C_BASVAL_TEST");

                entity.Property(e => e.StdCsiBoneCTest).HasColumnName("STD_CSI_BONE_C_TEST");

                entity.Property(e => e.StdCsiSoftLaBasvalTest).HasColumnName("STD_CSI_SOFT_LA_BASVAL_TEST");

                entity.Property(e => e.StdCsiSoftLaTest).HasColumnName("STD_CSI_SOFT_LA_TEST");

                entity.Property(e => e.StdCsiSoftLbBasvalTest).HasColumnName("STD_CSI_SOFT_LB_BASVAL_TEST");

                entity.Property(e => e.StdCsiSoftLbTest).HasColumnName("STD_CSI_SOFT_LB_TEST");

                entity.Property(e => e.StdCsiSoftLcBasvalTest).HasColumnName("STD_CSI_SOFT_LC_BASVAL_TEST");

                entity.Property(e => e.StdCsiSoftLcTest).HasColumnName("STD_CSI_SOFT_LC_TEST");

                entity.Property(e => e.StdCsiSoftRaBasvalTest).HasColumnName("STD_CSI_SOFT_RA_BASVAL_TEST");

                entity.Property(e => e.StdCsiSoftRaTest).HasColumnName("STD_CSI_SOFT_RA_TEST");

                entity.Property(e => e.StdCsiSoftRbBasvalTest).HasColumnName("STD_CSI_SOFT_RB_BASVAL_TEST");

                entity.Property(e => e.StdCsiSoftRbTest).HasColumnName("STD_CSI_SOFT_RB_TEST");

                entity.Property(e => e.StdCsiSoftRcBasvalTest).HasColumnName("STD_CSI_SOFT_RC_BASVAL_TEST");

                entity.Property(e => e.StdCsiSoftRcTest).HasColumnName("STD_CSI_SOFT_RC_TEST");

                entity.Property(e => e.StdDxaBoneABasvalTest).HasColumnName("STD_DXA_BONE_A_BASVAL_TEST");

                entity.Property(e => e.StdDxaBoneATest).HasColumnName("STD_DXA_BONE_A_TEST");

                entity.Property(e => e.StdDxaBoneBBasvalTest).HasColumnName("STD_DXA_BONE_B_BASVAL_TEST");

                entity.Property(e => e.StdDxaBoneBTest).HasColumnName("STD_DXA_BONE_B_TEST");

                entity.Property(e => e.StdDxaBoneCBasvalTest).HasColumnName("STD_DXA_BONE_C_BASVAL_TEST");

                entity.Property(e => e.StdDxaBoneCTest).HasColumnName("STD_DXA_BONE_C_TEST");

                entity.Property(e => e.StdDxaSoftLaBasvalTest).HasColumnName("STD_DXA_SOFT_LA_BASVAL_TEST");

                entity.Property(e => e.StdDxaSoftLaTest).HasColumnName("STD_DXA_SOFT_LA_TEST");

                entity.Property(e => e.StdDxaSoftLbBasvalTest).HasColumnName("STD_DXA_SOFT_LB_BASVAL_TEST");

                entity.Property(e => e.StdDxaSoftLbTest).HasColumnName("STD_DXA_SOFT_LB_TEST");

                entity.Property(e => e.StdDxaSoftLcBasvalTest).HasColumnName("STD_DXA_SOFT_LC_BASVAL_TEST");

                entity.Property(e => e.StdDxaSoftLcTest).HasColumnName("STD_DXA_SOFT_LC_TEST");

                entity.Property(e => e.StdDxaSoftRaBasvalTest).HasColumnName("STD_DXA_SOFT_RA_BASVAL_TEST");

                entity.Property(e => e.StdDxaSoftRaTest).HasColumnName("STD_DXA_SOFT_RA_TEST");

                entity.Property(e => e.StdDxaSoftRbBasvalTest).HasColumnName("STD_DXA_SOFT_RB_BASVAL_TEST");

                entity.Property(e => e.StdDxaSoftRbTest).HasColumnName("STD_DXA_SOFT_RB_TEST");

                entity.Property(e => e.StdDxaSoftRcBasvalTest).HasColumnName("STD_DXA_SOFT_RC_BASVAL_TEST");

                entity.Property(e => e.StdDxaSoftRcTest).HasColumnName("STD_DXA_SOFT_RC_TEST");

                entity.Property(e => e.StdGosBoneABasvalTest).HasColumnName("STD_GOS_BONE_A_BASVAL_TEST");

                entity.Property(e => e.StdGosBoneATest).HasColumnName("STD_GOS_BONE_A_TEST");

                entity.Property(e => e.StdGosBoneBBasvalTest).HasColumnName("STD_GOS_BONE_B_BASVAL_TEST");

                entity.Property(e => e.StdGosBoneBTest).HasColumnName("STD_GOS_BONE_B_TEST");

                entity.Property(e => e.StdGosBoneCBasvalTest).HasColumnName("STD_GOS_BONE_C_BASVAL_TEST");

                entity.Property(e => e.StdGosBoneCTest).HasColumnName("STD_GOS_BONE_C_TEST");

                entity.Property(e => e.StdGosSoftLaBasvalTest).HasColumnName("STD_GOS_SOFT_LA_BASVAL_TEST");

                entity.Property(e => e.StdGosSoftLaTest).HasColumnName("STD_GOS_SOFT_LA_TEST");

                entity.Property(e => e.StdGosSoftLbBasvalTest).HasColumnName("STD_GOS_SOFT_LB_BASVAL_TEST");

                entity.Property(e => e.StdGosSoftLbTest).HasColumnName("STD_GOS_SOFT_LB_TEST");

                entity.Property(e => e.StdGosSoftLcBasvalTest).HasColumnName("STD_GOS_SOFT_LC_BASVAL_TEST");

                entity.Property(e => e.StdGosSoftLcTest).HasColumnName("STD_GOS_SOFT_LC_TEST");

                entity.Property(e => e.StdGosSoftRaBasvalTest).HasColumnName("STD_GOS_SOFT_RA_BASVAL_TEST");

                entity.Property(e => e.StdGosSoftRaTest).HasColumnName("STD_GOS_SOFT_RA_TEST");

                entity.Property(e => e.StdGosSoftRbBasvalTest).HasColumnName("STD_GOS_SOFT_RB_BASVAL_TEST");

                entity.Property(e => e.StdGosSoftRbTest).HasColumnName("STD_GOS_SOFT_RB_TEST");

                entity.Property(e => e.StdGosSoftRcBasvalTest).HasColumnName("STD_GOS_SOFT_RC_BASVAL_TEST");

                entity.Property(e => e.StdGosSoftRcTest).HasColumnName("STD_GOS_SOFT_RC_TEST");

                entity.Property(e => e.StudyDatetime).HasColumnName("STUDY_DATETIME");

                entity.Property(e => e.StudyInstanceUid)
                    .HasColumnName("STUDY_INSTANCE_UID")
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.TechniqueCode).HasColumnName("TECHNIQUE_CODE");

                entity.Property(e => e.TypeName).HasColumnName("TYPE_NAME");

                entity.HasOne(d => d.DeviceS)
                    .WithMany(p => p.DtDxaQcLog)
                    .HasForeignKey(d => d.DeviceSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DT_DXA_QC_LOG_FK1");
            });

            modelBuilder.Entity<DtEquipmentUsage>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_EQUIPMENT_USAGE", "core");

                entity.HasIndex(e => e.MessageId)
                    .HasName("UNQ_DT_EQUIPMENT_USAGE_MESSAGE_ID")
                    .IsUnique();

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.CollectDatetime).HasColumnName("COLLECT_DATETIME");

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.DetailInfo).HasColumnName("DETAIL_INFO");

                entity.Property(e => e.DeviceSid).HasColumnName("DEVICE_SID");

                entity.Property(e => e.MessageId)
                    .HasColumnName("MESSAGE_ID")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.SourceEquipmentUid)
                    .HasColumnName("SOURCE_EQUIPMENT_UID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.DeviceS)
                    .WithMany(p => p.DtEquipmentUsage)
                    .HasForeignKey(d => d.DeviceSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DT_EQUIPMENT_USAGE_FK1");
            });

            modelBuilder.Entity<DtInstallResult>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_INSTALL_RESULT", "core");

                entity.HasIndex(e => e.MessageId)
                    .HasName("IDX_DT_INSTALL_RESULT_MESSAGE_ID")
                    .IsUnique()
                    .HasFilter("([MESSAGE_ID] IS NOT NULL)");

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.AfterVervion)
                    .HasColumnName("AFTER_VERVION")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.BeforeVersion)
                    .HasColumnName("BEFORE_VERSION")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.CollectDatetime).HasColumnName("COLLECT_DATETIME");

                entity.Property(e => e.ComputerName)
                    .HasColumnName("COMPUTER_NAME")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.DeliveryResultSid).HasColumnName("DELIVERY_RESULT_SID");

                entity.Property(e => e.DeviceSid).HasColumnName("DEVICE_SID");

                entity.Property(e => e.ErrorCode)
                    .HasColumnName("ERROR_CODE")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ErrorDescription)
                    .HasColumnName("ERROR_DESCRIPTION")
                    .HasColumnType("ntext");

                entity.Property(e => e.EventDatetime).HasColumnName("EVENT_DATETIME");

                entity.Property(e => e.HasRepairReport).HasColumnName("HAS_REPAIR_REPORT");

                entity.Property(e => e.InstallResultStatusSid).HasColumnName("INSTALL_RESULT_STATUS_SID");

                entity.Property(e => e.IpAddress)
                    .HasColumnName("IP_ADDRESS")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.IsAuto).HasColumnName("IS_AUTO");

                entity.Property(e => e.IsSuccess).HasColumnName("IS_SUCCESS");

                entity.Property(e => e.MessageId)
                    .HasColumnName("MESSAGE_ID")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.Method)
                    .HasColumnName("METHOD")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Process)
                    .HasColumnName("PROCESS")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.ReleaseVersion)
                    .HasColumnName("RELEASE_VERSION")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ServerClientKind)
                    .HasColumnName("SERVER_CLIENT_KIND")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.SourceEquipmentUid)
                    .HasColumnName("SOURCE_EQUIPMENT_UID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateEndDatetime).HasColumnName("UPDATE_END_DATETIME");

                entity.Property(e => e.UpdateStratDatetime).HasColumnName("UPDATE_STRAT_DATETIME");

                entity.HasOne(d => d.DeliveryResultS)
                    .WithMany(p => p.DtInstallResult)
                    .HasForeignKey(d => d.DeliveryResultSid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("DT_INSTALL_RESULT_FK3");

                entity.HasOne(d => d.DeviceS)
                    .WithMany(p => p.DtInstallResult)
                    .HasForeignKey(d => d.DeviceSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DT_INSTALL_RESULT_FK2");

                entity.HasOne(d => d.InstallResultStatusS)
                    .WithMany(p => p.DtInstallResult)
                    .HasForeignKey(d => d.InstallResultStatusSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DT_INSTALL_RESULT_FK1");
            });

            modelBuilder.Entity<DtInventory>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_INVENTORY", "core");

                entity.HasIndex(e => e.MessageId)
                    .HasName("UNQ_DT_INVENTORY_MESSAGE_ID")
                    .IsUnique();

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.CollectDatetime).HasColumnName("COLLECT_DATETIME");

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.DetailInfo).HasColumnName("DETAIL_INFO");

                entity.Property(e => e.DeviceSid).HasColumnName("DEVICE_SID");

                entity.Property(e => e.MessageId)
                    .HasColumnName("MESSAGE_ID")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.SourceEquipmentUid)
                    .HasColumnName("SOURCE_EQUIPMENT_UID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.DeviceS)
                    .WithMany(p => p.DtInventory)
                    .HasForeignKey(d => d.DeviceSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DT_INVENTORY_FK1");
            });

            modelBuilder.Entity<DtParentChildConnect>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_PARENT_CHILD_CONNECT", "core");

                entity.HasIndex(e => new { e.ParentDeviceSid, e.ChildDeviceSid })
                    .HasName("UNQ_DT_PARENT_CHILD_CONNECT_PARENT_CHILD_DEVICE")
                    .IsUnique();

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.ChildConfirmDatetime).HasColumnName("CHILD_CONFIRM_DATETIME");

                entity.Property(e => e.ChildDeviceSid).HasColumnName("CHILD_DEVICE_SID");

                entity.Property(e => e.ChildLastConnectDatetime).HasColumnName("CHILD_LAST_CONNECT_DATETIME");

                entity.Property(e => e.ChildResult).HasColumnName("CHILD_RESULT");

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.ParentConfirmDatetime).HasColumnName("PARENT_CONFIRM_DATETIME");

                entity.Property(e => e.ParentDeviceSid).HasColumnName("PARENT_DEVICE_SID");

                entity.Property(e => e.ParentLastConnectDatetime).HasColumnName("PARENT_LAST_CONNECT_DATETIME");

                entity.Property(e => e.ParentResult).HasColumnName("PARENT_RESULT");

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");

                entity.HasOne(d => d.ChildDeviceS)
                    .WithMany(p => p.DtParentChildConnectChildDeviceS)
                    .HasForeignKey(d => d.ChildDeviceSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DT_PARENT_CHILD_CONNECT_FK2");

                entity.HasOne(d => d.ParentDeviceS)
                    .WithMany(p => p.DtParentChildConnectParentDeviceS)
                    .HasForeignKey(d => d.ParentDeviceSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DT_PARENT_CHILD_CONNECT_FK1");
            });

            modelBuilder.Entity<DtPlusServiceBillLog>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_PLUS_SERVICE_BILL_LOG", "core");

                entity.HasIndex(e => new { e.StudyInstanceUid, e.TypeName })
                    .HasName("UNQ_DT_PLUS_SERVICE_BILL_LOG_STUDYUID_TNAME")
                    .IsUnique();

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.Age).HasColumnName("AGE");

                entity.Property(e => e.BillFlg).HasColumnName("BILL_FLG");

                entity.Property(e => e.CollectDatetime).HasColumnName("COLLECT_DATETIME");

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.DeviceSid).HasColumnName("DEVICE_SID");

                entity.Property(e => e.MeasureDatetime).HasColumnName("MEASURE_DATETIME");

                entity.Property(e => e.MeasureValue).HasColumnName("MEASURE_VALUE");

                entity.Property(e => e.PatientId)
                    .HasColumnName("PATIENT_ID")
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.Sex).HasColumnName("SEX");

                entity.Property(e => e.SopInstanceUid)
                    .HasColumnName("SOP_INSTANCE_UID")
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.SourceEquipmentUid)
                    .HasColumnName("SOURCE_EQUIPMENT_UID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.StudyDatetime).HasColumnName("STUDY_DATETIME");

                entity.Property(e => e.StudyInstanceUid)
                    .HasColumnName("STUDY_INSTANCE_UID")
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.TypeName)
                    .HasColumnName("TYPE_NAME")
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");

                entity.HasOne(d => d.DeviceS)
                    .WithMany(p => p.DtPlusServiceBillLog)
                    .HasForeignKey(d => d.DeviceSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DT_PLUS_SERVICE_BILL_LOG_FK1");
            });

            modelBuilder.Entity<DtScriptConfig>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_SCRIPT_CONFIG", "core");

                entity.HasIndex(e => new { e.InstallTypeSid, e.Name, e.Version })
                    .HasName("UNQ_DT_SCRIPT_CONFIG_INSTALL_TYPE_NAME_VERSION")
                    .IsUnique();

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.FileName)
                    .HasColumnName("FILE_NAME")
                    .HasMaxLength(64);

                entity.Property(e => e.InstallTypeSid).HasColumnName("INSTALL_TYPE_SID");

                entity.Property(e => e.Location)
                    .HasColumnName("LOCATION")
                    .HasMaxLength(300);

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Version).HasColumnName("VERSION");

                entity.HasOne(d => d.InstallTypeS)
                    .WithMany(p => p.DtScriptConfig)
                    .HasForeignKey(d => d.InstallTypeSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DT_SCRIPT_CONFIG_FK1");
            });

            modelBuilder.Entity<DtSoftVersion>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_SOFT_VERSION", "core");

                entity.HasIndex(e => e.MessageId)
                    .HasName("UNQ_DT_SOFT_VERSION_MESSAGE_ID")
                    .IsUnique();

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.CollectDatetime).HasColumnName("COLLECT_DATETIME");

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.DeviceSid).HasColumnName("DEVICE_SID");

                entity.Property(e => e.EquipmentModelSid).HasColumnName("EQUIPMENT_MODEL_SID");

                entity.Property(e => e.MessageId)
                    .HasColumnName("MESSAGE_ID")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.SourceEquipmentUid)
                    .HasColumnName("SOURCE_EQUIPMENT_UID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Version)
                    .HasColumnName("VERSION")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.DeviceS)
                    .WithMany(p => p.DtSoftVersion)
                    .HasForeignKey(d => d.DeviceSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DT_SOFT_VERSION_FK2");

                entity.HasOne(d => d.EquipmentModelS)
                    .WithMany(p => p.DtSoftVersion)
                    .HasForeignKey(d => d.EquipmentModelSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DT_SOFT_VERSION_FK1");
            });

            modelBuilder.Entity<DtStorageConfig>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_STORAGE_CONFIG", "core");

                entity.HasIndex(e => e.Name)
                    .HasName("UNQ_DT_STORAGE_CONFIG_NAME")
                    .IsUnique();

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("NAME")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Sas)
                    .HasColumnName("SAS")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");

                entity.Property(e => e.Url)
                    .HasColumnName("URL")
                    .HasMaxLength(64)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<MtConnectStatus>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("MT_CONNECT_STATUS", "core");

                entity.HasIndex(e => e.Code)
                    .HasName("UNQ_MT_CONNECT_STATUS_CODE")
                    .IsUnique();

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

            modelBuilder.Entity<MtDeliveryFileType>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("MT_DELIVERY_FILE_TYPE", "core");

                entity.HasIndex(e => e.Code)
                    .HasName("UNQ_MT_DELIVERY_FILE_TYPE_CODE")
                    .IsUnique();

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

            modelBuilder.Entity<MtDeliveryGroupStatus>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("MT_DELIVERY_GROUP_STATUS", "core");

                entity.HasIndex(e => e.Code)
                    .HasName("UNQ_MT_DELIVERY_GROUP_STATUS_CODE")
                    .IsUnique();

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

            modelBuilder.Entity<MtEquipmentModel>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("MT_EQUIPMENT_MODEL", "core");

                entity.HasIndex(e => e.Code)
                    .HasName("UNQ_MT_EQUIPMENT_MODEL_CODE")
                    .IsUnique();

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

                entity.HasOne(d => d.EquipmentTypeS)
                    .WithMany(p => p.MtEquipmentModel)
                    .HasForeignKey(d => d.EquipmentTypeSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("MT_EQUIPMENT_MODEL_FK1");
            });

            modelBuilder.Entity<MtEquipmentType>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("MT_EQUIPMENT_TYPE", "core");

                entity.HasIndex(e => e.Code)
                    .HasName("UNQ_MT_EQUIPMENT_TYPE_CODE")
                    .IsUnique();

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
            });

            modelBuilder.Entity<MtInstallResultStatus>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("MT_INSTALL_RESULT_STATUS", "core");

                entity.HasIndex(e => e.Code)
                    .HasName("UNQ_MT_INSTALL_RESULT_STATUS_CODE")
                    .IsUnique();

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

                entity.ToTable("MT_INSTALL_TYPE", "core");

                entity.HasIndex(e => e.Code)
                    .HasName("UNQ_MT_INSTALL_TYPE_CODE")
                    .IsUnique();

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

                entity.HasOne(d => d.EquipmentTypeS)
                    .WithMany(p => p.MtInstallType)
                    .HasForeignKey(d => d.EquipmentTypeSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("MT_INSTALL_TYPE_FK1");
            });

            modelBuilder.Entity<MtSoftVersionConvert>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("MT_SOFT_VERSION_CONVERT", "core");

                entity.HasIndex(e => new { e.EquipmentModelSid, e.DisplayVersion, e.InternalVersion })
                    .HasName("UNQ_MT_SOFT_VERSION_CONVERT_EQUIPMENT_MODEL_DISPLAY_INTERNAL_VERSION")
                    .IsUnique();

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.Description)
                    .HasColumnName("DESCRIPTION")
                    .HasMaxLength(200);

                entity.Property(e => e.DisplayVersion)
                    .IsRequired()
                    .HasColumnName("DISPLAY_VERSION")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.EquipmentModelSid).HasColumnName("EQUIPMENT_MODEL_SID");

                entity.Property(e => e.InternalVersion)
                    .IsRequired()
                    .HasColumnName("INTERNAL_VERSION")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.EquipmentModelS)
                    .WithMany(p => p.MtSoftVersionConvert)
                    .HasForeignKey(d => d.EquipmentModelSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("MT_SOFT_VERSION_CONVERT_FK1");
            });
        }
    }
}
