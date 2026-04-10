using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebPhim.Data;
using WebPhim.Models;

namespace WebPhim.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GheController : Controller
    {
        private readonly DatVeDbContext _context;

        public GheController(DatVeDbContext context)
        {
            _context = context;
        }

        // 1. Danh sách ghế của một phòng cụ thể
        public async Task<IActionResult> Index(int? phongId)
        {
            if (phongId == null) return NotFound();

            var gheCuaPhong = await _context.Ghes
                .Include(g => g.Phong)
                .Where(g => g.PhongId == phongId)
                // Sắp xếp theo Tên hàng (A->E) trước, sau đó mới đến số ghế
                .OrderBy(g => g.SoGhe.Substring(0, 1))
                .ThenBy(g => g.SoGhe.Length)
                .ThenBy(g => g.SoGhe)
                .ToListAsync();

            ViewBag.PhongId = phongId;
            var phong = await _context.Phongs.Include(p => p.Rap).FirstOrDefaultAsync(p => p.Id == phongId);
            ViewBag.TenPhong = $"{phong?.TenPhong} - {phong?.Rap?.TenRap}";

            return View(gheCuaPhong);
        }

        // 2. HÀM TỰ ĐỘNG TẠO GHẾ: Fix hàng D và E là VIP
        [HttpPost]
        public async Task<IActionResult> AutoGenerateSeats(int phongId)
        {
            // Kiểm tra xem phòng này đã có ghế chưa
            if (_context.Ghes.Any(g => g.PhongId == phongId))
            {
                return RedirectToAction(nameof(Index), new { phongId = phongId });
            }

            string[] hàng = { "A", "B", "C", "D", "E" }; // 5 hàng
            for (int i = 0; i < hàng.Length; i++)
            {
                for (int j = 1; j <= 10; j++) // Mỗi hàng 10 ghế
                {
                    var ghe = new Ghe
                    {
                        SoGhe = hàng[i] + j,
                        // FIX TẠI ĐÂY: Nếu hàng là D hoặc E thì set là VIP, còn lại là Thường
                        LoaiGhe = (hàng[i] == "D" || hàng[i] == "E") ? "VIP" : "Thường",
                        PhongId = phongId
                    };
                    _context.Ghes.Add(ghe);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { phongId = phongId });
        }
    }
}