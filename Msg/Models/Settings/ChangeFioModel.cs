using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Msg.Models.Settings
{
    public class ChangeFioModel
    {
        //id пользователя
        public string id { get; set; }
        //Новое Имя
        public string Name { get; set; }
        //Новая фамилия
        public string Surname { get; set; }
    }
}