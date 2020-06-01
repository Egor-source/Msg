using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Msg.Models
{
    /// <summary>
    /// Модель регистрации
    /// </summary>
    public class RegistrModel
    {

        [Required(ErrorMessage = "Поле Email обязательно к заполнению")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный Email")]
        //Почта
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле Имя обязательно к заполнению")]
        [RegularExpression(@"[A-Za-zА-Яа-я]{1,20}", ErrorMessage = "Имя может содержать только буквы")]
        //Имя
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле Фамилия обязательно к заполнению")]
        [RegularExpression(@"[A-Za-zА-Яа-я]{1,20}", ErrorMessage = "Фамилия может содержать только буквы")]
        //Фамилия
        public string Surname { get; set; }

        [Required(ErrorMessage = "Поле Логин обязательно к заполнению")]
        [RegularExpression(@"[A-Za-z0-9]{1,20}", ErrorMessage = "Логин может буквы \" A-Z\" и цифры \"0-9\"")]
        //Логин
        public string Login { get; set; }

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
        //Фотография
        public HttpPostedFileBase File { get; set; }

        [Required(ErrorMessage = "Выберете пол")]
        //Пол
         public string Gender { get; set; }


        // public DateTime DateOfBirthday { get; set; }
    }
}