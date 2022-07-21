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
       
        public bool RegisterUser(User u)
        {
            try
            {
                using (myconnection)
                {
                    string command = "SELECT * FROM math_app.users WHERE email='" + u.email + "';";
                    if (myconnection.State.ToString() != "Open") myconnection.Open();
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

        public User GetUserByEmail(string email)
        {
            User u = new User();
            try
            {
                using (myconnection)
                {
                    string command = "SELECT * FROM math_app.users WHERE email='" + email + "';";
                    if (myconnection.State.ToString() != "Open") myconnection.Open();
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

        public bool LogUserIn(User u)
        {
            try
            {
                using (myconnection)
                {
                    string realpassword = "";
                    string command = "SELECT * FROM math_app.users WHERE email='" + u.email + "';";
                    if (myconnection.State.ToString() != "Open") myconnection.Open();
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

        public bool CreateZadacha(Zadacha zadacha)
        {
            try
            {
                using (myconnection)
                {
                    int answerID = 0, categoryID = 0;
                    string command = "INSERT IGNORE INTO `math_app`.`zadachi` ( `uslovie`, `solution`, `creation_date`, `update_date`, `user`, `status`) VALUES ('" + zadacha.uslovie + "','" + zadacha.solution + "','" + zadacha.creationDate + "','" + zadacha.updateDate + "','" + zadacha.user + "','saved');";
                    if (myconnection.State.ToString() != "Open") myconnection.Open();
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
                            mycommand = new MySqlCommand(command, myconnection);
                            MySqlDataReader readerA = mycommand.ExecuteReader();
                            while (readerA.Read())
                            {
                                zadacha.answers[i].idAnswer = Convert.ToInt32(readerA["id_answer"].ToString());
                            }
                            readerA.Close();
                            command = "INSERT IGNORE INTO `math_app`.`junction_zadachi_answers` ( `zadacha`, `answer`) VALUES ('" + zadacha.idZadacha + "','" + zadacha.answers[i].idAnswer + "');";
                            mycommand = new MySqlCommand(command, myconnection);
                            mycommand.ExecuteNonQuery();
                        }
                    }

                    if (zadacha.categories.Count > 0)
                    {
                        for (int i = 0; i < zadacha.categories.Count; i++)
                        {
                            command = "SELECT `id_category` FROM `math_app`.`categories` WHERE `grade`='" + zadacha.categories[i].grade + "' AND `difficulty`='" + zadacha.categories[i].difficulty + "';";
                            mycommand = new MySqlCommand(command, myconnection);
                            MySqlDataReader readerC = mycommand.ExecuteReader();
                            while (readerC.Read())
                            {
                                zadacha.categories[i].idCategory = Convert.ToInt32(readerC["id_category"].ToString());
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



        public IEnumerable<Zadacha> GetZadachiByUser(int userID)
        {
            List<Zadacha> zadachi = new List<Zadacha>();
            try
            {
                using (myconnection)
                {
                    string command = "SELECT `id_zadacha`,`uslovie`,`update_date` FROM `math_app`.`zadachi` WHERE `user`='" + userID + "' AND `status`='saved' ORDER BY `update_date` DESC;";
                    if (myconnection.State.ToString() != "Open") myconnection.Open();
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
                    return CalculateTimeTillLastUpdateOfZadachi(zadachi);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return zadachi;
            }
        }
        public IEnumerable<Zadacha> GetRecycledZadachiByUser(int userID)
        {
            List<Zadacha> zadachi = new List<Zadacha>();
            try
            {
                using (myconnection)
                {
                    string command = "SELECT `id_zadacha`,`uslovie`,`update_date` FROM `math_app`.`zadachi` WHERE `user`='" + userID + "' AND `status`='recyclebin' ORDER BY `update_date` DESC;";
                    if (myconnection.State.ToString() != "Open") myconnection.Open();
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
                    return CalculateTimeTillLastUpdateOfZadachi(zadachi);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return zadachi;
            }
        }

        public Zadacha GetZadachaByID(int zadachaID)
        {
            Zadacha zadacha = new Zadacha();
            try
            {
                using (myconnection)
                {
                    string command = "SELECT * FROM `math_app`.`zadachi` WHERE `id_zadacha`='" + zadachaID + "';";
                    if (myconnection.State.ToString() != "Open") myconnection.Open();
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

                    zadacha.answers=GetAnswersOfZadacha(zadachaID);
                    zadacha.categories=GetCategoriesOfZadacha(zadachaID);

                    return zadacha;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return zadacha;
            }
        }

        private List<Category> GetCategoriesOfZadacha(int zadachaID)
        {
            List<Category> categories = new List<Category>();
            string command = "SELECT c.id_category, c.grade, c.difficulty FROM math_app.categories c JOIN math_app.`junction_zadachi_categories` jzc ON c.id_category=jzc.category JOIN math_app.zadachi z ON z.id_zadacha=jzc.zadacha WHERE z.id_zadacha='" + zadachaID + "';";
            mycommand = new MySqlCommand(command, myconnection);
            MySqlDataReader readerCategories = mycommand.ExecuteReader();
            while (readerCategories.Read())
            {
                categories.Add(new Category(Convert.ToInt32(readerCategories["id_category"].ToString()),
                    Convert.ToInt32(readerCategories["grade"].ToString()), (readerCategories["difficulty"].ToString())));
            }
            readerCategories.Close();
            return categories;
        }

        private List<Answer> GetAnswersOfZadacha(int zadachaID)
        {
            List<Answer> answers = new List<Answer>();
            string command = "SELECT a.answer, a.validity FROM math_app.answers a JOIN math_app.junction_zadachi_answers jza ON a.id_answer=jza.answer JOIN math_app.zadachi z ON z.id_zadacha=jza.zadacha WHERE z.id_zadacha='" + zadachaID + "';";
            mycommand = new MySqlCommand(command, myconnection);
            MySqlDataReader readerAnswer = mycommand.ExecuteReader();
            while (readerAnswer.Read())
            {
                answers.Add(new Answer(readerAnswer["answer"].ToString(),
                    Convert.ToBoolean(Convert.ToInt32(readerAnswer["validity"].ToString()))));
            }
            readerAnswer.Close();
            return answers;
        }

        public bool UpdateZadacha(Zadacha zadacha)
        {
            try
            {
                using (myconnection)
                {
                    int answerID = 0, categoryID=0;
                    string command = "UPDATE `math_app`.`zadachi` SET `uslovie`='"+zadacha.uslovie+"', `solution`='"+zadacha.solution+"', `update_date`='"+zadacha.updateDate+"' WHERE `id_zadacha`='"+zadacha.idZadacha+"';";
                    if (myconnection.State.ToString() != "Open") myconnection.Open();
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
        public bool AddZadachaToRecycleBin(int zadachaID)
        {
            try
            {
                using (myconnection)
                {
                    string command = "UPDATE `math_app`.`zadachi` SET `status`='recyclebin' WHERE `id_zadacha`='" + zadachaID + "';";
                    if (myconnection.State.ToString() != "Open") myconnection.Open();
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
        public bool RecoverZadachaFromRecycleBin(int zadachaID)
        {
            try
            {
                using (myconnection)
                {
                    string command = "UPDATE `math_app`.`zadachi` SET `status`='saved' WHERE `id_zadacha`='" + zadachaID + "';";
                    if (myconnection.State.ToString() != "Open") myconnection.Open();
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
        public bool DeleteZadacha(int zadachaID)
        {
            try
            {
                using (myconnection)
                {
                    string command = "DELETE FROM `math_app`.`zadachi` WHERE `id_zadacha`='" + zadachaID + "';";
                    if (myconnection.State.ToString() != "Open") myconnection.Open();
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
        private IEnumerable<Zadacha> CalculateTimeTillLastUpdateOfZadachi(List<Zadacha> zadachi)
        {
            int timespan = 0;
            DateTime todayDate = DateTime.Now;
            DateTime updateDate;
            for (int i = 0; i < zadachi.Count; i++)
            {
                updateDate = DateTime.Parse(zadachi[i].updateDate);
                timespan = (todayDate.Date - updateDate.Date).Days;

                if (timespan == 0) 
                    zadachi[i].timeago = "today";

                else if (timespan == 1) 
                    zadachi[i].timeago = "yesterday";

                else if (timespan < 7) 
                    zadachi[i].timeago = timespan.ToString() + " days ago";

                else if (timespan < 30)
                    zadachi[i].timeago = (timespan / 7).ToString() + " weeks ago";
                
                else if (timespan < 365) 
                    zadachi[i].timeago = (timespan / 30).ToString() + " months ago";
                
                else 
                    zadachi[i].timeago = (timespan / 365).ToString() + " years ago";
            }
            return zadachi;
        }
        private IEnumerable<Tema> CalculateTimeTillLastUpdateOfTemi(List<Tema> temi)
        {
            int timespan = 0;
            DateTime todayDate = DateTime.Now;
            DateTime updateDate;
            for (int i = 0; i < temi.Count; i++)
            {
                updateDate = DateTime.Parse(temi[i].updateDate);
                timespan = (todayDate.Date - updateDate.Date).Days;

                if (timespan == 0)
                    temi[i].timeago = "today";

                else if (timespan == 1)
                    temi[i].timeago = "yesterday";

                else if (timespan < 7)
                    temi[i].timeago = timespan.ToString() + " days ago";

                else if (timespan < 30)
                    temi[i].timeago = (timespan / 7).ToString() + " weeks ago";

                else if (timespan < 365)
                    temi[i].timeago = (timespan / 30).ToString() + " months ago";

                else
                    temi[i].timeago = (timespan / 365).ToString() + " years ago";
            }
            return temi;
        }
        public IEnumerable<Zadacha> SearchZadachi(int userID, SearchCriteriaZadacha criteria)
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

                    command += " ORDER BY `update_date` DESC;";
                    if (myconnection.State.ToString() != "Open") myconnection.Open();
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
                    return CalculateTimeTillLastUpdateOfZadachi(zadachi);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return zadachi;
            }
        }
        public bool CreateTema(Tema tema)
        {
            try
            {
                using (myconnection)
                {
                    if (myconnection.State.ToString() != "Open") myconnection.Open();
                    string command = "INSERT IGNORE INTO `math_app`.`temi` (`tema`,`description`,`type`,`creation_date`,`update_date`,`event_date`,`user`,`deletionstatus`) " +
                     "VALUES ('" + tema.temaName + "','" + tema.description + "','tema','" + tema.creationDate + "','" + tema.updateDate + "','" + tema.creationDate + "','" + tema.user + "','saved');";
                    mycommand = new MySqlCommand(command, myconnection);
                    mycommand.ExecuteNonQuery();

                    command = "SELECT * FROM math_app.temi WHERE tema='" + tema.temaName + "' AND creation_date='" + tema.creationDate + "';";
                    mycommand = new MySqlCommand(command, myconnection);
                    MySqlDataReader readerT = mycommand.ExecuteReader();
                    while (readerT.Read())
                    {
                        tema.id = (Convert.ToInt32(readerT["id_tema"].ToString()));
                    }
                    readerT.Close();

                    if (tema.zadachi.Count > 0)
                    {
                        for (int i = 0; i < tema.zadachi.Count(); i++)
                        {
                            if (!string.IsNullOrEmpty(tema.zadachi[i].uslovie))
                            {
                                CreateZadacha(tema.zadachi[i]);
                                if(myconnection.State.ToString()!="Open") myconnection.Open();
                                command = "SELECT `id_zadacha` FROM math_app.zadachi WHERE uslovie='" + tema.zadachi[i].uslovie + "' AND creation_date='" + tema.zadachi[i].creationDate + "';";
                                mycommand = new MySqlCommand(command, myconnection);
                                MySqlDataReader readerZ2 = mycommand.ExecuteReader();
                                int a = 0;
                                while (readerZ2.Read())
                                {
                                    tema.zadachi[i].idZadacha = (Convert.ToInt32(readerZ2["id_zadacha"].ToString()));
                                }
                                readerZ2.Close();

                                command = "INSERT IGNORE INTO `math_app`.`junction_zadachi_temi` ( `zadacha`, `tema`) VALUES ('" + tema.zadachi[i].idZadacha + "','" + tema.id + "');";
                                mycommand = new MySqlCommand(command, myconnection);
                                mycommand.ExecuteNonQuery();
                            }

                        }
                    }
                    if (tema.categories.Count > 0)
                    {
                        for (int i = 0; i < tema.categories.Count; i++)
                        {
                            command = "SELECT `id_category` FROM `math_app`.`categories` WHERE `grade`='" + tema.categories[i].grade + "' AND `difficulty`='" + tema.categories[i].difficulty + "';";
                            mycommand = new MySqlCommand(command, myconnection);
                            MySqlDataReader readerC = mycommand.ExecuteReader();
                            while (readerC.Read())
                            {
                                tema.categories[i].idCategory = Convert.ToInt32(readerC["id_category"].ToString());
                            }
                            readerC.Close();

                            command = "INSERT IGNORE INTO `math_app`.`junction_temi_categories` ( `tema`, `category`) VALUES ('" + tema.id + "','" + tema.categories[i].idCategory + "');";
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

        public IEnumerable<Tema> GetTemiByUser(int userID)
        {
            List<Tema> temi = new List<Tema>();
            try
            {
                using (myconnection)
                {
                    string command = "SELECT `id_tema`,`tema`,`description`,`update_date` FROM `math_app`.`temi` WHERE `user`='" + userID + "' AND `deletionstatus`='saved' ORDER BY `update_date` DESC;";
                    if (myconnection.State.ToString() != "Open") myconnection.Open();
                    mycommand = new MySqlCommand(command, myconnection);
                    MySqlDataReader reader = mycommand.ExecuteReader();
                    while (reader.Read())
                    {
                        temi.Add(new Tema(
                            Convert.ToInt32(reader["id_tema"].ToString()),
                            reader["tema"].ToString(),
                            reader["description"].ToString(),
                            reader["update_date"].ToString()));
                    }
                    reader.Close();
                    return CalculateTimeTillLastUpdateOfTemi(temi);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return temi;
            }
        }
        public IEnumerable<Tema> SearchTemi(int userID, SearchCriteriaTema criteria)
        {
            List<Tema> temi = new List<Tema>();
            try
            {
                using (myconnection)
                {
                    string command = "SELECT DISTINCT t.id_tema, t.tema, t.description, t.update_date FROM math_app.temi t " +
                        "LEFT JOIN math_app.junction_temi_categories jtc ON t.id_tema = jtc.tema " +
                        "LEFT JOIN math_app.categories c ON jtc.category = c.id_category " +
                        "WHERE t.`user`= '" + userID + "' AND t.`deletionstatus`= 'saved'";
                    if (!string.IsNullOrEmpty(criteria.name)) command += " AND t.`tema` LIKE '%" + criteria.name + "%'";
                    if (!string.IsNullOrEmpty(criteria.description)) command += " AND t.`description` LIKE '%" + criteria.description + "%'";
                    if (!string.IsNullOrEmpty(criteria.anywhere)) command += " AND (t.`tema` LIKE '%" + criteria.anywhere + "%' OR t.`description` LIKE '%" + criteria.anywhere + "%')";
                    if (!string.IsNullOrEmpty(criteria.fromDate)) command += " AND t.creation_date>='" + criteria.fromDate + "'";
                    if (!string.IsNullOrEmpty(criteria.toDate)) command += " AND t.creation_date<='" + criteria.toDate + " 23:59:59'";
                    if (criteria.category.grade != 0) command += " AND c.grade='" + criteria.category.grade + "'";
                    if (criteria.category.difficulty != "X") command += " AND c.difficulty='" + criteria.category.difficulty + "'";

                    command += " ORDER BY `update_date` DESC;";
                    if (myconnection.State.ToString() != "Open") myconnection.Open();
                    mycommand = new MySqlCommand(command, myconnection);
                    MySqlDataReader reader = mycommand.ExecuteReader();
                    while (reader.Read())
                    {
                        temi.Add(new Tema(
                            Convert.ToInt32(reader["id_tema"].ToString()),
                            reader["tema"].ToString(),
                            reader["description"].ToString(),
                            reader["update_date"].ToString()));
                    }
                    reader.Close();
                    return CalculateTimeTillLastUpdateOfTemi(temi);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return temi;
            }
        }

    }
}
