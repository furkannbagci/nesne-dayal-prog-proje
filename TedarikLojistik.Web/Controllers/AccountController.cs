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

                if (await _userManager.IsInRoleAsync(user, AppRoles.Personel))
                    return RedirectToAction("Logistics", "Home");
            }
            
            return RedirectToAction("Customer", "Home");
        }

        _logger.LogWarning("Yetkilendirme", "Başarısız giriş denemesi.", email);
        TempData["Error"] = "Giriş başarısız. Lütfen veritabanının seed edildiğinden emin olun.";
        return View();
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
