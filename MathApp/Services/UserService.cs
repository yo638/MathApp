using MathApp.Models;
using MathApp.Models.BusinessModels;
using MathApp.Models.DbModels;
using MathApp.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MathApp.Services
{
    public class UserService : IUserService
    {
        private readonly math_appContext _context;
        private readonly IHashService _hashContext;

        public UserService(math_appContext context, IHashService hashContext)
        {
            _context = context;
            _hashContext = hashContext;
        }

        public bool RegisterUser(User user)
        {
            try
            {
                var users = _context.Users.Where(u => u.Email == user.Email).Select(u => new { u.Email }).ToList();
                if (users.Count == 0)
                {
                    user.IsDisabled = 0;
                    user.CreationDate = DateTime.Now;
                    user.IdDirectory = 1;
                    string NonHashedPassword = user.Password;
                    user.Password = _hashContext.toSHA256(NonHashedPassword, _hashContext.createSalt());
                    _context.Users.Add(user);
                    _context.SaveChanges();
                    return true;
                }
                else return false;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public bool IsUserActive(User user)
        {
            try
            {
                var isDisabled = _context.Users.Where(u => u.Email == user.Email).Select(u => u.IsDisabled).FirstOrDefault();
                return isDisabled == 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public User GetUserByEmail(string email)
        {
            var user = new List<User>();
            try
            {
                user = _context.Users.Where(u => u.Email == email).ToList();
                return user[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public User GetUserById(int id)
        {
            var user = new List<User>();
            try
            {
                user = _context.Users.Where(u => u.Id == id).ToList();
                return user[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return user[0];
            }
        }
        public bool LogUserIn(User user)
        {
            try
            {

                var users = _context.Users.Select(u => new { u.Email, u.Password }).Where(u => u.Email == user.Email).ToList();
                if (users.Count > 0)
                {
                    if (_hashContext.comparePasswords(users[0].Password, user.Password)) return true;
                    else return false;
                }
                else return false;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public async Task ToggleUserDisableOrEnable(User request)
        {
            var user = await _context.Users.FindAsync(request.Id);
            if (user != null)
            {
                user.IsDisabled = request.IsDisabled;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException($"User with Id {request.Id} not found.");
            }
        }
        public bool ChangeUserPassword(User u)
        {
            try
            {
                var updatedUser = _context.Users.FirstOrDefault(x => x.Id == u.Id);
                if (updatedUser != null)
                {
                    if (_hashContext.comparePasswords(updatedUser.Password, u.OldPassword))
                    {
                        string NonHashedPassword = u.Password;
                        updatedUser.Password = _hashContext.toSHA256(NonHashedPassword, _hashContext.createSalt());
                        _context.SaveChanges();
                        return true;
                    }
                    return false;
                }
                else return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public bool ChangeForgottenUserPassword(User u)
        {
            try
            {
                var updatedUser = _context.Users.FirstOrDefault(x => x.Email == u.Email);
                if (updatedUser != null)
                {
                    string NonHashedPassword = u.Password;
                    updatedUser.Password = _hashContext.toSHA256(NonHashedPassword, _hashContext.createSalt());
                    _context.SaveChanges();
                    return true;
                }
                else return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public bool EditUser(User u)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(e => e.Id == u.Id);
                if (user != null)
                {
                    user.Username = u.Username;
                    user.Name = u.Name;
                    user.Email = u.Email;
                    user.IdRole = u.IdRole;
                    _context.SaveChanges();
                    return true;
                }
                else return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public bool DeleteUser(int idUser)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(z => z.Id == idUser);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    _context.SaveChanges();
                    return true;
                }
                else return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public bool CheckIfUserCodesAreMoreThanFive(int id)
        {
            DateTime fifteenMinutesAfterNow = DateTime.Now.AddMinutes(15);
            DateTime fifteenMinutesAgo = DateTime.Now.AddMinutes(-15);
            var alreadyExistingCodes = _context.ChangePasswordCodes
                .Where(x => x.ExpiresAt >= fifteenMinutesAgo && x.ExpiresAt <= fifteenMinutesAfterNow && x.IdUser == id && x.IsValid == 0)
                .ToList();
            if (alreadyExistingCodes.Count > 5)
            {
                return false;
            }
            return true;
        }
        public bool ValidateCodeForChangingPassword(User user)
        {
            var code = _context.ChangePasswordCodes.Where(x => x.IdUser == user.Id && x.IsValid == 1).FirstOrDefault();
            if (code != null)
            {
                if (code.ExpiresAt < DateTime.Now)
                {
                    code.IsValid = 0;
                    _context.SaveChanges();
                    return false;
                }
                else
                {
                    if (code.Code == user.ChangePasswordCode) return true;
                    else return false;
                }
            }
            return false;

        }


        public bool SendCodeToAlterPassword(string email)
        {
            try
            {
                User user = GetUserByEmail(email);


                var codeString = new byte[6];
                var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
                rng.GetBytes(codeString);
                ChangePasswordCode code = new ChangePasswordCode(Convert.ToBase64String(codeString), user.Id, 1, DateTime.Now.AddMinutes(15));

                _context.ChangePasswordCodes.Add(code);
                var oldCode = _context.ChangePasswordCodes.Where(x => x.IdUser == user.Id).OrderByDescending(x => x.ExpiresAt).FirstOrDefault();
                if (oldCode != null) oldCode.IsValid = 0;
                _context.SaveChanges();

                SmtpClient smtpClient = new SmtpClient("smtp-mail.outlook.com", 587);
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("mathematikateam@outlook.com", "Nula000000");


                MailMessage message = new MailMessage();
                message.From = new MailAddress("mathematikateam@outlook.com");
                message.To.Add(user.Email);
                message.Subject = "Код за промяна на забравена парола";
                message.Body = string.Format(@"Драги {0},<br>Вашият код за възстановяване на паролата е {1}",
                        user.Name, code.Code);
                message.IsBodyHtml = true;

                smtpClient.Send(message);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public IEnumerable<User> SearchUsers(SearchCriteriaUser criteria)
        {
            var users = new List<User>();
            try
            {
                users = _context.Users.ToList();

                //Searching in Username
                if (criteria.idRole != 0) users = users.Where(z => z.IdRole == criteria.idRole).ToList();

                //Searching in Username
                if (criteria.username != null) users = users.Where(z => z.Username.Contains(criteria.username, StringComparison.OrdinalIgnoreCase)).ToList();

                //Searching in Name
                if (criteria.name != null) users = users.Where(z => z.Name.Contains(criteria.name, StringComparison.OrdinalIgnoreCase)).ToList();

                //Searching in Email
                if (criteria.email != null) users = users.Where(z => z.Email.Contains(criteria.email, StringComparison.OrdinalIgnoreCase)).ToList();

                //Searching if user IsDiabled
                if (criteria.isDisabled != "2") users = users.Where(z => z.IsDisabled == int.Parse(criteria.isDisabled)).ToList();

                //Searching in CreationDate
                if (criteria.fromDateCreated != null) users = users.Where(z => z.CreationDate >= DateTime.Parse(criteria.fromDateCreated)).ToList();
                if (criteria.toDateCreated != null) users = users.Where(z => z.CreationDate <= DateTime.Parse(criteria.toDateCreated)).ToList();
                return users;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return users;
            }
        }
        public IEnumerable<User> SortUsers(IEnumerable<User> users, string sortBy)
        {
            try
            {
                switch (sortBy)
                {

                    case "creation_date_ascending":
                        users = users.OrderBy(u => u.CreationDate).ToList();
                        break;
                    case "name_descending":
                        users = users.OrderByDescending(u => u.Name).ToList();
                        break;
                    case "name_ascending":
                        users = users.OrderBy(u => u.Name).ToList();
                        break;
                    case "username_ascending":
                        users = users.OrderByDescending(u => u.Username).ToList();
                        break;
                    case "username_descending":
                        users = users.OrderBy(u => u.Username).ToList();
                        break;
                    default:
                        users = users.OrderByDescending(u => u.CreationDate).ToList();
                        break;
                }
                return users;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return users;
            }
        }
        public bool SendConfirmationEmail(string email, HttpContext httpContext)
        {
            try
            {
                User user = GetUserByEmail(email);

                SmtpClient smtpClient = new SmtpClient("smtp-mail.outlook.com", 587);
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("mathematikateam@outlook.com", "Nula000000");

                MailMessage message = new MailMessage();
                message.From = new MailAddress("mathematikateam@outlook.com");
                message.To.Add(user.Email);
                message.Subject = "Потвърждение на имейл";
                message.IsBodyHtml = true;


                var request = httpContext.Request;
                var hostUrl = $"{request.Scheme}://{request.Host.Value}";

                var emailConfirmationUrl = $"{hostUrl}/Account/EmailConfirmed/{user.Id}";

                var encodedUrl = HtmlEncoder.Default.Encode(emailConfirmationUrl);

                message.Body = string.Format(@"Драги {0},<br>Моля, потвърдете вашият имейл, като кликнете <a href='{1}'>тук</a>.",
                                             user.Name, encodedUrl);

                smtpClient.Send(message);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public bool ConfirmUserEmailByToken(int token)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == token);
                if (user != null)
                {
                    user.EmailConfirmation = (sbyte)1;
                    _context.SaveChanges();
                    return true;
                }
                else return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }
        public IEnumerable<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            try
            {
                users = _context.Users.OrderByDescending(u => u.CreationDate).ToList();
                return users;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return users;
            }
        }
    }

}
