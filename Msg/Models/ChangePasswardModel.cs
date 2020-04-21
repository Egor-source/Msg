using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Msg.Models
{
    public class ChangePasswardModel
    {
        [Required(ErrorMessage = "Поле Пароль обязательно к заполнению")]
        [RegularExpression(@"\S{1,50}", ErrorMessage = "Пароль не может содержать пробелы")]
        [DataType(DataType.Password)]
        //Пароль
        public string Password { get; set; }

        [Required(ErrorMessage = "Поле Повторения пароля обязательно к заполнению")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        //Подтверждение пароля
        public string PasswordConfirm { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Code { get; set; }
    }
}