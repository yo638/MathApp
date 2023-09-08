using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MathApp.Models.DbModels
{
    public partial class JunctionTemiCategories
    {
        public int Tema { get; set; }
        public int Category { get; set; }

        public virtual Categories CategoryNavigation { get; set; }
        public virtual Temi TemaNavigation { get; set; }
    }
}
