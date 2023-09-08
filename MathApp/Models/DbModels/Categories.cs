using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MathApp.Models.DbModels
{
    public partial class Categories
    {
        public Categories()
        {
            this.JunctionTemiCategories = new HashSet<JunctionTemiCategories>();
            this.JunctionZadachiCategories = new HashSet<JunctionZadachiCategories>();
        }

        public int IdCategory { get; set; }
        public int? Grade { get; set; }
        public string Difficulty { get; set; }

        public virtual ICollection<JunctionTemiCategories> JunctionTemiCategories { get; set; }
        public virtual ICollection<JunctionZadachiCategories> JunctionZadachiCategories { get; set; }
    }
}
