using System.ComponentModel.DataAnnotations;

namespace WebPhim.Models
{
    public class Phong
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên phòng không được để trống")]
        public string TenPhong { get; set; }

        public int RapId { get; set; }
        public virtual Rap? Rap { get; set; }

        public virtual ICollection<Ghe> Ghes { get; set; } = new HashSet<Ghe>();
        public virtual ICollection<LichChieu> LichChieus { get; set; } = new HashSet<LichChieu>();
    }
}