using System;

namespace MathApp.Models.DbModels
{
    public partial class ChangePasswordCode
    {
        public ChangePasswordCode() { }
        public ChangePasswordCode(string code, int idUser, int isvalid, DateTime expiresAt)
        {
            Code = code;
            IdUser = idUser;
            IsValid = (sbyte)(isvalid != 0 ? 1 : 0);
            ExpiresAt = expiresAt;
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public int IdUser { get; set; }
        public sbyte IsValid { get; set; }
        public DateTime ExpiresAt { get; set; }


        public virtual User IdUserNavigation { get; set; }
        

    }
}
