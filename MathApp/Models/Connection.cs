using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace MathApp.Models
{
    public class Connection
    {
        MySqlConnection myconnection;
        MySqlCommand mycommand;
        private void ConnectionTo()
        {
            myconnection = new MySqlConnection("server=localhost;user=root;database=bank;port=3307;password=%s1WnX6*");
        }
        public Connection()
        {
            ConnectionTo();
        }
        public List<Answer> getAnswer()
        {
            List<Answer> answers = new List<Answer>();
            using (myconnection)
            {
                myconnection.Open();
                mycommand= new MySqlCommand("select * from answers", myconnection);
                MySqlDataReader reader = mycommand.ExecuteReader();
                while (reader.Read())
                {
                    //Extract you data
                    Answer answer = new Answer();
                    //answer.SetIdAnswer(Convert.ToInt32(reader["id_answer"]));
                    answer.idAnswer=reader["id_answer"].ToString();
                    answer.answer=reader["answer"].ToString();
                    HttpContext.Session.SetString("AnswerSessionKey", JsonConvert.SerializeObject(answer));
                    answers.Add(answer);
                }
                reader.Close();
            }
            return answers;
        }


    }
}
