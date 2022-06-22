using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathApp.Models
{
    public class Answer
    {
        public int idAnswer { get; set; }
        public string answer { get; set; }
        public bool validity { get; set; }
        public Answer() { }
        public Answer(String answer, bool validity){
            this.answer = answer;
            this.validity = validity;
        }

    }

}
