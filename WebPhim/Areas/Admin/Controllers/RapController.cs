using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPhim.Data;
using WebPhim.Models;

namespace WebPhim.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RapController : Controller
    {
        private readonly DatVeDbContext _context;

        public RapController(DatVeDbContext context)
        {
            _context = context;
        }

        // 1. Trang danh sách rạp
        public async Task<IActionResult> Index()
        {
            var dsRap = await _context.Raps.ToListAsync();
            return View(dsRap);
        }

        // 2. Trang thêm rạp mới (GET)
        public IActionResult Create()
        {
            return View();
        }

        // 3. Xử lý thêm rạp (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Rap rap)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rap);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(rap);
        }

        // 4. Trang sửa rạp (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var rap = await _context.Raps.FindAsync(id);
            if (rap == null) return NotFound();
            return View(rap);
        }

        // 5. Xử lý sửa rạp (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Rap rap)
        {
            if (id != rap.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(rap);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(rap);
        }

        // 6. Xử lý xóa rạp
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var rap = await _context.Raps.FindAsync(id);
            if (rap != null)
            {
                _context.Raps.Remove(rap);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}