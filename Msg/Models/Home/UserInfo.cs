using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Msg.Models.Home
{
    public class UserInfo
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Photo { get; set; }

        public int FriendStatus { get; set; }
    }
}