using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TedarikLojistik.Web.Models.Entities;
using TedarikLojistik.Web.Interfaces.Patterns;
using TedarikLojistik.Web.Interfaces.Repositories;
using TedarikLojistik.Web.Data;
using TedarikLojistik.Web.Data.Repositories;
using TedarikLojistik.Web.Services.Logging;
using TedarikLojistik.Web.Services.Stock;
using TedarikLojistik.Web.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Veritabanı Bağlantısı (SQLite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity Konfigürasyonu
builder.Services.AddIdentity<AppUser, IdentityRole<int>>(options => {
    // Geliştirme kolaylığı için şifre kurallarını esnetiyoruz
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 3;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Cookie Ayarları
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Bağımlılık Enjeksiyonu (DI) - Repositories
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Bağımlılık Enjeksiyonu (DI) - Design Patterns & Services
builder.Services.AddSingleton<IAppLogger>(AppLogger.Instance); // Singleton Pattern
builder.Services.AddScoped<IStockSubject, StockManager>();     // Observer Subject
builder.Services.AddScoped<IStockObserver, EmailNotifier>();   // Observer 1
builder.Services.AddScoped<IStockObserver, SystemNotifier>();  // Observer 2

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Singleton AppLogger için IServiceProvider ataması
var logger = app.Services.GetRequiredService<IAppLogger>() as AppLogger;
logger?.Initialize(app.Services);

// Veritabanını otomatik oluşturma ve Rolleri/Kullanıcıları Ekleme
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    
    try
    {
        context.Database.Migrate();

        // Rolleri Ekle
        string[] roles = { AppRoles.Admin, AppRoles.DepoGorevlisi, AppRoles.Kurye, AppRoles.Musteri };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(role));
            }
        }

        // Default Kullanıcıları Ekle
        if (await userManager.FindByEmailAsync("admin@test.com") == null)
        {
            var adminUser = new AppUser { UserName = "admin@test.com", Email = "admin@test.com", AdSoyad = "Sistem Manager", EmailConfirmed = true };
            await userManager.CreateAsync(adminUser, "123");
            await userManager.AddToRoleAsync(adminUser, AppRoles.Admin);
        }
        
        if (await userManager.FindByEmailAsync("depo@test.com") == null)
        {
            var depoUser = new AppUser { UserName = "depo@test.com", Email = "depo@test.com", AdSoyad = "Depo Görevlisi", Rol = TedarikLojistik.Web.Models.Enums.UserRole.DepoGorevlisi, EmailConfirmed = true };
            await userManager.CreateAsync(depoUser, "123");
            await userManager.AddToRoleAsync(depoUser, AppRoles.DepoGorevlisi);
        }

        if (await userManager.FindByEmailAsync("kurye@test.com") == null)
        {
            var kuryeUser = new AppUser { UserName = "kurye@test.com", Email = "kurye@test.com", AdSoyad = "Kurye", Rol = TedarikLojistik.Web.Models.Enums.UserRole.Kurye, EmailConfirmed = true };
            await userManager.CreateAsync(kuryeUser, "123");
            await userManager.AddToRoleAsync(kuryeUser, AppRoles.Kurye);
        }

        var oldPersonel = await userManager.FindByEmailAsync("personel@test.com");
        if (oldPersonel != null && !await userManager.IsInRoleAsync(oldPersonel, AppRoles.Kurye))
        {
            await userManager.AddToRoleAsync(oldPersonel, AppRoles.Kurye);
        }

        if (await userManager.FindByEmailAsync("musteri@test.com") == null)
        {
            var musteriUser = new AppUser { UserName = "musteri@test.com", Email = "musteri@test.com", AdSoyad = "Müşteri Bir", EmailConfirmed = true };
            await userManager.CreateAsync(musteriUser, "123");
            await userManager.AddToRoleAsync(musteriUser, AppRoles.Musteri);
        }

        if (await userManager.FindByEmailAsync("musteri2@test.com") == null)
        {
            var musteriUser = new AppUser { UserName = "musteri2@test.com", Email = "musteri2@test.com", AdSoyad = "Müşteri İki", EmailConfirmed = true };
            await userManager.CreateAsync(musteriUser, "123");
            await userManager.AddToRoleAsync(musteriUser, AppRoles.Musteri);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Migration / Seed Hatası: {ex.Message}");
    }
}

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

