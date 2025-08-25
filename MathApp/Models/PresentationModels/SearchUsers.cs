using MathApp.Models.DbModels;
using System.Collections.Generic;

namespace MathApp.Models.BusinessModels
{
    public class SearchUsers
    {
        public SearchCriteriaUser criteria { get; set; }
        public IEnumerable<User> users { get; set; }
        public SearchUsers()
        {
            this.users = new List<User>();
        }
        public SearchUsers(SearchCriteriaUser criteria, IEnumerable<User> users)
        {
            this.criteria = criteria;
            this.users = users;
        }
    }
}
