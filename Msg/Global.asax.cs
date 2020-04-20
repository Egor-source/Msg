using Microsoft.Owin.Security;
using Msg.App_Start;
using Msg.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;


namespace Msg
{
    public class MvcApplication : System.Web.HttpApplication
    {
      
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //”дал€ет всех не подтвержденных пользователей
            DbCleaner.DeleteNoConfirmedUser();
            //AppMsgDbContext db = new AppMsgDbContext();

            //for (int i = 0; i < 10000; i++)
            //{
            //    db.Users.Add(new AppUser { UserName = i.ToString(), Name = "s", Surname = "r", Email = "dwdw", DateOfRegistration = DateTime.Today.Date, Photo = "gee" });
            //    break;
            //}
            //db.SaveChanges();
        }

    }
}
