using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Msg.Models
{
    public class PasswordResetModel
    {
        [Required(ErrorMessage = "Введите логин")]
        public string Login { get; set; }
    }
}