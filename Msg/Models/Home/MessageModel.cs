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
        [Key, Column(Order = 1)]
        public string senderId { get; set; }
        [Key, Column(Order = 2)]
        public string recipientId { get; set; }

        [Required]
        public string text { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime sendingTime { get; set; }

    }
}