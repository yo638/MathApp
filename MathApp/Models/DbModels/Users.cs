using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MathApp.Models.DbModels
{
    public partial class Users
    {
        public Users()
        {
            this.Temi = new HashSet<Temi>();
            this.Zadachi = new HashSet<Zadachi>();
        }

        public int IdUser { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public virtual ICollection<Temi> Temi { get; set; }
        public virtual ICollection<Zadachi> Zadachi { get; set; }
    }
}
