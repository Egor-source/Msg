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
using System.Net.Mail;
using Microsoft.AspNet.Identity.EntityFramework;
using Msg.Infstruct;
using System.Threading;

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
        /// <summary>
        /// Метод для смены пароля
        /// </summary>
        /// <param name="model">Модель смены пароля</param>
        /// <returns></returns>
        public async Task<ActionResult> ChPassword(ChPasswordModel model)
        {

            if(ModelState.IsValid)
            {
                UserManager.UserValidator = new UserValidator<AppUser>(UserManager);
                //Меняет пароль пользователя в базе данных
                IdentityResult result = await UserManager.ChangePasswordAsync(model.id, model.OldPassword, model.NewPassword);
                UserManager.UserValidator = new CustomUserValidator(UserManager);

                //Проверяет был ли пароль изменен
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
        /// <summary>
        /// Метод для смени ФИО пользователя
        /// </summary>
        /// <param name="model">Модель смены ФИО</param>
        /// <returns></returns>
        public async Task<ActionResult> ChangeFIO(ChangeFioModel model)
        {
            AppUser user = new AppUser();
            //Ищет пользователя в базе данных
            await Task.Run(() =>
            {
                user = db.Users.Find(model.id);
            });

            //Если пользователь не указывал новое имя и фамилию
            if (model.Name == null && model.Surname == null)
            {
                return RedirectToAction("Index");
            }
            //Если пользователь указал только имя
            else if (model.Surname == null)
            {
                user.Name = model.Name;
            }
            //Если пользователь указал только фамилию
            else if (model.Name == null)
            {
                user.Surname = model.Surname;
            }
            //Если пользователь указал имя и фамилию
            else
            {
                user.Name = model.Name;
                user.Surname = model.Surname;
            }
            //Сохраняет изменения в базу данных
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            //Заменяет старые куки пользователя
            HttpCookie cookie = new HttpCookie("User");

            cookie["id"] = user.Id;
            cookie["Name"] = user.Name;
            cookie["Surname"] = user.Surname;
            cookie["Photo"] = user.Photo;
            cookie.Expires = DateTime.Now.AddYears(10);
            Response.Cookies.Add(cookie);


            return RedirectToAction("Index");
        }
        /// <summary>
        /// Метод для смены Email
        /// </summary>
        /// <param name="model">Модель смены Email</param>
        /// <returns></returns>
        public async Task<ActionResult> ChangeEmail(ChangeEmailModel model)
        {
            if (ModelState.IsValid)
            {

                AppUser user = await UserManager.FindByEmailAsync(model.NewEmail);
                //Проверяет зарегестрирован ли пользователь с указанным Email
                if(user!=null)
                {
                    ModelState.AddModelError("", "Пользователь с указанным Email уже зарегестрирован");

                    return PartialView(model);
                }

                 user = await UserManager.FindByIdAsync(model.id);

                bool result = await UserManager.CheckPasswordAsync(user, model.Password);



                if (result)
                {
                    string oldEmail = user.Email;
                    //Меняет Email пользователя на новый
                    await UserManager.SetEmailAsync(user.Id, model.NewEmail);

                    //Запускает метод,который по истечению времени изменить Email пользователя на старый,если пользователь не подтвердит новый Email
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        SetOldEmail.Set(user.Id, oldEmail, 10);

                    });

                    //Создает токен подтверждения Email
                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // Создаем ссылку для подтверждения
                    var callbackUrl = Url.Action("ConfirmEmail", "Settings", new { userId = user.Id, code = code},
                               protocol: Request.Url.Scheme);
                   

                    try
                    {
                        // Отправка письма
                        await UserManager.SendEmailAsync(user.Id, "Подтверждение электронной почты",
                           "Для завершения смены Email перейдите по ссылке: <a href=\""
                                                           + callbackUrl + "\">Смена Email</a>");
                    }
                    //Возникает,если указанного Email не существует
                    catch (SmtpException e)
                    {

                        ModelState.AddModelError("", e.Message);

                        return PartialView(model);
                    }


                    ModelState.AddModelError("", "На указанный электронный адрес отправлены дальнейшие инструкции по завершению смены Email");
                }
                else
                {
                    ModelState.AddModelError("", "Неверный пароль");
                }

            }

            return PartialView(model);
        }
        /// <summary>
        /// Метод для подтверждения нового Email
        /// </summary>
        /// <param name="userId">id пользователя</param>
        /// <param name="code">Токен подтверждения Email</param>
        /// <returns></returns>
        public async Task<ActionResult> ConfirmEmail(string userId = null, string code = null)
        {
            //Проверяет перешел ли пользователь по ссылке в письме
            if (userId == null || code == null )
            {
                return RedirectToAction("Index", "Home");
            }


            UserManager.UserValidator = new UserValidator<AppUser>(UserManager);
            //подтверждает новый Email пользователя
            IdentityResult result = await UserManager.ConfirmEmailAsync(userId,code);
            UserManager.UserValidator = new CustomUserValidator(UserManager);
            //Проверяет был ли Email подтвержден удачно
            if (result.Succeeded)
            {              

                ViewBag.Message = "Почта была успешно изменена";

            }
            else
            {
                ViewBag.Message = "Время на смену Email истекло";

            }


            return View("Message");
        }
    }
}
