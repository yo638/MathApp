using MathApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using Aspose.Words;
using MathApp.Models.DbModels;

namespace MathApp.Services
{
    public class HashService : IHashService
    {
        private readonly IFileService _fileService;
        public HashService(IFileService fileService)
        {
            _fileService = fileService;
        }
        public string createSalt()
        {
            var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            var salt = new byte[14];
            rng.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }
        public string toSHA256(string inputpsswrd, string salt)
        {
            using HashAlgorithm algorithm = SHA256.Create();
            byte[] bytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputpsswrd + salt));
            var sbuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sbuilder.Append(bytes[i].ToString("x2"));
            }
            sbuilder.Append(salt);
            return sbuilder.ToString();
        }

        public bool comparePasswords(string hashedOldpassword, string newpassword)
        {
            string salt = hashedOldpassword.Substring(hashedOldpassword.Length - 20);
            string hashedNewpassword = toSHA256(newpassword, salt);
            if (hashedOldpassword == hashedNewpassword) return true;
            else return false;
        }

    }
}
