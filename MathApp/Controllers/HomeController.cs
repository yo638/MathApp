using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MathApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace MathApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        Connection connection = new Connection();
        public IActionResult Index()
        {
            List<Answer> answers = new List<Answer>();
            //Connect to MySQL
            using (MySqlConnection con = new MySqlConnection("server=localhost;user=root;database=bank;port=3307;password=%s1WnX6*"))
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("select * from answers", con);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    //Extract you data
                    Answer answer = new Answer();
                    //answer.SetIdAnswer(Convert.ToInt32(reader["id_answer"]));
                    answer.idAnswer=reader["id_answer"].ToString();
                    answer.answer=reader["answer"].ToString();
                    HttpContext.Session.SetString("AnswerSessionKey", JsonConvert.SerializeObject(answer));
                    answers.Add(answer);
                }
                reader.Close();
            }


            return View(connection.getAnswer());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
