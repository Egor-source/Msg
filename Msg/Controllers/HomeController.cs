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

        
        public ActionResult Exit()
        {
            AuthManager.SignOut();
            return RedirectToAction("Login","Account");
        }
    }
}