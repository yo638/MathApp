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
using Microsoft.Extensions.Logging;
using MathApp.Services.Interfaces;

namespace MathApp.Controllers
{
    public class MathProblemController : Controller
    {
        private readonly IMathProblemService _mathProblemService;
        private readonly ILogger<MathProblemController> _logger;
        public MathProblemController(ILogger<MathProblemController> logger, IMathProblemService mathProblemService)
        {
            _logger = logger;
            _mathProblemService = mathProblemService;
        }
        public IActionResult Create()
        {
            MathProblem mathProblem = new MathProblem();
            mathProblem.Categories.Add(new Category(1, "A"));
            mathProblem.Answers.Add(new Answer("", true));
            return View(mathProblem);
        }
        [HttpPost]
        public IActionResult Create(MathProblem mathProblem)
        {
            if (!String.IsNullOrEmpty(mathProblem.Conditions))
            {
                mathProblem.Deletionstatus = (sbyte)(true ? 1 : 0);
                User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserSessionKey"));
                mathProblem.IdUser = user.Id;
                mathProblem.CreationDate = DateTime.Now/*.ToString("yyyy-MM-dd HH:mm:ss")*/;
                mathProblem.UpdateDate = mathProblem.CreationDate;
                if(_mathProblemService.CreateMathProblem(mathProblem))
                {
                    TempData["MessageType"] = "success";
                    TempData["Message"] = "Задачата беше създадена.";
                    return RedirectToAction("Browse", "MathProblems");
                }
                else
                {
                    ViewBag.MessageType = string.Format("error");
                    ViewBag.Message = string.Format("Моля, опитайте отново.");
                    return View(mathProblem);
                }
            }
            else
            {
                ViewBag.MessageType = string.Format("warning");
                ViewBag.Message = string.Format("Моля, попълнете условие на задачата.");
                return View(mathProblem);
            }
        }
        [HttpPost]
        public IActionResult AddAnswerCreate(MathProblem mathProblem)
        {
            mathProblem.Answers.Add(new Answer("",true));
            return View("Create",mathProblem);
        }
        [HttpPost]
        public IActionResult RemoveAnswerCreate(MathProblem mathProblem)
        {
            if(mathProblem.Answers.Count > 0) {
            mathProblem.Answers.RemoveAt(mathProblem.Answers.Count - 1);
            }
            else
            {
                ViewBag.MessageType = string.Format("warning");
                ViewBag.Message = string.Format("Няма отговор за премахване.");
            }
            return View("Create", mathProblem);
        }
        [HttpPost]
        public IActionResult AddCategoryCreate(MathProblem mathProblem)
        {
            mathProblem.Categories.Add(new Category(1,"A"));
            return View("Create", mathProblem);
        }
        [HttpPost]
        public IActionResult RemoveCategoryCreate(MathProblem mathProblem)
        {
            if (mathProblem.Categories.Count > 0)
            {
                mathProblem.Categories.RemoveAt(mathProblem.Categories.Count - 1);
            }
            else
            {
                ViewBag.MessageType = string.Format("warning");
                ViewBag.Message = string.Format("Няма степен на сложност за премахване.");
            }
            return View("Create", mathProblem);
        }
        public IActionResult Edit(int id)
        {
            return View("Edit",_mathProblemService.GetMathProblemById(id));
        }
        [HttpPost]
        public IActionResult Edit(MathProblem mathProblem)
        {
            if (!String.IsNullOrEmpty(mathProblem.Conditions))
            {
                mathProblem.UpdateDate = DateTime.Now;
                if(_mathProblemService.UpdateMathProblem(mathProblem))
                {
                    TempData["MessageType"] = "success";
                    TempData["Message"] = "Промените бяха запазени.";
                    return RedirectToAction("Browse", "Zadachi");
                }
                else
                {
                    ViewBag.MessageType = string.Format("error");
                    ViewBag.Message = string.Format("Моля, опитайте отново.");
                    return View("Edit",mathProblem);
                }
            }
            else
            {
                ViewBag.MessageType = string.Format("warning");
                ViewBag.Message = string.Format("Моля, попълнете условие на задачата.");
                return View("Edit",mathProblem);
            }
        }
        [HttpPost]
        public IActionResult AddAnswerEdit(MathProblem mathProblem)
        {
            mathProblem.Answers.Add(new Answer("", true));
            return View("Edit", mathProblem);
        }
        [HttpPost]
        public IActionResult RemoveAnswerEdit(MathProblem mathProblem)
        {
            if (mathProblem.Answers.Count > 0)
            {
                mathProblem.Answers.RemoveAt(mathProblem.Answers.Count - 1);
            }
            else
            {
                ViewBag.MessageType = string.Format("warning");
                ViewBag.Message = string.Format("Няма отговор за премахване.");
            }
            return View("Edit", mathProblem);
        }
        [HttpPost]
        public IActionResult AddCategoryEdit(MathProblem mathProblem)
        {
            mathProblem.Categories.Add(new Category(1, "A"));
            return View("Edit", mathProblem);
        }
        [HttpPost]
        public IActionResult RemoveCategoryEdit(MathProblem mathProblem)
        {
            if (mathProblem.Categories.Count > 0)
            {
                mathProblem.Categories.RemoveAt(mathProblem.Categories.Count - 1);
            }
            else
            {
                ViewBag.MessageType = string.Format("warning");
                ViewBag.Message = string.Format("Няма сложност за премахване.");
            }
            return View("Edit", mathProblem);
        }
        public IActionResult Browse()
        {

            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserSessionKey"));
            SearchMathProblems model = new SearchMathProblems();
            model.criteria = new SearchCriteriaMathProblem();
            model.mathProblems = _mathProblemService.GetMathProblemsByUser(user.Id);
            if (!string.IsNullOrEmpty((string)TempData["MessageType"]) && !string.IsNullOrEmpty((string)TempData["Message"]))
            {
                ViewBag.MessageType = string.Format((string)TempData["MessageType"]);
                ViewBag.Message = string.Format((string)TempData["Message"]);
            }
            ViewBag.UserRoleId = user.IdRole;
            return View(model);
        }
        public IActionResult AddToRecycleBin(int id)
        {
            if (_mathProblemService.AddMathProblemToRecycleBin(id))
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
        public IActionResult Search(SearchMathProblems model)
        {
            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserSessionKey"));
            model.mathProblems = _mathProblemService.SearchMathProblems(user.Id, model.criteria);
            return View("Browse", model);
        }
    }
}
