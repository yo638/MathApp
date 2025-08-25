using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathApp.Models.DbModels;

namespace MathApp.Models.DbModels
{
    public partial class Answer
    {
        public Answer() { }
        public Answer (string answer, bool validity) {
            this.Name=answer;
            this.Validity = (sbyte)(validity ? 1 : 0);
        }

    }

}
