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

        public string GetPeople(string man,string id)
        {
            string[] requered = man.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            string json=null;

            foreach (var i in requered)
            {
                var users = from user in db.Users where user.Id != id&& (user.Name.Contains(i) || user.Surname.Contains(i)) select new {user.Name,user.Surname,user.Photo};
               json+=JsonConvert.SerializeObject(users);
            }

            return json;
        }
    }
}