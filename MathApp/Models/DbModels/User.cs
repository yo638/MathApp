using System;
using System.Collections.Generic;

namespace MathApp.Models.DbModels
{
    public partial class User
    {
        public User()
        {
            Topics = new List<Topic>();
            MathProblems = new List<MathProblem>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public sbyte EmailConfirmation { get; set; }
        public sbyte IsDisabled { get; set; }
        public string Password { get; set; }
        public int IdRole { get; set; }
        public int IdDirectory { get; set; }
        public DateTime CreationDate { get; set; }


        public virtual Role IdRoleNavigation { get; set; }
        public virtual IList<Topic> Topics { get; set; }
        public virtual IList<MathProblem> MathProblems { get; set; }
        public virtual IList<ChangePasswordCode> PasswordCodes { get; set; }
    }
}
