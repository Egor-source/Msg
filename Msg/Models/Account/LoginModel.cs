using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Msg.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage ="Введите логин")]
        [RegularExpression(@"[A-Za-z0-9]{1,20}", ErrorMessage = "Логин может буквы \" A-Z\" и цифры \"0-9\"")]
        //Логин пользователя
        public string Login { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        //Пароль
        public string Password { get; set; }
    }
}