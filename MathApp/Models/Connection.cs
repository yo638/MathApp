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
                string command = "SELECT * FROM users where email='" + u.email + "'";
                myconnection.Open();
                mycommand = new MySqlCommand(command, myconnection);
                mycommand.ExecuteNonQuery();
                int count = Convert.ToInt32(mycommand.ExecuteScalar());
                if (count > 0){
                    //Display an error however that's supposed to happen "This email has already been registered.");
                }
                else{
                    u.password = Hashing.toSHA256(u.password,Hashing.createSalt());
                    //command="Insert into users([first_name], [last_name], [email], [password]) values ('" + u.first_name + "'" + ",'" + u.last_name + "'" + ",'" + u.email + "'" + ",'" + u.password + "')";
                    mycommand = new MySqlCommand(command, myconnection);
                    mycommand.ExecuteNonQuery();
                    //Display a message again "You have successfully registered and logged in.");
                    return true;
                }
            }
            return false;
        }

        public string getUserID(User u)
        {
            using (myconnection)
            {
                u.idUser = "";
                string command = "SELECT id_user FROM users where email='" + u.email  + "'";
                myconnection.Open();
                mycommand = new MySqlCommand(command, myconnection);
                MySqlDataReader reader = mycommand.ExecuteReader();
                while (reader.Read())
                {
                    u.idUser= (reader.GetInt32(reader.GetOrdinal("id_user"))).ToString();
                }
                reader.Close();
            }
            return u.idUser;
        }

        public bool logUserIn(User u)
        {
            using (myconnection)
            {
                string realpassword = "";
                string command = "SELECT password FROM users where email='" + u.email + "'";
                myconnection.Open();
                mycommand = new MySqlCommand(command, myconnection);
                MySqlDataReader reader = mycommand.ExecuteReader();
                while (reader.Read())
                {
                    realpassword = (reader.GetInt32(reader.GetOrdinal("password"))).ToString();
                }
                reader.Close();
                //if the real password exists
                if (realpassword != "")
                {
                    if (Hashing.comparePasswords(realpassword, u.password)) return true;//OR DISPLAY A MESSAGE
                    else return false;//OR DISPLAY A MESSAGE THE EMAIL DOES NOT EXIST HERE
                }
                return false; // OR DISPLAY MESSAGE SOMETHING WENT WRONG
            }
        }



    }
}
