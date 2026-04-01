using System.ComponentModel.DataAnnotations;

namespace WebPhim.Models
{
    public class Rap
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên rạp không được để trống")]
        public string TenRap { get; set; }
        public string? DiaChi { get; set; }

        public virtual ICollection<Phong> Phongs { get; set; } = new HashSet<Phong>();
    }
}