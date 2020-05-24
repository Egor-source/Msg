using Msg.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Msg.Infstruct
{
 public static class SetOldEmail
    {
       static AppMsgDbContext db = new AppMsgDbContext();

        public static async void Set(string id,string oldEmail,int delay)
        {
            //Останавливает метод на указанное количество минут
            await Task.Delay(TimeSpan.FromMinutes(delay));


            AppUser user = new AppUser();

            //Ищет пользователя в базе данных
            await Task.Run(() =>
            {
                user = db.Users.Find(id);
            });
            //Проверяет подтвержден ли пользователь
            if (!user.EmailConfirmed)
            {
                //Изменяет Email пользователя на старый,если новый не был подтвержден 
                user.Email = oldEmail;
                user.EmailConfirmed = true;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}