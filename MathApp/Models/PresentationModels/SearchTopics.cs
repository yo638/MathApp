using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathApp.Models.DbModels;

namespace MathApp.Models.BusinessModels
{
    public class SearchTopics
    {
        public SearchCriteriaTopic criteria { get; set; }
        public IEnumerable<Topic> topics { get; set; }
        public SearchTopics()
        {
            this.topics = new List<Topic>();
        }
        public SearchTopics(SearchCriteriaTopic searchCriteriaTema, IEnumerable<Topic> temi)
        {
            this.criteria = searchCriteriaTema;
            this.topics = temi;
        }
    }
}
