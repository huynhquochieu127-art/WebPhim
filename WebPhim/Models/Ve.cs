namespace WebPhim.Models
{
    public class Ve
    {
        public int Id { get; set; }
        public DateTime NgayDat { get; set; }
        public decimal TongTien { get; set; }

        // Liên kết với Identity User a
        public string UserId { get; set; }

        // Liên kết với Lịch Chiếu
        public int LichChieuId { get; set; }
        public LichChieu LichChieu { get; set; }

        // Liên kết với Ghế (để biết vé này cho ghế nào)
        public int GheId { get; set; }
        public Ghe Ghe { get; set; }
    }
}
