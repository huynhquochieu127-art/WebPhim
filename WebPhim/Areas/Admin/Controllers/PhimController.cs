using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPhim.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using WebPhim.Data; // Đảm bảo folder chứa DatVeDbContext là 'Data'

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

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Phim phim)
        {
            ModelState.Remove("HinhAnh");
            ModelState.Remove("LichChieus");

            if (ModelState.IsValid)
            {
                if (phim.FileAnh != null)
                {
                    // Dùng _hostEnvironment.WebRootPath để lấy đường dẫn wwwroot chuẩn
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(phim.FileAnh.FileName);

                    // FIX: Thêm "posters" vào đường dẫn kết hợp
                    string folderPath = Path.Combine(wwwRootPath, "images", "posters");

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    string filePath = Path.Combine(folderPath, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await phim.FileAnh.CopyToAsync(fileStream);
                    }

                    // Gán đường dẫn chuẩn để hiển thị trên web
                    phim.HinhAnh = "/images/posters/" + fileName;
                }

                try
                {
                    _context.Add(phim);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi lưu vào DB: " + ex.Message);
                }
            }
            return View(phim);
        }
        // 1. Giao diện Sửa
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var phim = await _context.Phims.FindAsync(id);
            if (phim == null) return NotFound();
            return View(phim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Phim phim, IFormFile? FileAnh)
        {
            if (id != phim.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (FileAnh != null)
                    {
                        string wwwRootPath = _hostEnvironment.WebRootPath;

                        // --- KHÚC NÀY LÀ ĐỂ XÓA ẢNH CŨ ---
                        // Lấy lại thông tin phim từ DB để biết tên file cũ
                        var oldPhim = await _context.Phims.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                        if (oldPhim != null && !string.IsNullOrEmpty(oldPhim.HinhAnh))
                        {
                            // Chuyển đường dẫn ảo (/images/posters/...) thành đường dẫn vật lý (C:\...)
                            string oldFilePath = Path.Combine(wwwRootPath, oldPhim.HinhAnh.TrimStart('/'));

                            // Nếu file tồn tại thì xóa đi
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }
                        // ---------------------------------

                        // Lưu file mới như bình thường
                        string fileName = Guid.NewGuid().ToString() + "_" + FileAnh.FileName;
                        string folderPath = Path.Combine(wwwRootPath, "images", "posters");

                        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                        string filePath = Path.Combine(folderPath, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await FileAnh.CopyToAsync(stream);
                        }

                        phim.HinhAnh = "/images/posters/" + fileName;
                    }

                    _context.Update(phim);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException) { /* ... */ }
            }
            return View(phim);
        }
        // 1. Hiển thị trang xác nhận xóa (Giao diện Hiếu vừa gửi)
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var phim = await _context.Phims.FindAsync(id);
            if (phim == null) return NotFound();
            return View(phim);
        }

        // 2. Thực hiện xóa vĩnh viễn (Khớp với asp-action="DeleteConfirmed")
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var phim = await _context.Phims.FindAsync(id);
            if (phim != null)
            {
                // XÓA FILE ẢNH VẬT LÝ TRONG THƯ MỤC POSTERS
                if (!string.IsNullOrEmpty(phim.HinhAnh))
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    // Loại bỏ dấu gạch chéo ở đầu đường dẫn ảo để nối chuỗi chuẩn
                    string filePath = Path.Combine(wwwRootPath, phim.HinhAnh.TrimStart('/'));

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                // Xóa dữ liệu trong Database
                _context.Phims.Remove(phim);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}