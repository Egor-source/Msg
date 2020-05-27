using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Msg.Models;
using Msg.Models.Home;
using Newtonsoft.Json;

namespace Msg.Hubs
{
    public class FriendHub : Hub
    {
        //Список подключенных пользователей
        static List<FriendHubModel> Users = new List<FriendHubModel>();

        static AppMsgDbContext db = new AppMsgDbContext();

        /// <summary>
        /// Вызывается при отправке запроса на дружбу
        /// </summary>
        /// <param name="requestRecipientUserId">UserId получателя запроса на дружбу</param>
        public void FriendshipRequest(string requestRecipientUserId)
        {
            //Получатель запроса на дружбу
            var requestRecipient = Users.First(f => f.UserId == requestRecipientUserId);

            //UserId отправителя запроса на дружбу
            var sendingRequest = Users.First(f => f.ConnectionId == Context.ConnectionId);

            //Добавление новой пары друзей в бд
            db.Friends.Add(new Friend { FriendOneId = requestRecipientUserId, FriendTwoId = sendingRequest.UserId, Status = 0, RequestSenderId = sendingRequest.UserId });
            db.SaveChanges();

            //Проверяет подключен ли к серверу получатель запроса на дружбу
            if (requestRecipient != null)
            {
                //Получает пользовательскую информацию отправителя запроса на дружбу сереализует ее в JSON строку
                var jsonSendingRequestUserInfo = JsonConvert.SerializeObject(db.Users.Where(u => u.Id == sendingRequest.UserId).Select(u => new { u.Id, u.Name, u.Surname, u.Photo }));

                Clients.Client(requestRecipient.ConnectionId).friendshepRequest(jsonSendingRequestUserInfo);
            }


        }

        /// <summary>
        /// Вызывается при подключении нового пользователя к серверу
        /// </summary>
        /// <param name="userId">UserId нового пользователя</param>
        public void Connect(string userId)
        {
            //Проверяет находится ли новый пользователь в списке подключенных
            if (!Users.Any(u => u.ConnectionId == Context.ConnectionId))
            {
                //Добавляет нового пользователя в список подключенных
                Users.Add(new FriendHubModel { ConnectionId = Context.ConnectionId, UserId = userId });
            }
        }

        /// <summary>
        /// Вызывается при отключении пользователя от сервера
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            //Ищет отключившегося пользователя в списке подключенных
            var user = Users.First(u => u.ConnectionId == Context.ConnectionId);

            if (user != null)
            {
                //Удаляет отключившегося пользователя из списка подключенных
                Users.Remove(user);
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}