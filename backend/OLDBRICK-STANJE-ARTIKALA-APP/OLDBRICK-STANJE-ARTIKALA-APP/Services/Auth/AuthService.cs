using Microsoft.EntityFrameworkCore;
using OLDBRICK_STANJE_ARTIKALA_APP.Data;
using OLDBRICK_STANJE_ARTIKALA_APP.DTOs.Auth;

namespace OLDBRICK_STANJE_ARTIKALA_APP.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IPasswordService _passwords;
        private readonly ITokenService _tokens;

        public AuthService(AppDbContext db, IPasswordService passwords, ITokenService tokens)
        {
            _db = db;
            _passwords = passwords;
            _tokens = tokens;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Username and password are required");

            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("User is inactive.");

            var ok = _passwords.VerifyPassword(request.Password, user.PasswordHash);
            if(!ok)
                throw new UnauthorizedAccessException("Invalid credentials.");

            var token = _tokens.CreateToken(user);

            return new LoginResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Role = user.Role,
                Token= token
            };
        }
    }
}
