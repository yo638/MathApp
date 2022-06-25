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
            try
            {
                using (myconnection)
                {
                    myconnection.Open();
                    mycommand = new MySqlCommand("select * from answers", myconnection);
                    MySqlDataReader reader = mycommand.ExecuteReader();
                    while (reader.Read())
                    {
                        //Extract you data
                        Answer answer = new Answer();
                        //answer.SetIdAnswer(Convert.ToInt32(reader["id_answer"]));
                        answer.idAnswer = Convert.ToInt32(reader["id_answer"].ToString());
                        answer.answer = reader["answer"].ToString();
                        //HttpContext.Session.SetString("AnswerSessionKey", JsonConvert.SerializeObject(answer));
                        answers.Add(answer);
                    }
                    reader.Close();
                }
                return answers;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return answers;
            }
        }
        public bool RegisterUser(User u)
        {
            try
            {
                using (myconnection)
                {
                    string command = "SELECT * FROM math_app.users WHERE email='" + u.email + "';";
                    myconnection.Open();
                    mycommand = new MySqlCommand(command, myconnection);
                    mycommand.ExecuteNonQuery();
                    int count = Convert.ToInt32(mycommand.ExecuteScalar());
                    if (count > 0)
                    {
                        return false;
                    }
                    else
                    {
                        u.password = Hashing.toSHA256(u.repeatpassword, Hashing.createSalt());
                        command = "INSERT INTO `math_app`.`users` ( `username`, `email`, `password`) VALUES ('" + u.username + "','" + u.email + "','" + u.password + "');";
                        mycommand = new MySqlCommand(command, myconnection);
                        mycommand.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public User getUserByEmail(string email)
        {
            User u = new User();
            try
            {
                using (myconnection)
                {
                    string command = "SELECT * FROM math_app.users WHERE email='" + email + "';";
                    myconnection.Open();
                    mycommand = new MySqlCommand(command, myconnection);
                    MySqlDataReader reader = mycommand.ExecuteReader();
                    while (reader.Read())
                    {
                        u.idUser = Convert.ToInt32(reader["id_user"].ToString());
                        u.username = reader["username"].ToString();
                        u.email = reader["email"].ToString();
                        u.password = reader["password"].ToString();
                    }
                    reader.Close();
                }

                return u;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return u;
            }
        }

        public bool logUserIn(User u)
        {
            try
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
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        //ZADACHI
        public bool createZadacha(Zadacha zadacha)
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
                    MySqlDataReader readerZ = mycommand.ExecuteReader();
                    while (readerZ.Read())
                    {
                        zadacha.idZadacha = (Convert.ToInt32(readerZ["id_zadacha"].ToString()));
                    }
                    readerZ.Close();

                    if (zadacha.answers.Count > 0)
                    {
                        for (int i = 0; i < zadacha.answers.Count; i++)
                        {
                            command = "INSERT IGNORE INTO `math_app`.`answers` ( `answer`, `validity`) VALUES ('" + zadacha.answers[i].answer + "','" + Convert.ToInt32(zadacha.answers[i].validity) + "');";
                            mycommand = new MySqlCommand(command, myconnection);
                            mycommand.ExecuteNonQuery();
                            command = "SELECT `id_answer` FROM `math_app`.`answers` WHERE `answer`='" + zadacha.answers[i].answer + "' AND `validity`='" + Convert.ToInt32(zadacha.answers[i].validity) + "';";
                            myconnection.Open();
                            mycommand = new MySqlCommand(command, myconnection);
                            MySqlDataReader readerA = mycommand.ExecuteReader();
                            while (readerA.Read())
                            {
                                answerID = Convert.ToInt32(readerA["id_answer"].ToString());
                            }
                            readerA.Close();
                            command = "INSERT IGNORE INTO `math_app`.`junction_zadachi_answers` ( `zadacha`, `answer`) VALUES ('" + zadacha.idZadacha + "','" + answerID + "');";
                            mycommand = new MySqlCommand(command, myconnection);
                            mycommand.ExecuteNonQuery();
                        }
                    }

                    if (zadacha.categories.Count > 0)
                    {
                        for (int i = 0; i < zadacha.categories.Count; i++)
                        {
                            command = "SELECT `id_category` FROM `math_app`.`categories` WHERE `grade`='" + zadacha.categories[i].grade + "' AND `difficulty`='" + zadacha.categories[i].difficulty + "';";
                            myconnection.Open();
                            mycommand = new MySqlCommand(command, myconnection);
                            MySqlDataReader readerC = mycommand.ExecuteReader();
                            while (readerC.Read())
                            {
                                categoryID = Convert.ToInt32(readerC["id_category"].ToString());
                            }
                            readerC.Close();

                            command = "INSERT IGNORE INTO `math_app`.`junction_zadachi_categories` ( `zadacha`, `category`) VALUES ('" + zadacha.idZadacha + "','" + zadacha.categories[i].idCategory + "');";
                            mycommand = new MySqlCommand(command, myconnection);
                            mycommand.ExecuteNonQuery();
                        }
                    }
                    return true;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }



        public IEnumerable<Zadacha> getZadachiByUser(int userID)
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
                    int time = 0;
                    DateTime nowdate=DateTime.Now;
                    for (int i = 0; i < zadachi.Count; i++)
                    {
                        DateTime update = DateTime.Parse(zadachi[i].updateDate);
                        time=(nowdate.Date - update.Date).Days;

                        if(time==0) zadachi[i].timeago = "today";
                        else if(time==1) zadachi[i].timeago = "yesterday";
                        else if (time < 7) zadachi[i].timeago = time.ToString() + " days ago";
                        else if (time < 30) { time = time / 7; zadachi[i].timeago = time.ToString() + " weeks ago"; }
                        else if (time < 365) { time = time / 30; zadachi[i].timeago = time.ToString() + " months ago"; }
                        else { time = time / 365; zadachi[i].timeago = time.ToString() + " years ago"; }

                    }
                    return zadachi;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return zadachi;
            }
        }
        public IEnumerable<Zadacha> getRecycledZadachiByUser(int userID)
        {
            List<Zadacha> zadachi = new List<Zadacha>();
            try
            {
                using (myconnection)
                {
                    string command = "SELECT `id_zadacha`,`uslovie`,`update_date` FROM `math_app`.`zadachi` WHERE `user`='" + userID + "' AND `status`='recyclebin' ORDER BY `update_date` DESC;";
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
                    int time = 0;
                    DateTime nowdate = DateTime.Now;
                    for (int i = 0; i < zadachi.Count; i++)
                    {
                        DateTime update = DateTime.Parse(zadachi[i].updateDate);
                        time = (nowdate.Date - update.Date).Days;

                        if (time == 0) zadachi[i].timeago = "today";
                        else if (time == 1) zadachi[i].timeago = "yesterday";
                        else if (time < 7) zadachi[i].timeago = time.ToString() + " days ago";
                        else if (time < 30) { time = time / 7; zadachi[i].timeago = time.ToString() + " weeks ago"; }
                        else if (time < 365) { time = time / 30; zadachi[i].timeago = time.ToString() + " months ago"; }
                        else { time = time / 365; zadachi[i].timeago = time.ToString() + " years ago"; }

                    }
                    return zadachi;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return zadachi;
            }
        }

        public Zadacha getZadachaByID(int zadachaID)
        {
            Zadacha zadacha = new Zadacha();
            zadacha.idZadacha = zadachaID;
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
                        zadacha.user = Convert.ToInt32(readerZadacha["user"].ToString());
                    }
                    readerZadacha.Close();

                    command = "SELECT a.answer, a.validity FROM math_app.answers a JOIN math_app.junction_zadachi_answers jza ON a.id_answer=jza.answer JOIN math_app.zadachi z ON z.id_zadacha=jza.zadacha WHERE z.id_zadacha='"+zadachaID+"';";
                    mycommand = new MySqlCommand(command, myconnection);
                    MySqlDataReader readerAnswr = mycommand.ExecuteReader();
                    while (readerAnswr.Read())
                    {
                        zadacha.answers.Add(new Answer(readerAnswr["answer"].ToString(), Convert.ToBoolean(Convert.ToInt32(readerAnswr["validity"].ToString()))));
                    }
                    readerAnswr.Close();

                    command = "SELECT c.id_category, c.grade, c.difficulty FROM math_app.categories c JOIN math_app.`junction_zadachi_categories` jzc ON c.id_category=jzc.category JOIN math_app.zadachi z ON z.id_zadacha=jzc.zadacha WHERE z.id_zadacha='" + zadachaID+"';";
                    mycommand = new MySqlCommand(command, myconnection);
                    MySqlDataReader readerCatg = mycommand.ExecuteReader();
                    while (readerCatg.Read())
                    {
                        zadacha.categories.Add(new Category(Convert.ToInt32(readerCatg["id_category"].ToString()), Convert.ToInt32(readerCatg["grade"].ToString()), (readerCatg["difficulty"].ToString())));
                    }
                    readerCatg.Close();

                    return zadacha;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return zadacha;
            }
        }

        public bool updateZadacha(Zadacha zadacha)
        {
            try
            {
                using (myconnection)
                {
                    int answerID = 0, categoryID=0;
                    string command = "UPDATE `math_app`.`zadachi` SET `uslovie`='"+zadacha.uslovie+"', `solution`='"+zadacha.solution+"', `update_date`='"+zadacha.updateDate+"' WHERE `id_zadacha`='"+zadacha.idZadacha+"';";
                    myconnection.Open();
                    mycommand = new MySqlCommand(command, myconnection);
                    mycommand.ExecuteNonQuery();

                    command = "DELETE FROM `math_app`.`junction_zadachi_answers` WHERE `zadacha`='"+zadacha.idZadacha+"';";
                    mycommand = new MySqlCommand(command, myconnection);
                    mycommand.ExecuteNonQuery();

                    if (zadacha.answers.Count > 0)
                    {
                        for (int i = 0; i < zadacha.answers.Count; i++)
                        {
                            command = "INSERT IGNORE INTO `math_app`.`answers` (`answer`, `validity`) VALUES ('" + zadacha.answers[i].answer + "','" + Convert.ToInt32(zadacha.answers[i].validity) + "');";
                            mycommand = new MySqlCommand(command, myconnection);
                            mycommand.ExecuteNonQuery();
                            command = "SELECT `id_answer` FROM `math_app`.`answers` WHERE `answer`='" + zadacha.answers[i].answer + "' AND `validity`='" + Convert.ToInt32(zadacha.answers[i].validity) + "';";
                            mycommand = new MySqlCommand(command, myconnection);
                            MySqlDataReader readerIDAnsr = mycommand.ExecuteReader();
                            while (readerIDAnsr.Read())
                            {
                                answerID = Convert.ToInt32(readerIDAnsr["id_answer"].ToString());
                            }
                            readerIDAnsr.Close();
                            command = "INSERT IGNORE INTO `math_app`.`junction_zadachi_answers` (`zadacha`, `answer`) VALUES ('" + zadacha.idZadacha + "','" + answerID + "');";
                            mycommand = new MySqlCommand(command, myconnection);
                            mycommand.ExecuteNonQuery();
                        }
                    }

                    command = "DELETE FROM `math_app`.`junction_zadachi_categories` WHERE `zadacha`='" + zadacha.idZadacha + "';";
                    mycommand = new MySqlCommand(command, myconnection);
                    mycommand.ExecuteNonQuery();
                    if (zadacha.categories.Count > 0)
                    {
                        for (int i = 0; i < zadacha.categories.Count; i++)
                        {
                            command = "SELECT `id_category` FROM `math_app`.`categories` WHERE `grade`='" + zadacha.categories[i].grade + "' AND `difficulty`='" + zadacha.categories[i].difficulty + "';";
                            mycommand = new MySqlCommand(command, myconnection);
                            MySqlDataReader readerIDCatgr = mycommand.ExecuteReader();
                            while (readerIDCatgr.Read())
                            {
                                categoryID = Convert.ToInt32(readerIDCatgr["id_category"].ToString());
                            }
                            readerIDCatgr.Close();

                            command = "INSERT IGNORE INTO `math_app`.`junction_zadachi_categories` (`zadacha`, `category`) VALUES ('" + zadacha.idZadacha + "','" + categoryID + "');";
                            mycommand = new MySqlCommand(command, myconnection);
                            mycommand.ExecuteNonQuery();
                        }
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public bool addZadachaToRecycleBin(int zadachaID)
        {
            try
            {
                using (myconnection)
                {
                    string command = "UPDATE `math_app`.`zadachi` SET `status`='recyclebin' WHERE `id_zadacha`='" + zadachaID + "';";
                    myconnection.Open();
                    mycommand = new MySqlCommand(command, myconnection);
                    mycommand.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public bool recoverZadachaFromRecycleBin(int zadachaID)
        {
            try
            {
                using (myconnection)
                {
                    string command = "UPDATE `math_app`.`zadachi` SET `status`='saved' WHERE `id_zadacha`='" + zadachaID + "';";
                    myconnection.Open();
                    mycommand = new MySqlCommand(command, myconnection);
                    mycommand.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public bool deleteZadacha(int zadachaID)
        {
            try
            {
                using (myconnection)
                {
                    string command = "DELETE FROM `math_app`.`zadachi` WHERE `id_zadacha`='" + zadachaID + "';";
                    myconnection.Open();
                    mycommand = new MySqlCommand(command, myconnection);
                    mycommand.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public IEnumerable<Zadacha> searchZadachi(int userID, SearchCriteriaZadacha criteria)
        {
            List<Zadacha> zadachi = new List<Zadacha>();
            try
            {
                using (myconnection)
                {
                    string command = "SELECT DISTINCT z.id_zadacha, z.uslovie, z.update_date FROM math_app.zadachi z "+
                        "LEFT JOIN math_app.junction_zadachi_answers jza ON z.id_zadacha = jza.zadacha "+
                        "LEFT JOIN math_app.answers a ON jza.answer = a.id_answer "+
                        "LEFT JOIN math_app.junction_zadachi_categories jzc ON z.id_zadacha = jzc.zadacha "+
                        "LEFT JOIN math_app.categories c ON jzc.category = c.id_category "+
                        "WHERE z.`user`= '"+userID+"' AND z.`status`= 'saved'";
                    if (!string.IsNullOrEmpty(criteria.uslovie)) command += " AND z.`uslovie` LIKE '%"+criteria.uslovie+"%'";
                    if (!string.IsNullOrEmpty(criteria.solution)) command += " AND z.`solution` LIKE '%"+criteria.solution+"%'";
                    if (!string.IsNullOrEmpty(criteria.answer)) command += " AND a.`answer` LIKE '%"+criteria.answer+"%'";
                    if (!string.IsNullOrEmpty(criteria.anywhere)) command += " AND (z.`uslovie` LIKE '%"+criteria.anywhere+"%' OR z.`solution` LIKE '%"+criteria.anywhere+"%' OR a.`answer` LIKE '%"+criteria.anywhere+"%')";
                    if (!string.IsNullOrEmpty(criteria.fromDate)) command += " AND z.creation_date>='" + criteria.fromDate + "'";
                    if (!string.IsNullOrEmpty(criteria.toDate)) command += " AND z.creation_date<='" + criteria.toDate + " 23:59:59'";
                    if (criteria.category.grade!=0) command += " AND c.grade='"+ criteria.category.grade + "'";
                    if (criteria.category.difficulty!="X") command += " AND c.difficulty='"+criteria.category.difficulty+"'";
                    command += ";";

                    command += " ORDER BY `update_date` DESC;";
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
                    int time = 0;
                    DateTime nowdate = DateTime.Now;
                    for (int i = 0; i < zadachi.Count; i++)
                    {
                        DateTime update = DateTime.Parse(zadachi[i].updateDate);
                        time = (nowdate.Date - update.Date).Days;

                        if (time == 0) zadachi[i].timeago = "today";
                        else if (time == 1) zadachi[i].timeago = "yesterday";
                        else if (time < 7) zadachi[i].timeago = time.ToString() + " days ago";
                        else if (time < 30) { time = time / 7; zadachi[i].timeago = time.ToString() + " weeks ago"; }
                        else if (time < 365) { time = time / 30; zadachi[i].timeago = time.ToString() + " months ago"; }
                        else { time = time / 365; zadachi[i].timeago = time.ToString() + " years ago"; }

                    }
                    return zadachi;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return zadachi;
            }
        }
    }
}
