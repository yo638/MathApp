using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathApp.Models.DbModels
{
    public partial class Category
    {
        public Category(int grade, string difficulty)
        {
            this.Grade = grade;
            this.Difficulty = difficulty;
        }

    }
}
