using Microsoft.AspNet.Identity.EntityFramework;
using Msg.Models.Home;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Msg.Models
{
    /// <summary>
    /// Контекст данных пользователей
    /// </summary>
    public class AppMsgDbContext: IdentityDbContext<AppUser>
    {
        public DbSet<FriendModel> Friends { get; set; }

        public DbSet<MessageModel> Messages { get; set; }

        public AppMsgDbContext() : base("DefaultConnection") { }

        static AppMsgDbContext()
        {
            Database.SetInitializer<AppMsgDbContext>(new IdentityDbInit());
        }

        public static AppMsgDbContext Create()
        {
            return new AppMsgDbContext();
        }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));
        //}

    }

    public class IdentityDbInit : DropCreateDatabaseIfModelChanges<AppMsgDbContext>
    {
        protected override void Seed(AppMsgDbContext context)
        {
            PerformInitialSetup(context);
            base.Seed(context);
        }
        public void PerformInitialSetup(AppMsgDbContext context)
        {
            // настройки конфигурации контекста будут указываться здесь
        }
    }
}