using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebPhim.Data;
using WebPhim.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình Chuỗi kết nối
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// 2. Đăng ký DbContext
builder.Services.AddDbContext<DatVeDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. Cấu hình Identity
builder.Services.AddDefaultIdentity<NguoiDung>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DatVeDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// --- CẤU HÌNH ROUTE (THAY ĐỔI Ở ĐÂY) ---

// 1. Route dành cho Areas (Phải nằm TRƯỚC default route)
app.MapControllerRoute(
    name: "MyAreas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// 2. Route mặc định cho khách xem phim
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// --- PHẦN SEED DATA (GÁN QUYỀN ADMIN) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<NguoiDung>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        var adminEmail = "huynhquochieu127@gmail.com";
        var user = await userManager.FindByEmailAsync(adminEmail);

        if (user != null)
        {
            var isInRole = await userManager.IsInRoleAsync(user, "Admin");
            if (!isInRole)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Lỗi khi gán quyền Admin: " + ex.Message);
    }
}

app.Run();