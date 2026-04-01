using Microsoft.AspNetCore.Identity;

namespace WebPhim.Models
{
    public class NguoiDung : IdentityUser
    {
        // Thêm các trường bạn muốn lưu thêm (ví dụ)
        public string HoTen { get; set; }
        public string DiaChi { get; set; }
    }
}