using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using MathApp.Models;
using MathApp.Models.DbModels;
using MathApp.Models.BusinessModels;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using System.Dynamic;
using System.Net;
using System.Net.Mail;
using MathApp.Services.Interfaces;

namespace MathApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;

        private readonly IUserService _userService;
        public AccountController(ILogger<AccountController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(User user)
        {
            if (String.IsNullOrEmpty(user.Email) || String.IsNullOrEmpty(user.Password))
            {
                ViewBag.MessageType = string.Format("warning");
                ViewBag.Message = string.Format("Моля, попълнете всички полета.");
            }
            else
            {
                if (_userService.LogUserIn(user))
                {
                    if (_userService.IsUserActive(user))
                    {
                        ViewBag.MessageType = string.Format("success");
                        ViewBag.Message = string.Format("Вие се логнахте успешно.");
                        HttpContext.Session.SetString("UserSessionKey", JsonConvert.SerializeObject(_userService.GetUserByEmail(user.Email)));
                        return LocalRedirect("/Zadachi/Browse");
                    }
                    else{
                        ViewBag.MessageType = string.Format("error");
                        ViewBag.Message = string.Format("Профилът Ви е деактивиран.");
                    }
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
            user.IdRole = 2;
            if (_userService.RegisterUser(user))
            {
                _userService.SendConfirmationEmail(user.Email, HttpContext);
                user = _userService.GetUserByEmail(user.Email);
                HttpContext.Session.SetString("UserSessionKey", JsonConvert.SerializeObject(user));
                ViewBag.MessageType = string.Format("success");
                ViewBag.Message = string.Format("Вие се регистрирахте успешно.");
                ViewBag.MessageType = string.Format("info");
                ViewBag.Message = string.Format("Моля, потвърдете имейла си през линка, който изпратихме на пощата Ви.");
                return LocalRedirect("/Zadachi/Browse");
            }
            else
            {
                ViewBag.MessageType = string.Format("error");
                ViewBag.Message = string.Format("Този имейл вече е бил регистриран.");
            }

            return View(user);
        }
        public IActionResult ForgottenPasswordEnterEmail()
        {
            return View();
        }
        [HttpPost]
        [Route("Account/ForgottenPasswordSendEmail")]
        public IActionResult ForgottenPasswordSendEmail(User u)
        {
            User user = _userService.GetUserByEmail(u.Email);
            if (user == null)
            {
                ViewBag.MessageType = string.Format("error");
                ViewBag.Message = string.Format("Този имейл не съществува в системата.");
            }
            else
            {
                if (!_userService.CheckIfUserCodesAreMoreThanFive(user.Id))
                {
                    ViewBag.MessageType = string.Format("error");
                    ViewBag.Message = string.Format("Надвишихте броя на позволени опити. Моля, опитайте по-късно.");
                }
                else
                {
                    if (_userService.SendCodeToAlterPassword(user.Email))
                    {
                        return View("ForgottenPasswordInsertNewPassword",user);
                    }
                    ViewBag.MessageType = string.Format("error");
                    ViewBag.Message = string.Format("Възникна грешка. Опитайте отново по-късно.");
                }
            }
            return View("ForgottenPasswordEnterEmail",user);
        }

        [HttpGet]
        [Route("Account/EmailConfirmed/{token}")]
        public IActionResult EmailConfirmed(int token)
        {

            if (_userService.ConfirmUserEmailByToken(token))
            {
                ViewBag.MessageType = string.Format("success");
                ViewBag.Message = string.Format("Благодаря Ви че потвърдихте имейла.");
                return View("Login");
            }
            else
            {
                ViewBag.MessageType = string.Format("error");
                ViewBag.Message = string.Format("Възникна грешка.");
                return View("Login");
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Login");
        }
        public IActionResult BrowseUsers()
        {
            SearchUsers model = new SearchUsers();
            model.criteria = new SearchCriteriaUser();
            model.users = _userService.GetAllUsers();
            if (!string.IsNullOrEmpty((string)TempData["MessageType"]) && !string.IsNullOrEmpty((string)TempData["Message"]))
            {
                ViewBag.MessageType = string.Format((string)TempData["MessageType"]);
                ViewBag.Message = string.Format((string)TempData["Message"]);
            }
            return View("Users",model);
        }
        [HttpPost]
        public IActionResult Search(SearchUsers model)
        {
            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserSessionKey"));
            model.users = _userService.SearchUsers(model.criteria);
            return View("Users", model);
        }
        [HttpPost]
        public IActionResult Sort(SearchUsers model)
        {
            model.users = _userService.SearchUsers(model.criteria);
            model.users = _userService.SortUsers(model.users,model.criteria.sortBy);
            return View("Users", model);
        }
        public IActionResult CreateUser()
        {
            User user = new User();
            user.IdRole = 2;
            return View(user);
        }
        [HttpPost]
        [Route("Account/RegisterUserByAdmin")]
        public IActionResult RegisterUserByAdmin(User user)
        {
            if (_userService.RegisterUser(user))
            {
                ViewBag.MessageType = string.Format("success");
                ViewBag.Message = string.Format("Вие се регистрирахте успешно.");
                return LocalRedirect("Users");
            }
            else
            {
                ViewBag.MessageType = string.Format("error");
                ViewBag.Message = string.Format("Този имейл вече е бил регистриран.");
            }
            return View();
        }
        public IActionResult EditMyUser()
        {
            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserSessionKey"));
            return View("EditUser", _userService.GetUserById(user.Id));
        }
        public IActionResult EditUser(int id)
        {
            return View("EditUser", _userService.GetUserById(id));
        }
        [HttpPost]
        public IActionResult EditUser(User user)
        {
            if (_userService.EditUser(user))
            {
                TempData["MessageType"] = "success";
                TempData["Message"] = "Промените бяха запазени.";
                return RedirectToAction("BrowseUsers", "Account");
            }
            else
            {
                ViewBag.MessageType = string.Format("error");
                ViewBag.Message = string.Format("Моля, опитайте отново.");
                return View("Edit", user);
            }
            return View("");
        }
        [HttpPost]
        public IActionResult ChangeUserPassword(User user)
        {
            if (_userService.ChangeUserPassword(user))
            {
                ViewBag.MessageType = string.Format("success");
                ViewBag.Message = string.Format("Успешно променихте паролата.");
            }
            else
            {
                ViewBag.MessageType = string.Format("error");
                ViewBag.Message = string.Format("Неправилна стара парола.");
            }
            return View("EditUser", user);
        }
        public IActionResult ChangeForgottenUserPassword(User user)
        {

            if (!_userService.ValidateCodeForChangingPassword(user))
            {
                ViewBag.MessageType = string.Format("error");
                ViewBag.Message = string.Format("Кодът за смяна е грешен или изтекъл.");
                return View("ForgottenPasswordInsertNewPassword", user);
            }
            else if (_userService.ChangeForgottenUserPassword(user))
            {
                ViewBag.MessageType = string.Format("success");
                ViewBag.Message = string.Format("Успешно променихте паролата.");
            }
            else
            {
                ViewBag.MessageType = string.Format("error");
                ViewBag.Message = string.Format("Моля, опитайте отново.");
                return View("ForgottenPasswordInsertNewPassword", user);
            }
            return View("Login");
        }
        public IActionResult DeleteUser(int id)
        {
            _userService.DeleteUser(id);
            return RedirectToAction("BrowseUsers", "Account");
        }
        [HttpPost("ToggleUserDisabled")]
        [Route("Account/ToggleUserDisabled")]
        public async Task<IActionResult> ToggleUserDisabled([FromBody] User request)
        {
            await _userService.ToggleUserDisableOrEnable(request);
            return NoContent();
        }

    }
}
