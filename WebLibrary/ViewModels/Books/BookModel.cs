﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebLibrary.ViewModels.Books
{
    public class BookModel
    {        
        public int Id { get; set; }
        [Required(ErrorMessage = "Вы не указали название!")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Вы не указали описание!")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Вы не указали жанр!")]
        public string Genre { get; set; }
        [Required(ErrorMessage = "Вы не указали автора!")]
        public string Author { get; set; }
        [Required(ErrorMessage = "Вы не указали год!")]
        public int Year { get; set; }
    }
}
