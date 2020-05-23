using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Msg.Models.Settings
{
    public class ChangePhotoModel
    {
        [Required(ErrorMessage = "Выберите фото")]
        //Новое фото
        public HttpPostedFileBase photo { get; set; }

        //Id пользователя
        public string id { get; set; }
    }
}