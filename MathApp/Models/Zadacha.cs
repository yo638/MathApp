using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathApp.Models
{
    public class Zadacha
    {
        public string idZadacha { get; set; }
        public string text { get; set; }
        public string solution { get; set; }
        public string creationDate { get; set; }
        public string updateDate { get; set; }
        public string eventDate { get; set; }
        public string user { get; set; }
        public string deletionStatus { get; set; }
    }
}
