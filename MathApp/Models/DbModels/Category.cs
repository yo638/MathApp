using System;
using System.Collections.Generic;

namespace MathApp.Models.DbModels
{
    public partial class Category
    {
        public Category()
        {
            IdTopics = new List<Topic>();
            IdMathProblems = new List<MathProblem>();
        }

        public int Id { get; set; }
        public int? Grade { get; set; }
        public string Difficulty { get; set; }

        public virtual IList<Topic> IdTopics { get; set; }
        public virtual IList<MathProblem> IdMathProblems { get; set; }
    }
}
