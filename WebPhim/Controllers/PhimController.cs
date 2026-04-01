using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPhim.Data;
using WebPhim.Models;

namespace WebPhim.Controllers
{
    public class PhimController : Controller
    {
        private readonly DatVeDbContext _context;

        public PhimController(DatVeDbContext context)
        {
            _context = context;
        }

        // Trang danh sách tất cả phim cho người dùng
        public async Task<IActionResult> Index()
        {
            var movies = await _context.Phims.AsNoTracking().ToListAsync();
            return View(movies ?? new List<Phim>());
        }

        // Trang chi tiết phim - Đã fix để gọi đúng View trong Views/Phim/Details.cshtml
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var phim = await _context.Phims
                .Include(p => p.LichChieus)
                    .ThenInclude(lc => lc.Phong)
                    .ThenInclude(ph => ph.Rap)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (phim == null) return NotFound();

            return View(phim);
        }
    }
}