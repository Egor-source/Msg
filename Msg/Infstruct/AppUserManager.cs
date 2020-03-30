using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;
using Msg.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Msg.App_Start.Identity
{
    public class AppUserManager: UserManager<AppUser>
    {
        public AppUserManager(IUserStore<AppUser> store) : base(store) { }

        public static AppUserManager Create(IdentityFactoryOptions<AppUserManager> options, IOwinContext context)
        {
            AppMsgDbContext db = context.Get<AppMsgDbContext>();
            AppUserManager manager = new AppUserManager(new UserStore<AppUser>(db));

            manager.PasswordValidator = new CustomPasswardValidator
            {
                MinLength = 6,               
            };

            manager.UserValidator = new CustomUserValidator(manager);

            manager.EmailService = new EmailService();

            var provider = new DpapiDataProtectionProvider("Msg");

            manager.UserTokenProvider = new DataProtectorTokenProvider<AppUser>(
                provider.Create("EmailConfirmation"));

            return manager;
        }
    }
}