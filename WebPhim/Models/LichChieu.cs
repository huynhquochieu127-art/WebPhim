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

        public int PhimId { get; set; }
        public virtual Phim? Phim { get; set; }

        public int PhongId { get; set; }
        public virtual Phong? Phong { get; set; }

        public virtual ICollection<Ve> Ves { get; set; } = new HashSet<Ve>();
    }
}