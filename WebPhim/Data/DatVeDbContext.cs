using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebPhim.Models;

namespace WebPhim.Data // ĐÃ FIX: Đổi từ .Models thành .Data để khớp với folder mới
{
    public class DatVeDbContext : IdentityDbContext<NguoiDung>
    {
        public DatVeDbContext(DbContextOptions<DatVeDbContext> options) : base(options) { }

        // Khai báo các bảng dữ liệu - Lưu ý tên là Phims (số nhiều)
        public DbSet<Phim> Phims { get; set; }
        public DbSet<Rap> Raps { get; set; }
        public DbSet<Phong> Phongs { get; set; }
        public DbSet<Ghe> Ghes { get; set; }
        public DbSet<LichChieu> LichChieus { get; set; }
        public DbSet<Ve> Ves { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Cấu hình bảng Vé: Tắt xóa dây chuyền từ Lịch Chiếu
            builder.Entity<Ve>()
                .HasOne(v => v.LichChieu)
                .WithMany(lc => lc.Ves)
                .HasForeignKey(v => v.LichChieuId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình bảng Vé: Tắt xóa dây chuyền từ Ghế
            builder.Entity<Ve>()
                .HasOne(v => v.Ghe)
                .WithMany()
                .HasForeignKey(v => v.GheId)
                .OnDelete(DeleteBehavior.Restrict);
            // Cấu hình Lịch Chiếu: Tắt xóa dây chuyền từ Phòng
            builder.Entity<LichChieu>()
                .HasOne(lc => lc.Phong)
                .WithMany(p => p.LichChieus)
                .HasForeignKey(lc => lc.PhongId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình Phòng: Tắt xóa dây chuyền từ Rạp (Tùy chọn, nếu Hiếu muốn an toàn tuyệt đối)
            builder.Entity<Phong>()
                .HasOne(p => p.Rap)
                .WithMany(r => r.Phongs)
                .HasForeignKey(p => p.RapId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}