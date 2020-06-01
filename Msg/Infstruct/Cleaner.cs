using Msg.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Msg.App_Start
{
    /// <summary>
    /// Класс для очистки базы данных
    /// </summary>
    public static class Cleaner
    {
      static AppMsgDbContext db = new AppMsgDbContext();

        /// <summary>
        /// Удаляет не подтвержденного пользователя через 10 минут после вызова метода
        /// </summary>
        /// <param name="id">Id пользователя</param>
        public static async void DeleteNoConfirmedUser(string id)
        {
            //Останавливает метод на 10 минут
            await Task.Delay(TimeSpan.FromMinutes(10));

            
            AppUser user=new AppUser();

            //Ищет пользователя в базе данных
            await Task.Run(() =>
            {
                 user = db.Users.Find(id);
            });
                    //Проверяет подтвержден ли пользователь
                if (!user.EmailConfirmed)
                {                  
                    db.Users.Remove(user);
                    db.SaveChanges();
                }                      
        }


        /// <summary>
        /// Удаляет всех не подтвержденных пользователей
        /// </summary>
        public static async void DeleteNoConfirmedUser()
        {

            await Task.Run(() =>
            {
                //Отберает всех пользователей с неподтвержденным аккаунтом
                var users = db.Users.ToArray().Where((m)=>m.EmailConfirmed==false);

                //Удаляет пользователей с неподтвержденным аккаунтом
                foreach (var i in users)
                {
                        db.Users.Remove(i);                   
                }
                db.SaveChanges();
            });

        }
    }
}