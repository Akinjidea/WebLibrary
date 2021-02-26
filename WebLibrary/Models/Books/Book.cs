using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebLibrary.Models.Users;

namespace WebLibrary.Models.Books
{
    public class Book
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public int Year { get; set; }
        public int PageCount { get; set; }
        public string ISBN { get; set; }
        public DateTime AdditionDate { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }

        public int GenreId { get; set; }
        public Genre Genre { get; set; }

        public int UserId { get; set; }
        public User Users { get; set; }

        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
