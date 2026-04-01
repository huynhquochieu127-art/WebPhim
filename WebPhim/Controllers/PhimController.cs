using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPhim.Data; // Phải khớp với namespace trong DatVeDbContext.cs
using WebPhim.Models;

namespace WebPhim.Controllers
{
    // Controller dành cho người dùng xem, không dùng nhãn [Area("Admin")]
    public class PhimController : Controller
    {
        private readonly DatVeDbContext _context;

        public PhimController(DatVeDbContext context)
        {
            _context = context;
        }

        // 1. Trang danh sách phim (Nếu bạn muốn hiện tất cả phim)
        public async Task<IActionResult> Index()
        {
            // Dùng .AsNoTracking() để tải nhanh hơn vì chỉ xem, không sửa
            var movies = await _context.Phims.AsNoTracking().ToListAsync();
            return View(movies);
        }

        // 2. Trang chi tiết phim (Rất quan trọng để nhấn vào MUA VÉ)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var phim = await _context.Phims
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (phim == null) return NotFound();

            return View(phim);
        }
    }
}