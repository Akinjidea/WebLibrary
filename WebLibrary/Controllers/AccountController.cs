using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebLibrary.Models;
using WebLibrary.Models.Users;
using WebLibrary.ViewModels.Accounts;

namespace WebLibrary.Controllers
{
    public class AccountController : Controller
    {
        ApplicationContext _db;

        public AccountController(ApplicationContext context)
        {
            _db = context;
        }

        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                User user = await _db.Users.FirstOrDefaultAsync(e => e.Email == model.Email && e.Password == model.Password);
                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, model.Email)
                    };
                    ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));

                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("BooksCollection", "Library");
                    }                    
                }
                ModelState.AddModelError("", "Некорректный логин или пароль");
            }
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if(ModelState.IsValid)
            {
                User user = await _db.Users.FirstOrDefaultAsync(e => e.Email == model.Email);
                if (user == null)
                {
                    _db.Users.Add(new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Age = model.Age,
                        Email = model.Email,
                        Password = model.Password
                    });

                    await _db.SaveChangesAsync();
                    return RedirectToAction("Login");
                }
                ModelState.AddModelError("", "Некорректныq логин или пароль");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
