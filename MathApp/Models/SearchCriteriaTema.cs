using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathApp.Models
{
    public class SearchCriteriaTema
    {
        public string name { get; set; }
        public string description { get; set; }
        public string anywhere { get; set; }
        public Category category { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public SearchCriteriaTema() 
        {
            this.category = new Category();
        }
    }
}
