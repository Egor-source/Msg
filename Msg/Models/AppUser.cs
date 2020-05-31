using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Msg.Models
{
    /// <summary>
    /// Модель пользователя
    /// </summary>
    public class AppUser: IdentityUser
    {
       //Дириктория фотографий пользователей
        public static string PhotoDir = "~/Content/Photo/";
        public static string DefaultPhoto = "Default.bmp";

        [Required]
        //Имя пользователя
        public string Name { get; set; }

        [Required]
        //Название его фотографии
        public string Photo { get; set; }

        [Required]
        //Фамилия пользователя
        public string Surname { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        //Дата регистрации
        public DateTime DateOfRegistration { get; set; }
        //[DataType(DataType.DateTime)]
        //public DateTime DateOfLastVisit { get; set; }

        [MaxLength(1)]
        //Пол пользователя
        public string Gender { get; set; }
        //[DataType(DataType.DateTime)]
        ////public DateTime DateOfBirthday { get; set; }

        public bool Online { get; set; }

    }
}