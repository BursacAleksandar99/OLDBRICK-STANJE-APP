namespace OLDBRICK_STANJE_ARTIKALA_APP.Entities
{
    public class Users
    {
        public long Id { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = "USER";
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }
}
