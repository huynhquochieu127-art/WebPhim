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
                // Sắp xếp theo độ dài trước, sau đó mới đến tên ghế để A1 < A10
                .OrderBy(g => g.SoGhe.Length)
                .ThenBy(g => g.SoGhe)
                .ToListAsync();

            ViewBag.PhongId = phongId;
            var phong = await _context.Phongs.Include(p => p.Rap).FirstOrDefaultAsync(p => p.Id == phongId);
            ViewBag.TenPhong = $"{phong?.TenPhong} - {phong?.Rap?.TenRap}";

            return View(gheCuaPhong);
        }

        // 2. HÀM CỰC HAY: Tự động tạo 50 ghế cho phòng
        [HttpPost]
        public async Task<IActionResult> AutoGenerateSeats(int phongId)
        {
            // Kiểm tra xem phòng này đã có ghế chưa để tránh tạo trùng
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
                        SoGhe = hàng[i] + j, // Kết quả: A1, A2... E10
                        LoaiGhe = (hàng[i] == "E") ? "VIP" : "Thường", // Hàng E cho làm VIP luôn
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