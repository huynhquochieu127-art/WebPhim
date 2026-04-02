using System.ComponentModel.DataAnnotations;

namespace WebPhim.Models
{
    public class LichChieu
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thời gian chiếu")]
        [Display(Name = "Thời gian chiếu")]
        public DateTime ThoiGianChieu { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá vé")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá vé không được âm")]
        [Display(Name = "Giá vé (VNĐ)")]
        public decimal GiaVe { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phim")]
        public int PhimId { get; set; }
        public virtual Phim? Phim { get; set; } // Nullable để tránh lỗi Validation

        [Required(ErrorMessage = "Vui lòng chọn phòng")]
        public int PhongId { get; set; }
        public virtual Phong? Phong { get; set; } // Nullable để tránh lỗi Validation

        public virtual ICollection<Ve> Ves { get; set; } = new HashSet<Ve>();
    }
}