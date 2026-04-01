using System.ComponentModel.DataAnnotations;

namespace WebPhim.Models
{
    public class LichChieu
    {
        public int Id { get; set; }

        [Required]
        public DateTime ThoiGianChieu { get; set; }

        [Required]
        public decimal GiaVe { get; set; }

        // Liên kết với Phim
        public int PhimId { get; set; }
        public Phim Phim { get; set; }

        // Liên kết với Phòng
        public int PhongId { get; set; }
        public Phong Phong { get; set; }

        // Danh sách các vé đã đặt cho lịch chiếu này
        public ICollection<Ve> Ves { get; set; }
    }
}
