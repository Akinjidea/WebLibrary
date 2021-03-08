using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebLibrary.Models.Books;
using WebLibrary.ViewModels.Common;

namespace WebLibrary.ViewModels.Books
{
    public class BookPageViewModel
    {
        public IEnumerable<Book> Book { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
    }
}
