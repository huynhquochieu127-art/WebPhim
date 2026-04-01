using System.ComponentModel.DataAnnotations;

namespace WebPhim.Models
{
    public class Ghe
    {
        public int Id { get; set; }
        [Required]
        public string SoGhe { get; set; } // Ví dụ: A1, B10
        public string? LoaiGhe { get; set; } // Thường, VIP, Couple

        public int PhongId { get; set; }
        public virtual Phong? Phong { get; set; }

        public virtual ICollection<Ve> Ves { get; set; } = new HashSet<Ve>();
    }
}