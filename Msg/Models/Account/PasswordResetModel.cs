using System.ComponentModel.DataAnnotations;


namespace Msg.Models
{
    public class PasswordResetModel
    {
        [Required(ErrorMessage = "Введите логин")]
        //Логин пользователя
        public string Login { get; set; }
    }
}