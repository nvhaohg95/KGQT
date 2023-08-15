using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace KGQT.Models
{
    public partial class KGNewContext : DbContext
    {
        public KGNewContext()
        {
        }

        public KGNewContext(DbContextOptions<KGNewContext> options)
            : base(options)
        {
        }

        public virtual DbSet<tbl_Account> tbl_Accounts { get; set; } = null!;
        public virtual DbSet<tbl_AccountInfo> tbl_AccountInfos { get; set; } = null!;
        public virtual DbSet<tbl_CheckSmallPackage> tbl_CheckSmallPackages { get; set; } = null!;
        public virtual DbSet<tbl_Comment> tbl_Comments { get; set; } = null!;
        public virtual DbSet<tbl_Complain> tbl_Complains { get; set; } = null!;
        public virtual DbSet<tbl_ComplainComment> tbl_ComplainComments { get; set; } = null!;
        public virtual DbSet<tbl_Configuration> tbl_Configurations { get; set; } = null!;
        public virtual DbSet<tbl_FeePackage> tbl_FeePackages { get; set; } = null!;
        public virtual DbSet<tbl_FeeWeight> tbl_FeeWeights { get; set; } = null!;
        public virtual DbSet<tbl_HistoryExport> tbl_HistoryExports { get; set; } = null!;
        public virtual DbSet<tbl_HistoryPayWallet> tbl_HistoryPayWallets { get; set; } = null!;
        public virtual DbSet<tbl_Notification> tbl_Notifications { get; set; } = null!;
        public virtual DbSet<tbl_Package> tbl_Packages { get; set; } = null!;
        public virtual DbSet<tbl_TradeHistory> tbl_TradeHistories { get; set; } = null!;
        public virtual DbSet<tbl_Transaction> tbl_Transactions { get; set; } = null!;
        public virtual DbSet<tbl_UserLevel> tbl_UserLevels { get; set; } = null!;
        public virtual DbSet<tbll_ConfigurationNoti> tbll_ConfigurationNotis { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=LAPTOP-EOMC9SID;Database=KGNew;User Id=sa;Password=abc123;Integrated Security=True; Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tbl_Account>(entity =>
            {
                entity.ToTable("tbl_Account");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.RoleID).HasComment("0. Admin\r\n1. User\r\n2. Quản lý văn phòng\r\n3. Quản lý kho\r\n4. Nhân viên");

                entity.Property(e => e.Status).HasComment("1. Not Active\r\n2. Active\r\n3. Banned");

                entity.Property(e => e.UserLevel).HasComment("1. thành viên\r\n2. dịch vụ order\r\n3. shop đồng\r\n4. shop bạc\r\n5. shop vàng\r\n6. shop kim cương\r\n7. shop vip 1\r\n8. shop vip 2\r\n9. shop vip 3\r\n10. shop vip 4");
            });

            modelBuilder.Entity<tbl_AccountInfo>(entity =>
            {
                entity.ToTable("tbl_AccountInfo");

                entity.Property(e => e.BirthDay).HasColumnType("datetime");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<tbl_CheckSmallPackage>(entity =>
            {
                entity.ToTable("tbl_CheckSmallPackage");

                entity.Property(e => e.CreatedBy).HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedBy).HasMaxLength(250);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
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

                entity.Property(e => e.Type).HasComment("0. Khiếu nại thiếu hàng\r\n1. Khiếu nại sai mẫu (bồi thường)\r\n2. Khiếu nại sai mẫu (trả hàng)");
            });

            modelBuilder.Entity<tbl_ComplainComment>(entity =>
            {
                entity.ToTable("tbl_ComplainComment");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<tbl_Configuration>(entity =>
            {
                entity.ToTable("tbl_Configuration");
            });

            modelBuilder.Entity<tbl_FeePackage>(entity =>
            {
                entity.ToTable("tbl_FeePackage");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<tbl_FeeWeight>(entity =>
            {
                entity.ToTable("tbl_FeeWeight");

                entity.Property(e => e.CreatedBy).HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedBy).HasMaxLength(250);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Type).HasComment("1. Vận chuyển nhanh\r\n2. Vận chuyển tiết kiệm");
            });

            modelBuilder.Entity<tbl_HistoryExport>(entity =>
            {
                entity.ToTable("tbl_HistoryExport");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DateInSG).HasColumnType("date");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasComment("1. Chờ xử lý\r\n2. Đã xử lý\r\n");
            });

            modelBuilder.Entity<tbl_HistoryPayWallet>(entity =>
            {
                entity.ToTable("tbl_HistoryPayWallet");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.TradeType).HasComment("1: Đặt cọc\r\n2: Nhận lại tiền đặt cọc\r\n3: Thanh toán hóa đơn\r\n4: Admin nạp tiền\r\n5: Rút tiền\r\n6: Hủy rút tiền\r\n7. Nhận tiền khiếu nại đơn hàng\r\n8. Trừ tiền lưu kho\r\n9. Truy thu\r\n11.Nạp tiền tại kho\r\n12.Rút tiền tại kho\r\n");

                entity.Property(e => e.Type).HasComment("1: trừ\r\n2: cộng\r\n");
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

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DateExport).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.NgayGiaoHang).HasColumnType("datetime");

                entity.Property(e => e.Price).HasDefaultValueSql("((0))");

                entity.Property(e => e.ReceiveDate).HasColumnType("datetime");

                entity.Property(e => e.SGWarehouseDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasComment("1. Chưa về\r\n2. Đã về\r\n3. Đã giao");

                entity.Property(e => e.YCGWarehouseDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<tbl_TradeHistory>(entity =>
            {
                entity.ToTable("tbl_TradeHistory");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.PayCode).HasComment("Mã thanh toán");

                entity.Property(e => e.PayType).HasComment("1. Nạp tiền\r\n");

                entity.Property(e => e.PaymentMethod).HasComment("1. Tiền măt\r\n2. Chuyển khoản\r\n3. Chọn tài khoản thanh toán\r\n4. Khác");

                entity.Property(e => e.Status).HasComment("1. Chưa duyệt\r\n2. Đã duyệt");

                entity.Property(e => e.TradeType).HasComment("1. Tiền hóa đơn\r\n2. Nạp tiền\r\n3. Tiền khiếu nại\r\n4. Rút tiền");

                entity.Property(e => e.Type).HasComment("1. Admin Tạo\r\n2. User Tạo\r\n3. Đơn hàng đã duyệt tạo");
            });

            modelBuilder.Entity<tbl_Transaction>(entity =>
            {
                entity.ToTable("tbl_Transaction");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.INOUT).HasComment("1. Tiền vào\r\n2. Tiền ra");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasComment("1. Chờ duyệt\r\n2. Đã duyệt");

                entity.Property(e => e.Type).HasComment("1. User gửi yêu cầu\r\n2. Admin tạo");
            });

            modelBuilder.Entity<tbl_UserLevel>(entity =>
            {
                entity.ToTable("tbl_UserLevel");

                entity.Property(e => e.CreatedBy).HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.LevelName).HasMaxLength(250);

                entity.Property(e => e.ModifiedBy).HasMaxLength(250);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
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
