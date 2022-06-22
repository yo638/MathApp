using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
//using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MathApp.Models
{
    public class Zadacha
    {
        public int idZadacha { get; set; }
        public string uslovie { get; set; }
        [BindProperty]
        public List<Answer> answers { get; set; }
        public string solution { get; set; }
        //public IFormFile[] ImagesSolution { get; set; }
        public List<Category> categories { get; set; }
        public string creationDate { get; set; }
        public string updateDate { get; set; }
        public int user { get; set; }
        public string deletionStatus { get; set; }
        public string timeago { get; set; }

        public Zadacha()
        {
            this.answers=new List<Answer>();
            this.categories = new List<Category>();
        }
        public Zadacha(int idZadacha, string uslovie, string updateDate)
        {
            this.idZadacha = idZadacha;
            this.uslovie = uslovie;
            this.updateDate = updateDate;
        }
    }
}
