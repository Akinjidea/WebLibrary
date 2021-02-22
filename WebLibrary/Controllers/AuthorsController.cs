using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebLibrary.Models;
using WebLibrary.Models.Books;
using WebLibrary.ViewModels.Authors;

namespace WebLibrary.Controllers
{
    [Authorize]
    public class AuthorsController : Controller
    {
        ApplicationContext _db;
        public AuthorsController(ApplicationContext applicationContext)
        {
            _db = applicationContext;
        }

        [AllowAnonymous]
        public async Task<IActionResult> List()
        {
            List<AuthorModel> authors = await _db.Authors.Include(a => a.Books)
                .Select(a => new AuthorModel
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    YearBirth=a.YearBirth,
                    YearDeath=a.YearDeath,
                    BookCount=a.Books.Where(b => b.AuthorId == a.Id).Count()
                }).ToListAsync();
            return View(authors);
        }

        public IActionResult AddAuthor()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddAuthor(AuthorModel authorModel)
        {
            
            var sameAuthor = _db.Authors.FirstOrDefault(a => a.FullName == authorModel.FullName);
            if (sameAuthor != null)
                ModelState.AddModelError("FullName", "Такой автор уже существует в базе данных.");
            if (authorModel.YearBirth > authorModel.YearDeath)
                ModelState.AddModelError("YearBirth", "Год рождения не может быть больше года смерти!");
            if(ModelState.IsValid)
            {
                _db.Authors.Add(new Author
                {
                    FullName = authorModel.FullName,
                    YearBirth = (int)authorModel.YearBirth,
                    YearDeath = (int)authorModel.YearDeath,
                });
                await _db.SaveChangesAsync();
                return RedirectToAction("List");
            }
            return View(authorModel);
        }

        public async Task<IActionResult> UpdateAuthor(int? id)
        {
            if (id != null)
            {
                AuthorModel model = await _db.Authors.Where(i => i.Id == id)
                .Select(b => new AuthorModel 
                { 
                    Id = b.Id, 
                    FullName = b.FullName, 
                    YearBirth = b.YearBirth, 
                    YearDeath = b.YearDeath 
                }).FirstOrDefaultAsync();

                if (model != null)
                    return View(model);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateAuthor(AuthorModel authorModel)
        {
            var sameAuthor = _db.Authors.FirstOrDefault(a => a.FullName == authorModel.FullName);
            if (sameAuthor == null)
                ModelState.AddModelError("FullName", "Такого автора нету в базе данных.");
            if (authorModel.YearBirth > authorModel.YearDeath)
                ModelState.AddModelError("YearBirth", "Год рождения не может быть больше года смерти!");
            if (ModelState.IsValid)
            {
                Author author = _db.Authors.Where(i => i.Id == authorModel.Id).FirstOrDefault();
                author.FullName = authorModel.FullName;
                author.YearBirth = (int)authorModel.YearBirth;
                author.YearDeath = (int)authorModel.YearDeath;

                _db.Authors.Update(author);
                await _db.SaveChangesAsync();
                return RedirectToAction("List");
            }
            return View(authorModel);
        }

        [HttpGet]
        [ActionName("DeleteAuthor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                AuthorModel author = await _db.Authors.Where(i => i.Id == id)
                .Select(b => new AuthorModel { 
                    Id = b.Id, 
                    FullName = b.FullName, 
                    YearBirth= b.YearBirth,
                    YearDeath= b.YearDeath 
                }).FirstOrDefaultAsync();

                if (author != null)
                    return View(author);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAuthor(int? id)
        {
            if (id != null)
            {
                Author author = await _db.Authors.FirstOrDefaultAsync(p => p.Id == id);
                if (author != null)
                {
                    _db.Authors.Remove(author);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("List");
                }
            }
            return NotFound();
        }
    }
}
