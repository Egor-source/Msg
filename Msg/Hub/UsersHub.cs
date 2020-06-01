using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Msg.Models;
using Msg.Models.Home;
using Newtonsoft.Json;

namespace Msg.Hubs
{
    public class UsersHub : Hub
    {
        //Список подключенных пользователей
       static List<MainHubModel> Users = new List<MainHubModel>();

       readonly static AppMsgDbContext db = new AppMsgDbContext();

        

        /// <summary>
        /// Вызывается при подключении нового пользователя к серверу
        /// </summary>
        /// <param name="userId">UserId нового пользователя</param>
        public void Connect(string userId)
        {
            //Проверяет находится ли новый пользователь в списке подключенных
            if (!Users.Any(u => u.UserId==userId))
            {
                //Добавляет нового пользователя в список подключенных
                Users.Add(new MainHubModel(Context.ConnectionId,userId));
            }
            else
            {
                //Добавляет connectionId новой вкладки
                Users.First(u => u.UserId==userId).AddTab(Context.ConnectionId);
            }
            Groups.Add(Context.ConnectionId, userId);
        }

        /// <summary>
        /// Вызывается при отключении пользователя от сервера
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            //Ищет отключившегося пользователя в списке подключенных
            var user = Users.FirstOrDefault(u => u.ConnectionId.Contains(Context.ConnectionId));

            if (user != null)
            {
                if(user.TabsCount==1)
                {
                    //Удаляет отключившегося пользователя из списка подключенных
                    Users.Remove(user);
                }
                else
                {
                    //Уменьшает количество открытых вкладок
                    Users.Where(u => u == user).First().RemoveTab(Context.ConnectionId);
                }

                Groups.Remove(Context.ConnectionId,user.UserId);

            }

            return base.OnDisconnected(stopCalled);
        }

        /// <summary>
        /// Вызывается при отправке запроса на дружбу
        /// </summary>
        /// <param name="requestRecipientUserId">UserId получателя запроса на дружбу</param>
        public void FriendshipRequest(string requestRecipientUserId)
        {
            //Получатель запроса на дружбу
            var requestRecipient = Users.FirstOrDefault(f => f.UserId == requestRecipientUserId);

            //UserId отправителя запроса на дружбу
            var sendingRequest = Users.FirstOrDefault(f => f.ConnectionId.Contains(Context.ConnectionId));

            //Добавление новой пары друзей в бд
            db.Friends.Add(new FriendModel { FriendOneId = requestRecipientUserId, FriendTwoId = sendingRequest.UserId, Status = 0, HostRequestId = requestRecipientUserId });
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return;
            }

            //Проверяет подключен ли к серверу получатель запроса на дружбу
            if (requestRecipient != null)
            {
                //Получает пользовательскую информацию отправителя запроса на дружбу сереализует ее в JSON строку
                var jsonSendingRequestUserInfo = JsonConvert.SerializeObject(db.Users.Where(u => u.Id == sendingRequest.UserId).Select(u => new { u.Id, u.Name, u.Surname, u.Photo }));

                Clients.Group(requestRecipient.UserId).friendshepRequest(jsonSendingRequestUserInfo);
            }
        }
       /// <summary>
       /// Подтверждает добавление в друзья
       /// </summary>
       /// <param name="Userid">UserId подтверждаемого пользователя</param>
        public void ConfirmFriendship(string Userid)
        {
            //Подтверждаемый пользователь
            var confirmed = Users.FirstOrDefault(r => r.UserId == Userid);

            //Подтверждающий пользователь
            var confirming = Users.FirstOrDefault(d => d.ConnectionId.Contains(Context.ConnectionId));

            //Ищет пару друзей в базе данных
            var friends = db.Friends.FirstOrDefault(r=>((r.FriendOneId==Userid||r.FriendTwoId==Userid)&&(r.FriendOneId==confirming.UserId||r.FriendTwoId==confirming.UserId)));

            //Удаляет информацию об отправители запроса
            friends.HostRequestId = null;

            //Изменяет статус дружбы
            friends.Status = 1;

            //Сохраняет изменения в бд
            db.Entry(friends).State = EntityState.Modified;
            db.SaveChanges();

            if(confirmed!=null)
            {
                //Получает пользовательскую информацию отправителя запроса на подтверждение дружбы сереализует ее в JSON строку
                var jsonConfirmingUserInfo = JsonConvert.SerializeObject(db.Users.Where(u => u.Id == confirming.UserId).Select(u => new { u.Id, u.Name, u.Surname, u.Photo }));

                Clients.Group(confirmed.UserId).confirmFriendship(jsonConfirmingUserInfo);
            }
        }

        /// <summary>
        /// Изменяет статус дружбы
        /// </summary>
        /// <param name="Userid">UserId удаляемого пользователя</param>
        public void RemoveFriend(string Userid)
        {
            //Удаляемый из списка друзей
            var removable = Users.FirstOrDefault(r => r.UserId == Userid);
            //Удаляющий пользователь
            var deleting = Users.FirstOrDefault(d => d.ConnectionId.Contains(Context.ConnectionId));

            //Ищет пару друзей в базе данных
            var friends = db.Friends.FirstOrDefault(r => ((r.FriendOneId == Userid || r.FriendTwoId == Userid) && (r.FriendOneId == deleting.UserId || r.FriendTwoId == deleting.UserId)));

            //Изменяет статус дружбы
            friends.Status =0;

            //Делает получателем запроса на дружбу пользователя,который вызвал метод
            friends.HostRequestId = deleting.UserId;

            //Сохраняет изменения в бд
            db.Entry(friends).State = EntityState.Modified;
            db.SaveChanges();

            //Получает пользовательскую информацию отправителя запроса сереализует ее в JSON строку
            var json = JsonConvert.SerializeObject(db.Users.Where(u => u.Id == deleting.UserId).Select(u => new { u.Id, u.Name, u.Surname, u.Photo }));

            if(removable!=null)
            {
                Clients.Group(removable.UserId).removeFriend(json);
            }
        }

        /// <summary>
        /// Отменяет подписку на пользователя
        /// </summary>
        /// <param name="Userid">UserId удаляемо из подписок пользователя</param>
        public void Unsubscribe(string Userid)
        {
            //Удаляемый из списка подписок пользователь
            var removable = Users.FirstOrDefault(r=>r.UserId==Userid);
            //Удаляющий пользователь
            var deleting = Users.FirstOrDefault(d => d.ConnectionId.Contains(Context.ConnectionId));
            
            //Удаляет пару друзей из бд
            db.Friends.Remove(db.Friends.FirstOrDefault(r=> ( (r.FriendOneId==Userid||r.FriendTwoId==Userid) && (r.FriendOneId== deleting.UserId||r.FriendTwoId==deleting.UserId))));
            db.SaveChanges();

            if (removable != null)
            {
                Clients.Group(removable.UserId).unsubscribe(deleting.UserId);
            }
        }

        /// <summary>
        /// Отправляет сообщение пользователю
        /// </summary>
        /// <param name="senderId">UserId отправителя</param>
        /// <param name="recipientId">UserId получателя</param>
        /// <param name="messageText">Текст сообщения</param>
        public void SendMessage(string senderId,string recipientId,string messageText)
        {
            if (!string.IsNullOrWhiteSpace(messageText) && !string.IsNullOrEmpty(messageText))
            {
                //Создает модель сообщения
                MessageModel message= new MessageModel { Id = db.Messages.Count().ToString(), senderId = senderId, recipientId = recipientId, sendingTime = DateTime.Now, text =messageText};

                //Добавляет сообщение в бд
                db.Messages.Add(message);
                db.SaveChanges();

                //Отправляет получателю информацию о сообщении
                if (Users.Any(x => x.UserId == recipientId))
                {
                    var jsonMessage = JsonConvert.SerializeObject(db.Users.Where(x => x.Id == senderId).Select(x => new { x.Photo, x.Name, x.Surname, Text = messageText, message.sendingTime, senderId }));

                    Clients.Group(recipientId).getMessage(jsonMessage);
                }
            }

        }
    }
}