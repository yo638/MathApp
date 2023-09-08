using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MathApp.Models.DbModels
{
    public partial class JunctionZadachiAnswers
    {
        public int Zadacha { get; set; }
        public int Answer { get; set; }

        public virtual Answers AnswerNavigation { get; set; }
        public virtual Zadachi ZadachaNavigation { get; set; }
    }
}
