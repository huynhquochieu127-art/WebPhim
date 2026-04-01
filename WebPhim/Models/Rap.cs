namespace WebPhim.Models
{
    public class Rap
    {
        public int Id { get; set; }
        public string TenRap { get; set; }
        public string DiaChi { get; set; }

        public ICollection<Phong> Phongs { get; set; }
    }
}
