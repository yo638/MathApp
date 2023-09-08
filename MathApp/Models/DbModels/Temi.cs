using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MathApp.Models.DbModels
{
    public partial class Temi
    {
        public Temi()
        {
            this.JunctionTemiCategories = new HashSet<JunctionTemiCategories>();
            this.JunctionZadachiTemi = new HashSet<JunctionZadachiTemi>();
        }

        public int IdTema { get; set; }
        public string Tema { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime? EventDate { get; set; }
        public int? User { get; set; }
        public string Deletionstatus { get; set; }

        public virtual Users UserNavigation { get; set; }
        public virtual ICollection<JunctionTemiCategories> JunctionTemiCategories { get; set; }
        public virtual ICollection<JunctionZadachiTemi> JunctionZadachiTemi { get; set; }
    }
}
