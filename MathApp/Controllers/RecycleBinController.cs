using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MathApp.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace MathApp.Controllers
{
    public class RecycleBinController : Controller
    {
        Connection connection = new Connection();
        public IActionResult Zadachi()
        {
            if (!string.IsNullOrEmpty((string)TempData["MessageType"]) && !string.IsNullOrEmpty((string)TempData["Message"]))
            {
                ViewBag.MessageType = string.Format((string)TempData["MessageType"]);
                ViewBag.Message = string.Format((string)TempData["Message"]);
            }
            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserSessionKey"));
            return View(connection.GetRecycledZadachiByUser(user.IdUser));
        }
        public IActionResult Temi()
        {
            return View();
        }
        public IActionResult Testove()
        {
            return View();
        }
        public IActionResult Sastezaniq()
        {
            return View();
        }
        public IActionResult RecoverZadacha(int id)
        {
            if (connection.RecoverZadachaFromRecycleBin(id))
            {
                TempData["MessageType"] = "success";
                TempData["Message"] = "Задачата е възстановена.";
            }
            else
            {
                TempData["MessageType"] = "error";
                TempData["Message"] = "Моля, опитайте отново.";
            }
            return RedirectToAction("Zadachi","RecycleBin");
        }
        public IActionResult DeleteZadacha(int id)
        {
            if (connection.DeleteZadacha(id))
            {
                TempData["MessageType"] = "success";
                TempData["Message"] = "Задачата е изтрита.";
            }
            else
            {
                TempData["MessageType"] = "error";
                TempData["Message"] = "Моля, опитайте отново.";
            }
            return RedirectToAction("Zadachi", "RecycleBin");
        }
    }
}
