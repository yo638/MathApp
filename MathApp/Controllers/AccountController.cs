using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using MathApp.Models;
using MathApp.Models.NotificationTypes;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace MathApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        Connection connection = new Connection();
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(User user)
        {
            if (String.IsNullOrEmpty(user.email) || String.IsNullOrEmpty(user.password))
            {
                ViewBag.MessageType = string.Format("warning");
                ViewBag.Message = string.Format("Моля, попълнете всички полета.");
            }
            else
            {
                if (connection.logUserIn(user))
                {
                    ViewBag.MessageType = string.Format("success");
                    ViewBag.Message = string.Format("Вие се логнахте успешно.");
                    HttpContext.Session.SetString("UserSessionKey", JsonConvert.SerializeObject(connection.getUserByEmail(user.email)));
                    return LocalRedirect("/Home/Index");
                }
                else {
                    ViewBag.MessageType = string.Format("error");
                    ViewBag.Message = string.Format("Неправилен имейл или парола."); 
                }
            }
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (!String.IsNullOrEmpty(user.username) && !String.IsNullOrEmpty(user.email) && !String.IsNullOrEmpty(user.password) && !String.IsNullOrEmpty(user.repeatpassword))
            {
                string emailPattern = @"^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$"; // Email address pattern
                string passwordNumberPattern = @"[0-9]+";
                string passwordUpperCharPattern = @"[A-Z]+";
                string password8CharsPattern = @".{8,}";

                bool isEmailValid = Regex.IsMatch(user.email, emailPattern);
                bool passwordHasNumber = Regex.IsMatch(user.password, passwordNumberPattern);
                bool passwordHasUpperChar = Regex.IsMatch(user.password, passwordUpperCharPattern);
                bool passwordHas8Chars = Regex.IsMatch(user.password, password8CharsPattern);

                if (isEmailValid)
                {
                    if (passwordHasNumber)
                    {
                        if (passwordHasUpperChar)
                        {
                            if (passwordHas8Chars)
                            {
                                if (user.password == user.repeatpassword)
                                {
                                    if (connection.RegisterUser(user))
                                    {
                                        HttpContext.Session.SetString("UserSession", JsonConvert.SerializeObject(connection.getUserByEmail(user.email)));
                                        ViewBag.MessageType = string.Format("success");
                                        ViewBag.Message = string.Format("Вие се регистрирахте успешно.");
                                        return LocalRedirect("/Home/Index");
                                    }
                                    else
                                    {
                                        ViewBag.MessageType = string.Format("error");
                                        ViewBag.Message = string.Format("Този имейл вече е бил регистриран.");
                                    }
                                }
                                else
                                {
                                    ViewBag.MessageType = string.Format("warning");
                                    ViewBag.Message = string.Format("Паролите не съвпадат.");
                                }
                            }
                            else
                            {
                                ViewBag.MessageType = string.Format("warning");
                                ViewBag.Message = string.Format("Паролата трябва да съдържа поне 8 знака.");
                            }
                        }
                        else
                        {
                            ViewBag.MessageType = string.Format("warning");
                            ViewBag.Message = string.Format("Паролата трябва да съдържа поне една главна буква.");
                        }
                    }
                    else
                    {
                        ViewBag.MessageType = string.Format("warning");
                        ViewBag.Message = string.Format("Паролата трябва да съдържа поне една цифра.");
                    }
                }
                else
                {
                    ViewBag.MessageType = string.Format("error");
                    ViewBag.Message = string.Format("Невалиден имейл.");
                }
            }
            else
            {
                ViewBag.MessageType = string.Format("warning");
                ViewBag.Message = string.Format("Моля, попълнете всички полета.");
            }
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View();
        }

    }
}
