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

namespace Msg.Controllers
{
    [Authenticated]
    public class HomeController : Controller
    {
        AppMsgDbContext db = new AppMsgDbContext();
        private IAuthenticationManager AuthManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }
        public ActionResult Index()
        {
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
            var users = from user in db.Users where user.Id != id && (user.Name.Contains(first) || user.Surname.Contains(first)) select new { user.Name, user.Surname, user.Photo };
            for(int i=1;i<requered.Length;i++)
            {
                //Текущая строка поиска
                var current = requered[i];
                //Ищет совпадения по текущей строке поиска
                users =from user in users where user.Name.Contains(current) || user.Surname.Contains(current) select user;
            }
            //Конвертирует пользователей,удовлетворяющих запросу,в JSON строку
            string json = JsonConvert.SerializeObject(users);

            return json;
        }
    }
}