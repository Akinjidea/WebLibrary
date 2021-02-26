using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebLibrary.Models;
using WebLibrary.Models.Books;

namespace WebLibrary.Controllers
{
    public class CategoriesController : Controller
    {
        private ApplicationContext _db;

        public CategoriesController(ApplicationContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> List(int? id)
        {
            List<Book> books = await _db.Books.Include(a => a.Author).Where(g => g.GenreId == id).OrderBy(i => i.AdditionDate).ToListAsync();
            string name = await _db.Genres.Where(i => i.Id == id).Select(i => i.Name).FirstOrDefaultAsync();
            ViewData["GenreName"] = name;
            return View(books);
        }
    }
}
