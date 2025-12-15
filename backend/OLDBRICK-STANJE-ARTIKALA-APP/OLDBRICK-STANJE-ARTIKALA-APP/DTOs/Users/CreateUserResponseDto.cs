namespace OLDBRICK_STANJE_ARTIKALA_APP.DTOs.Users
{
    public class CreateUserResponseDto
    {
        public long Id { get; set; }
        public string Username { get; set; } = null!;
        public string Role { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
