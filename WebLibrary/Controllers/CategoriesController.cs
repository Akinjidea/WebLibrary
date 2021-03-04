using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebLibrary.Models;
using WebLibrary.Models.Books;
using WebLibrary.ViewModels.Books;
using WebLibrary.ViewModels.Common;

namespace WebLibrary.Controllers
{
    public class CategoriesController : Controller
    {
        private ApplicationContext _db;

        public CategoriesController(ApplicationContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> List(int? id, int page = 1)
        {
            int pageSize = 10;
            IQueryable<Book> source = _db.Books.Include(a => a.Author).Where(g => g.GenreId == id).OrderBy(i => i.AdditionDate);
            var count = await source.CountAsync();
            var books = await source.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            string name = await _db.Genres.Where(i => i.Id == id).Select(i => i.Name).FirstOrDefaultAsync();
            ViewData["GenreName"] = name;

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            BookPageViewModel viewModel = new BookPageViewModel
            {
                PageViewModel = pageViewModel,
                Book = books
            };

            return View(viewModel);
        }
    }
}
