using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MathApp.Models.DbModels
{
    public partial class Zadachi
    {
        public Zadachi()
        {
            this.JunctionZadachiAnswers = new HashSet<JunctionZadachiAnswers>();
            this.JunctionZadachiCategories = new HashSet<JunctionZadachiCategories>();
            this.JunctionZadachiImages = new HashSet<JunctionZadachiImages>();
            this.JunctionZadachiTemi = new HashSet<JunctionZadachiTemi>();
        }

        public int IdZadacha { get; set; }
        public string Uslovie { get; set; }
        public string Solution { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int? User { get; set; }
        public string Status { get; set; }

        public virtual Users UserNavigation { get; set; }
        public virtual ICollection<JunctionZadachiAnswers> JunctionZadachiAnswers { get; set; }
        public virtual ICollection<JunctionZadachiCategories> JunctionZadachiCategories { get; set; }
        public virtual ICollection<JunctionZadachiImages> JunctionZadachiImages { get; set; }
        public virtual ICollection<JunctionZadachiTemi> JunctionZadachiTemi { get; set; }
    }
}
