using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebPhim.Data;
using WebPhim.Models;

namespace WebPhim.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PhongController : Controller
    {
        private readonly DatVeDbContext _context;

        public PhongController(DatVeDbContext context)
        {
            _context = context;
        }

        // 1. Danh sách phòng (Hiển thị kèm tên Rạp)
        public async Task<IActionResult> Index()
        {
            var dsPhong = await _context.Phongs.Include(p => p.Rap).ToListAsync();
            return View(dsPhong);
        }

        // 2. Trang thêm phòng mới (GET)
        public IActionResult Create()
        {
            // Lấy danh sách rạp đổ vào Dropdown
            ViewBag.RapId = new SelectList(_context.Raps, "Id", "TenRap");
            return View();
        }

        // 3. Xử lý thêm phòng (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Phong phong)
        {
            if (ModelState.IsValid)
            {
                _context.Add(phong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.RapId = new SelectList(_context.Raps, "Id", "TenRap", phong.RapId);
            return View(phong);
        }

        // 4. Xóa phòng
        public async Task<IActionResult> Delete(int id)
        {
            var phong = await _context.Phongs.FindAsync(id);
            if (phong != null)
            {
                _context.Phongs.Remove(phong);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Hiếu có thể viết thêm Edit tương tự như Rap nhé
    }
}