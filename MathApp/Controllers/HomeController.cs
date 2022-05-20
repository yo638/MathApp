using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MathApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace MathApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            List<Position> positions = new List<Position>();
            //Connect to MySQL
            using (MySqlConnection con = new MySqlConnection("server=localhost;user=root;database=bank;port=3307;password=%s1WnX6*"))
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("select * from positions", con);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    //Extract you data
                    Position position = new Position();
                    position.setIDPosition(Convert.ToInt32(reader["id_position"]));
                    position.setPosition(reader["position"].ToString());

                    positions.Add(position);
                }
                reader.Close();
            }


            return View(positions);
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
