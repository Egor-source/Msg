using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Msg.Models.Home
{
    public class MainHubModel
    {
        public List<string> ConnectionId { get; set; }

        public string UserId { get; set; }

        public int TabsCount { get; set; }

        public MainHubModel(string connectionId,string userId)
        {
            ConnectionId = new List<string>();
            ConnectionId.Add(connectionId);
            UserId = userId;
            TabsCount = 1;
        }


        public void AddTab(string connectionId)
        {
            ConnectionId.Add(connectionId);
            TabsCount++;
        }

        public void RemoveTab(string connectionId)
        {
            ConnectionId.Remove(connectionId);
            TabsCount--;
        }
    }
}