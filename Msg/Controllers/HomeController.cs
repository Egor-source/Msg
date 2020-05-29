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
            var users = from user in db.Users where user.Id != id && (user.Name.Contains(first) || user.Surname.Contains(first)) select new UserInfo {Id= user.Id,Name= user.Name,Surname= user.Surname,Photo= user.Photo,FriendStatus=-1};
            for(int i=1;i<requered.Length;i++)
            {
                //Текущая строка поиска
                var current = requered[i];
                //Ищет совпадения по текущей строке поиска
                users = from user in users where user.Name.Contains(current) || user.Surname.Contains(current) select user;
                       
            }

            //var friends = db.Friends.Where(u => u.FriendOneId == id || u.FriendTwoId == id);

            //foreach(var user in users)
            //{
            //    var friend = friends.Where(f => f.FriendOneId == user.Id || f.FriendTwoId == user.Id).FirstOrDefault();

            //    if(friend!=null)
            //    {
            //        if(friend.Status==2&&friend.RequestSenderId==id)
            //        {
            //            user.FriendStatus = 3;
            //        }
            //        else
            //        {
            //            user.FriendStatus = friend.Status;
            //        }
            //    }

            //}

            //Конвертирует пользователей,удовлетворяющих запросу,в JSON строку
            string json = JsonConvert.SerializeObject(users);

            return json;
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

            ViewBag.Friends =friendsList.Count==0?null:friendsList.Select(x=>new UserModel{Id= x.Id, Name=x.Name,Surname=x.Surname,Photo=x.Photo}).ToList();

            ViewBag.Subscribers = subscribersList.Count==0?null:subscribersList.Select(x => new UserModel { Id=x.Id,Name=x.Name,Surname=x.Surname, Photo=x.Photo }).ToList();

            ViewBag.Subscriptions = subscriptionsList.Count==0 ?null: subscriptionsList.Select(x => new UserModel{Id=x.Id,Name=x.Name,Surname=x.Surname,Photo=x.Photo }).ToList();

        }
    }
}