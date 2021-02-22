using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebLibrary.ViewModels.Authors
{
    public class AuthorModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Вы не указали имя!")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Вы не указали год рождения!")]
        public int? YearBirth { get; set; }

        [Required(ErrorMessage = "Вы не указали год смерти!")]
        public int? YearDeath { get; set; }

        public int BookCount { get; set; }
    }
}
