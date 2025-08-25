using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathApp.Models.DbModels;

namespace MathApp.Models.BusinessModels
{
    public class SearchMathProblems
    {
        public SearchCriteriaMathProblem criteria { get; set; }
        public IEnumerable<MathProblem> mathProblems { get; set; }
        public SearchMathProblems()
        {
            this.mathProblems = new List<MathProblem>();
        }
    }
}
