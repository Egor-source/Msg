using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Msg.Models;
using Msg.Filters;
using Microsoft.Owin.Security;

namespace Msg.Controllers
{
    [Authenticated]
    public class HomeController : Controller
    {

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
    }
}