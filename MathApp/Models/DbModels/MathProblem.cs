using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace MathApp.Models.DbModels
{
    [Table("math_problems")]
    public partial class MathProblem
    {
        public MathProblem()
        {
            Answers = new List<Answer>();
            Categories = new List<Category>();
        }

        public int Id { get; set; }
        public string Conditions { get; set; }
        public string Solution { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int? IdUser { get; set; }
        public int? IdTopic { get; set; }
        public int? Position { get; set; }
        public sbyte Deletionstatus { get; set; }

        public virtual Topic IdTopicNavigation { get; set; }
        public virtual User IdUserNavigation { get; set; }
        public virtual IList<Answer> Answers { get; set; }

        public virtual IList<Category> Categories { get; set; }
    }
}
