using Microsoft.IdentityModel.Tokens;
using OLDBRICK_STANJE_ARTIKALA_APP.Entities;
using OLDBRICK_STANJE_ARTIKALA_APP.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace OLDBRICK_STANJE_ARTIKALA_APP.Services.Auth
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwt;

        public TokenService(JwtSettings jwt)
        {
            _jwt = jwt;
        }

        public string CreateToken(Users user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.ExpireMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
