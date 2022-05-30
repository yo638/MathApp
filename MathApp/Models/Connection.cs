using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;

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
        //ZADACHI
        public bool CreateZadacha(Zadacha zadacha)
        {
            try
            {
                using (myconnection)
                {
                    int answerID = 0, categoryID = 0;
                    string command = "INSERT INTO `math_app`.`zadachi` ( `uslovie`, `solution`, `creation_date`, `update_date`, `user`, `status`) VALUES ('" + zadacha.uslovie + "','" + zadacha.solution + "','" + zadacha.creationDate + "','" + zadacha.updateDate + "','" + zadacha.user + "','" + zadacha.deletionStatus + "');";
                    myconnection.Open();
                    mycommand = new MySqlCommand(command, myconnection);
                    mycommand.ExecuteNonQuery();

                    command = "SELECT * FROM math_app.zadachi WHERE uslovie='" + zadacha.uslovie + "' AND creation_date='" + zadacha.creationDate + "';";
                    mycommand = new MySqlCommand(command, myconnection);
                    MySqlDataReader readerIDZadacha = mycommand.ExecuteReader();
                    while (readerIDZadacha.Read())
                    {
                        zadacha.idZadacha = (Convert.ToInt32(readerIDZadacha["id_zadacha"].ToString()));
                    }
                    readerIDZadacha.Close();

                    if (zadacha.answers.Count > 0)
                    {
                        for (int i = 0; i < zadacha.answers.Count; i++)
                        {
                            command = "INSERT INTO `math_app`.`answers` ( `answer`, `validity`) VALUES ('" + zadacha.answers[i].answer + "','" + Convert.ToInt32(zadacha.answers[i].validity) + "');";
                            mycommand = new MySqlCommand(command, myconnection);
                            mycommand.ExecuteNonQuery();
                            command = "SELECT * FROM `math_app`.`answers` WHERE `answer`='" + zadacha.answers[i].answer + "' AND `validity`='" + Convert.ToInt32(zadacha.answers[i].validity) + "';";
                            mycommand = new MySqlCommand(command, myconnection);
                            MySqlDataReader readerIDAnswer = mycommand.ExecuteReader();
                            while (readerIDAnswer.Read())
                            {
                                answerID = Convert.ToInt32(readerIDAnswer["id_answer"].ToString());
                            }
                            readerIDAnswer.Close();
                            command = "INSERT INTO `math_app`.`junction_zadachi_answers` ( `zadacha`, `answer`) VALUES ('" + zadacha.idZadacha + "','" + answerID + "');";
                            mycommand = new MySqlCommand(command, myconnection);
                            mycommand.ExecuteNonQuery();
                        }
                    }

                    if (zadacha.categories.Count > 0)
                    {
                        for (int i = 0; i < zadacha.categories.Count; i++)
                        {
                            command = "SELECT * FROM math_app.categories WHERE grade='" + zadacha.categories[i].grade + "' AND difficulty='" + zadacha.categories[i].difficulty + "';";
                            mycommand = new MySqlCommand(command, myconnection);
                            MySqlDataReader readerIDCategory = mycommand.ExecuteReader();
                            while (readerIDCategory.Read())
                            {
                                categoryID = Convert.ToInt32(readerIDCategory["id_category"].ToString());
                            }
                            readerIDCategory.Close();
                            command = "INSERT INTO `math_app`.`junction_zadachi_categories` ( `zadacha`, `category`) VALUES ('" + zadacha.idZadacha + "','" + categoryID + "');";
                            mycommand = new MySqlCommand(command, myconnection);
                            mycommand.ExecuteNonQuery();
                        }
                    }
                    return true;
                }
            }
            catch(Exception)
            {
                return false;
            }
        }

        public IEnumerable<Zadacha> getZadachiByUser(string userID)
        {
            List<Zadacha> zadachi = new List<Zadacha>();
            try
            {
                using (myconnection)
                {
                    string command = "SELECT `id_zadacha`,`uslovie`,`update_date` FROM `math_app`.`zadachi` WHERE `user`='" + userID + "' AND `status`='saved' ORDER BY `update_date` DESC;";
                    myconnection.Open();
                    mycommand = new MySqlCommand(command, myconnection);
                    MySqlDataReader reader = mycommand.ExecuteReader();
                    while (reader.Read())
                    {
                        zadachi.Add(new Zadacha(
                            Convert.ToInt32(reader["id_zadacha"].ToString()),
                            reader["uslovie"].ToString(),
                            reader["update_date"].ToString()));
                    }
                    reader.Close();
                    return zadachi;
                }
            }
            catch (Exception)
            {
                return zadachi;
            }
        }

        public Zadacha getZadachaByID(int zadachaID)
        {
            Zadacha zadacha = new Zadacha();
            try
            {
                using (myconnection)
                {
                    string command = "SELECT * FROM `math_app`.`zadachi` WHERE `id_zadacha`='" + zadachaID + "';";
                    myconnection.Open();
                    mycommand = new MySqlCommand(command, myconnection);
                    MySqlDataReader readerZadacha = mycommand.ExecuteReader();
                    while (readerZadacha.Read())
                    {
                        zadacha.idZadacha = Convert.ToInt32(readerZadacha["id_zadacha"].ToString());
                        zadacha.uslovie = readerZadacha["uslovie"].ToString();
                        zadacha.solution = readerZadacha["solution"].ToString();
                        zadacha.creationDate = readerZadacha["creation_date"].ToString();
                        zadacha.updateDate = readerZadacha["update_date"].ToString();
                    }
                    readerZadacha.Close();

                    command = "SELECT a.answer, a.validity FROM math_app.answers a JOIN math_app.junction_zadachi_answers jza ON a.id_answer=jza.answer JOIN math_app.zadachi z ON z.id_zadacha=jza.zadacha WHERE z.id_zadacha='"+zadachaID+"';";
                    mycommand = new MySqlCommand(command, myconnection);
                    MySqlDataReader readerAnswr = mycommand.ExecuteReader();
                    while (readerAnswr.Read())
                    {
                        zadacha.answers.Add(new Answer(readerAnswr["answer"].ToString(), Convert.ToBoolean(readerAnswr["validity"].ToString())));
                    }
                    readerAnswr.Close();

                    command = "SELECT c.grade, c.difficulty FROM math_app.categories c JOIN math_app.`junction_zadachi_categories` jzc ON c.id_category=jzc.category JOIN math_app.zadachi z ON z.id_zadacha=jzc.zadacha WHERE z.id_zadacha='"+zadachaID+"';";
                    mycommand = new MySqlCommand(command, myconnection);
                    MySqlDataReader readerCatg = mycommand.ExecuteReader();
                    while (readerCatg.Read())
                    {
                        zadacha.categories.Add(new Category(Convert.ToInt32(readerCatg["grade"].ToString()), (readerCatg["difficulty"].ToString()));
                    }
                    readerCatg.Close();

                    return zadacha;
                }
            }
            catch (Exception)
            {
                return zadacha;
            }
        }



    }
}
