using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathApp.Models
{
    public class SearchZadachi
    {
        public SearchCriteriaZadacha criteria { get; set; }
        public IEnumerable<Zadacha> zadachi { get; set; }
        public SearchZadachi()
        {
            this.zadachi = new List<Zadacha>();
        }
        public SearchZadachi(SearchCriteriaZadacha searchCriteriaZadacha, IEnumerable<Zadacha> zadachi)
        {
            this.criteria = searchCriteriaZadacha;
            this.zadachi = zadachi;
        }
    }
}
