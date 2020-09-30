using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Rms.Server.Operation.DBAccessor.Models
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

        public virtual DbSet<DtAlarm> DtAlarm { get; set; }
        public virtual DbSet<DtAlarmConfig> DtAlarmConfig { get; set; }
        public virtual DbSet<DtEquipment> DtEquipment { get; set; }
        public virtual DbSet<DtInstallBase> DtInstallBase { get; set; }
        public virtual DbSet<DtWorkReportExport> DtWorkReportExport { get; set; }
        public virtual DbSet<MtNetworkRoute> MtNetworkRoute { get; set; }

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
            modelBuilder.Entity<DtAlarm>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_ALARM", "operation");

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.AlarmDatetime).HasColumnName("ALARM_DATETIME");

                entity.Property(e => e.AlarmDefId)
                    .HasColumnName("ALARM_DEF_ID")
                    .HasMaxLength(10);

                entity.Property(e => e.AlarmDescription)
                    .HasColumnName("ALARM_DESCRIPTION")
                    .HasMaxLength(1024);

                entity.Property(e => e.AlarmLevel).HasColumnName("ALARM_LEVEL");

                entity.Property(e => e.AlarmTitle)
                    .HasColumnName("ALARM_TITLE")
                    .HasMaxLength(200);

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.EquipmentSid).HasColumnName("EQUIPMENT_SID");

                entity.Property(e => e.ErrorCode)
                    .HasColumnName("ERROR_CODE")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.EventDatetime).HasColumnName("EVENT_DATETIME");

                entity.Property(e => e.HasMail).HasColumnName("HAS_MAIL");

                entity.Property(e => e.MessageId)
                    .HasColumnName("MESSAGE_ID")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.TypeCode)
                    .HasColumnName("TYPE_CODE")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.EquipmentS)
                    .WithMany(p => p.DtAlarm)
                    .HasForeignKey(d => d.EquipmentSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DT_ALARM_FK1");
            });

            modelBuilder.Entity<DtAlarmConfig>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_ALARM_CONFIG", "operation");

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.AlarmLevelFrom).HasColumnName("ALARM_LEVEL_FROM");

                entity.Property(e => e.AlarmLevelTo).HasColumnName("ALARM_LEVEL_TO");

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.Description)
                    .HasColumnName("DESCRIPTION")
                    .HasMaxLength(1024);

                entity.Property(e => e.MailAddress)
                    .IsRequired()
                    .HasColumnName("MAIL_ADDRESS")
                    .HasMaxLength(1024)
                    .IsUnicode(false);

                entity.Property(e => e.MailSendingInterval).HasColumnName("MAIL_SENDING_INTERVAL");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .HasColumnName("ROW_VERSION")
                    .IsRowVersion();

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");
            });

            modelBuilder.Entity<DtEquipment>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_EQUIPMENT", "operation");

                entity.HasIndex(e => e.EquipmentNumber)
                    .HasName("IDX_DT_EQUIPMENT_EQUIPMENT_NUMBER")
                    .IsUnique()
                    .HasFilter("([IS_DELETED]=(1))");

                entity.HasIndex(e => e.InstallBaseSid)
                    .HasName("IDX_DT_EQUIPMENT_INSTALL_BASE")
                    .IsUnique()
                    .HasFilter("([IS_DELETED]=(1))");

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.Description)
                    .HasColumnName("DESCRIPTION")
                    .HasMaxLength(1024);

                entity.Property(e => e.EquipmentNumber)
                    .IsRequired()
                    .HasColumnName("EQUIPMENT_NUMBER")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Hierarchy).HasColumnName("HIERARCHY");

                entity.Property(e => e.HierarchyOrder)
                    .IsRequired()
                    .HasColumnName("HIERARCHY_ORDER")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.HierarchyPath)
                    .IsRequired()
                    .HasColumnName("HIERARCHY_PATH")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.HostName)
                    .HasColumnName("HOST_NAME")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.InstallBaseSid).HasColumnName("INSTALL_BASE_SID");

                entity.Property(e => e.IpAddress)
                    .HasColumnName("IP_ADDRESS")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.IsDeleted).HasColumnName("IS_DELETED");

                entity.Property(e => e.NetworkRouteSid).HasColumnName("NETWORK_ROUTE_SID");

                entity.Property(e => e.ParentEquipmentSid).HasColumnName("PARENT_EQUIPMENT_SID");

                entity.Property(e => e.TopEquipmentSid).HasColumnName("TOP_EQUIPMENT_SID");

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");

                entity.HasOne(d => d.InstallBaseS)
                    .WithOne(p => p.DtEquipment)
                    .HasForeignKey<DtEquipment>(d => d.InstallBaseSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DT_EQUIPMENT_FK1");

                entity.HasOne(d => d.NetworkRouteS)
                    .WithMany(p => p.DtEquipment)
                    .HasForeignKey(d => d.NetworkRouteSid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DT_EQUIPMENT_FK4");

                entity.HasOne(d => d.ParentEquipmentS)
                    .WithMany(p => p.InverseParentEquipmentS)
                    .HasForeignKey(d => d.ParentEquipmentSid)
                    .HasConstraintName("DT_EQUIPMENT_FK3");

                entity.HasOne(d => d.TopEquipmentS)
                    .WithMany(p => p.InverseTopEquipmentS)
                    .HasForeignKey(d => d.TopEquipmentSid)
                    .HasConstraintName("DT_EQUIPMENT_FK2");
            });

            modelBuilder.Entity<DtInstallBase>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_INSTALL_BASE", "operation");

                entity.HasIndex(e => e.EquipmentNumber)
                    .HasName("UNQ_DT_INSTALL_BASE_EQUIPMENT_NUMBER")
                    .IsUnique();

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.Address)
                    .HasColumnName("ADDRESS")
                    .HasMaxLength(1024);

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.CustomerName)
                    .HasColumnName("CUSTOMER_NAME")
                    .HasMaxLength(64);

                entity.Property(e => e.CustomerNumber).HasColumnName("CUSTOMER_NUMBER");

                entity.Property(e => e.EquipmentName)
                    .HasColumnName("EQUIPMENT_NAME")
                    .HasMaxLength(60);

                entity.Property(e => e.EquipmentNumber)
                    .IsRequired()
                    .HasColumnName("EQUIPMENT_NUMBER")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.EquipmentSerialNumber)
                    .HasColumnName("EQUIPMENT_SERIAL_NUMBER")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ImportCreateDatetime).HasColumnName("IMPORT_CREATE_DATETIME");

                entity.Property(e => e.ImportUpdateDatetime).HasColumnName("IMPORT_UPDATE_DATETIME");

                entity.Property(e => e.InstallCompleteDate).HasColumnName("INSTALL_COMPLETE_DATE");

                entity.Property(e => e.InstallFeatures)
                    .HasColumnName("INSTALL_FEATURES")
                    .HasMaxLength(64);

                entity.Property(e => e.Outsourcer)
                    .HasColumnName("OUTSOURCER")
                    .HasMaxLength(60);

                entity.Property(e => e.RemoveDate).HasColumnName("REMOVE_DATE");

                entity.Property(e => e.ScssName)
                    .HasColumnName("SCSS_NAME")
                    .HasMaxLength(60);

                entity.Property(e => e.Telephone)
                    .HasColumnName("TELEPHONE")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");

                entity.Property(e => e.ZipCode)
                    .HasColumnName("ZIP_CODE")
                    .HasMaxLength(40);
            });

            modelBuilder.Entity<DtWorkReportExport>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("DT_WORK_REPORT_EXPORT", "operation");

                entity.Property(e => e.Sid)
                    .HasColumnName("SID")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreateDatetime).HasColumnName("CREATE_DATETIME");

                entity.Property(e => e.EndWorkDatetime).HasColumnName("END_WORK_DATETIME");

                entity.Property(e => e.EndWorkId).HasColumnName("END_WORK_ID");

                entity.Property(e => e.FileName)
                    .HasColumnName("FILE_NAME")
                    .HasMaxLength(256);

                entity.Property(e => e.StartWorkDatetime).HasColumnName("START_WORK_DATETIME");

                entity.Property(e => e.StartWorkId).HasColumnName("START_WORK_ID");
            });

            modelBuilder.Entity<MtNetworkRoute>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("MT_NETWORK_ROUTE", "operation");

                entity.HasIndex(e => e.Code)
                    .HasName("UNQ_MT_NETWORK_ROUTE_CODE")
                    .IsUnique();

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("CODE")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.CreateTimedate).HasColumnName("CREATE_TIMEDATE");

                entity.Property(e => e.NetworkBandwidth).HasColumnName("NETWORK_BANDWIDTH");

                entity.Property(e => e.NetworkName)
                    .HasColumnName("NETWORK_NAME")
                    .HasMaxLength(50);

                entity.Property(e => e.UpdateDatetime).HasColumnName("UPDATE_DATETIME");
            });
        }
    }
}
