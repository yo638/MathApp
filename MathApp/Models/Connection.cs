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
            myconnection = new MySqlConnection("server=localhost;user=root;database=math_app;port=3307;password=%s1WnX6*");
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
                    //HttpContext.Session.SetString("AnswerSessionKey", JsonConvert.SerializeObject(answer));
                    answers.Add(answer);
                }
                reader.Close();
            }
            return answers;
        }
        public bool RegisterUser(User u)
        {
            using (myconnection)
            {
                string command = "SELECT * FROM math_app.users WHERE email='" + u.email + "';";
                myconnection.Open();
                mycommand = new MySqlCommand(command, myconnection);
                mycommand.ExecuteNonQuery();
                int count = Convert.ToInt32(mycommand.ExecuteScalar());
                if (count > 0){
                    return false;
                }
                else{
                    u.password = Hashing.toSHA256(u.repeatpassword,Hashing.createSalt());
                    command= "INSERT INTO `math_app`.`users` ( `username`, `email`, `password`) VALUES ('" + u.username + "','" + u.email + "','" + u.password + "');";
                    mycommand = new MySqlCommand(command, myconnection);
                    mycommand.ExecuteNonQuery();
                    return true;
                }
            }
            return false;
        }

        public User getUserByEmail(string email)
        {
            User u = new User();
            using (myconnection)
            {
                string command = "SELECT * FROM math_app.users WHERE email='" + email + "';";
                myconnection.Open();
                mycommand = new MySqlCommand(command, myconnection);
                MySqlDataReader reader = mycommand.ExecuteReader();
                while (reader.Read())
                {
                    u.idUser= reader["id_user"].ToString();
                    u.username = reader["username"].ToString();
                    u.email = reader["email"].ToString();
                    u.password = reader["password"].ToString();
                }
                reader.Close();
            }
            return u;
        }

        public bool logUserIn(User u)
        {
            using (myconnection)
            {
                string realpassword = "";
                string command = "SELECT * FROM math_app.users WHERE email='" + u.email + "';";
                myconnection.Open();
                mycommand = new MySqlCommand(command, myconnection);
                int count = Convert.ToInt32(mycommand.ExecuteScalar());
                if (count > 0)
                {
                    MySqlDataReader reader = mycommand.ExecuteReader();
                    while (reader.Read())
                    {
                        realpassword = reader["password"].ToString();
                    }
                    reader.Close();
                    if (Hashing.comparePasswords(realpassword, u.password)) return true;
                    else return false;
                }
                else return false;
            }
        }



    }
}
