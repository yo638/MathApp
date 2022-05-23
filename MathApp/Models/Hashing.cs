using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace MathApp.Models
{
    public class Hashing
    {
        public static string createSalt()
        {
            var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            //the salt is 20 characters long
            var salt = new byte[14];
            rng.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }
        public static string toSHA256(string inputpsswrd, string salt)
        {
            using HashAlgorithm algorithm = SHA256.Create();
            byte[] bytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputpsswrd+salt));
            var sbuilder = new StringBuilder();
            for(int i = 0; i < bytes.Length; i++)
            {
                sbuilder.Append(bytes[i].ToString("x2"));
            }
            sbuilder.Append(salt);
            //the total string is 84 characters long
            return sbuilder.ToString();
        }

        public static bool comparePasswords(string hashedOldpassword, string newpassword)
        {
            //take the salt from the end of oldpass
            string salt = hashedOldpassword.Substring(hashedOldpassword.Length - 20);
            //heshirai newpass with the salt
            string hashedNewpassword=toSHA256(newpassword, salt);
            //compare the two
            if (hashedOldpassword == hashedNewpassword) return true;
            else return false;
        }

    }
}
