using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPhim.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using WebPhim.Data;

namespace WebPhim.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PhimController : Controller
    {
        private readonly DatVeDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public PhimController(DatVeDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // 1. DANH SÁCH PHIM
        public async Task<IActionResult> Index(string searchTerm)
        {
            var query = _context.Phims.AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.TenPhim.Contains(searchTerm) || (p.DaoDien != null && p.DaoDien.Contains(searchTerm)));
            }
            ViewBag.SearchTerm = searchTerm;
            return View(await query.ToListAsync());
        }

        // 2. CHI TIẾT PHIM (Ép đường dẫn để không hiện nút đặt vé của người dùng)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var phim = await _context.Phims
                .FirstOrDefaultAsync(m => m.Id == id);

            if (phim == null) return NotFound();

            // Chỉ định rõ ràng file View trong Admin để tránh nhận nhầm Layout người dùng
            return View("~/Areas/Admin/Views/Phim/Details.cshtml", phim);
        }

        // 3. TẠO MỚI PHIM (GET)
        public IActionResult Create() => View();

        // 4. TẠO MỚI PHIM (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Phim phim)
        {
            // Loại bỏ các trường không nhập trực tiếp từ form để ModelState hợp lệ
            ModelState.Remove("HinhAnh");
            ModelState.Remove("LichChieus");

            if (ModelState.IsValid)
            {
                if (phim.FileAnh != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(phim.FileAnh.FileName);
                    string folderPath = Path.Combine(wwwRootPath, "images", "posters");

                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                    string filePath = Path.Combine(folderPath, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await phim.FileAnh.CopyToAsync(fileStream);
                    }
                    phim.HinhAnh = "/images/posters/" + fileName;
                }

                _context.Add(phim);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(phim);
        }

        // 5. CHỈNH SỬA PHIM (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var phim = await _context.Phims.FindAsync(id);
            if (phim == null) return NotFound();
            return View(phim);
        }

        // 6. CHỈNH SỬA PHIM (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Phim phim, IFormFile? FileAnh)
        {
            if (id != phim.Id) return NotFound();

            // Hiếu lưu ý: Remove các trường này để tránh lỗi Validation khi không có ảnh mới hoặc List trống
            ModelState.Remove("FileAnh");
            ModelState.Remove("LichChieus");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPhim = await _context.Phims.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

                    if (FileAnh != null)
                    {
                        string wwwRootPath = _hostEnvironment.WebRootPath;

                        // Xóa ảnh cũ trên server
                        if (existingPhim != null && !string.IsNullOrEmpty(existingPhim.HinhAnh))
                        {
                            string oldFilePath = Path.Combine(wwwRootPath, existingPhim.HinhAnh.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath)) System.IO.File.Delete(oldFilePath);
                        }

                        // Lưu ảnh mới
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(FileAnh.FileName);
                        string folderPath = Path.Combine(wwwRootPath, "images", "posters");
                        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                        string filePath = Path.Combine(folderPath, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await FileAnh.CopyToAsync(stream);
                        }
                        phim.HinhAnh = "/images/posters/" + fileName;
                    }
                    else
                    {
                        // Giữ ảnh cũ nếu không chọn file mới
                        phim.HinhAnh = existingPhim?.HinhAnh;
                    }

                    _context.Update(phim);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Phims.Any(e => e.Id == phim.Id)) return NotFound();
                    else throw;
                }
            }
            return View(phim);
        }

        // 7. XÁC NHẬN XÓA (GET)
        public async Task<IActionResult> Delete(int id)
        {
            var phim = await _context.Phims.FindAsync(id);
            if (phim == null) return NotFound();
            return View(phim);
        }

        // 8. THỰC HIỆN XÓA (POST) - Khớp tên Action "Delete" với nút bấm ở View
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var phim = await _context.Phims.FindAsync(id);
            if (phim != null)
            {
                // Xóa file ảnh vật lý để đỡ tốn dung lượng host
                if (!string.IsNullOrEmpty(phim.HinhAnh))
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string filePath = Path.Combine(wwwRootPath, phim.HinhAnh.TrimStart('/'));
                    if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                }

                _context.Phims.Remove(phim);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}