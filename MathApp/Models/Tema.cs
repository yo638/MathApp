using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathApp.Models
{
    public class Tema
    {
        public string idTema { get; set; }
        public string temaName { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string creationDate { get; set; }
        public string updateDate { get; set; }
        public string eventDate { get; set; }
        public string user { get; set; }
        public string deletionstatus { get; set; }

    }
}
