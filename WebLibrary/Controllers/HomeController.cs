using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebLibrary.Models;
using WebLibrary.Models.Users;
using WebLibrary.ViewModels.Accounts.Profiles;

namespace WebLibrary.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        ApplicationContext _db;
        IWebHostEnvironment _appEnvironment;
        public HomeController(ApplicationContext applicationContext, IWebHostEnvironment appEnvironment)
        {
            _db = applicationContext;
            _appEnvironment = appEnvironment;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Profile()
        {
            string result = User.Identity.Name;
            if (result == null)
                return RedirectToAction("Login", "Account");
            User user = await _db.Users.FirstOrDefaultAsync(e => e.Email == result);
            ViewData["TotalBooks"] = await _db.Books.CountAsync(u => u.UserId == user.Id);
            return View(user);
        }

        [Route("[controller]/Profile/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            User currentUser = await _db.Users.FirstOrDefaultAsync(i => i.Id == id);
            if(currentUser != null )
            {
                if (currentUser.Email == User.Identity.Name)
                {
                    EditUserModel userModel = new EditUserModel();
                    return View("./Profile/Edit");
                }
                else return Forbid();
            }
            return NotFound();
        }


        [HttpPost]
        [Route("[controller]/Profile/Edit/{id}")]
        public async Task<IActionResult> Edit([FromRoute(Name = "id")] int id, EditUserModel userModel)
        {
            if (userModel.UserAvatar != null)
            {
                User user = await _db.Users.FirstOrDefaultAsync(i => i.Id == id);
                string folderPath = _appEnvironment.WebRootPath + "/Server/Users/Images/";
                if (user.PathAvatar != "noavatar.png" && System.IO.File.Exists(folderPath + user.PathAvatar))
                    System.IO.File.Delete(folderPath + user.PathAvatar);
                string filename = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + Path.GetExtension(userModel.UserAvatar.FileName);
                using (var fileStream = new FileStream(folderPath + filename, FileMode.Create))
                {
                    await userModel.UserAvatar.CopyToAsync(fileStream);
                }                
                user.PathAvatar = filename;
                await _db.SaveChangesAsync();
                return RedirectToAction("Profile");
            }
            else ModelState.AddModelError("UserAvatar", "Поле не должно быть пустым!");
            return View("./Profile/Edit");
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
