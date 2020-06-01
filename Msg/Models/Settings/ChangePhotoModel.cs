using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Msg.Models.Settings
{
    public class ChangePhotoModel
    {
        [Required(ErrorMessage = "Выберите фото")]
        //Новое фото
        public HttpPostedFileBase Photo { get; set; }

        //Id пользователя
        public string Id { get; set; }
    }
}