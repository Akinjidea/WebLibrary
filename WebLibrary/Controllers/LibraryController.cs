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
using WebLibrary.Services;

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

        [AllowAnonymous]
        [Route("{controller}/BooksCollection/Book/{id}")]
        public async Task<IActionResult> Book(int id)
        {
            Book currentBook = await _db.Books.Include(p => p.Author).Include(p => p.Genre).FirstOrDefaultAsync(p => p.Id == id);
            if (currentBook != null)
                return View(currentBook);
            return NotFound();
        }
        
        [AllowAnonymous]
        public async Task<IActionResult> BooksCollection(string search, string sortOrder)
        {
            ViewData["IdSortParam"] = string.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["NameSortParam"] = sortOrder == "book" ? "book_desc" : "book";
            ViewData["DescriptionSortParam"] = sortOrder == "desc" ? "desc_desc" : "desc";
            ViewData["AuthorSortParam"] = sortOrder == "author" ? "author_desc" : "author";
            ViewData["YearSortParam"] = sortOrder == "year" ? "year_desc" : "year";
            ViewData["GenreSortParam"] = sortOrder == "genre" ? "gener_desc" : "genre";
            ViewData["AddDateSortParam"] = sortOrder == "addDate" ? "addDate_desc" : "addDate";
            ViewData["SearchParam"] = search;

            List<Book> books = null;

            if (!string.IsNullOrEmpty(search))
            {
                books = await _db.Books.Include(a => a.Author).Include(a => a.Genre).Where(a => a.Name.Contains(search)).ToListAsync();
            }
            else books = await _db.Books.Include(a => a.Author).Include(a => a.Genre).ToListAsync();

            switch (sortOrder)
            {
                case "id_desc":
                    books = books.OrderByDescending(a => a.Id).ToList();
                    break;
                case "book":
                    books = books.OrderBy(a => a.Name).ToList();
                    break;
                case "book_desc":
                    books = books.OrderByDescending(a => a.Name).ToList();
                    break;
                case "desc":
                    books = books.OrderBy(a => a.Description).ToList();
                    break;
                case "desc_desc":
                    books = books.OrderByDescending(a => a.Description).ToList();
                    break;
                case "author":
                    books = books.OrderBy(a => a.Author.FullName).ToList();
                    break;
                case "author_desc":
                    books = books.OrderByDescending(a => a.Author.FullName).ToList();
                    break;
                case "year":
                    books = books.OrderBy(a => a.Year).ToList();
                    break;
                case "year_desc":
                    books = books.OrderByDescending(a => a.Year).ToList();
                    break;
                case "genre":
                    books = books.OrderBy(a => a.Genre.Name).ToList();
                    break;
                case "genre_desc":
                    books = books.OrderByDescending(a => a.Genre.Name).ToList();
                    break;
                case "addDate":
                    books = books.OrderBy(a => a.AdditionDate).ToList();
                    break;
                case "addDate_desc":
                    books = books.OrderByDescending(a => a.AdditionDate).ToList();
                    break;
                default:
                    books = books.OrderBy(a => a.Id).ToList();
                    break;
            }
            return View(books);
        }

        public IActionResult AddBook()
        {
            TempData.Put("GenreTypes", _db.Genres);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(BookModel bookModel)
        {
            if (bookModel.Year <= 0)
            {
                ModelState.AddModelError("Year", "Год должен быть положительным!");
                return View(bookModel);
            }
            var hasAuthor = _db.Authors.FirstOrDefault(a => a.FullName == bookModel.Author);
            if (hasAuthor == null)
            {
                ModelState.AddModelError("Author", "Такого автора не существует в базе данных!");
            }
            if(ModelState.IsValid)
            {
                _db.Books.Add(new Book { 
                    Name = bookModel.Name, 
                    Description = bookModel.Description, 
                    Year = (int)bookModel.Year,
                    GenreId = (int)bookModel.Genre,
                    AdditionDate = DateTime.Now,
                    AuthorId = hasAuthor.Id, 
                    UserId = _db.Users.Where(e => e.Email == User.Identity.Name).Select(i => i.Id).FirstOrDefault()
                });
                await _db.SaveChangesAsync();
                return RedirectToAction("BooksCollection");
            }
            return View(bookModel);
        }

        public async Task<IActionResult> UpdateBook(int? id)
        {
            if (id != null )
            {
                
                BookModel book = await _db.Books.Include(a => a.Author).Where(i => i.Id == id)
                .Select(b => new BookModel { 
                    Id = b.Id, 
                    Name = b.Name, 
                    Description = b.Description, 
                    Year = b.Year, 
                    Genre = b.GenreId, 
                    Author = b.Author.FullName
                }).FirstOrDefaultAsync();

                if (book != null)
                {
                    TempData.Put("GenreTypes", _db.Genres);
                    return View(book);
                }
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateBook(BookModel bookModel)
        {
            if (bookModel.Year <= 0)
            {
                ModelState.AddModelError("Year", "Год должен быть положительным!");
                return View(bookModel);
            }

            var hasAuthor = _db.Authors.FirstOrDefault(a => a.FullName == bookModel.Author);
            if (hasAuthor == null)
            {
                ModelState.AddModelError("Author", "Такого автора не существует в базе данных!");
            }
            if(ModelState.IsValid)
            {
                Book book = _db.Books.Where(i => i.Id == bookModel.Id).FirstOrDefault();
                book.Name = bookModel.Name;
                book.Description = bookModel.Description;
                book.Year = (int)bookModel.Year;
                book.GenreId = (int)bookModel.Genre;
                book.AdditionDate = DateTime.Now;
                book.AuthorId = hasAuthor.Id;

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
                Book book = await _db.Books.Include(a => a.Author).Include(a => a.Genre).Where(i => i.Id == id).FirstOrDefaultAsync();
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
