using System;
using System.Collections.Generic;
using KGQT.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace KGQT.Models
{
    public partial class nhanshiphangContext : DbContext
    {
        public nhanshiphangContext()
        {
        }

        public nhanshiphangContext(DbContextOptions<nhanshiphangContext> options)
            : base(options)
        {
        }

        public virtual DbSet<tbl_Account> tbl_Accounts { get; set; } = null!;
        public virtual DbSet<tbl_BigPackage> tbl_BigPackages { get; set; } = null!;
        public virtual DbSet<tbl_Casso> tbl_Cassos { get; set; } = null!;
        public virtual DbSet<tbl_Comment> tbl_Comments { get; set; } = null!;
        public virtual DbSet<tbl_Complain> tbl_Complains { get; set; } = null!;
        public virtual DbSet<tbl_Configuration> tbl_Configurations { get; set; } = null!;
        public virtual DbSet<tbl_FeeWeight> tbl_FeeWeights { get; set; } = null!;
        public virtual DbSet<tbl_HistoryPayWallet> tbl_HistoryPayWallets { get; set; } = null!;
        public virtual DbSet<tbl_Images> tbl_Images { get; set; } = null!;
        public virtual DbSet<tbl_Notification> tbl_Notifications { get; set; } = null!;
        public virtual DbSet<tbl_Package> tbl_Packages { get; set; } = null!;
        public virtual DbSet<tbl_Point> tbl_Points { get; set; } = null!;
        public virtual DbSet<tbl_Questions> tbl_Questions { get; set; } = null!;
        public virtual DbSet<tbl_Role> tbl_Roles { get; set; } = null!;
        public virtual DbSet<tbl_ShippingOrder> tbl_ShippingOrders { get; set; } = null!;
        public virtual DbSet<tbl_SystemLog> tbl_SystemLogs { get; set; } = null!;
        public virtual DbSet<tbl_TrackShippingOrder> tbl_TrackShippingOrders { get; set; } = null!;
        public virtual DbSet<tbl_Withdraw> tbl_Withdraws { get; set; } = null!;
        public virtual DbSet<tbl_Zalo> tbl_Zalos { get; set; } = null!;
        public virtual DbSet<tbl_ZaloFollewer> tbl_ZaloFollewers { get; set; } = null!;
        public virtual DbSet<tbl_ZaloLog> tbl_ZaloLogs { get; set; } = null!;
        public virtual DbSet<tbl_ZaloWebHook> tbl_ZaloWebHooks { get; set; } = null!;
        public virtual DbSet<tbll_ConfigurationNoti> tbll_ConfigurationNotis { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer(Config.Connections["KGQT"]);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tbl_Account>(entity =>
            {
                entity.ToTable("tbl_Account");

                entity.Property(e => e.Address).HasComment("0. Admin\r\n1. User\r\n2. Quản lý văn phòng\r\n3. Quản lý kho\r\n4. Nhân viên");

                entity.Property(e => e.AvailableSearch).HasDefaultValueSql("((0))");

                entity.Property(e => e.BirthDay).HasColumnType("datetime");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.FullName)
                    .HasMaxLength(50)
                    .HasComment("0. Admin\r\n1. User\r\n2. Quản lý văn phòng\r\n3. Quản lý kho\r\n4. Nhân viên");

                entity.Property(e => e.Gender).HasComment("0. Admin\r\n1. User\r\n2. Quản lý văn phòng\r\n3. Quản lý kho\r\n4. Nhân viên");

                entity.Property(e => e.IsLinkedZalo).HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.Phone)
                    .HasMaxLength(15)
                    .HasComment("0. Admin\r\n1. User\r\n2. Quản lý văn phòng\r\n3. Quản lý kho\r\n4. Nhân viên");

                entity.Property(e => e.RoleID).HasComment("0. Admin\r\n1. User\r\n2. Quản lý văn phòng\r\n3. Quản lý kho\r\n4. Nhân viên");

                entity.Property(e => e.Status).HasComment("1. Not Active\r\n2. Active\r\n3. Banned");

                entity.Property(e => e.UserID).HasMaxLength(50);

                entity.Property(e => e.UserLevel).HasComment("1. thành viên\r\n2. dịch vụ order\r\n3. shop đồng\r\n4. shop bạc\r\n5. shop vàng\r\n6. shop kim cương\r\n7. shop vip 1\r\n8. shop vip 2\r\n9. shop vip 3\r\n10. shop vip 4");

                entity.Property(e => e.Username).HasMaxLength(50);

                entity.Property(e => e.Wallet).HasMaxLength(20);
            });

            modelBuilder.Entity<tbl_BigPackage>(entity =>
            {
                entity.ToTable("tbl_BigPackage");

                entity.Property(e => e.BigPackageDateExpectation).HasColumnType("datetime");

                entity.Property(e => e.BigPackageDateExport).HasColumnType("datetime");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FileName).HasMaxLength(250);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<tbl_Casso>(entity =>
            {
                entity.HasKey(e => e.RecID);

                entity.ToTable("tbl_Casso");

                entity.Property(e => e.RecID).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<tbl_Comment>(entity =>
            {
                entity.ToTable("tbl_Comment");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<tbl_Complain>(entity =>
            {
                entity.ToTable("tbl_Complain");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasComment("1. Chờ duyệt\r\n2. Đã duyệt\r\n3. Hủy");

                entity.Property(e => e.TransId).HasMaxLength(50);

                entity.Property(e => e.Type).HasComment("0. Khiếu nại hệ thống; 1. Khiếu nại kiện;\r\n2. Khiếu nại đơn; 3. Khiếu nại nạp tiền");
            });

            modelBuilder.Entity<tbl_Configuration>(entity =>
            {
                entity.ToTable("tbl_Configuration");
            });

            modelBuilder.Entity<tbl_FeeWeight>(entity =>
            {
                entity.ToTable("tbl_FeeWeight");

                entity.Property(e => e.Amount).HasMaxLength(20);

                entity.Property(e => e.CreatedBy).HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedBy).HasMaxLength(250);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.PriceBrand).HasMaxLength(20);

                entity.Property(e => e.Type).HasComment("1. Vận chuyển nhanh\r\n2. Vận chuyển tiết kiệm");
            });

            modelBuilder.Entity<tbl_HistoryPayWallet>(entity =>
            {
                entity.ToTable("tbl_HistoryPayWallet");

                entity.Property(e => e.Amount).HasMaxLength(20);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.MoneyLeft).HasMaxLength(20);

                entity.Property(e => e.MoneyPrevious).HasMaxLength(20);

                entity.Property(e => e.TradeType).HasComment("1: Thanh toán đơn hàng, \r\n2: Nhận lại tiền hang, 3: Admin nạp tiền\r\n, 4: Rút tiền\r\n5: Hủy rút tiền, 6:Nạp tiền tại kho\r\n7.Rút tiền tại kho\r\n;8: Được tặng");

                entity.Property(e => e.Type).HasComment("1: trừ\r\n2: cộng\r\n");
            });

            modelBuilder.Entity<tbl_Images>(entity =>
            {
                entity.HasKey(e => e.RecID)
                    .HasName("PK__tbl_Imag__360414FF2D414CCC");

                entity.Property(e => e.RecID)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<tbl_Notification>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.NotifType).HasComment("1. Đơn hàng\r\n2. Nạp tiền\r\n3. Rút tiền\r\n4. Yêu cầu giao\r\n5. Khiếu nại\r\n6. Nạp tiền tại kho\r\n7. Rút tiền tại kho\r\n8. Truy thu\r\n9. Phí giao hàng");

                entity.Property(e => e.Status).HasComment("0. chưa đọc\r\n1. đã xem");

                entity.Property(e => e.SuportID).HasComment("ID người hỗ trợ");
            });

            modelBuilder.Entity<tbl_Package>(entity =>
            {
                entity.ToTable("tbl_Package");

                entity.Property(e => e.AirPackagePrice).HasMaxLength(10);

                entity.Property(e => e.ComfirmDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DateExpectation).HasMaxLength(150);

                entity.Property(e => e.DeclarePrice).HasMaxLength(10);

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.Exported).HasDefaultValueSql("((0))");

                entity.Property(e => e.ExportedCNWH)
                    .HasColumnType("datetime")
                    .HasComment("Chờ giao");

                entity.Property(e => e.Height).HasMaxLength(5);

                entity.Property(e => e.ImportedSGWH)
                    .HasColumnType("datetime")
                    .HasComment("Ngày bắt đầu vận chuyển");

                entity.Property(e => e.IsInsurancePrice).HasMaxLength(10);

                entity.Property(e => e.Length).HasMaxLength(5);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.MoreCharge).HasMaxLength(10);

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.Phone).HasMaxLength(12);

                entity.Property(e => e.ReceivedDate)
                    .HasColumnType("datetime")
                    .HasComment("Ngày nhận hàng");

                entity.Property(e => e.Status).HasComment("1. Chưa về\r\n2. Đã về\r\n3. Đã giao");

                entity.Property(e => e.SurCharge).HasMaxLength(10);

                entity.Property(e => e.TotalPrice).HasMaxLength(12);

                entity.Property(e => e.TrackingShipping).HasDefaultValueSql("((0))");

                entity.Property(e => e.TransID).HasMaxLength(50);

                entity.Property(e => e.TransportingToSGWH)
                    .HasColumnType("datetime")
                    .HasComment("Đang giao");

                entity.Property(e => e.WareHouse).HasMaxLength(100);

                entity.Property(e => e.Weight).HasMaxLength(5);

                entity.Property(e => e.WeightExchange).HasMaxLength(5);

                entity.Property(e => e.WeightPrice).HasMaxLength(10);

                entity.Property(e => e.WeightReal).HasMaxLength(5);

                entity.Property(e => e.Width).HasMaxLength(5);

                entity.Property(e => e.WoodPackagePrice).HasMaxLength(10);
            });

            modelBuilder.Entity<tbl_Point>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.OrderID).HasMaxLength(100);

                entity.Property(e => e.Type).HasComment("1: trừ\r\n2: cộng\r\n");
            });

            modelBuilder.Entity<tbl_Questions>(entity =>
            {
                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.MidifiedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            });

            modelBuilder.Entity<tbl_Role>(entity =>
            {
                entity.HasKey(e => e.RoleID);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.RoleName).HasMaxLength(50);
            });

            modelBuilder.Entity<tbl_ShippingOrder>(entity =>
            {
                entity.ToTable("tbl_ShippingOrder");

                entity.Property(e => e.AirPackagePrice).HasMaxLength(20);

                entity.Property(e => e.ChinaExportDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedBy).HasMaxLength(150);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateExpectation).HasColumnType("datetime");

                entity.Property(e => e.DateExpectationEdit).HasColumnType("datetime");

                entity.Property(e => e.InsurancePrice).HasMaxLength(20);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.MoreCharge).HasMaxLength(20);

                entity.Property(e => e.RecID).HasMaxLength(50);

                entity.Property(e => e.ShippingMethodName).HasMaxLength(50);

                entity.Property(e => e.ShippingOrderCode).HasMaxLength(50);

                entity.Property(e => e.SurCharge).HasMaxLength(20);

                entity.Property(e => e.TotalPrice).HasMaxLength(20);

                entity.Property(e => e.Weight).HasMaxLength(10);

                entity.Property(e => e.WeightPrice).HasMaxLength(20);

                entity.Property(e => e.WoodPackagePrice).HasMaxLength(20);
            });

            modelBuilder.Entity<tbl_SystemLog>(entity =>
            {
                entity.Property(e => e.ID).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            });

            modelBuilder.Entity<tbl_TrackShippingOrder>(entity =>
            {
                entity.ToTable("tbl_TrackShippingOrder");

                entity.Property(e => e.CreatedBy).HasMaxLength(100);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifieldBy).HasMaxLength(100);

                entity.Property(e => e.ModifieldOn).HasColumnType("datetime");

                entity.Property(e => e.Type).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<tbl_Withdraw>(entity =>
            {
                entity.ToTable("tbl_Withdraw");

                entity.Property(e => e.AcceptDate).HasColumnType("datetime");

                entity.Property(e => e.Amount).HasMaxLength(20);

                entity.Property(e => e.CreatedBy).HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DateSend).HasColumnType("datetime");

                entity.Property(e => e.ModifiedBy).HasMaxLength(250);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.PaymentMethod).HasComment("1. Tiền mặt\r\n2. Chuyển khoản\r\n");

                entity.Property(e => e.Status).HasComment("1: Đang chờ\r\n2: Thành công\r\n3: Hủy");

                entity.Property(e => e.TradeType).HasComment("1. Phí giao hàng\r\n2. Khác");

                entity.Property(e => e.Type).HasComment("1. Nạp tiền\r\n2. Rút tiền\r\n3. Truy thu\r\n4. Truy thu khác\r\n5. Chi\r\n6. Nạp tiền tại kho\r\n7. Rút tiền tại kho\r\n8. Chi công ty\r\n9. Thu công ty");

                entity.Property(e => e.Username).HasMaxLength(250);
            });

            modelBuilder.Entity<tbl_Zalo>(entity =>
            {
                entity.HasKey(e => e.RecID);

                entity.ToTable("tbl_Zalo");

                entity.Property(e => e.RecID).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.accesstoken_expire).HasColumnType("datetime");

                entity.Property(e => e.expires_in).HasMaxLength(50);

                entity.Property(e => e.freshtoken_expire).HasColumnType("datetime");
            });

            modelBuilder.Entity<tbl_ZaloFollewer>(entity =>
            {
                entity.HasKey(e => e.RecID);

                entity.ToTable("tbl_ZaloFollewer");

                entity.Property(e => e.RecID).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.SendRequestDate).HasColumnType("datetime");

                entity.Property(e => e.Username).HasMaxLength(100);

                entity.Property(e => e.phone).HasMaxLength(12);

                entity.Property(e => e.user_id).HasMaxLength(100);
            });

            modelBuilder.Entity<tbl_ZaloLog>(entity =>
            {
                entity.HasKey(e => e.RecID);

                entity.Property(e => e.RecID).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.user_id).HasMaxLength(50);

                entity.Property(e => e.user_name).HasMaxLength(100);
            });

            modelBuilder.Entity<tbl_ZaloWebHook>(entity =>
            {
                entity.HasKey(e => e.RecID);

                entity.ToTable("tbl_ZaloWebHook");

                entity.Property(e => e.RecID).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.app_id).HasMaxLength(100);

                entity.Property(e => e.event_name).HasMaxLength(100);

                entity.Property(e => e.name).HasMaxLength(100);

                entity.Property(e => e.phone).HasMaxLength(15);

                entity.Property(e => e.recipient).HasMaxLength(100);

                entity.Property(e => e.sender).HasMaxLength(100);

                entity.Property(e => e.timestamp).HasMaxLength(100);
            });

            modelBuilder.Entity<tbll_ConfigurationNoti>(entity =>
            {
                entity.ToTable("tbll_ConfigurationNoti");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
