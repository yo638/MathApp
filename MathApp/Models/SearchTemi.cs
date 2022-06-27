using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathApp.Models
{
    public class SearchTemi
    {
        public SearchCriteriaTema criteria { get; set; }
        public IEnumerable<Tema> temi { get; set; }
        public SearchTemi()
        {
            this.temi = new List<Tema>();
        }
        public SearchTemi(SearchCriteriaTema searchCriteriaTema, IEnumerable<Tema> temi)
        {
            this.criteria = searchCriteriaTema;
            this.temi = temi;
        }
    }
}
