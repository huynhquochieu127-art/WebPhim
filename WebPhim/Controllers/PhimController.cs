using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPhim.Data;
using WebPhim.Models;
using Microsoft.AspNetCore.Authorization; // 1. Thêm namespace này để dùng [Authorize]

namespace WebPhim.Controllers
{
    public class PhimController : Controller
    {
        private readonly DatVeDbContext _context;

        public PhimController(DatVeDbContext context)
        {
            _context = context;
        }

        // Trang danh sách: Cho phép mọi người xem (kể cả chưa đăng nhập)
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var movies = await _context.Phims.AsNoTracking().ToListAsync();
            return View(movies ?? new List<Phim>());
        }

        // Trang chi tiết: BẮT BUỘC ĐĂNG NHẬP
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            // Lấy phim và các suất chiếu từ hiện tại trở đi
            var phim = await _context.Phims
                .Include(p => p.LichChieus.Where(l => l.ThoiGianChieu >= DateTime.Now))
                    .ThenInclude(l => l.Phong)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (phim == null) return NotFound();

            // Nhóm lịch chiếu theo NGÀY để làm tab "Thứ"
            // Key của GroupBy sẽ là phần Ngày (bỏ phần Giờ)
            var lichTheoNgay = phim.LichChieus
                .GroupBy(l => l.ThoiGianChieu.Date)
                .OrderBy(g => g.Key)
                .ToList();

            ViewBag.LichTheoNgay = lichTheoNgay;
            return View(phim);
        }
    }
}