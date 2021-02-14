using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebLibrary.Models;
using WebLibrary.Models.Users;

namespace WebLibrary.Controllers
{
    public class HomeController : Controller
    {
        ApplicationContext _db;

        public HomeController(ApplicationContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            string result = User.Identity.Name;
            if (result == null)
                return RedirectToAction("Login", "Account");
            User user = await _db.Users.FirstOrDefaultAsync(e => e.Email == result);
            ViewData["TotalBooks"] = await _db.Books.CountAsync(u => u.UserId == user.Id);
            return View(user);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
