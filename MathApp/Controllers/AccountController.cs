using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using MathApp.Models;
using Microsoft.Extensions.Logging;


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
            //User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserSessionKey"));
            //Check if it's empty
            //Display a message if it is
            //else user.email=
            //connection.logUserIn(user);


            //Get session info
            //var answer = JsonConvert.DeserializeObject<Answer>(HttpContext.Session.GetString("AnswerSessionKey"));
            return View(/*answer*/);
        }
        [HttpPost]
        public IActionResult Login(User user)
        {

            string email = user.email;
            string pass = user.password;
            if (String.IsNullOrEmpty(user.email) && String.IsNullOrEmpty(user.password))
            {
                //display message it is empty or incorrect
            }
            else
            {
                if (/*connection.logUserIn(user)*/true)
                {
                    //Display message it is succesful
                    ViewBag.Message = string.Format("True HELLo Marty, it's Alex");
                    //Redirect to ProfilePage
                }
                else ViewBag.Message = string.Format("False HELLo Alex, it's Marty");
            }
            return View();
        }
            /*public IActionResult Register(User user)
        {
            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserSessionKey"));
            return View();
        }*/
    }
}
