using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebLibrary.Models.Books;

namespace WebLibrary.ViewModels.Books
{
    public class BookComments
    {
        public Book Book { get; set; }
        public List<Comment> Comments { get; set; }
        public Comment NewComment { get; set; }
    }
}
