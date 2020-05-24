using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Msg.Models.Settings
{
    public class ChangeEmailModel
    {
        //id пользователя
        public string id { get; set; }

        [Required(ErrorMessage ="Введите пароль")]
        //Пароль пользователя
        public string Password{ get; set; }

        [Required(ErrorMessage = "Введите Email")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный Email")]
        //Новый Email
        public string NewEmail { get; set; }
    }
}