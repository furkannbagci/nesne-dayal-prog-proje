using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TedarikLojistik.Web.Authorization;
using TedarikLojistik.Web.Interfaces.Patterns;
using TedarikLojistik.Web.Models.Entities;

namespace TedarikLojistik.Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IAppLogger _logger;

    public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IAppLogger logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login()
    {
        // Kullanıcı zaten giriş yapmışsa ana sayfaya yönlendir
        if (User.Identity != null && User.Identity.IsAuthenticated)
            return RedirectToAction("Index", "Home");
            
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);
        
        if (result.Succeeded)
        {
            _logger.LogInfo("Yetkilendirme", "Kullanıcı sisteme giriş yaptı.", email);

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                // Giriş yapan role göre ilgili panele yönlendir.
                if (await _userManager.IsInRoleAsync(user, AppRoles.Admin))
                    return RedirectToAction("Admin", "Home");

                if (await _userManager.IsInRoleAsync(user, AppRoles.DepoGorevlisi))
                    return RedirectToAction("Admin", "Home");

                if (await _userManager.IsInRoleAsync(user, AppRoles.Kurye))
                    return RedirectToAction("Logistics", "Home");
            }
            
            return RedirectToAction("Customer", "Home");
        }

        _logger.LogWarning("Yetkilendirme", "Başarısız giriş denemesi.", email);
        TempData["Error"] = "Giriş başarısız. Lütfen veritabanının seed edildiğinden emin olun.";
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string email, string password, string fullName)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            TempData["Error"] = "Bu e-posta adresi zaten kayıtlı.";
            return View();
        }

        // Yeni kayıtlar müşteri rolüyle başlar.
        var user = new AppUser
        {
            UserName = email,
            Email = email,
            AdSoyad = fullName,
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            TempData["Error"] = string.Join(" ", createResult.Errors.Select(e => e.Description));
            return View();
        }

        await _userManager.AddToRoleAsync(user, AppRoles.Musteri);
        _logger.LogInfo("Yetkilendirme", "Yeni müşteri kaydı oluşturuldu.", email, user.Id);
        TempData["Message"] = "Kayıt oluşturuldu. E-posta ve şifrenizle giriş yapabilirsiniz.";
        return RedirectToAction("Login");
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
