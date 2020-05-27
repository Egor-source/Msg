using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Msg.Models.Home
{
    public class Friend
    {
        [Key,Column(Order =1)]
        public string FriendOneId { get; set; }
        [Key, Column(Order = 2)]
        public string FriendTwoId { get; set; }

        public int Status
        {
            get { return Status; }
            set
            {
                if (value >= 0 && value <= 3)
                    Status = value;
            }
        }

        //Инициатор запроса
        public string RequestSenderId { get; set; }
    }
}