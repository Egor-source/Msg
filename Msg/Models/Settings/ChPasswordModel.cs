using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Msg.Models.Settings
{
    public class ChPasswordModel
    {
        //Id пользователя
        public string id { get; set; }

        [Required(ErrorMessage = "Введите старый пароль")]
        //Старый пароль
        public string OldPassword { get; set; }


        [Required(ErrorMessage = "Введите новый пароль")]
        [RegularExpression(@"\S{1,50}", ErrorMessage = "Пароль не может содержать пробелы")]
        [DataType(DataType.Password)]
        //Новый пароль
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Подтвердите пароль")]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        //Подтверждение пароля
        public string ConfirmPassword { get; set; }

    }
}