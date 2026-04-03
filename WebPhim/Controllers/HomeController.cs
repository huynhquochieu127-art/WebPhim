using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPhim.Data;
using WebPhim.Models;

public class HomeController : Controller
{
    private readonly DatVeDbContext _context;

    public HomeController(DatVeDbContext context)
    {
        _context = context;
    }

    // ============================================================
    // ACTION TRANG CHỦ - CẬP NHẬT TÌM KIẾM PHIM
    // ============================================================
    public async Task<IActionResult> Index(string searchString)
    {
        // 1. Khởi tạo truy vấn lấy phim đang chiếu
        var query = _context.Phims
            .Where(p => p.KhuVuc == "Phim Đang Chiếu")
            .AsNoTracking();

        // 2. Nếu có nhập từ khóa tìm kiếm (searchString)
        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.ToLower().Trim();

            // Tìm theo tên phim HOẶC thể loại phim (Khớp với placeholder ở Header)
            query = query.Where(p => p.TenPhim.ToLower().Contains(searchString)
                                  || p.LoaiPhim.ToLower().Contains(searchString));

            // Lưu lại từ khóa để hiển thị lại trên ô Input nếu cần
            ViewData["CurrentFilter"] = searchString;
        }

        var danhSachPhim = await query.ToListAsync();

        return View(danhSachPhim ?? new List<Phim>());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var phim = await _context.Phims
            .Include(p => p.LichChieus)
            .ThenInclude(lc => lc.Phong)
            .ThenInclude(ph => ph.Rap)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (phim == null) return NotFound();

        return View(phim);
    }
}