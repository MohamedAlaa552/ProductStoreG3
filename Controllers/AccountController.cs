using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductStore.Data;
using ProductStore.Models;
using ProductStore.ViewModels;

namespace ProductStore.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _db;
    private readonly PasswordHasher<User> _hasher = new PasswordHasher<User>();

    public AccountController(AppDbContext db)
    {
        _db = db;
    }

    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var email = model.Email.Trim().ToLower();

        var exists = await _db.Users.AnyAsync(u => u.Email == email);
        if (exists)
        {
            ModelState.AddModelError("Email", "Email already registered");
            return View(model);
        }

        var user = new User();
        user.Email = email;
        user.Role = "User";
        user.PasswordHash = _hasher.HashPassword(user, model.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Account created. You can log in now.";
        return RedirectToAction("Login");
    }

    public IActionResult Login(string? returnUrl)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        var email = model.Email.Trim().ToLower();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

        // the same message for both cases, so nobody can find out which emails exist
        if (user == null)
        {
            ModelState.AddModelError("", "Email or password is wrong");
            return View(model);
        }

        var result = _hasher.VerifyHashedPassword(
            user, user.PasswordHash, model.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "Email or password is wrong");
            return View(model);
        }

        var claims = new List<Claim>();
        claims.Add(new Claim(ClaimTypes.Name, user.Email));
        claims.Add(new Claim(ClaimTypes.Role, user.Role));

        var identity = new ClaimsIdentity(claims,
            CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, principal);

        // only go back to a page inside our own site
        if (returnUrl != null && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}
