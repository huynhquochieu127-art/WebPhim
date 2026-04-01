using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebPhim.Data;
using WebPhim.Models;

namespace WebPhim.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LichChieuController : Controller
    {
        private readonly DatVeDbContext _context;

        public LichChieuController(DatVeDbContext context)
        {
            _context = context;
        }

        // 1. Danh sách lịch chiếu
        public async Task<IActionResult> Index()
        {
            var lichChieus = await _context.LichChieus
                .Include(l => l.Phim)
                .Include(l => l.Phong)
                .ThenInclude(p => p.Rap) // Lấy luôn tên Rạp để hiển thị cho rõ
                .OrderByDescending(l => l.ThoiGianChieu)
                .ToListAsync();
            return View(lichChieus);
        }

        // 2. Trang thêm lịch chiếu (GET)
        public IActionResult Create()
        {
            ViewBag.PhimId = new SelectList(_context.Phims, "Id", "TenPhim");

            // Hiển thị tên Phòng kèm tên Rạp để Admin không bị nhầm
            var dsPhong = _context.Phongs.Include(p => p.Rap).Select(p => new {
                Id = p.Id,
                TenHienThi = p.TenPhong + " - " + p.Rap.TenRap
            });
            ViewBag.PhongId = new SelectList(dsPhong, "Id", "TenHienThi");

            return View();
        }

        // 3. Xử lý thêm lịch chiếu (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LichChieu lc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.PhimId = new SelectList(_context.Phims, "Id", "TenPhim", lc.PhimId);
            return View(lc);
        }

        // 4. Xóa lịch chiếu
        public async Task<IActionResult> Delete(int id)
        {
            var lc = await _context.LichChieus.FindAsync(id);
            if (lc != null)
            {
                _context.LichChieus.Remove(lc);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}