﻿
using System.ComponentModel.DataAnnotations;


namespace Msg.Models
{
    public class ChangePasswordModel
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
        //Id пользователя
        public string UserId { get; set; }

        [Required]
        //Токен для сброса пароля
        public string Code { get; set; }
    }
}