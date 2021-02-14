using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebLibrary.Models;
using WebLibrary.Models.Books;
using WebLibrary.ViewModels.Books;

namespace WebLibrary.Controllers
{
    [Authorize]
    public class LibraryController : Controller
    {
        ApplicationContext _db;
        public LibraryController(ApplicationContext applicationContext)
        {
            _db = applicationContext;
        }
        
        public async Task<IActionResult> BooksCollection()
        {
            List<BookModel> books = await _db.Books.Include(a => a.Author)
                .Select(b => new BookModel { Id = b.Id, Name = b.Name, Description = b.Description, Year = b.Year, Author = b.Author.FullName }).ToListAsync();
            return View(books);
        }

        public IActionResult AddBook()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(BookModel bookModel)
        {            
            if(bookModel.Year <= 0)
            {
                ModelState.AddModelError("Year", "Год должен быть положительным.");
                return View();
            }
            var hasAuthor = _db.Books.Include(a => a.Author).FirstOrDefault(a => a.Author.FullName == bookModel.Author);
            if (hasAuthor == null)
            {
                ModelState.AddModelError("Author", "Такого автора не существует в базе данных.");
            }
            else
            {
                _db.Books.Add(new Book { 
                    Name = bookModel.Name, 
                    Description = bookModel.Description, 
                    Year = bookModel.Year, 
                    AuthorId = hasAuthor.Author.Id, 
                    UserId = _db.Users.Where(e => e.Email == User.Identity.Name).Select(i => i.Id).FirstOrDefault()
                });
                await _db.SaveChangesAsync();
                return RedirectToAction("BooksCollection");
            }
            return View(bookModel);
        }

        public async Task<IActionResult> UpdateBook(int? id)
        {
            if (id != null)
            {
                BookModel book = await _db.Books.Include(a => a.Author).Where(i => i.Id == id)
                .Select(b => new BookModel { Id = b.Id, Name = b.Name, Description = b.Description, Year = b.Year, Author = b.Author.FullName }).FirstOrDefaultAsync();

                if (book != null)
                    return View(book);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateBook(BookModel bookModel)
        {
            if (bookModel.Year <= 0)
            {
                ModelState.AddModelError("Year", "Год должен быть положительным.");
                return View();
            }

            var hasAuthor = _db.Books.Include(a => a.Author).FirstOrDefault(a => a.Author.FullName == bookModel.Author);
            if (hasAuthor == null)
            {
                ModelState.AddModelError("Author", "Такого автора не существует в базе данных.");
            }
            else
            {
                Book book = _db.Books.Where(i => i.Id == bookModel.Id).FirstOrDefault();
                book.Name = bookModel.Name;
                book.Description = bookModel.Description;
                book.Year = bookModel.Year;
                book.AuthorId = hasAuthor.Author.Id;

                _db.Books.Update(book);
                await _db.SaveChangesAsync();
                return RedirectToAction("BooksCollection");
            }
            return View(bookModel);
        }

        [HttpGet]
        [ActionName("DeleteBook")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                BookModel book = await _db.Books.Include(a => a.Author).Where(i => i.Id == id)
                .Select(b => new BookModel { Id = b.Id, Name = b.Name, Description = b.Description, Year = b.Year, Author = b.Author.FullName }).FirstOrDefaultAsync();

                if (book != null)
                    return View(book);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteBook(int? id)
        {
            if (id != null)
            {
                Book book = await _db.Books.FirstOrDefaultAsync(p => p.Id == id);
                if (book != null)
                {
                    _db.Books.Remove(book);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("BooksCollection");
                }
            }
            return NotFound();
        }
    }
}
