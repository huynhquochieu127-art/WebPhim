namespace WebPhim.Models
{
    public class Ghe
    {
        public int Id { get; set; }
        public string SoGhe { get; set; } // Ví dụ: A1, B10
        public string LoaiGhe { get; set; } // Thường, VIP, Couple

        public int PhongId { get; set; }
        public Phong Phong { get; set; }
    }
}
