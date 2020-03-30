using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using Msg.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net.Mail;
using Msg.App_Start.Identity;
using System.Threading;
using Msg.App_Start;

namespace Msg.Controllers
{
    /// <summary>
    /// Контроллер для управления аккаунтами поьзователей
    /// </summary>
    public class AccountController : Controller
    {

        private AppUserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }
        }

        /// <summary>
        /// Get метод регистрации
        /// </summary>
        /// <returns>Представление для регистрации</returns>
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Post метод регистрации
        /// </summary>
        /// <param name="model">Зарегестрировавшийся пользователь</param>
        /// <returns>Представлени DisplayEmail если регистрация прошла успешно или представление регистрации со встретившимися ошибками,если регистрация не прошла успешно </returns>
        [HttpPost]
        public async Task <ActionResult> Register(RegistrModel model)
        {
            //Проверяет правильность заполнения формы
            if (ModelState.IsValid)
            {
                //Создает модель пользователя
                AppUser user = new AppUser { Email=model.Email,UserName=model.Login, Name=model.Name,Surname = model.Surname, DateOfRegistration = DateTime.Today.Date,Gender=model.Gender};
                user.Photo = model.File == null ? "Default.png" :user.Id;
                //Добавляет пользователя в бд
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);
              
                //Проверяет был ли пользователь добавлен в бд
                if (result.Succeeded)
                {
                    //Создает токен подтверждения Email
                   var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // Создаем ссылку для подтверждения
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code },
                               protocol: Request.Url.Scheme);
                    try
                    {
                        // Отправка письма
                        await UserManager.SendEmailAsync(user.Id, "Подтверждение электронной почты",
                           "Для завершения регистрации перейдите по ссылке: <a href=\""
                                                           + callbackUrl + "\">завершить регистрацию</a>");
                    }
                    //Возникает,если указанного Email не существует
                    catch (SmtpException e)
                    {

                        ModelState.AddModelError("",e.Message);
                        await UserManager.DeleteAsync(user);
                        return View(model);
                    }
                    //Проверяет загрузил ли пользователь фото
                    if (model.File != null)
                    {
                        //Добаляет фото пользователя в проект
                        LoadPhoto(model.File, user.Id);
                    }

                    //Удаляет пользователя,если он не подтвердил свою учетную записть в течении 10 минут
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        DbCleaner.DeleteNoConfirmedUser(user.Id);
                    });


                    return View("DisplayEmail");
                }
                else
                {
                    foreach (string error in result.Errors)
                    {
                       ModelState.AddModelError("", error);
                    }
                }
            }
            return View(model);
        }
        /// <summary>
        /// Сохраняет фото пользователя
        /// </summary>
        /// <param name="upload">Фото</param>
        /// <param name="id">id пользователя</param>
        private void LoadPhoto(HttpPostedFileBase upload,string id)
        {

            // Получаем имя файла
            string fileName = upload.FileName;
            // Сохраняем файл в проект
            upload.SaveAs(Server.MapPath(AppUser.PhotoDir + fileName));
            //Меняет имя файла на id пользователя.Нужно из-за возможности добаления нескольких файлов с одинаковыми именами
            FileInfo dw = new FileInfo(Server.MapPath(AppUser.PhotoDir + fileName));
            dw.MoveTo(Server.MapPath(AppUser.PhotoDir + id+".png"));

        }

        /// <summary>
        /// Get метод для подтверждения аккаунта 
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <param name="code">Токен подтверждения</param>
        /// <returns>Представление подтверждения аккаунт</returns>
        public async Task<ActionResult> ConfirmEmail(string userId=null,string code=null)
        {
            
            if (userId == null || code == null)
            {
                return View("Login");
            }

            IdentityResult result;
           
            try
            {
                UserManager.UserValidator = new UserValidator<AppUser>(UserManager);
                //Подтверждает аккаунт пользователя
               result = await UserManager.ConfirmEmailAsync(userId, code);
                UserManager.UserValidator = new CustomUserValidator(UserManager);
            }
            //Возникает если пользователь не был найден в бд
            catch (InvalidOperationException e)
            {
                ViewBag.Messege="Время на подтверждение аккаунта истекло. Повторите регистрацию";
                return View();
            }

            if (result.Succeeded)
            {
                ViewBag.Messege="Почта подтверждена успешно";
                return View();
            }

            return View("Login");
        }

        public ActionResult Login()
        {
            return View();
        }
    }
}