using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebPhim.Models
{
    public class Phim
    {
        public int Id { get; set; }
        [Required]
        public string TenPhim { get; set; }
        public string MoTa { get; set; }
        public int ThoiLuong { get; set; } // Phút
        public string HinhAnh { get; set; }
        public string? LoaiPhim { get; set; }
        public string DaoDien { get; set; }
        public string? KhuVuc { get; set; }

        // QUAN TRỌNG: Dùng để nhận file từ Form, không lưu vào DB
        [NotMapped]
        public IFormFile? FileAnh { get; set; }

        public virtual ICollection<LichChieu>? LichChieus { get; set; }
    }
}
