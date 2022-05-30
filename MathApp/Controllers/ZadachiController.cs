using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MathApp.Controllers
{
    public class ZadachiController : Controller
    {
        Connection connection = new Connection();
        public IActionResult Create()
        {
            Zadacha zadacha = new Zadacha();
            zadacha.categories.Add(new Category(1, "A"));
            zadacha.answers.Add(new Answer("Отговор", true));
            return View(zadacha);
        }
        [HttpPost]
        public IActionResult Create(Zadacha zadacha)
        {
            if (!String.IsNullOrEmpty(zadacha.uslovie))
            {
                zadacha.deletionStatus = "saved";
                User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserSessionKey"));
                zadacha.user = user.idUser;
                zadacha.creationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                zadacha.updateDate = zadacha.creationDate;
                if(connection.CreateZadacha(zadacha))
                {
                    ViewBag.MessageType = string.Format("success");
                    ViewBag.Message = string.Format("Задачата беше създадена.");
                    return View("Browse");
                }
                else
                {
                    ViewBag.MessageType = string.Format("error");
                    ViewBag.Message = string.Format("Нещо се обърка.");
                    return View(zadacha);
                }
            }
            else
            {
                ViewBag.MessageType = string.Format("warning");
                ViewBag.Message = string.Format("Моля, попълнете условие на задачата.");
                return View(zadacha);
            }
        }
        [HttpPost]
        public IActionResult AddAnswer( Zadacha zadacha)
        {
            zadacha.answers.Add(new Answer("Отговор",true));
            return View("Create",zadacha);
        }
        [HttpPost]
        public IActionResult RemoveAnswer(Zadacha zadacha)
        {
            if(zadacha.answers.Count > 0) {
            zadacha.answers.RemoveAt(zadacha.answers.Count - 1);
            }
            else
            {
                ViewBag.MessageType = string.Format("warning");
                ViewBag.Message = string.Format("Няма отговор за премахване.");
            }
            return View("Create", zadacha);
        }
        [HttpPost]
        public IActionResult AddCategory(Zadacha zadacha)
        {
            zadacha.categories.Add(new Category(1,"A"));
            return View("Create", zadacha);
        }
        [HttpPost]
        public IActionResult RemoveCategory(Zadacha zadacha)
        {
            if (zadacha.categories.Count > 0)
            {
                zadacha.categories.RemoveAt(zadacha.categories.Count - 1);
            }
            else
            {
                ViewBag.MessageType = string.Format("warning");
                ViewBag.Message = string.Format("Няма сложност за премахване.");
            }
            return View("Create", zadacha);
        }
        public IActionResult Edit(int id)
        {
            return View(connection.getZadachaByID(id));
        }
        public IActionResult Browse()
        {
            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserSessionKey"));
            return View(connection.getZadachiByUser(user.idUser));
        }
    }
}
