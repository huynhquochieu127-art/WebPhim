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

    public async Task<IActionResult> Index()
    {
        // Lấy danh sách phim và lọc theo KhuVuc
        // Thêm .AsNoTracking() để tăng tốc độ tải trang chủ vì chúng ta chỉ đọc dữ liệu
        var danhSachPhim = await _context.Phims
            .Where(p => p.KhuVuc == "Phim Đang Chiếu")
            .AsNoTracking()
            .ToListAsync();

        // Nếu danh sách trống, view sẽ nhận một List rỗng thay vì null, giúp tránh lỗi giao diện
        return View(danhSachPhim ?? new List<Phim>());
    }
   
}