using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebLibrary.Models.Users;

namespace WebLibrary.Models.Books
{
    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }
        public int Year { get; set; }
        public DateTime AdditionDate { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }

        public int UserId { get; set; }
        public User Users { get; set; }
    }
}
