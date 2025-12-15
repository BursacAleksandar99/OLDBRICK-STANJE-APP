namespace OLDBRICK_STANJE_ARTIKALA_APP.Services.Auth
{
    public interface IPasswordService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string passwordHash);
    }
}
