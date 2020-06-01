using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Msg.Models.Home
{
    public class MessageModel
    {
        [Key]
        //Id сообщения
        public string Id { get; set; }

        [Required]
        //UserId отправителя 
        public string senderId { get; set; }
        [Required]
        //UserId получателя
        public string recipientId { get; set; }

        [Required]
        //Текст сообщения
        public string text { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        //Дата отправки
        public DateTime sendingTime { get; set; }

    }
}