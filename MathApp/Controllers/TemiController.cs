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
    public class TemiController : Controller
    {
        //private readonly ApplicationDbContext _context;
        Connection connection = new Connection();
        public IActionResult Create()
        {
            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserSessionKey"));
            Tema tema = new Tema();
            string creationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            tema.zadachi.Add(new Zadacha(0,user.IdUser,creationDate,creationDate));
            ViewBag.ModalVisibility = string.Format("none");
            return View(tema);
        }
        [HttpPost]
        public IActionResult Create(Tema tema)
        {
            if (!String.IsNullOrEmpty(tema.temaName)) {
                User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserSessionKey"));
                tema.user = user.IdUser;
                tema.creationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                tema.updateDate = tema.creationDate;
                if (connection.CreateTema(tema))
                {
                    TempData["MessageType"] = "success";
                    TempData["Message"] = "Темата беше създадена.";
                    return RedirectToAction("Browse", "Temi");
                }
                else
                {
                    ViewBag.MessageType = string.Format("error");
                    ViewBag.Message = string.Format("Моля, опитайте отново.");
                    ViewBag.ModalVisibility = string.Format("none");
                    return View(tema);
                }
            }
            else
            {
                ViewBag.MessageType = string.Format("warning");
                ViewBag.Message = string.Format("Моля, попълнете име на темата.");
                return View(tema);
            }

        }
        [HttpPost]
        public IActionResult AddZadacha(Tema tema)
        {
            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserSessionKey"));
            string creationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            int idZadacha=tema.zadachi.Count;
            tema.zadachi.Add(new Zadacha(idZadacha, user.IdUser,creationDate, creationDate));
            ViewBag.ModalVisibility = string.Format("none");
            return View("Create", tema);
        }
        [HttpPost]
        public IActionResult RemoveZadacha(Tema tema)
        {
            if (tema.zadachi.Count() == 1)
            {
                ViewBag.MessageType = string.Format("warning");
                ViewBag.Message = string.Format("Няма задача за премахване.");
            }
            else {
                for (int i = 0; i < tema.zadachi.Count(); i++) { 
                if(tema.zadachi[i].deletionStatus=="on") tema.zadachi.RemoveAt(i);
                }
            }
            ViewBag.ModalVisibility = string.Format("none");
            return View("Create", tema);
        }
        [HttpPost]
        public IActionResult AddAnswerCreate(Tema tema)
        {
            tema.zadachi[tema.zadachi.Count()-1].answers.Add(new Answer("", true));
            ViewBag.ModalVisibility = string.Format("block");
            return View("Create", tema);
        }
        [HttpPost]
        public IActionResult RemoveAnswerCreate(Tema tema)
        {
            if (tema.zadachi[tema.zadachi.Count() - 1].answers.Count > 0)
            {
                tema.zadachi[tema.zadachi.Count() - 1].answers.RemoveAt(tema.zadachi[tema.zadachi.Count() - 1].answers.Count - 1);
            }
            else
            {
                ViewBag.MessageType = string.Format("warning");
                ViewBag.Message = string.Format("Няма отговор за премахване.");
            }
            ViewBag.ModalVisibility = string.Format("block");
            return View("Create", tema);
        }

        [HttpPost]
        public IActionResult AddCategoryCreate(Tema tema)
        {
            tema.zadachi[tema.zadachi.Count() - 1].categories.Add(new Category(1, "A"));
            ViewBag.ModalVisibility = string.Format("block");
            return View("Create", tema);
        }
        [HttpPost]
        public IActionResult RemoveCategoryCreate(Tema tema)
        {
            if (tema.zadachi[tema.zadachi.Count() - 1].categories.Count > 0)
            {
                tema.zadachi[tema.zadachi.Count() - 1].categories.RemoveAt(tema.zadachi[tema.zadachi.Count() - 1].categories.Count - 1);
            }
            else
            {
                ViewBag.MessageType = string.Format("warning");
                ViewBag.Message = string.Format("Няма сложност за премахване.");
            }
            ViewBag.ModalVisibility = string.Format("block");
            return View("Create", tema);
        }
        [HttpPost]
        public IActionResult AddCategoryToTemaCreate(Tema tema)
        {
            tema.categories.Add(new Category(1, "A"));
            ViewBag.ModalVisibility = string.Format("none");
            return View("Create", tema);
        }
        [HttpPost]
        public IActionResult RemoveCategoryFromTemaCreate(Tema tema)
        {
            if (tema.categories.Count > 0)
            {
                tema.categories.RemoveAt(tema.categories.Count - 1);
            }
            else
            {
                ViewBag.MessageType = string.Format("warning");
                ViewBag.Message = string.Format("Няма сложност за премахване.");
            }
            ViewBag.ModalVisibility = string.Format("none");
            return View("Create", tema);
        }
        public IActionResult Browse()
        {
            if (!string.IsNullOrEmpty((string)TempData["MessageType"]) && !string.IsNullOrEmpty((string)TempData["Message"]))
            {
                ViewBag.MessageType = string.Format((string)TempData["MessageType"]);
                ViewBag.Message = string.Format((string)TempData["Message"]);
            }
            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserSessionKey"));
            SearchTemi model = new SearchTemi();
            model.criteria = new SearchCriteriaTema();
            model.temi = connection.GetTemiByUser(user.IdUser);
            return View(model);
        }
        [HttpPost]
        public IActionResult Search(SearchTemi model)
        {
            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserSessionKey"));
            model.temi = connection.SearchTemi(user.IdUser, model.criteria);
            return View("Browse", model);
        }
        /*public IActionResult EditZadacha(int id)
        {
            //var zadacha=_context.
            //return PartialView("_EditZadacha", model);
        }*/
    }
}
