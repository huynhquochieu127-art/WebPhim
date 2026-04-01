using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPhim.Data;

namespace WebPhim.Controllers
{
    public class DatVeController : Controller
    {
        private readonly DatVeDbContext _context;

        public DatVeController(DatVeDbContext context)
        {
            _context = context;
        }

        // Trang chọn ghế dựa trên ID của Lịch Chiếu
        public async Task<IActionResult> ChonGhe(int? id)
        {
            if (id == null) return NotFound();

            var lichChieu = await _context.LichChieus
                .Include(l => l.Phim)
                .Include(l => l.Phong)
                    .ThenInclude(p => p.Ghes) // QUAN TRỌNG: Phải có dòng này để lấy danh sách ghế
                .FirstOrDefaultAsync(m => m.Id == id);

            if (lichChieu == null) return NotFound();

            return View(lichChieu);
        }
    }
}