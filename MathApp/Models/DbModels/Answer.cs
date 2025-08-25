using System;
using System.Collections.Generic;

namespace MathApp.Models.DbModels
{
    public partial class Answer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public sbyte Validity { get; set; }
        public int MathProblem { get; set; }

        public virtual MathProblem MathProblemNavigation { get; set; }
    }
}
