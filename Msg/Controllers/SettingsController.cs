using Microsoft.AspNet.Identity.Owin;
using Msg.App_Start.Identity;
using Msg.Filters;
using Msg.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using System.Web.ModelBinding;
using Msg.Models.Settings;
using Msg.App_Start;

namespace Msg.Controllers
{
    [Authenticated]
    public class SettingsController : Controller
    {

        AppMsgDbContext db = new AppMsgDbContext();

        private AppUserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }


        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Смена фотографии
        /// </summary>
        /// <param name="photo">Фотография</param>
        /// <param name="id">id пользователя</param>
        /// <returns>Представление настроек</returns>
        public async Task<ActionResult> ChangePhoto(ChangePhotoModel model)
        {

            //Проверяет выбрал ли пользователь фото
            if (model.photo == null)
            {
                return RedirectToAction("Index");
            }

            AppUser user = new AppUser();

                //Ищет пользователя в базе данных
                await Task.Run(() =>
                {
                    user = db.Users.Find(model.id);
                });

                FileInfo dw = new FileInfo(Server.MapPath(AppUser.PhotoDir + user.Photo));

                //Проверяет не стоит ли у пользователя стандартное фото
                if (user.Photo != "Default.bmp")
                {
                    //Удаляет старое фото пользователя
                    dw.Delete();
                }
                else
                {
                  
                    //Изменяет название фото на новое
                    user.Photo = model.id.GetHashCode() + ".jpg";

                    //Изменяет название фото в базе данных
                    db.Entry(user).State = EntityState.Modified;
                    //Сохраняет изменения
                    db.SaveChanges();

                    //Заменяет куки пользователя на куки с новым названием фото 
                    HttpCookie cookie = new HttpCookie("User");

                    cookie["id"] = user.Id;
                    cookie["Name"] = user.Name;
                    cookie["Surname"] = user.Surname;
                    cookie["Photo"] = user.Photo;
                    cookie.Expires = DateTime.Now.AddYears(10);
                    Response.Cookies.Add(cookie);
                }

                //Получает название фото
                string photoName = model.photo.FileName;

                //Сохраняет фото на сервере
                model.photo.SaveAs(Server.MapPath(AppUser.PhotoDir + photoName));

                //Переименовывает фото
                dw = new FileInfo(Server.MapPath(AppUser.PhotoDir + photoName));
                dw.MoveTo(Server.MapPath(AppUser.PhotoDir + model.id.GetHashCode() + ".jpg"));


                return RedirectToAction("Index");
            
            
        }

        public async Task<ActionResult> ChPassword(ChPasswordModel model)
        {

            if(ModelState.IsValid)
            {
                UserManager.UserValidator = new UserValidator<AppUser>(UserManager);
                IdentityResult result = await UserManager.ChangePasswordAsync(model.id, model.OldPassword, model.NewPassword);
                UserManager.UserValidator = new CustomUserValidator(UserManager);

                if (result.Succeeded)
                {
                    ModelState.AddModelError("", "Пароль был успешно изменен");
                }
                else
                {
                    foreach(var i in result.Errors)
                    {
                        if(i == "Incorrect password.")
                        {
                            ModelState.AddModelError("", "Неверный пароль");
                        }
                        else
                        ModelState.AddModelError("", i);
                    }
                }
            }        

            return PartialView(model);
        }
    }
}