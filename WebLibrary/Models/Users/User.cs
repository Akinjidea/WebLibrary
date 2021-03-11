using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebLibrary.Models.Books;

namespace WebLibrary.Models.Users
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public int Age { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string PathAvatar { get; set; }
        [Required]
        public string Password { get; set; }

        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<Book> Books { get; set; } = new List<Book>();
    }
}
