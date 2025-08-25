using System.ComponentModel.DataAnnotations.Schema;

namespace MathApp.Models.DbModels
{
    public partial class User
    {
        [NotMapped]
        public string RepeatPassword { get; set; }
        [NotMapped]
        public string OldPassword { get; set; }
        [NotMapped] 
        public string ChangePasswordCode { get; set; }
    }
}