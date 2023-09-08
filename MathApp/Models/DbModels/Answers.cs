using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MathApp.Models.DbModels
{
    public partial class Answers
    {
        public Answers()
        {
            this.JunctionZadachiAnswers = new HashSet<JunctionZadachiAnswers>();
        }

        public int IdAnswer { get; set; }
        public string Answer { get; set; }
        public byte Validity { get; set; }

        public virtual ICollection<JunctionZadachiAnswers> JunctionZadachiAnswers { get; set; }
    }
}
