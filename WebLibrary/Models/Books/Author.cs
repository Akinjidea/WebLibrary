﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebLibrary.Models.Books
{
    public class Author
    {
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
        public int YearBirth { get; set; }
        public int YearDeath { get; set; }

        public List<Book> Books { get; set; } = new List<Book>();
    }
}
