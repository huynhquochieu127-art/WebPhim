namespace WebPhim.Models
{
    public class Phong
    {
        public int Id { get; set; }
        public string TenPhong { get; set; }

        public int RapId { get; set; }
        public Rap Rap { get; set; }

        public ICollection<Ghe> Ghes { get; set; }
        public ICollection<LichChieu> LichChieus { get; set; }
    }
}
