using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Msg.Models.Home
{
    /// <summary>
    /// Модель друзей
    /// </summary>
    public class Friend
    {
        [Key,Column(Order =1)]
        //Id Первого друга
        public string FriendOneId { get; set; }
        [Key, Column(Order = 2)]
        //Id второго друга
        public string FriendTwoId { get; set; }

        //Статус запроса на дружбу {0-Отправлен запрос на дружбу,1-Запрос на дружбу принят}
        public int Status { get; set; }
       

        //Инициатор запроса
        public string HostRequestId { get; set; }
    }
}