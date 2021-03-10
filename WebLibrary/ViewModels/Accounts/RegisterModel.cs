using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebLibrary.ViewModels.Accounts
{
    public class RegisterModel
    {
        [Required(ErrorMessage ="Не указано имя!")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Не указана фамилия!")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Не указан возраст!")]
        public int? Age { get; set; }

        [Required(ErrorMessage = "Не указана почта!")]
        [EmailAddress(ErrorMessage ="Некорректный адрес почты!")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Не указан логин!")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Не указан пароль!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public IFormFile FormPicture { get; set; }
        public string PathAvatar { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage ="Пароли разные!")]
        public string ConfirmPassword { get; set; }
    }
}
