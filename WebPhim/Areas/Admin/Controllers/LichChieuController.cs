using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebPhim.Data;
using WebPhim.Models;
using Microsoft.AspNetCore.Authorization;

namespace WebPhim.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class LichChieuController : Controller
    {
        private readonly DatVeDbContext _context;

        public LichChieuController(DatVeDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var lichChieus = await _context.LichChieus
                .Include(l => l.Phim)
                .Include(l => l.Phong)
                    .ThenInclude(p => p.Rap)
                .OrderByDescending(l => l.ThoiGianChieu) // Suất mới nhất hiện lên đầu
                .ToListAsync();

            return View(lichChieus);
        }

        public IActionResult Create()
        {
            PrepareViewBags();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PhimId,PhongId,ThoiGianChieu,GiaVe")] LichChieu lc)
        {
            // Loại bỏ kiểm tra object Phim/Phong vì form chỉ gửi ID
            ModelState.Remove("Phim");
            ModelState.Remove("Phong");

            if (ModelState.IsValid)
            {
                if (lc.ThoiGianChieu < DateTime.Now)
                {
                    ModelState.AddModelError("ThoiGianChieu", "Thời gian chiếu không được ở quá khứ.");
                }
                else
                {
                    // Kiểm tra trùng lịch: cùng phòng, cách nhau dưới 120 phút
                    var isOverlapped = await _context.LichChieus
                        .AnyAsync(l => l.PhongId == lc.PhongId &&
                                     Math.Abs(EF.Functions.DateDiffMinute(l.ThoiGianChieu, lc.ThoiGianChieu)) < 120);

                    if (isOverlapped)
                    {
                        ModelState.AddModelError("ThoiGianChieu", "Phòng này đã có suất chiếu khác trong khoảng thời gian này.");
                    }
                    else
                    {
                        _context.Add(lc);
                        await _context.SaveChangesAsync();
                        TempData["Success"] = "Tạo suất chiếu thành công!";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            PrepareViewBags(lc.PhimId, lc.PhongId);
            return View(lc);
        }

        private void PrepareViewBags(int? selectedPhim = null, int? selectedPhong = null)
        {
            ViewBag.PhimId = new SelectList(_context.Phims.OrderBy(p => p.TenPhim), "Id", "TenPhim", selectedPhim);

            var dsPhong = _context.Phongs.Include(p => p.Rap).Select(p => new {
                Id = p.Id,
                TenHienThi = p.TenPhong + " - " + (p.Rap != null ? p.Rap.TenRap : "N/A")
            }).OrderBy(p => p.TenHienThi).ToList();

            ViewBag.PhongId = new SelectList(dsPhong, "Id", "TenHienThi", selectedPhong);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var lc = await _context.LichChieus.FindAsync(id);
            if (lc != null)
            {
                _context.LichChieus.Remove(lc);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa suất chiếu thành công!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}