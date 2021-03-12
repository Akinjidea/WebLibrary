using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebLibrary.ViewModels.Accounts.Profiles
{
    public class EditUserModel
    {
        public IFormFile UserAvatar { get; set; }
    }
}
