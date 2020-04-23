using Microsoft.AspNet.Identity;
using Msg.App_Start.Identity;
using Msg.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace Msg.App_Start
{
    /// <summary>
    /// Класс для отправки сообщений по почте
    /// </summary>
    public class EmailService : IIdentityMessageService
    {
        /// <summary>
        /// Асинхроно отправляет сообщени на почту
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task SendAsync(IdentityMessage message)
        {
            var from = "msg.sdw.15@mail.ru";
            var pass = "SoiYPLyft12_";

            // адрес и порт smtp-сервера, с которого мы и будем отправлять письмо
            SmtpClient client = new SmtpClient("smtp.mail.ru",587);

            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(from, pass);
            client.EnableSsl = true;

            // создает письмо: message.Destination - адрес получателя
            var mail = new MailMessage(from, message.Destination);
            mail.Subject = message.Subject;
            mail.Body = message.Body;
            mail.IsBodyHtml = true;

            return client.SendMailAsync(mail);
        }
    }

    /// <summary>
    /// Класс для валидации пароля
    /// </summary>
    public class CustomPasswardValidator : IIdentityValidator<string>
    {
        //Минимальная длинна пароля
        public int MinLength { get; set; } = 1;

        /// <summary>
        /// Проверяет валидность пароля
        /// </summary>
        /// <param name="item">Пароль пользователя</param>
        /// <returns>Результат проверки валидности пароля</returns>
        public async Task<IdentityResult> ValidateAsync(string item)
        {
            List<string> errors = new List<string>();

            //Проверяет соответствует-ли длинна пароля минимально заданной
            if (item.Length < MinLength)
            {
                errors.Add($"Пароль должен состоять не менее чем из {MinLength} символов");
            }

            if (errors.Count > 0)
                return IdentityResult.Failed(errors.ToArray());

            return IdentityResult.Success;
        }

      
    }
    /// <summary>
    /// Класс для валидации пользователя
    /// </summary>
    public class CustomUserValidator : UserValidator<AppUser>
    {
        public CustomUserValidator(AppUserManager manager)
        : base(manager)
        { }

        /// <summary>
        /// Проверяет валидность пользователя
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <returns>Результат проверки валидности пользователя</returns>
        public override async Task<IdentityResult> ValidateAsync(AppUser user)
        {
            //Возвращает результат базовой проверки валидности пользователя
            IdentityResult result = await base.ValidateAsync(user);
            AppMsgDbContext db = new AppMsgDbContext();
            var errors = result.Errors.ToList();


            //Проверяет занят ли указанный Email
            var us = new AppMsgDbContext().Users.Where((m)=>m.Email==user.Email);
            
            if (us.FirstOrDefault()!=null)
            {
                errors.Add($"Пользователь с Email {user.Email} уже зарегестрирован");
            }

            //Проверяет занят ли указанный Логин
            if (errors.Contains($"Name {user.UserName} is already taken."))
            {
                errors.Remove($"Name {user.UserName} is already taken.");
                errors.Add($"Логин {user.UserName} уже занят");
            }

            //Проверяет были-ли ошибки валидации
           if(errors.Count>0)
            {
                return IdentityResult.Failed(errors.ToArray());
            }

            return result;
        }
    }
}