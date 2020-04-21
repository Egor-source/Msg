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
using Microsoft.Owin.Security;
using System.Security.Claims;

namespace Msg.Controllers
{
    /// <summary>
    /// Контроллер для управления аккаунтами поьзователей
    /// </summary>
    /// 
    [AllowAnonymous]
    public class AccountController : Controller
    {

        private AppUserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }
        }

        private IAuthenticationManager AuthManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
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
        [ValidateAntiForgeryToken]
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

                    ViewBag.Message ="На указанный электронный адрес отправлены дальнейшие инструкции по завершению регистрации";
                    return View("Confirm");
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
                ViewBag.Message="Время на подтверждение аккаунта истекло. Повторите регистрацию";
                return View("Confirm");
            }

            if (result.Succeeded)
            {      
                ViewBag.Message="Почта подтверждена успешно";
                return View("Confirm");
            }

            return View("Login");
        }
        /// <summary>
        /// Get метод для аутентификации
        /// </summary>
        /// <returns>Представление аутентификации</returns>
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Post метод аутентификации
        /// </summary>
        /// <param name="model">Данные с формы</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                //Получает пользователя из бд
                AppUser user = await UserManager.FindAsync(model.Login, model.Password);
           

                if (user != null && user.EmailConfirmed)
                {
                    ClaimsIdentity claims = await UserManager.CreateIdentityAsync(user,
                                    DefaultAuthenticationTypes.ApplicationCookie);
                    //Очищает куки аутентификации
                    AuthManager.SignOut();
                    //Создает куки аутентификации
                    AuthManager.SignIn(new AuthenticationProperties
                    {
                        //Сохранять ли куки при закрытии сайта
                        IsPersistent = true

                    }, claims); ;
                    return RedirectToAction("index", "Home");
                }
                else if(user==null)
                {
                    ModelState.AddModelError("", "Неверно указан логин или пароль");
                }
                else
                {
                    ModelState.AddModelError("", "Пользователь не подтвержден");
                }
            }

            return View(model);
        }
        /// <summary>
        /// Get метод для сброса пароля
        /// </summary>
        /// <returns>Представление сброса пароля</returns>
        [HttpGet]
        public ActionResult PasswardReset()
        {
            return View();
        }
        /// <summary>
        /// Post метод для сроса пароля
        /// </summary>
        /// <param name="model">Данные с формы</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PasswardReset(PasswardResetModel model)
        {

            if (ModelState.IsValid)
            {
                //Получает пользователя из бд
                var user = await UserManager.FindByNameAsync(model.Login);

                if(user==null)
                {
                    ModelState.AddModelError("", $"Пользователь {model.Login} не найден");
                }
                //Создает токен смены пароля
                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                //Создает ссылку для сброса пароля
                var callbackUrl = Url.Action("ChangePassward", "Account", new { userId = user.Id, code = code },
                              protocol: Request.Url.Scheme);
                //Отправляет письмо для сброса пароля
                await UserManager.SendEmailAsync(user.Id, "Сброс пароля",
                          "Для сброса пароля перейдите по ссылке: <a href=\""
                                                          + callbackUrl + "\">сбросить пароль</a>");
                ViewBag.Message = "На вашу почту отправленно письмо для сброса пароля";
                return View("Confirm");
            }

            return View(model);
        }
        /// <summary>
        /// Get метод для смены пароля
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <param name="code">Токен</param>
        /// <returns>Представление смены пароля</returns>
        [HttpGet]
        public ActionResult ChangePassward(string userId = null, string code = null)
        {
            if (userId == null || code == null)
                return View("Login");
        

            return View(new ChangePasswardModel { UserId=userId,Code=code});
        }

        /// <summary>
        /// Post метод для смены пароля
        /// </summary>
        /// <param name="model">Данные с формы</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassward(ChangePasswardModel model)
        {
            if (ModelState.IsValid) {
                //Изменяет валидатор пользователя на стандартный
                UserManager.UserValidator = new UserValidator<AppUser>(UserManager);
                //Меняет пароль пользователя
                IdentityResult result = await UserManager.ResetPasswordAsync(model.UserId, model.Code, model.Password);
                //Изменяет валидатор пользователя на кастомный
                UserManager.UserValidator = new CustomUserValidator(UserManager);
                if (result.Succeeded)
                {
                    ViewBag.Message = "Пароль успешно изменен";
                    return View("Confirm"); 
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            return View(model);
        }
    }
}