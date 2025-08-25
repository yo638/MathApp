using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathApp.Models.DbModels;

namespace MathApp.Models.BusinessModels
{
    public class SearchCriteriaMathProblem
    {
        public string uslovie { get; set; }
        public string solution { get; set; }
        public string answer { get; set; }
        public string anywhere { get; set; }
        public Category category { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public SearchCriteriaMathProblem()
        {
            this.category = new Category();
        }
        public SearchCriteriaMathProblem(string uslovie, string solution, string answer, Category category, string fromDate, string toDate )
        {
            this.uslovie = uslovie;
            this.solution = solution;
            this.answer = answer;
            this.category = category;
            this.fromDate = fromDate;
            this.toDate = toDate;
        }
    }
}
