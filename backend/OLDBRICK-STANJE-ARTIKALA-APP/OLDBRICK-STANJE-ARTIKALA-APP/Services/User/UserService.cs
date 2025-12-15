using Microsoft.EntityFrameworkCore;
using OLDBRICK_STANJE_ARTIKALA_APP.Data;
using OLDBRICK_STANJE_ARTIKALA_APP.DTOs.Users;
using OLDBRICK_STANJE_ARTIKALA_APP.Services.Auth;

namespace OLDBRICK_STANJE_ARTIKALA_APP.Services.User
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        private readonly IPasswordService _password;

        public UserService(AppDbContext db,  IPasswordService password)
        {
            _db = db;
            _password = password;
        }

        public async Task<CreateUserResponseDto> CreateUserAsync(CreateUserRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
                throw new ArgumentException("Username is required.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Password is required.");

            var username = request.Username.Trim();

            var role = (request.Role ?? "USER").Trim().ToUpperInvariant();
            if (role != "USER" && role != "ADMIN")
                throw new ArgumentException("Role must be USER or ADMIN.");

            var exists = await _db.Users
                .AsNoTracking()
                .AnyAsync(u => u.Username == username);

            if (exists)
                throw new InvalidOperationException("Username already exists.");

            var hash = _password.HashPassword(request.Password);

            var user = new Entities.Users
            {
                Username = username,
                PasswordHash = hash,
                Role = role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();


            return new CreateUserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                IsActive = user.IsActive,
            };
        }
    }
}
