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
        // Lấy danh sách phim đang chiếu
        var danhSachPhim = await _context.Phims
            .Where(p => p.KhuVuc == "Phim Đang Chiếu")
            .AsNoTracking()
            .ToListAsync();

        return View(danhSachPhim ?? new List<Phim>());
    }

    // ============================================================
    // ACTION CHI TIẾT PHIM - FIX LỖI 404
    // ============================================================
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        // Tìm phim theo Id, kèm theo danh sách Lịch Chiếu của phim đó
        var phim = await _context.Phims
            .Include(p => p.LichChieus) // Load lịch chiếu
            .ThenInclude(lc => lc.Phong) // Load thông tin phòng của lịch chiếu đó
            .ThenInclude(ph => ph.Rap)   // Load thông tin rạp của phòng đó
            .FirstOrDefaultAsync(m => m.Id == id);

        if (phim == null)
        {
            return NotFound();
        }

        return View(phim);
    }
}