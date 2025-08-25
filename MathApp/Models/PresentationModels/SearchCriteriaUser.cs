namespace MathApp.Models.BusinessModels
{
    public class SearchCriteriaUser
    {
        public int idRole { get; set; }
        public string username { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string isDisabled { get; set; }
        public string fromDateCreated { get; set; }
        public string toDateCreated { get; set; }
        public string sortBy { get; set; }
        public SearchCriteriaUser(){}
        public SearchCriteriaUser(string name, string username, string email, string isDisabled, string fromDateCreated, string toDateCreated)
        {
            this.name = name;
            this.username = username;
            this.name = email;
            this.isDisabled = isDisabled;
            this.fromDateCreated = fromDateCreated;
            this.toDateCreated = toDateCreated;
        }
    }
}
