using System;
using System.Collections.Generic;

namespace MathApp.Models.DbModels
{
    public partial class Role
    {
        public Role()
        {
            Users = new List<User>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual IList<User> Users { get; set; }
    }
}
