using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MathApp.Models;
using MathApp.Models.DbModels;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using MathApp.Services.Interfaces;

namespace MathApp.Controllers
{
    public class RecycleBinController : Controller
    {
        private readonly IMathProblemService _mathProblemService;
        private readonly ITopicService _topicService;
        public RecycleBinController(IMathProblemService mathProblemService, ITopicService topicService)
        {
            _mathProblemService = mathProblemService;
            _topicService = topicService;
        }
        public IActionResult MathProblems()
        {
            if (!string.IsNullOrEmpty((string)TempData["MessageType"]) && !string.IsNullOrEmpty((string)TempData["Message"]))
            {
                ViewBag.MessageType = string.Format((string)TempData["MessageType"]);
                ViewBag.Message = string.Format((string)TempData["Message"]);
            }
            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserSessionKey"));
            return View(_mathProblemService.GetRecycledMathProblemsByUser(user.Id));
        }
        public IActionResult Topics()
        {
            if (!string.IsNullOrEmpty((string)TempData["MessageType"]) && !string.IsNullOrEmpty((string)TempData["Message"]))
            {
                ViewBag.MessageType = string.Format((string)TempData["MessageType"]);
                ViewBag.Message = string.Format((string)TempData["Message"]);
            }
            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserSessionKey"));
            return View(_topicService.GetRecycledTopicsByUser(user.Id));
        }
        public IActionResult RecoverMathProblem(int id)
        {
            if (_mathProblemService.RecoverMathProblemFromRecycleBin(id))
            {
                TempData["MessageType"] = "success";
                TempData["Message"] = "Задачата е възстановена.";
            }
            else
            {
                TempData["MessageType"] = "error";
                TempData["Message"] = "Моля, опитайте отново.";
            }
            return RedirectToAction("MathProblems","RecycleBin");
        }
        public IActionResult DeleteMathProblem(int id)
        {
            if (_mathProblemService.DeleteMathProblem(id))
            {
                TempData["MessageType"] = "success";
                TempData["Message"] = "Задачата е изтрита.";
            }
            else
            {
                TempData["MessageType"] = "error";
                TempData["Message"] = "Моля, опитайте отново.";
            }
            return RedirectToAction("MathProblems", "RecycleBin");
        }
        public IActionResult RecoverTopic(int id)
        {
            if (_topicService.RecoverTopicFromRecycleBin(id))
            {
                TempData["MessageType"] = "success";
                TempData["Message"] = "Съдържанието е възстановено.";
            }
            else
            {
                TempData["MessageType"] = "error";
                TempData["Message"] = "Моля, опитайте отново.";
            }
            return RedirectToAction("Topics", "RecycleBin");
        }
        public IActionResult RecoverTopicAndMathProblems(int id)
        {
            if (_topicService.RecoverTopicAndMathProblemsFromRecycleBin(id))
            {
                TempData["MessageType"] = "success";
                TempData["Message"] = "Съдържанието е възстановено.";
            }
            else
            {
                TempData["MessageType"] = "error";
                TempData["Message"] = "Моля, опитайте отново.";
            }
            return RedirectToAction("Topics", "RecycleBin");
        }
        public IActionResult DeleteTopic(int id)
        {
            if (_topicService.DeleteTopic(id))
            {
                TempData["MessageType"] = "success";
                TempData["Message"] = "Съдържанието е изтрито.";
            }
            else
            {
                TempData["MessageType"] = "error";
                TempData["Message"] = "Моля, опитайте отново.";
            }
            return RedirectToAction("Topics", "RecycleBin");
        }
        public IActionResult DeleteTopicAndMathProblems(int id)
        {
            if (_topicService.DeleteTopicAndMathProblems(id))
            {
                TempData["MessageType"] = "success";
                TempData["Message"] = "Съдържанието е изтрито.";
            }
            else
            {
                TempData["MessageType"] = "error";
                TempData["Message"] = "Моля, опитайте отново.";
            }
            return RedirectToAction("Topics", "RecycleBin");
        }
    }
}
