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
using WebLibrary.ViewModels.Common;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace WebLibrary.Controllers
{
    [Authorize]
    public class LibraryController : Controller
    {
        ApplicationContext _db;
        IWebHostEnvironment _appEnvironment;
        public LibraryController(ApplicationContext applicationContext, IWebHostEnvironment appEnvironment)
        {
            _db = applicationContext;
            _appEnvironment = appEnvironment;
        }

        [AllowAnonymous]
        [Route("[controller]/BooksCollection/Book/{id}")]
        public async Task<IActionResult> Book(int id)
        {
            BookComments currentBook = new BookComments { Book = await _db.Books.Include(p => p.Author).Include(p => p.Genre).FirstOrDefaultAsync(p => p.Id == id) };
            if (currentBook.Book != null)
            {
                currentBook.Comments = await _db.Comments.Where(i => i.BookId==id).Include(i => i.User).OrderByDescending(i => i.PublicationDate).ToListAsync();
                return View(currentBook);
            }
                
            return NotFound();
        }

        [HttpPost]
        [Route("[controller]/BooksCollection/Book/{id}")]
        public async Task<IActionResult> AddComment ([FromRoute(Name ="id")] int id, BookComments bookComments)
        {
            if (bookComments.NewComment.Content.Length > 3)
            {
                _db.Comments.Add(new Comment
                {
                    UserId = _db.Users.Where(i => i.Email == User.Identity.Name).Select(i => i.Id).FirstOrDefault(),
                    Content = bookComments.NewComment.Content,
                    PublicationDate = DateTime.Now,
                    BookId = id
                });
                await _db.SaveChangesAsync();
            }
            else ModelState.AddModelError("NewComment.Content", "Комментарий слишком короткий!");
            return RedirectToAction("Book", new { id = id });
        }

        [AllowAnonymous]
        public async Task<IActionResult> BooksCollection(string search, SortState sortOrder = SortState.DateDesc, string view = "list", int page = 1)
        {
            ViewData["SearchParam"] = search;
            ViewData["ViewType"] = view;

            int pageSize = 10;
            IQueryable<Book> source = !string.IsNullOrEmpty(search) ? source = _db.Books
                .Include(a => a.Author)
                .Include(a => a.Genre)
                .Include(a => a.Users)
                .Include(a => a.Comments)
                .Where(a => a.Name.Contains(search)) : _db.Books
                .Include(a => a.Author)
                .Include(a => a.Genre)
                .Include(a => a.Users)
                .Include(a => a.Comments);
            var count = await source.CountAsync();
            var books = await source.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            books = sortOrder switch
            {
                SortState.BookAsc => books.OrderBy(a => a.Name).ToList(),
                SortState.BookDesc => books.OrderByDescending(a => a.Name).ToList(),
                SortState.AuthorAsc => books.OrderBy(a => a.Author.FullName).ToList(),
                SortState.AuthorDesc => books.OrderByDescending(a => a.Author.FullName).ToList(),
                SortState.YearAsc => books.OrderBy(a => a.Year).ToList(),
                SortState.YearDesc => books.OrderByDescending(a => a.Year).ToList(),
                SortState.GenreAsc => books.OrderBy(a => a.Genre.Name).ToList(),
                SortState.GenreDesc => books.OrderByDescending(a => a.Genre.Name).ToList(),
                SortState.DateAsc => books.OrderBy(a => a.AdditionDate).ToList(),
                _ => books.OrderByDescending(a => a.AdditionDate).ToList()
            };

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            BookPageViewModel viewModel = new BookPageViewModel
            {
                PageViewModel = pageViewModel,
                Book = books,
                SortViewModel = new SortViewModel(sortOrder)
            };

            return View(viewModel);
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
            }
            if (bookModel.PageCount <=0)
            {
                ModelState.AddModelError("PageCount", "Количество страниц должно быть положительным!");
                return View(bookModel);
            }
            var hasAuthor = _db.Authors.FirstOrDefault(a => a.FullName == bookModel.Author);
            if (hasAuthor == null)
            {
                ModelState.AddModelError("Author", "Такого автора не существует в базе данных!");
            }
            if(ModelState.IsValid)
            {
                Book newBook = new Book
                {
                    Name = bookModel.Name,
                    Description = bookModel.Description,
                    Year = (int)bookModel.Year,
                    PageCount = (int)bookModel.PageCount,
                    ISBN = bookModel.ISBN,
                    GenreId = (int)bookModel.Genre,
                    AdditionDate = DateTime.Now,
                    AuthorId = hasAuthor.Id,
                    UserId = _db.Users.Where(e => e.Email == User.Identity.Name).Select(i => i.Id).FirstOrDefault()
                };
                if (bookModel.CoverBook != null)
                {
                    string filename = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + Path.GetExtension(bookModel.CoverBook.FileName);
                    string path = "/Server/Books/Images/" + filename;
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await bookModel.CoverBook.CopyToAsync(fileStream);
                    }
                    newBook.PathCover = filename;
                }
                else newBook.PathCover = "nocover.jpg";
                await _db.Books.AddAsync(newBook);
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
                    PageCount = b.PageCount,
                    ISBN = b.ISBN,
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
            }
            if (bookModel.PageCount <= 0)
            {
                ModelState.AddModelError("PageCount", "Количество страниц должно быть положительным!");
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
                book.PageCount = (int)bookModel.PageCount;
                book.ISBN = bookModel.ISBN;
                book.GenreId = (int)bookModel.Genre;
                book.AdditionDate = DateTime.Now;
                book.AuthorId = hasAuthor.Id;

                string folderPath = _appEnvironment.WebRootPath + "/Server/Books/Images/";
                if (book.PathCover != "nocover.jpg" && System.IO.File.Exists(folderPath + book.PathCover))
                    System.IO.File.Delete(folderPath + book.PathCover);

                if (bookModel.CoverBook != null)
                {
                    string filename = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + Path.GetExtension(bookModel.CoverBook.FileName);
                    using (var fileStream = new FileStream(folderPath + filename, FileMode.Create))
                    {
                        await bookModel.CoverBook.CopyToAsync(fileStream);
                    }
                    book.PathCover = filename;
                }
                else book.PathCover = "nocover.jpg";

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
