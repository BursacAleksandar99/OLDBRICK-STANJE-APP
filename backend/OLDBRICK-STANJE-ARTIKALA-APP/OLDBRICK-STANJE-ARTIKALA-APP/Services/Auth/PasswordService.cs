namespace OLDBRICK_STANJE_ARTIKALA_APP.Services.Auth
{
    public class PasswordService : IPasswordService
    {

        private const int WorkFactor = 11;

        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required", nameof(password));
                    return BCrypt.Net.BCrypt.HashPassword(password, workFactor: WorkFactor);
        }
        public bool VerifyPassword(string password, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (string.IsNullOrWhiteSpace(passwordHash))
                return false;

            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
