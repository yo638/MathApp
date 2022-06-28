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
                if(connection.createZadacha(zadacha))
                {
                    TempData["MessageType"] = "success";
                    TempData["Message"] = "Задачата беше създадена.";
                    return RedirectToAction("Browse", "Zadachi");
                }
                else
                {
                    ViewBag.MessageType = string.Format("error");
                    ViewBag.Message = string.Format("Моля, опитайте отново.");
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
        public IActionResult AddAnswerCreate(Zadacha zadacha)
        {
            zadacha.answers.Add(new Answer("Отговор",true));
            return View("Create",zadacha);
        }
        [HttpPost]
        public IActionResult RemoveAnswerCreate(Zadacha zadacha)
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
        public IActionResult AddCategoryCreate(Zadacha zadacha)
        {
            zadacha.categories.Add(new Category(1,"A"));
            return View("Create", zadacha);
        }
        [HttpPost]
        public IActionResult RemoveCategoryCreate(Zadacha zadacha)
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
            return View("Edit",connection.getZadachaByID(id));
        }
        [HttpPost]
        public IActionResult Edit(Zadacha zadacha)
        {
            if (!String.IsNullOrEmpty(zadacha.uslovie))
            {
                zadacha.updateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                if(connection.updateZadacha(zadacha))
                {
                    ViewBag.MessageType = string.Format("success");
                    ViewBag.Message = string.Format("Промените бяха запазени.");
                    return View("Browse", connection.getZadachiByUser(zadacha.user));
                }
                else
                {
                    ViewBag.MessageType = string.Format("error");
                    ViewBag.Message = string.Format("Моля, опитайте отново.");
                    return View("Edit",zadacha);
                }
            }
            else
            {
                ViewBag.MessageType = string.Format("warning");
                ViewBag.Message = string.Format("Моля, попълнете условие на задачата.");
                return View("Edit",zadacha);
            }
        }
        [HttpPost]
        public IActionResult AddAnswerEdit(Zadacha zadacha)
        {
            zadacha.answers.Add(new Answer("Отговор", true));
            return View("Edit", zadacha);
        }
        [HttpPost]
        public IActionResult RemoveAnswerEdit(Zadacha zadacha)
        {
            if (zadacha.answers.Count > 0)
            {
                zadacha.answers.RemoveAt(zadacha.answers.Count - 1);
            }
            else
            {
                ViewBag.MessageType = string.Format("warning");
                ViewBag.Message = string.Format("Няма отговор за премахване.");
            }
            return View("Edit", zadacha);
        }
        [HttpPost]
        public IActionResult AddCategoryEdit(Zadacha zadacha)
        {
            zadacha.categories.Add(new Category(1, "A"));
            return View("Edit", zadacha);
        }
        [HttpPost]
        public IActionResult RemoveCategoryEdit(Zadacha zadacha)
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
            return View("Edit", zadacha);
        }
        public IActionResult Browse()
        {
            if (!string.IsNullOrEmpty((string)TempData["MessageType"]) && !string.IsNullOrEmpty((string)TempData["Message"]))
            {
                ViewBag.MessageType = string.Format((string)TempData["MessageType"]);
                ViewBag.Message = string.Format((string)TempData["Message"]);
            }
            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserSessionKey"));
            SearchZadachi model = new SearchZadachi();
            model.criteria = new SearchCriteriaZadacha();
            model.zadachi = connection.getZadachiByUser(user.idUser);
            return View(model);
        }
        public IActionResult AddToRecycleBin(int id)
        {
            if (connection.addZadachaToRecycleBin(id))
            {
                TempData["MessageType"] = "success";
                TempData["Message"] = "Промените бяха запазени.";
            }
            else
            {
                TempData["MessageType"] = "error";
                TempData["Message"] = "Моля, опитайте отново.";
            }
            return RedirectToAction("Browse", "Zadachi");
        }
        [HttpPost]
        public IActionResult Search(SearchZadachi model)
        {
            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserSessionKey"));
            model.zadachi = connection.searchZadachi(user.idUser, model.criteria);
            return View("Browse", model);
        }
    }
}
