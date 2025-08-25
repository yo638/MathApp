namespace MathApp.Services.Interfaces
{
    public interface IHashService
    {
        string createSalt();
        string toSHA256(string inputpsswrd, string salt);
        bool comparePasswords(string hashedOldpassword, string newpassword);
    }
}
