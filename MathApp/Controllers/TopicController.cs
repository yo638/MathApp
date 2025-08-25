using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathApp.Models;
using MathApp.Models.DbModels;
using MathApp.Models.BusinessModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using MathApp.Services.Interfaces;
using MathApp.Services;
using System.IO;

namespace MathApp.Controllers
{
    public class TopicController : Controller
    {
        private readonly ITopicService _temaService;
        private readonly IFileService _fileService;
        public TopicController(ITopicService temaService, IFileService fileService)
        {
            _temaService = temaService;
            _fileService = fileService;
        }
        public IActionResult Create(int id)
        {
            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserSessionKey"));
            Topic tema = new Topic();
            if (id == 1) tema.Type = "lesson";
            else tema.Type = "competition";
            tema.EventDate = DateTime.Now;
            tema.Deletionstatus = 1;
            string creationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            tema.MathProblems.Add(new MathProblem(user.Id,creationDate,creationDate));
            tema.MathProblems.ElementAt(0).Position = 1;
            ViewBag.ModalVisibility = string.Format("none");
            return View(tema);
        }
        [HttpPost]
        public IActionResult Create(Topic tema)
        {
            tema.MathProblems = tema.MathProblems.OrderBy(x => x.Position).ToList();
            if (!String.IsNullOrEmpty(tema.Name))
            {
                User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserSessionKey"));
                tema.IdUser = user.Id;
                tema.CreationDate = DateTime.Now/*.ToString("yyyy-MM-dd HH:mm:ss")*/;
                tema.UpdateDate = tema.CreationDate;
                if (_temaService.CreateTopic(tema))
                {
                    TempData["MessageType"] = "success";
                    TempData["Message"] = "Създаването беше успешно.";
                    if (tema.Type == "lesson") return RedirectToAction("BrowseLessons", "Temi");
                    else return RedirectToAction("BrowseCompetitions", "Temi");
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
                ViewBag.Message = string.Format("Моля, попълнете заглавие.");
                return View(tema);
            }

        }
        public IActionResult Edit(int id)
        {
            var tema = _temaService.GetTopicByID(id);
            tema.MathProblems.Add(new MathProblem());
            tema.MathProblems.ElementAt(tema.MathProblems.Count() - 1).Position = tema.MathProblems.Count();
            return View("Edit",tema);
        }
        [HttpPost]
        public IActionResult Edit(Topic tema)
        {
            tema.MathProblems= tema.MathProblems.OrderBy(x => x.Position).ToList();
            if (!String.IsNullOrEmpty(tema.Name))
            {
                tema.MathProblems.RemoveAt(tema.MathProblems.Count() - 1);
                tema.UpdateDate = DateTime.Now;
                if (_temaService.UpdateTopic(tema))
                {
                    TempData["MessageType"] = "success";
                    TempData["Message"] = "Промените бяха запазени.";
                    if(tema.Type=="lesson") return RedirectToAction("BrowseLessons", "Temi");
                    else return RedirectToAction("BrowseCompetitions", "Temi");
                }
                else
                {
                    ViewBag.MessageType = string.Format("error");
                    ViewBag.Message = string.Format("Моля, опитайте отново.");
                    return View("Edit", tema);
                }
            }
            else
            {
                ViewBag.MessageType = string.Format("warning");
                ViewBag.Message = string.Format("Моля, попълнете заглавие.");
                return View("Edit", tema);
            }
        }

        [HttpPost]
        public IActionResult AddZadachaToTemaWhenCreatingTema(Topic tema)
        {
            tema.MathProblems = tema.MathProblems.OrderBy(x => x.Position).ToList();
            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserSessionKey"));
            string creationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            tema.MathProblems.Add(new MathProblem(user.Id,creationDate, creationDate));
            tema.MathProblems.ElementAt(tema.MathProblems.Count() - 1).Position = tema.MathProblems.Count();
            ViewBag.ModalVisibility = string.Format("none");
            return View("Create", tema);
        }
        [HttpPost]
        public IActionResult AddZadachaToTemaWhenEditingTema(Topic tema)
        {
            tema.MathProblems = tema.MathProblems.OrderBy(x => x.Position).ToList();
            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserSessionKey"));
            string creationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            tema.MathProblems.ElementAt(tema.MathProblems.Count() - 1).Deletionstatus = 1;
            tema.MathProblems.Add(new MathProblem(user.Id, creationDate, creationDate));
            tema.MathProblems.ElementAt(tema.MathProblems.Count() - 1).Position = tema.MathProblems.Count();
            ViewBag.ModalVisibility = string.Format("none");
            return View("Edit", tema);
        }
        [HttpPost]
        [Route("Temi/RemoveZadachaFromTemaWhenCreatingTema/{idZadacha}")]
        public IActionResult RemoveZadachaFromTemaWhenCreatingTema(Topic tema, int idZadacha)
        {
            tema.MathProblems.RemoveAt(idZadacha);
            tema.MathProblems = tema.MathProblems.OrderBy(x => x.Position).ToList();
            for (int i = 0; i < tema.MathProblems.Count(); i++)
            {
                tema.MathProblems.ElementAt(i).Position = i + 1;
            }
            ViewBag.ModalVisibility = string.Format("none");
            return View("Create", tema);
        }
        [HttpPost]
        public IActionResult RemoveUnsavedZadachaFromTemaWhenCreatingTema(Topic tema)
        {
            tema.MathProblems.RemoveAt(tema.MathProblems.Count - 1);
            tema.MathProblems = tema.MathProblems.OrderBy(x => x.Position).ToList();
            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserSessionKey"));
            string creationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            tema.MathProblems.Add(new MathProblem(user.Id, creationDate, creationDate));
            tema.MathProblems.ElementAt(tema.MathProblems.Count() - 1).Position = tema.MathProblems.Count();
            ViewBag.ModalVisibility = string.Format("none");
            return View("Create", tema);
        }
        [HttpPost]
        [Route("Temi/RemoveZadachaFromTemaWhenEditingTema/{idZadacha}")]
        public IActionResult RemoveZadachaFromTemaWhenEditingTema(Topic tema, int idZadacha)
        {
            tema.MathProblems.RemoveAt(idZadacha);
            tema.MathProblems = tema.MathProblems.OrderBy(x => x.Position).ToList();
            for (int i = 0; i < tema.MathProblems.Count(); i++)
            {
                tema.MathProblems.ElementAt(i).Position = i + 1;
            }
            return View("Edit", tema);
        }
        [HttpPost]
        public IActionResult RemoveUnsavedZadachaFromTemaWhenEditingTema(Topic tema)
        {
            tema.MathProblems.RemoveAt(tema.MathProblems.Count-1);
            tema.MathProblems = tema.MathProblems.OrderBy(x => x.Position).ToList();
            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserSessionKey"));
            string creationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            tema.MathProblems.Add(new MathProblem(user.Id, creationDate, creationDate));
            tema.MathProblems.ElementAt(tema.MathProblems.Count() - 1).Position = tema.MathProblems.Count();
            ViewBag.ModalVisibility = string.Format("none");
            return View("Edit", tema);
        }
        [HttpPost]
        [Route("Temi/AddAnswerToZadachaWhenCreatingTema/{idZadacha}")]
        public IActionResult AddAnswerToZadachaWhenCreatingTema(Topic tema, int idZadacha)
        {
            tema.MathProblems.ElementAt(idZadacha).Answers.Add(new Answer("", true));
            ViewBag.ModalVisibility = string.Format("block");
            tema.MathProblems = tema.MathProblems.OrderBy(x => x.Position).ToList();
            return View("Create", tema);
        }
        [HttpPost]
        [Route("Temi/AddAnswerToZadachaWhenEditingTema/{idZadacha}")]
        public IActionResult AddAnswerToZadachaWhenEditingTema(Topic tema, int idZadacha)
        {
            tema.MathProblems.ElementAt(idZadacha).Answers.Add(new Answer("", true));
            //TempData["ModalVisibility"] = "block";
            //ViewBag.ModalVisibility = string.Format("block");
            tema.MathProblems = tema.MathProblems.OrderBy(x => x.Position).ToList();
            return View("Edit", tema);
        }
        [HttpPost]
        [Route("Temi/RemoveAnswerFromZadachaWhenCreatingTema/{idZadacha}")]
        public IActionResult RemoveAnswerFromZadachaWhenCreatingTema(Topic tema, int idZadacha)
        {
            if (tema.MathProblems.ElementAt(idZadacha).Answers.Count > 0)
            {
                tema.MathProblems.ElementAt(idZadacha).Answers.RemoveAt(tema.MathProblems.ElementAt(idZadacha).Answers.Count - 1);
            }
            else
            {
                ViewBag.MessageType = string.Format("warning");
                ViewBag.Message = string.Format("Няма отговор за премахване.");
            }
            ViewBag.ModalVisibility = string.Format("block");
            tema.MathProblems = tema.MathProblems.OrderBy(x => x.Position).ToList();
            return View("Create", tema);
        }
        [HttpPost]
        [Route("Temi/RemoveAnswerFromZadachaWhenEditingTema/{idZadacha}")]
        public IActionResult RemoveAnswerFromZadachaWhenEditingTema(Topic tema,int idZadacha)
        {
            if (tema.MathProblems.ElementAt(idZadacha).Answers.Count > 0)
            {
                tema.MathProblems.ElementAt(idZadacha).Answers.RemoveAt(tema.MathProblems.ElementAt(idZadacha).Answers.Count - 1);
            }
            else
            {
                ViewBag.MessageType = string.Format("warning");
                ViewBag.Message = string.Format("Няма отговор за премахване.");
            }
            ViewBag.ModalVisibility = string.Format("block");
            tema.MathProblems = tema.MathProblems.OrderBy(x => x.Position).ToList();
            return View("Edit", tema);
        }

        [HttpPost]
        [Route("Temi/AddCategoryToZadachaWhenCreatingTema/{idZadacha}")]
        public IActionResult AddCategoryToZadachaWhenCreatingTema(Topic tema, int idZadacha)
        {
            tema.MathProblems.ElementAt(idZadacha).Categories.Add(new Models.DbModels.Category(1, "A"));
            ViewBag.ModalVisibility = string.Format("block");
            tema.MathProblems = tema.MathProblems.OrderBy(x => x.Position).ToList();
            return View("Create", tema);
        }
        [HttpPost]
        [Route("Temi/AddCategoryToZadachaWhenEditingTema/{idZadacha}")]
        public IActionResult AddCategoryToZadachaWhenEditingTema(Topic tema, int idZadacha)
        {
            tema.MathProblems.ElementAt(idZadacha).Categories.Add(new Models.DbModels.Category(1, "A"));
            ViewBag.ModalVisibility = string.Format("block");
            tema.MathProblems = tema.MathProblems.OrderBy(x => x.Position).ToList();
            return View("Edit", tema);
        }
        [HttpPost]
        [Route("Temi/RemoveCategoryFromZadachaWhenCreatingTema/{idZadacha}")]
        public IActionResult RemoveCategoryFromZadachaWhenCreatingTema(Topic tema, int idZadacha)
        {
            if (tema.MathProblems.ElementAt(idZadacha).Categories.Count > 0)
            {
                tema.MathProblems.ElementAt(idZadacha).Categories.RemoveAt(tema.MathProblems.ElementAt(idZadacha).Categories.Count - 1);
            }
            else
            {
                ViewBag.MessageType = string.Format("warning");
                ViewBag.Message = string.Format("Няма сложност за премахване.");
            }
            ViewBag.ModalVisibility = string.Format("block");
            tema.MathProblems = tema.MathProblems.OrderBy(x => x.Position).ToList();
            return View("Create", tema);
        }
        [HttpPost]
        [Route("Temi/RemoveCategoryFromZadachaWhenEditingTema/{idZadacha}")]
        public IActionResult RemoveCategoryFromZadachaWhenEditingTema(Topic tema, int idZadacha)
        {
            if (tema.MathProblems.ElementAt(idZadacha).Categories.Count > 0)
            {
                tema.MathProblems.ElementAt(idZadacha).Categories.RemoveAt(tema.MathProblems.ElementAt(idZadacha).Categories.Count - 1);
            }
            else
            {
                ViewBag.MessageType = string.Format("warning");
                ViewBag.Message = string.Format("Няма сложност за премахване.");
            }
            ViewBag.ModalVisibility = string.Format("block");
            tema.MathProblems = tema.MathProblems.OrderBy(x => x.Position).ToList();
            return View("Edit", tema);
        }
        [HttpPost]
        public IActionResult AddCategoryToTemaWhenCreatingTema(Topic tema)
        {
            tema.IdCategories.Add(new Models.DbModels.Category(1, "A"));
            ViewBag.ModalVisibility = string.Format("none");
            tema.MathProblems = tema.MathProblems.OrderBy(x => x.Position).ToList();
            return View("Create", tema);
        }
        [HttpPost]
        public IActionResult AddCategoryToTemaWhenEditingTema(Topic tema)
        {
            tema.IdCategories.Add(new Models.DbModels.Category(1, "A"));
            ViewBag.ModalVisibility = string.Format("none");
            tema.MathProblems = tema.MathProblems.OrderBy(x => x.Position).ToList();
            return View("Edit", tema);
        }
        [HttpPost]
        public IActionResult RemoveCategoryFromTemaWhenCreatingTema(Topic tema)
        {
            tema.MathProblems = tema.MathProblems.OrderBy(x => x.Position).ToList();
            if (tema.IdCategories.Count > 0)
            {
                tema.IdCategories.RemoveAt(tema.IdCategories.Count - 1);
            }
            else
            {
                ViewBag.MessageType = string.Format("warning");
                ViewBag.Message = string.Format("Няма сложност за премахване.");
            }
            ViewBag.ModalVisibility = string.Format("none");
            return View("Create", tema);
        }
        [HttpPost]
        public IActionResult RemoveCategoryFromTemaWhenEditingTema(Topic tema)
        {
            tema.MathProblems = tema.MathProblems.OrderBy(x => x.Position).ToList();
            if (tema.IdCategories.Count > 0)
            {
                tema.IdCategories.RemoveAt(tema.IdCategories.Count - 1);
            }
            else
            {
                ViewBag.MessageType = string.Format("warning");
                ViewBag.Message = string.Format("Няма сложност за премахване.");
            }
            ViewBag.ModalVisibility = string.Format("none");
            return View("Edit", tema);
        }
        [HttpPost]
        public IActionResult SaveChangesToZadachaWhenEditingTema(Topic tema)
        {
            tema.MathProblems = tema.MathProblems.OrderBy(x => x.Position).ToList();
            return View("Edit", tema);
        }
        public IActionResult SaveChangesToZadachaWhenCreatingTema(Topic tema)
        {
            tema.MathProblems = tema.MathProblems.OrderBy(x => x.Position).ToList();
            return View("Create", tema);
        }
        [Route("Temi/BrowseLessons")]
        public IActionResult BrowseLessons()
        {
            if (!string.IsNullOrEmpty((string)TempData["MessageType"]) && !string.IsNullOrEmpty((string)TempData["Message"]))
            {
                ViewBag.MessageType = string.Format((string)TempData["MessageType"]);
                ViewBag.Message = string.Format((string)TempData["Message"]);
            }
            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserSessionKey"));
            SearchTopics model = new SearchTopics();
            model.criteria = new SearchCriteriaTopic();
            model.criteria.type = "lesson";
            model.topics = _temaService.GetTopicsByUser(user.Id, model.criteria.type);
            return View("Browse", model);
        }
        [Route("Temi/BrowseCompetitions")]
        public IActionResult BrowseCompetitions()
        {
            if (!string.IsNullOrEmpty((string)TempData["MessageType"]) && !string.IsNullOrEmpty((string)TempData["Message"]))
            {
                ViewBag.MessageType = string.Format((string)TempData["MessageType"]);
                ViewBag.Message = string.Format((string)TempData["Message"]);
            }
            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserSessionKey"));
            SearchTopics model = new SearchTopics();
            model.criteria = new SearchCriteriaTopic();
            model.criteria.type = "competition";
            model.topics = _temaService.GetTopicsByUser(user.Id, model.criteria.type);
            return View("Browse", model);
        }
        [HttpPost]
        public IActionResult Search(SearchTopics model)
        {
            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserSessionKey"));
            model.topics = _temaService.SearchTopics(user.Id, model.criteria);
            return View("Browse", model);
        }
        public IActionResult AddTemaToRecycleBin(int id)
        {
            if (_temaService.AddTopicToRecycleBin(id))
            {
                TempData["MessageType"] = "success";
                TempData["Message"] = "Промените бяха запазени.";
            }
            else
            {
                TempData["MessageType"] = "error";
                TempData["Message"] = "Моля, опитайте отново.";
            }
            if (_temaService.GetTopicByID(id).Type == "lesson") return RedirectToAction("BrowseLessons", "Temi");
            else return RedirectToAction("BrowseCompetitions", "Temi");
        }
        public IActionResult AddTemaAndZadachiToRecycleBin(int id)
        {
            if (_temaService.AddTopicAndMathProblemsToRecycleBin(id))
            {
                TempData["MessageType"] = "success";
                TempData["Message"] = "Промените бяха запазени.";
            }
            else
            {
                TempData["MessageType"] = "error";
                TempData["Message"] = "Моля, опитайте отново.";
            }
            if(_temaService.GetTopicByID(id).Type=="lesson") return RedirectToAction("BrowseLessons", "Temi");
            else return RedirectToAction("BrowseCompetitions", "Temi");

        }
        public IActionResult GenerateTemaFile(Topic tema)
        {
            Topic temaToFile = tema;
            temaToFile.MathProblems.RemoveAt(tema.MathProblems.Count - 1);
            var recycledZadachi = temaToFile.MathProblems.Where(x => x.Deletionstatus == 0).Select(x => x.Id).ToList();
            foreach (var id in recycledZadachi)
            {
                var itemToRemove = temaToFile.MathProblems.FirstOrDefault(x => x.Id == id);
                if (itemToRemove != null)
                {
                    temaToFile.MathProblems.Remove(itemToRemove);
                }
            }
            temaToFile.MathProblems = temaToFile.MathProblems.OrderBy(x => x.Position).ToList();
            for (int i = 0; i < tema.MathProblems.Count(); i++)
            {
                tema.MathProblems.ElementAt(i).Position = i + 1;
            }
            var (stream, contentType, filename) = _fileService.GenerateTopicFile(temaToFile);
            return File(stream, contentType, filename);
        }
        public IActionResult GenerateTemaFileWithPassword(Topic tema)
        {
            Topic temaToFile = tema;
            temaToFile.MathProblems.RemoveAt(tema.MathProblems.Count - 1);
            var recycledZadachi = temaToFile.MathProblems.Where(x => x.Deletionstatus == 0).Select(x => x.Id).ToList();
            foreach (var id in recycledZadachi)
            {
                var itemToRemove = temaToFile.MathProblems.FirstOrDefault(x => x.Id == id);
                if (itemToRemove != null)
                {
                    temaToFile.MathProblems.Remove(itemToRemove);
                }
            }
            temaToFile.MathProblems = temaToFile.MathProblems.OrderBy(x => x.Position).ToList();
            for (int i = 0; i < tema.MathProblems.Count(); i++)
            {
                tema.MathProblems.ElementAt(i).Position = i + 1;
            }
            var (stream, contentType, filename) = _fileService.CreateAndEncryptTopicFile(temaToFile, temaToFile.Password);
            return File(stream, contentType, filename);
        }
        public IActionResult GenerateTemaFileWithAnswers(Topic tema)
        {
            Topic temaToFile = tema;
            temaToFile.MathProblems.RemoveAt(tema.MathProblems.Count - 1);
            var recycledZadachi = temaToFile.MathProblems.Where(x => x.Deletionstatus == 0).Select(x => x.Id).ToList();
            foreach (var id in recycledZadachi)
            {
                var itemToRemove = temaToFile.MathProblems.FirstOrDefault(x => x.Id == id);
                if (itemToRemove != null)
                {
                    temaToFile.MathProblems.Remove(itemToRemove);
                }
            }
            temaToFile.MathProblems = temaToFile.MathProblems.OrderBy(x => x.Position).ToList();
            for (int i = 0; i < tema.MathProblems.Count(); i++)
            {
                tema.MathProblems.ElementAt(i).Position = i + 1;
            }
            var (stream, contentType, filename) = _fileService.GenerateTopicFileWithAnswers(temaToFile);
            return File(stream, contentType, filename);

        }
        public IActionResult GenerateTemaFileWithAnswersWithPassword(Topic tema)
        {
            Topic temaToFile = tema;
            temaToFile.MathProblems.RemoveAt(tema.MathProblems.Count - 1);
            var recycledZadachi = temaToFile.MathProblems.Where(x => x.Deletionstatus == 0).Select(x => x.Id).ToList();
            foreach (var id in recycledZadachi)
            {
                var itemToRemove = temaToFile.MathProblems.FirstOrDefault(x => x.Id == id);
                if (itemToRemove != null)
                {
                    temaToFile.MathProblems.Remove(itemToRemove);
                }
            }
            temaToFile.MathProblems = temaToFile.MathProblems.OrderBy(x => x.Position).ToList();
            for (int i = 0; i < tema.MathProblems.Count(); i++)
            {
                tema.MathProblems.ElementAt(i).Position = i + 1;
            }
            var (stream, contentType, filename) = _fileService.CreateAndEncryptTopicFileWithAnswers(temaToFile,temaToFile.Password);
            return File(stream, contentType, filename);

        }

    }
}
