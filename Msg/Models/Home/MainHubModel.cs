using System.Collections.Generic;


namespace Msg.Models.Home
{
    public class MainHubModel
    {
        //Список подключенных вкладок
        public List<string> ConnectionId { get; set; }

        //UserId пользователя
        public string UserId { get; set; }
        //Количество подключенных вкладок
        public int TabsCount { get; set; }

        public MainHubModel(string connectionId,string userId)
        {
            ConnectionId = new List<string>();
            ConnectionId.Add(connectionId);
            UserId = userId;
            TabsCount = 1;
        }

        //Добавление вкладки
        public void AddTab(string connectionId)
        {
            ConnectionId.Add(connectionId);
            TabsCount++;
        }
        //Удаление вкладки
        public void RemoveTab(string connectionId)
        {
            ConnectionId.Remove(connectionId);
            TabsCount--;
        }
    }
}