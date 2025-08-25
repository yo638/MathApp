using MathApp.Models.BusinessModels;
using MathApp.Models.DbModels;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathApp.Services.Interfaces
{
    public interface IUserService
    {
        bool RegisterUser(User user);
        bool IsUserActive(User user);
        User GetUserByEmail(string email);
        User GetUserById(int id);
        bool LogUserIn(User user);
        Task ToggleUserDisableOrEnable(User request);
        bool ChangeForgottenUserPassword(User u);
        bool ChangeUserPassword(User user);
        bool EditUser(User user);
        bool DeleteUser(int idUser);
        bool CheckIfUserCodesAreMoreThanFive(int id);
        bool ValidateCodeForChangingPassword(User user);
        bool SendCodeToAlterPassword(string email);
        IEnumerable<User> SearchUsers(SearchCriteriaUser criteria);
        IEnumerable<User> SortUsers(IEnumerable<User> users, string sortBy);
        bool ConfirmUserEmailByToken(int token);
        bool SendConfirmationEmail(string email, HttpContext httpContext);
        IEnumerable<User> GetAllUsers();
    }
}
