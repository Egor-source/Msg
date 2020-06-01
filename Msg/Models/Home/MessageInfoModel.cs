using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Msg.Models.Home
{
    public class MessageInfoModel
    {
        public string Photo { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Text { get; set; }
        public string SendingTime { get; set; }
        public string SenderId { get; set; }
       
    }
}