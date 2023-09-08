using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MathApp.Models.DbModels
{
    public partial class JunctionZadachiImages
    {
        public int Zadacha { get; set; }
        public int Image { get; set; }
        public int Place { get; set; }

        public virtual Images ImageNavigation { get; set; }
        public virtual Zadachi ZadachaNavigation { get; set; }
    }
}
