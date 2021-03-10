using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
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
        IWebHostEnvironment _appEnvironment;
        public AccountController(ApplicationContext applicationContext, IWebHostEnvironment appEnvironment)
        {
            _db = applicationContext;
            _appEnvironment = appEnvironment;
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
                    User newUser = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Age = (int)model.Age,
                        Email = model.Email,
                        Login = model.Login,
                        Password = model.Password
                    };
                    if (model.FormPicture != null)
                    {
                        string filename = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + Path.GetExtension(model.FormPicture.FileName);
                        string path = "/Server/Users/Images/" + filename;
                        using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                        {
                            await model.FormPicture.CopyToAsync(fileStream);
                        }
                        newUser.PathAvatar = filename;
                    }
                    else newUser.PathAvatar = "noavatar.png";
                    await _db.Users.AddAsync(newUser);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Login");
                }
                ModelState.AddModelError("", "Некорректный логин или пароль");
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
