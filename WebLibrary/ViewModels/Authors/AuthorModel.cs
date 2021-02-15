using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebLibrary.ViewModels.Authors
{
    public class AuthorModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int YearBirth { get; set; }
        public int YearDeath { get; set; }
        public int BookCount { get; set; }
    }
}
