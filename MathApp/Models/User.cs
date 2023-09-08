using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathApp.Models
{
    public class User
    {
        public int IdUser { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string repeatpassword { get; set; }
        public User() { }
    }
}
