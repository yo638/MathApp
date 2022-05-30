using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathApp.Models
{
    public class Category
    {
        public string idCategory { get; set; }
        public int grade { get; set; }
        public string difficulty { get; set; }
        public Category() { }
        public Category(int grade, string difficulty)
        {
            this.grade = grade;
            this.difficulty = difficulty;
        }
    }
}
