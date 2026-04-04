using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebPhim.Data;
using WebPhim.Models;
using Microsoft.AspNetCore.Localization; // Thêm dòng này
using System.Globalization; // Thêm dòng này

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

// --- FIX: Cấu hình Cookie ---
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.LogoutPath = "/Identity/Account/Logout";
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// --- FIX: Cấu hình Localization để nhận diện ngày tháng Tiếng Việt (dd/MM/yyyy) ---
var supportedCultures = new[] { new CultureInfo("vi-VN") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("vi-VN"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

// --- GIỮ NGUYÊN PHẦN MIDDLEWARE ---
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

// Route cho Areas (Admin) và Default
// Đặt cái Admin lên TRƯỚC
// Đặt cái Route cho Area lên TRƯỚC
app.MapControllerRoute(
    name: "MyAreas", // Tên này ông đặt là Admin hay MyAreas đều được
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Rồi mới tới Route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// --- SEED DATA CHO ADMIN ------
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