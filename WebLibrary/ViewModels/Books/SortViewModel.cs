using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebLibrary.Models;

namespace WebLibrary.ViewModels.Books
{
    public class SortViewModel
    {
        public SortState BookSort { get; set; }
        public SortState AuthorSort { get; set; }
        public SortState YearSort { get; set; }
        public SortState GenreSort { get; set; }
        public SortState DateSort { get; set; }
        public SortState Current { get; set; }
        public bool Up { get; set; } 

        public SortViewModel(SortState sortOrder)
        {
            BookSort = SortState.BookAsc;
            AuthorSort = SortState.AuthorAsc;
            YearSort = SortState.YearAsc;
            GenreSort = SortState.GenreAsc;
            DateSort = SortState.DateAsc;
            Up = true;

            if (sortOrder == SortState.BookDesc || sortOrder == SortState.AuthorDesc
                || sortOrder == SortState.YearDesc || sortOrder == SortState.GenreDesc || sortOrder == SortState.DateDesc)
            {
                Up = false;
            }

            switch (sortOrder)
            {
                case SortState.BookAsc:
                    Current = BookSort = SortState.BookDesc;
                    break;
                case SortState.BookDesc:
                    Current = BookSort = SortState.BookAsc;
                    break;
                case SortState.AuthorAsc:
                    Current = AuthorSort = SortState.AuthorDesc;
                    break;
                case SortState.AuthorDesc:
                    Current = AuthorSort = SortState.AuthorAsc;
                    break;
                case SortState.YearAsc:
                    Current = YearSort = SortState.YearDesc;
                    break;
                case SortState.YearDesc:
                    Current = YearSort = SortState.YearAsc;
                    break;
                case SortState.GenreAsc:
                    Current = GenreSort = SortState.GenreDesc;
                    break;
                case SortState.GenreDesc:
                    Current = GenreSort = SortState.GenreAsc;
                    break;
                case SortState.DateAsc:
                    Current = DateSort = SortState.DateDesc;
                    break;
                default:
                    Current = DateSort = SortState.DateAsc;
                    break;
            }
        }
    }
}
