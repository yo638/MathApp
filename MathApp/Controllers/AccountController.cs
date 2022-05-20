using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using MathApp.Models;


namespace MathApp.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            //Get session info
            var answer = JsonConvert.DeserializeObject<Answer>(HttpContext.Session.GetString("AnswerSessionKey"));
            return View(answer);
        }
    }
}
