using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathApp.Models.DbModels;

namespace MathApp.Models.BusinessModels
{
    public class SearchCriteriaTopic
    {
        public string name { get; set; }
        public string description { get; set; }
        public string anywhere { get; set; }
        public Category category { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public string type { get; set; }
        public SearchCriteriaTopic() 
        {
            this.category = new Category();
        }
    }
}
