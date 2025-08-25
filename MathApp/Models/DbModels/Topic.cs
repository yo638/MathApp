using System;
using System.Collections.Generic;
using System.Linq;

namespace MathApp.Models.DbModels
{
    public partial class Topic
    {
        public Topic()
        {
            MathProblems = new List<MathProblem>();
            IdCategories = new List<Category>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime EventDate { get; set; }
        public int? IdUser { get; set; }
        public sbyte Deletionstatus { get; set; }

        public virtual User IdUserNavigation { get; set; }
        public virtual IList<MathProblem> MathProblems { get; set; }

        public virtual IList<Category> IdCategories { get; set; }
    }
}
