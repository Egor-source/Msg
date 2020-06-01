using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Msg.Models;
using Msg.Filters;
using Microsoft.Owin.Security;
using System.IO;
using Newtonsoft.Json;
using Msg.Models.Home;
using System.Threading.Tasks;

namespace Msg.Controllers
{
    [Authenticated]
    public class HomeController : Controller
    {
       readonly AppMsgDbContext db = new AppMsgDbContext();
        private IAuthenticationManager AuthManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }
        public ActionResult Index()
        {
            HttpCookie cookie = Request.Cookies["User"];

            GetAllFamiliarUsers(cookie["Id"]);

            return View();
        }

        /// <summary>
        /// Метод для перезагрузки сайдбара
        /// </summary>
        /// <returns></returns>
        public ActionResult SidbarReboot()
        {
            HttpCookie cookies = Request.Cookies["User"];
            GetAllFamiliarUsers(cookies["Id"]);

            return PartialView("Sidebar");
        }


         /// <summary>
         /// Выход
         /// </summary>
         /// <returns></returns>
        public ActionResult Exit()
        {
            //Сбрасывает куки пользователя
            var cookie = new HttpCookie("User")
            {
                Expires = DateTime.Now.AddDays(-1d)
            };
            Response.Cookies.Add(cookie);
            //Сбрасывает куки авторизации
            AuthManager.SignOut();
            return RedirectToAction("Login","Account");
        }

        /// <summary>
        /// Поиск человека
        /// </summary>
        /// <param name="man">Искомый человек</param>
        /// <param name="id">id пользователя</param>
        /// <returns>JSON строку с возможными вариантами поиска</returns>
        public string GetPeople(string man,string id)
        {
            //Массив искомых строк
            string[] requered = man.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            //Если не было передано строк
            if (requered.Length<=0)
                return "";

            var first = requered[0];

            //Ищет совпадения по первому элементу массива requered
            var users = (from user in db.Users where user.Id != id && (user.Name.Contains(first) || user.Surname.Contains(first)) select new  { user.Id,user.Name, user.Surname,user.Photo,Status=-1,HostRequestId=""}).ToList();
            for(int i=1;i<requered.Length;i++)
            {
                //Текущая строка поиска
                var current = requered[i];
                //Ищет совпадения по текущей строке поиска
                users = (from user in users where user.Name.Contains(current) || user.Surname.Contains(current) select user).ToList();
                       
            }

            //Получает список всех друзей текущего пользователя
            var frends = db.Friends.Where(u => u.FriendOneId == id || u.FriendTwoId == id).Select(u => u.FriendOneId == id ? new { Id = u.FriendTwoId, u.Status, u.HostRequestId } :new { Id = u.FriendOneId, u.Status, u.HostRequestId });

            //Отбирает всех друзей пользователя из списка искомых пользователей
           var result = users.ToList().Join(frends,
                                        u => u.Id,
                                        f => f.Id,
                                       (userInfo, friendInfo) => new
                                       {
                                           userInfo.Id,
                                           userInfo.Name,
                                           userInfo.Surname,
                                           userInfo.Photo,
                                           friendInfo.HostRequestId,
                                           friendInfo.Status
                
                                       }).ToList();
        
            //Удаляет всех друзей из искомых пользователей
            users.RemoveAll(x=>result.Select(y=>y.Id).Contains(x.Id));

            //Сереализует искомых пользователей в JSON строку
            string userJson = JsonConvert.SerializeObject(users);

            //Сереализует искомых друзей в JSON строку
            string resultJson = JsonConvert.SerializeObject(result);

            //Если среди искомых пользователей не было друзей
            if (result.Count == 0)
            {
                
                return  userJson;
            }
            //Усли среди искомых пользователей были только друзья
            else if (users.Count == 0)
            {
                return  resultJson;
            }
            else
            {
                //Составляет одну JSON строку из искомых пользователей и друзей
                return userJson.Substring(0, userJson.Length - 1) + ',' + resultJson.Substring(1, resultJson.Length - 1);
            }

        }

       
        /// <summary>
        /// Метод для поиска всех друзей текущего пользователя
        /// </summary>
        /// <param name="UserId">UserId текущего пользователя</param>
        private  void GetAllFamiliarUsers(string UserId)
        {
            List<AppUser> friendsList = new List<AppUser>();
            List<AppUser> subscribersList = new List<AppUser>();
            List<AppUser> subscriptionsList = new List<AppUser>();

                //Ищет в бд всех друзей пользователя
            var users = db.Friends.Where(x => x.FriendOneId == UserId || x.FriendTwoId == UserId).ToList();

              foreach (var user in users)
              {
                    //UserId друга
                var fId = user.FriendOneId == UserId ? user.FriendTwoId : user.FriendOneId;
                //Информация о друге
                var userInfo = db.Users.FirstOrDefault(x => x.Id == fId);
                    //Проверяет статус дружбы
                switch (user.Status)
                {
                        //Если друзья не подтверждены
                    case 0:
                            //Если текущему пользователю отправили заявку в друзя
                        if (UserId == user.HostRequestId)
                        {
                            subscribersList.Add(userInfo);

                        }
                        //Если текущий пользователь отправил заявку в друзья
                        else
                        {
                            subscriptionsList.Add(userInfo);
                        }
                        break;
                            //Если дружба подтверждена
                    case 1:
                        friendsList.Add(userInfo);
                        break;

                }
             
              }

              //Добавляет списки полученных пользователей в соответствующий ViewBag
            ViewBag.Friends =friendsList.Count==0?null:friendsList.Select(x=>new UserInfoModel{Id= x.Id, Name=x.Name,Surname=x.Surname,Photo=x.Photo}).ToList();

            ViewBag.Subscribers = subscribersList.Count==0?null:subscribersList.Select(x => new UserInfoModel { Id=x.Id,Name=x.Name,Surname=x.Surname, Photo=x.Photo }).ToList();

            ViewBag.Subscriptions = subscriptionsList.Count==0 ?null: subscriptionsList.Select(x => new UserInfoModel{Id=x.Id,Name=x.Name,Surname=x.Surname,Photo=x.Photo }).ToList();

        }


        public ActionResult ShowDialog(string userId,string interlocutorId)
        {
            List<MessageInfoModel> dialog = new List<MessageInfoModel>();

            ViewBag.InterlocutorId = interlocutorId;

            var messages = db.Messages.Where(m => (m.senderId == userId && m.recipientId == interlocutorId) || (m.senderId == interlocutorId && m.recipientId == userId)).OrderBy(x=>x.sendingTime).ToList();

            

            if (messages.Count != 0)
            {
                var user = db.Users.Find(userId);

                var interlocutor = db.Users.Find(interlocutorId);

                foreach(var i in messages)
                {
                    dialog.Add(i.senderId==userId?new MessageInfoModel{Photo=user.Photo,Name=user.Name,Surname=user.Surname,SenderId=user.Id,SendingTime=i.sendingTime.ToString("H:mm"),Text=i.text}: new MessageInfoModel { Photo = interlocutor.Photo, Name = interlocutor.Name, Surname = interlocutor.Surname, SenderId = interlocutor.Id, SendingTime = i.sendingTime.ToString("H:mm"), Text = i.text });
                }

                ViewBag.Dialog = dialog;
            }


            return PartialView("Dialog");
        }
    }
}