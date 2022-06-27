using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathApp.Models
{
    public class Tema
    {
        public int id { get; set; }
        public string temaName { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public List<Category> categories { get; set; }
        public List<Zadacha> zadachi { get; set; }
        public string creationDate { get; set; }
        public string updateDate { get; set; }
        public string eventDate { get; set; }
        public int user { get; set; }
        public string deletionstatus { get; set; }
        public string timeago { get; set; }
        public Tema()
        {
            categories = new List<Category>();
            zadachi = new List<Zadacha>();
        }
        public Tema(int id, string temaName, string description, string updateDate)
        {
            this.id = id;
            this.temaName = temaName;
            this.description = description;
            this.updateDate = updateDate;
        }

    }
}
