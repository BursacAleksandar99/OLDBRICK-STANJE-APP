namespace OLDBRICK_STANJE_ARTIKALA_APP.DTOs.Users
{
    public class CreateUserRequestDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;

        public string Role { get; set; } = "USER";
    }
}
