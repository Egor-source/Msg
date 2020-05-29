﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
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
            db.Friends.Add(new Friend { FriendOneId = requestRecipientUserId, FriendTwoId = sendingRequest.UserId, Status = 0, HostRequestId = requestRecipientUserId });
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

            var friends = db.Friends.FirstOrDefault(r=>((r.FriendOneId==Userid||r.FriendTwoId==Userid)&&(r.FriendOneId==confirming.UserId||r.FriendTwoId==confirming.UserId)));

            friends.HostRequestId = null;

            friends.Status = 1;

            db.Entry(friends).State = EntityState.Modified;
            db.SaveChanges();

            if(confirmed!=null)
            {
                var jsonConfirmingUserInfo = JsonConvert.SerializeObject(db.Users.Where(u => u.Id == confirming.UserId).Select(u => new { u.Id, u.Name, u.Surname, u.Photo }));

                Clients.Group(confirmed.UserId).confirmFriendship(jsonConfirmingUserInfo);
            }
        }

        public void RemoveFriend(string Userid)
        {
            //Удаляемый из списка друзей
            var removable = Users.FirstOrDefault(r => r.UserId == Userid);
            //Удаляющий пользователь
            var deleting = Users.FirstOrDefault(d => d.ConnectionId.Contains(Context.ConnectionId));

            var friends = db.Friends.FirstOrDefault(r => ((r.FriendOneId == Userid || r.FriendTwoId == Userid) && (r.FriendOneId == deleting.UserId || r.FriendTwoId == deleting.UserId)));

            friends.Status =0;

            friends.HostRequestId = deleting.UserId;

            db.Entry(friends).State = EntityState.Modified;
            db.SaveChanges();

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
    }
}