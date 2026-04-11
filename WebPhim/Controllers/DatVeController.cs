using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPhim.Data;
using WebPhim.Models;
using System.Security.Claims;

namespace WebPhim.Controllers
{
    public class DatVeController : Controller
    {
        private readonly DatVeDbContext _context;

        public DatVeController(DatVeDbContext context)
        {
            _context = context;
        }

        // 1. Trang chọn ghế
        public async Task<IActionResult> ChonGhe(int? id)
        {
            if (id == null) return NotFound();

            var lichChieu = await _context.LichChieus
                .Include(l => l.Phim)
                .Include(l => l.Phong)
                    .ThenInclude(p => p.Ghes)
                .Include(l => l.Ves)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (lichChieu == null) return NotFound();

            return View(lichChieu);
        }

        // 2. Xử lý lưu vé vào DB và chuyển sang trang QR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanDatVe(int LichChieuId, string SelectedSeats)
        {
            if (string.IsNullOrEmpty(SelectedSeats))
            {
                return RedirectToAction("ChonGhe", new { id = LichChieuId });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var lichChieu = await _context.LichChieus.FindAsync(LichChieuId);
            if (lichChieu == null) return NotFound();

            var seatIds = SelectedSeats.Split(',').Select(int.Parse).ToList();
            decimal tongTienDatVe = 0;

            foreach (var seatId in seatIds)
            {
                var isBooked = await _context.Ves.AnyAsync(v => v.LichChieuId == LichChieuId && v.GheId == seatId);

                if (!isBooked)
                {
                    var ghe = await _context.Ghes.FindAsync(seatId);
                    decimal giaVe = lichChieu.GiaVe;

                    if (ghe != null)
                    {
                        if (ghe.SoGhe.StartsWith("D") || ghe.SoGhe.StartsWith("E") || ghe.LoaiGhe == "VIP")
                            giaVe += 20000;
                        else if (ghe.SoGhe.StartsWith("A"))
                            giaVe -= 10000;
                    }

                    tongTienDatVe += giaVe;

                    var ve = new Ve
                    {
                        LichChieuId = LichChieuId,
                        GheId = seatId,
                        UserId = userId,
                        NgayDat = DateTime.Now,
                        TongTien = giaVe
                    };

                    _context.Ves.Add(ve);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("ThanhToan", new
            {
                lichChieuId = LichChieuId,
                tongTien = tongTienDatVe,
                gheCount = seatIds.Count
            });
        }

        // 3. Trang hiển thị QR Code
        public IActionResult ThanhToan(int lichChieuId, decimal tongTien, int gheCount)
        {
            ViewBag.TongTien = tongTien;
            ViewBag.GheCount = gheCount;

            string stk = "123456789";
            string nganHang = "MB";
            string noiDung = $"Thanh toan ve lich chieu {lichChieuId}";

            ViewBag.QrUrl = $"https://img.vietqr.io/image/{nganHang}-{stk}-compact.png?amount={tongTien}&addInfo={noiDung}";

            return View();
        }

        // 4. MỚI: Xử lý khi bấm nút "TÔI ĐÃ THANH TOÁN" -> Chuyển sang trang thông báo
        [HttpPost]
        public IActionResult XacNhanThanhToanXong()
        {
            return RedirectToAction("ThongBao");
        }

        // 5. MỚI: Trang thông báo thành công (Có nút Quay về trang chủ)
        public IActionResult ThongBao()
        {
            return View();
        }

        // 6. Trang danh sách lịch sử đặt vé
        public async Task<IActionResult> LichSuDatVe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var lichSu = await _context.Ves
                .Include(v => v.LichChieu)
                    .ThenInclude(l => l.Phim)
                .Include(v => v.LichChieu)
                    .ThenInclude(l => l.Phong)
                        .ThenInclude(p => p.Rap)
                .Include(v => v.Ghe)
                .Where(v => v.UserId == userId)
                .OrderByDescending(v => v.NgayDat)
                .ToListAsync();

            return View(lichSu);
        }
    }
}