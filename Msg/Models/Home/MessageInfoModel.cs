using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Msg.Models.Home
{
    public class MessageInfoModel
    {
        //Фото отправителя сообщения
        public string Photo { get; set; }
        //Имя отправителя 
        public string Name { get; set; }
        //Фамилия отправителя 
        public string Surname { get; set; }
        //Текст сообщения
        public string Text { get; set; }
        //Время отправки сообщения
        public string SendingTime { get; set; }
        //UserId отправителя
        public string SenderId { get; set; }
       
    }
}