    using System.ComponentModel.DataAnnotations;

    namespace WebPhim.Models
    {
        public class Ve
        {
            public int Id { get; set; }
            public DateTime NgayDat { get; set; } = DateTime.Now;
            public decimal TongTien { get; set; }

            // Liên kết với Identity User
            public string UserId { get; set; }

            public int LichChieuId { get; set; }
            public virtual LichChieu? LichChieu { get; set; }

            public int GheId { get; set; }
            public virtual Ghe? Ghe { get; set; }
        }
    }